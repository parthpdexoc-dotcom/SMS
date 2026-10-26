using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos;

/// <summary>
/// Tenure process / express-interest dashboard: request header, lookback column definitions, and per-post rows with employee statistics.
/// Statistics align with <see cref="TenureEmployeeStatisticsService"/> and department <c>TenureRulesJson</c> (e.g. L24 = 24-day lookback).
/// </summary>
public class TenureExpressInterestDashboardDto
{
    public Guid RequestID { get; set; }
    public string? RequestTitle { get; set; }
    public string? RequesterName { get; set; }
    public TenureMetricType TenureMetricType { get; set; }

    /// <summary>Lookback windows used for columns (L24, L56, …) from department tenure rules.</summary>
    public List<TenureLookbackColumnDto> LookbackColumns { get; set; } = new();

    /// <summary>Tenure release schedules (process date/time) when configured.</summary>
    public List<TenureProcessScheduleRowDto> TenureProcessSchedules { get; set; } = new();

    public List<TenureExpressInterestJobDto> Jobs { get; set; } = new();
}

public class TenureLookbackColumnDto
{
    public int Level { get; set; }
    public int LookbackDays { get; set; }
    /// <summary>Display label e.g. &quot;L24&quot; for a 24-day window.</summary>
    public string Label { get; set; } = string.Empty;
}

public class TenureProcessScheduleRowDto
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateOnly? ProcessDate { get; set; }
    public TimeOnly? ProcessTime { get; set; }
}

public class TenureExpressInterestJobDto
{
    public Guid PostID { get; set; }
    public string PostTitle { get; set; } = string.Empty;
    public DateOnly DateOfWork { get; set; }
    public TimeOnly StartTime { get; set; }
    public string? Location { get; set; }
    /// <summary>Scheduled shift length in minutes (same as post).</summary>
    public int PostLengthMinutes { get; set; }
    public int TotalPositions { get; set; }

    public List<TenureInterestedEmployeeRowDto> InterestedEmployees { get; set; } = new();
}

public class TenureInterestedEmployeeRowDto
{
    public Guid? WaitingPostPositionId { get; set; }
    public Guid EmployeeID { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? EmployeeNumber { get; set; }
    /// <summary>Employee type short description (e.g. rank/title).</summary>
    public string? EmployeeTypeName { get; set; }
    public int OrderNumber { get; set; }
    /// <summary>
    /// True when the employee has already been applied/assigned from this tenure waiting-list row.
    /// </summary>
    public bool HasAppliedInTenure { get; set; }
    public TenureMetricType tenureType { get; set; }
    /// <summary>Values keyed by <see cref="TenureLookbackColumnDto.Label"/> (e.g. L24).</summary>
    public Dictionary<string, decimal> StatisticsByLabel { get; set; } = new();
}
