using System.ComponentModel.DataAnnotations;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos;

/// <summary>
/// Time-of-day settings for a scheduled job; the API converts this to a standard 5-field cron string.
/// </summary>
public class DepartmentCronJobTimeItemDto
{
    [Required]
    public JobType Job { get; set; }

    [Range(0, 23)]
    public int Hour { get; set; }

    [Range(0, 59)]
    public int Minute { get; set; }

    /// <summary>
    /// When true, the job runs every day at the given local time; when false, only on <see cref="DayOfWeek"/>.
    /// </summary>
    public bool IsDaily { get; set; } = true;

    /// <summary>
    /// 0 = Sunday through 6 = Saturday; used when <see cref="IsDaily"/> is false.
    /// </summary>
    [Range(0, 6)]
    public int? DayOfWeek { get; set; }
}
