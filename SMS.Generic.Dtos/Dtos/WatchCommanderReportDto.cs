namespace SMS.Generic.Dtos.Dtos;

/// <summary>
/// Watch commander report row (post-position work date, assigned employee, requester, vehicle flag).
/// </summary>
public sealed class WatchCommanderReportDto
{
    /// <summary>Post position work / start date.</summary>
    public DateOnly? StartDate { get; set; }

    public string? EmployeeName { get; set; }

    /// <summary>Payroll column (not populated by the API; reserved for manual use / future integration).</summary>
    public string? Payroll { get; set; }

    public string? CallSign { get; set; }

    /// <summary><c>EmployeeType.EmployeeTypeShort</c> (short name), not the long description.</summary>
    public string? EmployeeType { get; set; }

    /// <summary>Scheduled start time on the post position.</summary>
    public TimeOnly? StartTime { get; set; }

    /// <summary>Hours from parent <c>Post.PostLength</c> (minutes ÷ 60).</summary>
    public decimal? HoursWorked { get; set; }

    /// <summary>Scheduled end time on the post position.</summary>
    public TimeOnly? EndTime { get; set; }

    public string? Requester { get; set; }

    /// <summary>Request site single line from parent <c>Request</c> (<c>Address</c>, <c>City</c>, <c>State</c> <c>Zip</c>).</summary>
    public string? RequesterAddress { get; set; }

    /// <summary>Whether the job requires a vehicle (from the parent post).</summary>
    public bool Vehicle { get; set; }
}
