using SMS.Generic.Dtos.Dtos;

namespace SMS.Generic.Services.Interfaces;

public interface ITenureEmployeeStatisticsService
{
    /// <summary>
    /// Read-only: computes tenure-based metrics for employees using department policy rules
    /// and returns employees sorted by all rule levels in order, then hire date when tied.
    /// The lookback anchor is <paramref name="statisticsWindowEndDate"/> (usually the tenure schedule process date).
    /// Each rule counts assignments from <c>anchor - rule.Days</c> through <c>anchor</c>, plus every assignment on a later date of work.
    /// </summary>
    Task<List<TenureEmployeeStatisticsDto>> GetTenureEmployeeStatisticsAsync(
        DepartmentPolicyDto departmentPolicy,
        List<Guid> employeeIds,
        DateOnly statisticsWindowEndDate,
        Guid? departmentId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Builds UI column definitions from department policy <c>TenureRulesJson</c>, using the same parsing and labels as tenure statistics.
    /// </summary>
    List<TenureLookbackColumnDto> BuildLookbackColumns(string? tenureRulesJson);
}

