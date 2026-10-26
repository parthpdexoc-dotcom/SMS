using System.ComponentModel.DataAnnotations;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos;

/// <summary>Upsert payload for a single <see cref="JobType"/> under a department.</summary>
public class DepartmentCronScheduleUpsertDto
{
    public Guid? DepartmentCronJobManagementId { get; set; }

    [Required]
    public Guid DepartmentID { get; set; }

    [Required]
    public JobType Job { get; set; }

    [Required]
    public ScheduleFrequency Frequency { get; set; }

    [Range(0, 23)]
    public int Hour { get; set; }

    [Range(0, 59)]
    public int Minute { get; set; }

    /// <summary>0 = Sunday, …, 6 = Saturday when <see cref="Frequency"/> is Weekly. Ignored in favor of <see cref="DaysOfWeek" /> when that list is non-empty.</summary>
    [Range(0, 6)]
    public int? DayOfWeek { get; set; }

    /// <summary>When <see cref="Frequency"/> is <see cref="ScheduleFrequency.Weekly" />, one or more run weekdays (0–6). If set and non-empty, takes priority over <see cref="DayOfWeek" />. (Stored as a comma list in the cron DOW field.)</summary>
    public int[]? DaysOfWeek { get; set; }

    [Range(1, 31)]
    public int? DayOfMonth { get; set; }

    /// <summary>When <see cref="ScheduleFrequency.Monthly"/>: which monthly rule to apply. Defaults to <see cref="MonthlySubPattern.ByCalendarDay"/> (legacy).</summary>
    public MonthlySubPattern MonthlySubPattern { get; set; }

    /// <summary>1–5 = first..fifth weekday; only for <see cref="MonthlySubPattern.ByWeekdayOrdinal"/>.</summary>
    [Range(1, 5)]
    public int? MonthWeekOrdinal { get; set; }

    /// <summary>1–59 for <see cref="ScheduleFrequency.EveryNMinutes"/> (<c>*/N * * * *</c>).</summary>
    [Range(1, 59)]
    public int? IntervalMinutes { get; set; }

    /// <summary>Full 5-field cron (Cronos Standard format) when <see cref="ScheduleFrequency.Custom"/>.</summary>
    [StringLength(128)]
    public string? CustomCronExpression { get; set; }
}
