using System.Collections.Concurrent;
using Cronos;
using Microsoft.Extensions.Logging;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Enums;

namespace SMS.Generic.Services.Helpers;

/// <summary>
/// Resolves the latest cron occurrence that is already due on the department's current local date,
/// using the department cron with the appsettings expression as fallback.
/// </summary>
public sealed class DepartmentScheduleEvaluator
{
    /// <summary>Covers a full local day plus any daylight-saving shift.</summary>
    private static readonly TimeSpan LookBack = TimeSpan.FromHours(26);

    private readonly ConcurrentDictionary<string, CronExpression> _expressionCache = new();
    private readonly string _fallbackCron;
    private readonly ILogger _logger;
    private readonly JobType _jobType;

    public DepartmentScheduleEvaluator(string fallbackCron, ILogger logger, JobType jobType = JobType.PayrollExport)
    {
        _fallbackCron = fallbackCron;
        _logger = logger;
        _jobType = jobType;
    }

    /// <summary>
    /// Only the most recent due occurrence is returned, so recovering from downtime runs once instead
    /// of replaying every occurrence that was missed.
    /// </summary>
    public bool TryGetLatestDueOccurrence(DepartmentCronJobScheduleDto department, DateTime nowUtc, out ScheduleOccurrenceDto occurrence)
    {
        occurrence = default;

        var zone = department.TimeZoneId.ToSafeTimeZone();
        var localNow = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, zone);
        var cronText = string.IsNullOrWhiteSpace(department.CronExpression)
            ? _fallbackCron
            : department.CronExpression.Trim();

        try
        {
            var expression = _expressionCache.GetOrAdd(cronText, text => CronExpression.Parse(text, CronFormat.Standard));
            var to = new DateTimeOffset(nowUtc, TimeSpan.Zero);
            var localDate = DateOnly.FromDateTime(localNow);
            var found = false;

            foreach (var candidate in expression.GetOccurrences(to - LookBack, to, zone, fromInclusive: true, toInclusive: true))
            {
                var candidateLocal = TimeZoneInfo.ConvertTimeFromUtc(candidate.UtcDateTime, zone);
                if (DateOnly.FromDateTime(candidateLocal) != localDate)
                    continue;

                occurrence = new ScheduleOccurrenceDto(candidateLocal, candidate.UtcDateTime, localNow);
                found = true;
            }

            return found;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Invalid cron expression for {JobType}: {Cron}", _jobType, cronText);
            return false;
        }
    }
}
