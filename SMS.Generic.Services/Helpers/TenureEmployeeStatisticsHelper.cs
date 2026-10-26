using SMS.Generic.Dtos.Dtos;

namespace SMS.Generic.Services.Helpers;

/// <summary>
/// Shared helpers for tenure ranking display and waiting-list ordering
/// (aligned with <see cref="SMS.Generic.Services.Interfaces.ITenureEmployeeStatisticsService"/>).
/// </summary>
public static class TenureEmployeeStatisticsHelper
{
    /// <summary>
    /// Maps <see cref="TenureEmployeeStatisticsDto.LevelValues"/> to column labels for UI (same labels as <c>BuildLookbackColumns</c> on <see cref="SMS.Generic.Services.Interfaces.ITenureEmployeeStatisticsService"/>).
    /// </summary>
    public static Dictionary<string, decimal> MapLevelValuesToLabels(
        TenureEmployeeStatisticsDto? dto,
        IReadOnlyList<TenureLookbackColumnDto> columns)
    {
        var dict = new Dictionary<string, decimal>();
        if (dto?.LevelValues == null || columns.Count == 0)
            return dict;

        foreach (var col in columns)
        {
            if (dto.LevelValues.TryGetValue(col.Level, out var val))
                dict[col.Label] = val;
        }

        return dict;
    }

    /// <summary>
    /// Within one employee, preferred waiting-row order used by the tenure job and express-interest list:
    /// <c>OrderNumber</c>, then <c>CreatedDate</c>, then <c>WaitingPostPositionID</c>.
    /// </summary>
    public static IOrderedEnumerable<WaitingPostPositionDto> OrderWaitingRowsWithinEmployee(
        IEnumerable<WaitingPostPositionDto> waitingRows)
    {
        return (waitingRows ?? Enumerable.Empty<WaitingPostPositionDto>())
            .OrderBy(r => r.OrderNumber)
            .ThenBy(r => r.CreatedDate ?? DateTime.MaxValue)
            .ThenBy(r => r.WaitingPostPositionID);
    }

    /// <summary>
    /// Preferred waiting row for an employee (first after <see cref="OrderWaitingRowsWithinEmployee"/>).
    /// </summary>
    public static WaitingPostPositionDto? GetPreferredWaitingRowForEmployee(
        IEnumerable<WaitingPostPositionDto> waitingRows,
        Guid employeeId)
    {
        return OrderWaitingRowsWithinEmployee(
                (waitingRows ?? Enumerable.Empty<WaitingPostPositionDto>())
                    .Where(r => r.EmployeeID == employeeId))
            .FirstOrDefault();
    }

    /// <summary>
    /// Orders waiting rows by tenure statistics employee order (lowest tenure first),
    /// then by preferred waiting-row order within each employee.
    /// </summary>
    public static List<(WaitingPostPositionDto Row, TenureEmployeeStatisticsDto Statistics)> OrderWaitingRowsByTenureStatistics(
        IEnumerable<WaitingPostPositionDto> waitingRows,
        IReadOnlyList<TenureEmployeeStatisticsDto> orderedStatistics)
    {
        var waitingByEmployee = (waitingRows ?? Enumerable.Empty<WaitingPostPositionDto>())
            .GroupBy(r => r.EmployeeID)
            .ToDictionary(
                g => g.Key,
                g => OrderWaitingRowsWithinEmployee(g).ToList());

        var result = new List<(WaitingPostPositionDto Row, TenureEmployeeStatisticsDto Statistics)>();
        foreach (var stat in orderedStatistics ?? Array.Empty<TenureEmployeeStatisticsDto>())
        {
            if (!waitingByEmployee.TryGetValue(stat.EmployeeID, out var employeeRows))
                continue;

            foreach (var row in employeeRows)
                result.Add((row, stat));
        }

        return result;
    }
}
