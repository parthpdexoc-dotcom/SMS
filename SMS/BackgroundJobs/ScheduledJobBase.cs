using System.Collections.Concurrent;
using Cronos;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Helpers;
using SMS.Helpers;

namespace SMS.BackgroundJobs;

public abstract class ScheduledJobBase : IScheduledJob
{

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ConcurrentDictionary<Guid, DateTime> _lastRun = new();
    private readonly ConcurrentDictionary<string, CronExpression> _cronCache = new();

    protected ILogger Logger { get; }
    protected string ConfiguredCron { get; }
    public JobType JobType { get; }
    protected DepartmentScheduleEvaluator ScheduleEvaluator { get; }
    protected TimeSpan CatchUpWindow { get; }
    protected ConcurrentDictionary<string, byte> FinishedOccurrences { get; } = new(StringComparer.OrdinalIgnoreCase);

    protected ScheduledJobBase(IServiceScopeFactory scopeFactory, ILogger logger, IConfiguration configuration, JobType jobType)
    {
        _scopeFactory = scopeFactory;
        Logger = logger;
        ConfiguredCron = configuration[$"ScheduledJobs:{jobType.GetAppSettingCronKey()}"]?.Trim();
        if (string.IsNullOrWhiteSpace(ConfiguredCron))
            ConfiguredCron = "0 0 * * *";
        JobType = jobType;
        ScheduleEvaluator = new DepartmentScheduleEvaluator(ConfiguredCron, logger, jobType);
        CatchUpWindow = configuration.ReadPayrollDuration("CatchUpWindowMinutes", TimeSpan.FromHours(12));
    }

    public async Task RunIfDueAsync(CancellationToken token = default)
    {
        var rows = await GetDepartmentSchedulesAsync(token).ConfigureAwait(false);
        if (rows.Count == 0)
        {
            Logger.LogWarning("No active departments found for {JobType}", JobType); 
            return;
        }

        foreach (var row in rows)
        {
            if (token.IsCancellationRequested)
                return;

            try
            {
                await RunScheduleDepartmentAsync(row, token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error running {JobType} for Department {DepartmentId}", JobType, row.DepartmentId);
            }
        }
    }

    protected abstract Task ExecuteAsync(IServiceProvider serviceProvider, DepartmentCronJobScheduleDto department, CancellationToken token);

    protected virtual async Task RunScheduleDepartmentAsync(DepartmentCronJobScheduleDto row, CancellationToken token)
    {
        var nowUtc = DateTime.UtcNow.TruncateToUtcMinute();
        var last = _lastRun.GetOrAdd(row.DepartmentId, DateTime.MinValue);
        var from = last == DateTime.MinValue ? nowUtc.AddSeconds(-1) : last.AddSeconds(1);
        var cron = string.IsNullOrWhiteSpace(row.CronExpression) ? ConfiguredCron : row.CronExpression.Trim();

        Logger.LogInformation("Checking Department {DepartmentId} | JobType {JobType} | TimeZone {TimeZone} | DbCron {DbCron} | FinalCron {FinalCron}", row.DepartmentId, JobType, row.TimeZoneId, row.CronExpression ?? "NULL", cron);
        
        if (!TryGetDueUtc(row.CronExpression, row.TimeZoneId.ToSafeTimeZone(), from, nowUtc, out var due))
        {
            Logger.LogInformation("Department {DepartmentId} not due yet for JobType {JobType}", row.DepartmentId, JobType);
            return;
        }
        Logger.LogInformation("Executing Department {DepartmentId} | JobType {JobType} | DueUtc {DueUtc}", row.DepartmentId, JobType, due);
        using var scope = _scopeFactory.CreateScope();

        await ExecuteAsync(scope.ServiceProvider, row, token).ConfigureAwait(false);

        _lastRun[row.DepartmentId] = due;
        Logger.LogInformation("Completed Department {DepartmentId} | JobType {JobType}", row.DepartmentId, JobType);
    }

    protected async Task<IReadOnlyList<DepartmentCronJobScheduleDto>> GetDepartmentSchedulesAsync(CancellationToken token = default)
    {
        using var scope = _scopeFactory.CreateScope();
        return await scope.ServiceProvider
            .GetRequiredService<IDepartmentCronJobManagementRepository>()
            .GetDepartmentCronJobSchedulesByJobTypeAsync(JobType, token)
            .ConfigureAwait(false);
    }

    protected static string OccurrenceKey(Guid departmentId, DateTime occurrenceLocal)
        => $"{departmentId:D}:{occurrenceLocal:yyyyMMdd'T'HHmm}";

    protected async Task RunFileImportDepartmentScheduleAsync(
        DepartmentCronJobScheduleDto department,
        string relativeSubPath,
        string departmentsRootPath,
        IConfiguration configuration,
        Func<IServiceProvider, Task<bool>> runImportAsync,
        CancellationToken cancellationToken)
    {
        var nowUtc = DateTime.UtcNow;

        if (!ScheduleEvaluator.TryGetLatestDueOccurrence(department, nowUtc, out var occurrence))
        {
            Logger.LogDebug("No {JobType} import occurrence due yet today for department {DepartmentId}.", JobType, department.DepartmentId);
            return;
        }

        if (occurrence.Delay > CatchUpWindow)
        {
            Logger.LogWarning(
                "{JobType} import occurrence {Occurrence:o} for department {DepartmentId} is {MinutesLate} minute(s) late, past the {WindowMinutes}-minute catch-up window. Not running.",
                JobType, occurrence.LocalTime, department.DepartmentId, (int)occurrence.Delay.TotalMinutes, (int)CatchUpWindow.TotalMinutes);
            return;
        }

        var occurrenceKey = OccurrenceKey(department.DepartmentId, occurrence.LocalTime);
        if (FinishedOccurrences.ContainsKey(occurrenceKey))
        {
            Logger.LogDebug(
                "{JobType} import occurrence {Occurrence:o} for department {DepartmentId} already finished this process. Skipping.",
                JobType, occurrence.LocalTime, department.DepartmentId);
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var paths = await ExcelImportFileMover.ResolveDepartmentImportPathsAsync(
            serviceProvider,
            configuration,
            departmentsRootPath,
            department.DepartmentId,
            relativeSubPath).ConfigureAwait(false);
        if (paths.Success
            && !ExcelImportFileMover.HasPendingSourceCsv(paths.SourcePath)
            && ExcelImportFileMover.HasArchivedImportForDate(paths.ArchivePath, occurrence.BusinessDate))
        {
            Logger.LogInformation(
                "{JobType} import already completed for department {DepartmentId} occurrence {Occurrence:o} (no source CSV; archive present). Skipping.",
                JobType, department.DepartmentId, occurrence.LocalTime);
            FinishedOccurrences.TryAdd(occurrenceKey, 0);
            return;
        }

        if (occurrence.Delay > TimeSpan.Zero)
        {
            Logger.LogWarning(
                "{JobType} catch-up for department {DepartmentId}: occurrence {Occurrence:o} was missed and is {MinutesLate} minute(s) late. Running now.",
                JobType, department.DepartmentId, occurrence.LocalTime, (int)occurrence.Delay.TotalMinutes);
        }
        else
        {
            Logger.LogInformation(
                "Executing {JobType} import for department {DepartmentId} | Occurrence {Occurrence:o}",
                JobType, department.DepartmentId, occurrence.LocalTime);
        }

        bool finished;
        try
        {
            finished = await runImportAsync(serviceProvider).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            Logger.LogWarning("{JobType}: import cancelled for department {DepartmentId}.", JobType, department.DepartmentId);
            return;
        }

        if (!finished)
        {
            Logger.LogWarning(
                "{JobType} import for department {DepartmentId} did not finish. Occurrence {Occurrence:o} stays eligible for catch-up.",
                JobType, department.DepartmentId, occurrence.LocalTime);
            return;
        }

        FinishedOccurrences.TryAdd(occurrenceKey, 0);
        Logger.LogInformation(
            "Completed {JobType} import for department {DepartmentId} | Occurrence {Occurrence:o}",
            JobType, department.DepartmentId, occurrence.LocalTime);
    }

    private bool TryGetDueUtc(string? cron, TimeZoneInfo zone, DateTime fromUtc, DateTime nowUtc, out DateTime dueUtc)
    {
        dueUtc = default;
        try
        {
            cron = string.IsNullOrWhiteSpace(cron) ? ConfiguredCron : cron.Trim();

            var expr = _cronCache.GetOrAdd(cron,x => CronExpression.Parse(x, CronFormat.Standard));
            var next = expr.GetNextOccurrence(new DateTimeOffset(fromUtc, TimeSpan.Zero),zone);
            if (!next.HasValue) return false;
            dueUtc = next.Value.UtcDateTime;
            return dueUtc <= nowUtc;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Invalid cron expression for {JobType}: {Cron}", JobType, cron);
            return false;
        }
    }
}