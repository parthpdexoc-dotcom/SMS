using System.Collections.Concurrent;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Enums;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Helpers;
using SMS.Generic.Services.Interfaces;

namespace SMS.BackgroundJobs;

/// <summary>
/// Scheduled job to process payroll data from database and write CSV exports under DepartmentPath.
/// Schedule: <c>DepartmentCronJobManagement.CronExpression</c> for <see cref="JobType.PayrollExport"/>,
/// falling back to <c>ScheduledJobs:PayrollExportJobCron</c>.
/// </summary>
/// <remarks>
/// This job overrides <see cref="ScheduledJobBase.RunScheduleDepartmentAsync"/> instead of using the base
/// due-check, because that check is forward-only against an in-memory
/// last-run timestamp: if the host is down or restarting at the scheduled minute, the occurrence is
/// lost until the next day. Payroll treats an occurrence as outstanding until
/// <see cref="IPayrollProcessingService.FindExportedPayrollFile"/> finds an export for it, and re-runs
/// within a same-day catch-up window. Other jobs keep the base behaviour.
/// </remarks>
public class PayrollExportJob : ScheduledJobBase
{
    private readonly ILogger<PayrollExportJob> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _retryWindow;
    private readonly TimeSpan _retryInterval;

    public PayrollExportJob(
        IServiceScopeFactory scopeFactory,
        ILogger<PayrollExportJob> logger,
        IConfiguration configuration)
        : base(
            scopeFactory,
            logger,
            configuration,
            JobType.PayrollExport)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _retryWindow = configuration.ReadPayrollDuration("RetryWindowMinutes", TimeSpan.FromMinutes(20));
        _retryInterval = configuration.ReadPayrollDuration("RetryIntervalMinutes", TimeSpan.FromMinutes(5));
    }

    /// <summary>
    /// Satisfies the <see cref="ScheduledJobBase"/> contract by delegating to the same catch-up aware path
    /// so there is a single place that runs a department.
    /// </summary>
    protected override Task ExecuteAsync(IServiceProvider serviceProvider, DepartmentCronJobScheduleDto department, CancellationToken cancellationToken)
        => RunScheduleDepartmentAsync(department, cancellationToken);

    protected override async Task RunScheduleDepartmentAsync(DepartmentCronJobScheduleDto department, CancellationToken cancellationToken)
    {
        var nowUtc = DateTime.UtcNow;

        if (!ScheduleEvaluator.TryGetLatestDueOccurrence(department, nowUtc, out var occurrence))
        {
            _logger.LogDebug("No payroll occurrence due yet today for department {DepartmentId}.", department.DepartmentId);
            return;
        }

        if (IsTuesdaySkipped(occurrence.BusinessDate, department.DepartmentId))
            return;

        if (occurrence.Delay > CatchUpWindow)
        {
            _logger.LogWarning(
                "Payroll occurrence {Occurrence:o} for department {DepartmentId} is {MinutesLate} minute(s) late, past the {WindowMinutes}-minute catch-up window. Not running.",
                occurrence.LocalTime, department.DepartmentId, (int)occurrence.Delay.TotalMinutes, (int)CatchUpWindow.TotalMinutes);
            return;
        }

        var occurrenceKey = OccurrenceKey(department.DepartmentId, occurrence.LocalTime);
        if (FinishedOccurrences.ContainsKey(occurrenceKey))
        {
            _logger.LogDebug(
                "Payroll occurrence {Occurrence:o} for department {DepartmentId} already finished this process. Skipping.",
                occurrence.LocalTime, department.DepartmentId);
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var departmentConfig = await serviceProvider
            .GetRequiredService<IDepartmentConfigurationService>()
            .GetByDepartmentIdAsync(department.DepartmentId)
            .ConfigureAwait(false);

        if (departmentConfig?.EnablePayrollCronJob != true)
        {
            _logger.LogInformation(
                "Skipping payroll export for department {DepartmentId}. EnablePayrollCronJob is disabled.",
                department.DepartmentId);
            return;
        }

        var payrollProcessingService = serviceProvider.GetRequiredService<IPayrollProcessingService>();
        var departmentDetails = await serviceProvider
            .GetRequiredService<IDepartmentService>()
            .GetByIdAsync(department.DepartmentId)
            .ConfigureAwait(false);

        // An export written at or after the scheduled time means this occurrence already ran (including
        // after a host restart). Naming and folder layout live in PayrollProcessingService.
        var existingExport = payrollProcessingService.FindExportedPayrollFile(
            department.DepartmentId,
            departmentDetails?.EmployeeDataFolderName,
            occurrence.Utc);

        if (!string.IsNullOrWhiteSpace(existingExport))
        {
            _logger.LogInformation(
                "Payroll CSV already generated for department {DepartmentId} occurrence {Occurrence:o}: {CsvPath}. Skipping generation.",
                department.DepartmentId, occurrence.LocalTime, existingExport);
            FinishedOccurrences.TryAdd(occurrenceKey, 0);
            return;
        }

        if (occurrence.Delay > TimeSpan.Zero)
        {
            _logger.LogWarning(
                "Payroll catch-up for department {DepartmentId}: occurrence {Occurrence:o} was missed and is {MinutesLate} minute(s) late. Running now.",
                department.DepartmentId, occurrence.LocalTime, (int)occurrence.Delay.TotalMinutes);
        }
        else
        {
            _logger.LogInformation(
                "Executing payroll export for department {DepartmentId} | Occurrence {Occurrence:o}",
                department.DepartmentId, occurrence.LocalTime);
        }

        var outcome = await ProcessDepartmentPayrollAsync(
                serviceProvider,
                department,
                departmentDetails,
                occurrence.BusinessDate,
                cancellationToken)
            .ConfigureAwait(false);

        if (!outcome.IsFinished)
        {
            _logger.LogWarning(
                "Payroll export for department {DepartmentId} was cancelled before finishing. Occurrence {Occurrence:o} stays eligible for catch-up.",
                department.DepartmentId, occurrence.LocalTime);
            return;
        }

        FinishedOccurrences.TryAdd(occurrenceKey, 0);

        _logger.LogInformation(
            "Completed payroll export for department {DepartmentId} | Occurrence {Occurrence:o} | Status {Status}",
            department.DepartmentId, occurrence.LocalTime, outcome.Status);
    }

    /// <summary>
    /// Builds the payroll export for <paramref name="businessDate"/>, keeping the existing retry window
    /// and Success/Retry/Failed notifications.
    /// </summary>
    private async Task<PayrollRunOutcomeDto> ProcessDepartmentPayrollAsync(
        IServiceProvider serviceProvider,
        DepartmentCronJobScheduleDto department,
        DepartmentDto? departmentDetails,
        DateOnly businessDate,
        CancellationToken cancellationToken)
    {
        var payrollProcessingService = serviceProvider.GetRequiredService<IPayrollProcessingService>();
        var notificationService = serviceProvider.GetRequiredService<IPayrollNotificationService>();

        var departmentName = string.IsNullOrWhiteSpace(departmentDetails?.DepartmentName)
            ? department.DepartmentId.ToString()
            : departmentDetails.DepartmentName.Trim();

        // Dates come from the scheduled occurrence, not the current clock, so a late catch-up run
        // exports the same range the on-time run would have.
        var payrollStartDate = businessDate.DayOfWeek == DayOfWeek.Wednesday
            ? businessDate.AddDays(-2)
            : businessDate.AddDays(-1);

        var retryDeadlineUtc = DateTime.UtcNow + _retryWindow;
        var retryEmailSent = false;

        while (!cancellationToken.IsCancellationRequested)
        {
            var result = await TryProcessPayrollDataAsync(
                    payrollProcessingService,
                    department.DepartmentId,
                    payrollStartDate,
                    businessDate)
                .ConfigureAwait(false);

            if (result.IsSuccess)
            {
                await notificationService.NotifyAsync(
                    result, departmentName, PayrollRunStatus.Success, department.TimeZoneId, cancellationToken: cancellationToken);

                if (result.PayrollDataRecords.Any())
                {
                    _logger.LogInformation("Payroll export completed successfully for department {DepartmentId}.", department.DepartmentId);
                    return PayrollRunOutcomeDto.Completed(result.GeneratedDocumentPath);
                }

                // Success with no rows writes no file; recorded as a finished occurrence so catch-up does not loop on it.
                _logger.LogInformation("No payroll data found for department {DepartmentId}. Export skipped.", department.DepartmentId);
                return PayrollRunOutcomeDto.NoRecords();
            }

            if (DateTime.UtcNow >= retryDeadlineUtc)
            {
                await notificationService.NotifyAsync(
                    result, departmentName, PayrollRunStatus.Failed, department.TimeZoneId, cancellationToken: cancellationToken);

                _logger.LogError("Payroll export file generation failed for department {DepartmentId} after retrying for {WindowMinutes} minutes. Errors: {Errors}",
                    department.DepartmentId, (int)_retryWindow.TotalMinutes, string.Join("; ", result.Errors));
                return PayrollRunOutcomeDto.Failed(result.Errors);
            }

            if (!retryEmailSent)
            {
                await notificationService.NotifyAsync(
                    result, departmentName, PayrollRunStatus.Retry, department.TimeZoneId, retryDeadlineUtc, cancellationToken);
                retryEmailSent = true;
            }

            _logger.LogWarning("Payroll export file generation failed for department {DepartmentId}. Retrying in {IntervalMinutes} minutes.",
                department.DepartmentId, (int)_retryInterval.TotalMinutes);
            await Task.Delay(_retryInterval, cancellationToken).ConfigureAwait(false);
        }

        return PayrollRunOutcomeDto.Cancelled();
    }

    private async Task<PayrollProcessingResultDto> TryProcessPayrollDataAsync(
        IPayrollProcessingService payrollProcessingService,
        Guid departmentId,
        DateOnly payrollStartDate,
        DateOnly businessDate)
    {
        try
        {
            return await payrollProcessingService
                .ProcessPayrollDataAsync(payrollStartDate, businessDate, null, departmentId)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred during payroll processing for department {DepartmentId}.", departmentId);

            return new PayrollProcessingResultDto(DateTime.UtcNow, $"{payrollStartDate:yyyy-MM-dd} to {businessDate:yyyy-MM-dd}")
            {
                IsSuccess = false,
                Errors = new List<string> { ex.Message }
            };
        }
    }

    private bool IsTuesdaySkipped(DateOnly businessDate, Guid departmentId)
    {
        // Keep existing Tuesday logic for daily cron runs
        if (businessDate.DayOfWeek != DayOfWeek.Tuesday)
            return false;

        _logger.LogInformation("Skipping Tuesday payroll export for department {DepartmentId}. Wednesday export will cover Monday through Wednesday.", departmentId);
        return true;
    }
}
