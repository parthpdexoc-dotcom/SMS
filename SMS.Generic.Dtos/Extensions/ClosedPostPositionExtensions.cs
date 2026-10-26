using SMS.Generic.Dtos.Dtos;

namespace SMS.Generic.Dtos.Extensions;

public static class ClosedPostPositionExtensions
{
    /// <summary>
    /// Applies department minimum-hours-paid policy. Actual <see cref="ClosedPostPositionDto.HoursWorked"/> is unchanged;
    /// <see cref="ClosedPostPositionDto.IsMinimumDue"/> and <see cref="ClosedPostPositionDto.MinimumHoursDue"/> are set when billing uses the minimum.
    /// </summary>
    public static void ApplyMinimumHoursPaidPolicy(this ClosedPostPositionDto closed, DepartmentPolicyDto? policy)
    {
        ArgumentNullException.ThrowIfNull(closed);

        if (policy == null || !policy.IsMinHoursPaid || policy.MinHoursPaid <= 0)
        {
            closed.IsMinimumDue = false;
            closed.MinimumHoursDue = null;
            return;
        }

        var minMinutes = policy.MinHoursPaid * 60;
        var workedMinutes = closed.HoursWorked ?? closed.GetPostLength() ?? 0;

        if (workedMinutes > 0 && workedMinutes <= minMinutes)
        {
            closed.IsMinimumDue = true;
            closed.MinimumHoursDue = minMinutes;
            return;
        }

        closed.IsMinimumDue = false;
        closed.MinimumHoursDue = null;
    }

    /// <summary>
    /// Hours used for invoice/payroll calculation (minimum applied when due).
    /// </summary>
    public static decimal GetBillableHours(this ClosedPostPositionDto? closed)
    {
        if (closed == null)
            return 0m;

        if (closed.IsMinimumDue == true && closed.MinimumHoursDue is int minDue && minDue > 0)
            return minDue / 60m;

        return (closed.HoursWorked ?? closed.GetPostLength() ?? 0) / 60m;
    }
}
