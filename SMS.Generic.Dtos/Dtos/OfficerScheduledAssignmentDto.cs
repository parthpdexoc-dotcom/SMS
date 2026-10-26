using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos;

/// <summary>
/// Minimal officer schedule row for the dashboard buckets.
/// </summary>
public sealed class OfficerScheduledAssignmentDto
{
    public OfficerScheduledAssignmentDto()
    {
    }

    public OfficerScheduledAssignmentDto(
        string? employeeName,
        string? payroll,
        string? employeeType,
        string? post,
        DateOnly? dateOfWork,
        TimeOnly? startTime,
        TimeOnly? endTime,
        string? callSign = null,
        decimal? hoursWorked = null)
    {
        EmployeeName = employeeName;
        Payroll = payroll;
        EmployeeType = employeeType;
        Post = post;
        DateOfWork = dateOfWork;
        StartTime = startTime;
        EndTime = endTime;
        CallSign = callSign;
        HoursWorked = hoursWorked;
    }

    public string? EmployeeName { get; set; }
    public string? Payroll { get; set; }
    public string? EmployeeType { get; set; }
    public string? Post { get; set; }
    public DateOnly? DateOfWork { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }

    public string? CallSign { get; set; }

    /// <summary>Scheduled hours from post length (minutes ÷ 60), same as watch commander report.</summary>
    public decimal? HoursWorked { get; set; }

    /// <summary>High-level schedule status: RecentlyEnded, CurrentlyWorking, or Upcoming.</summary>
    public OfficerScheduleStatus Status { get; set; }
}
