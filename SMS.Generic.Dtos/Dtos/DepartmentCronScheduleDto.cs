using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos;

/// <summary>
/// One row in the department cron configuration UI: one <see cref="JobType" /> and its schedule,
/// with structured fields and optional persisted row id.
/// </summary>
public class DepartmentCronScheduleDto
{
    public Guid? DepartmentCronJobManagementId { get; set; }

    public Guid DepartmentID { get; set; }

    public JobType Job { get; set; }

    public string JobName { get; set; } = string.Empty;

    public ScheduleFrequency Frequency { get; set; }

    public int Hour { get; set; }

    public int Minute { get; set; }

    /// <summary>0 = Sunday, …, 6 = Saturday when <see cref="Frequency"/> is Weekly. When multiple weekdays are set, the first of <see cref="DaysOfWeek"/> (sorted).</summary>
    public int? DayOfWeek { get; set; }

    /// <summary>When <see cref="Frequency"/> is Weekly, one or more run days of week (0 = Sunday, …, 6 = Saturday). Sorted ascending when returned from the API.</summary>
    public int[]? DaysOfWeek { get; set; }

    public int? DayOfMonth { get; set; }

    /// <summary>When <see cref="Frequency"/> is <see cref="ScheduleFrequency.Monthly"/>, how the run day is chosen.</summary>
    public MonthlySubPattern MonthlySubPattern { get; set; }

    /// <summary>1 = first .. 5 = fifth; only for <see cref="MonthlySubPattern.ByWeekdayOrdinal" /> with <see cref="DayOfWeek" />.</summary>
    public int? MonthWeekOrdinal { get; set; }

    /// <summary>When <see cref="Frequency"/> is <see cref="ScheduleFrequency.EveryNMinutes"/>, the N in <c>*/N * * * *</c> (1–59).</summary>
    public int? IntervalMinutes { get; set; }

    /// <summary>When <see cref="Frequency"/> is <see cref="ScheduleFrequency.Custom"/>, the editable expression (same as <see cref="CronExpression"/> for display).</summary>
    public string? CustomCronExpression { get; set; }

    /// <summary>Persisted expression (always set when saved; source of truth for the scheduler).</summary>
    public string? CronExpression { get; set; }

    /// <summary>Plain-language summary of <see cref="CronExpression" /> (e.g. "Every 10 minutes", "Every day at 1:00 AM").</summary>
    public string? CronExpressionDescription { get; set; }
}
