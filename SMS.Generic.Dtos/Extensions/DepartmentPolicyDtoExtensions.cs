using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Extensions;

/// <summary>
/// Shared tenure-policy helpers for APIs aligned with tenure statistics calculation.
/// </summary>
public static class DepartmentPolicyDtoExtensions
{
    /// <summary>
    /// Metric used for tenure lookbacks: assignment counts versus hours in each rule window (department default is assignments).
    /// </summary>
    public static TenureMetricType GetEffectiveTenureMetricType(this DepartmentPolicyDto policy)
    {
        ArgumentNullException.ThrowIfNull(policy);
        return policy.TenureType ?? TenureMetricType.Assignments;
    }
}
