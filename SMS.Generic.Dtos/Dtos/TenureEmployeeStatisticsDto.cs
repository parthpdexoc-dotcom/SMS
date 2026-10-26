using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos;

/// <summary>
/// Read-only result for tenure-based ordering calculations.
/// </summary>
public class TenureEmployeeStatisticsDto
{
    public Guid EmployeeID { get; set; }
    public int Level { get; set; }
    public TenureMetricType TenureType { get; set; }

    /// <summary>
    /// Assignments count or Hours (depending on TenureType) within the calculated window.
    /// </summary>
    public decimal AssignmentOrHoursValue { get; set; }

    /// <summary>
    /// Backward-compatible alias for previous field name.
    /// </summary>
    public decimal MetricValue { get; set; }

    public DateOnly WindowStart { get; set; }
    public DateOnly WindowEnd { get; set; }

    /// <summary>
    /// Final tie-break source from EmployeeTenure.DateOfHire.
    /// </summary>
    public DateOnly? EmployeeHireDateUtc { get; set; }

    /// <summary>
    /// Per-level values used for lexicographic ordering (L1 then L2 then ...).
    /// </summary>
    public Dictionary<int, decimal> LevelValues { get; set; } = new();
}

