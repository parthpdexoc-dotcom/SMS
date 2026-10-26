using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos;

/// <summary>
/// Department cron and time zone for due-checks when rows exist. If a <see cref="JobType" /> has no
/// <c>DepartmentCronJobManagement</c> data, the scheduler does not use this and falls back to appsettings cron + UTC for that job.
/// </summary>
public class DepartmentCronJobScheduleDto
{
    public Guid DepartmentId { get; set; }

    public JobType JobType { get; set; }

    public string? CronExpression { get; set; } = null;

    public string? TimeZoneId { get; set; }
}
