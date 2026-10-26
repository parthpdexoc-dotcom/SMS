using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;

namespace SMS.Generic.Services.Helpers;

/// <summary>
/// Computes invoice officer and agency fee totals from department fees.
/// </summary>
public static class DepartmentFeeInvoiceTotalsHelper
{
    /// <summary>
    /// Whether a fee is active and effective for the given request start date.
    /// </summary>
    public static bool IsEffectiveForRequestDate(DepartmentFeeDto fee, DateOnly requestStartDate) =>
        fee.IsActive
        && fee.EffectiveStartDate.HasValue
        && fee.EffectiveStartDate.Value <= requestStartDate
        && (!fee.EffectiveEndDate.HasValue || fee.EffectiveEndDate.Value >= requestStartDate);

    /// <summary>
    /// Filters department fees to those selected and effective for invoice calculation.
    /// </summary>
    public static IEnumerable<DepartmentFeeDto> FilterForInvoice(
        IEnumerable<DepartmentFeeDto> departmentFees,
        IReadOnlyCollection<Guid> selectedFeeIds,
        DateOnly requestStartDate,
        bool enableFees)
    {
        if (!enableFees)
            return Enumerable.Empty<DepartmentFeeDto>();

        var selected = selectedFeeIds.ToHashSet();
        return departmentFees.Where(f =>
            f.FeeId.HasValue
            && selected.Contains(f.FeeId.Value)
            && IsEffectiveForRequestDate(f, requestStartDate));
    }

    /// <summary>
    /// Accumulates officer and agency totals for a single fee into the running totals.
    /// </summary>
    private static void AccumulateFee(DepartmentFeeDto fee, decimal amount, ref decimal officerFeesTotal, ref decimal agencyFeesTotal)
    {
        switch (fee.ReimburseTo)
        {
            case ReimburseTo.City:
                agencyFeesTotal += amount;
                break;
            case ReimburseTo.Employee:
                officerFeesTotal += amount;
                break;
        }
    }

    /// <summary>
    /// Calculates officer and agency fee totals across all supported fee types.
    /// <list type="bullet">
    ///   <item><see cref="FeeType.PerRequest"/> — <see cref="DepartmentFeeDto.DefaultFee"/> charged once per request.</item>
    ///   <item><see cref="FeeType.PerOfficer"/> — scaled by officer (closed post position) count.</item>
    ///   <item><see cref="FeeType.PerPost"/> — scaled by distinct post/job count.</item>
    /// </list>
    /// Fee amount formula: <c>DefaultFee + (AdditionalFee × (count - 1))</c>.
    /// Fees reimbursed to <see cref="ReimburseTo.Employee"/> go to officer fees;
    /// fees reimbursed to <see cref="ReimburseTo.City"/> go to agency fees.
    /// Fees reimbursed to <see cref="ReimburseTo.Department"/> are excluded from the invoice.
    /// </summary>
    /// <param name="departmentFees">Department fees eligible for this invoice.</param>
    /// <param name="requestCount">Number of distinct requests on the invoice (typically 1).</param>
    /// <param name="officerCount">Number of eligible closed post position lines (officers).</param>
    /// <param name="postCount">Number of distinct posts/jobs under the invoice.</param>
    /// <returns>Officer fees total and agency fees total.</returns>
    public static (decimal OfficerFeesTotal, decimal AgencyFeesTotal) CalculatePerOfficerFeeTotals(
        IEnumerable<DepartmentFeeDto> departmentFees,
        int requestCount,
        int officerCount,
        int postCount)
    {
        decimal officerFeesTotal = 0m;
        decimal agencyFeesTotal = 0m;

        foreach (var fee in departmentFees)
        {
            var count = fee.FeeType switch
            {
                FeeType.PerRequest => requestCount,
                FeeType.PerOfficer => officerCount,
                FeeType.PerPost    => postCount,
                _                  => 0
            };

            if (count <= 0)
                continue;

            var amount = fee.DefaultFee + (fee.AdditionalFee * (count - 1));
            AccumulateFee(fee, amount, ref officerFeesTotal, ref agencyFeesTotal);
        }

        return (officerFeesTotal, agencyFeesTotal);
    }

    /// <summary>
    /// Calculates fee totals from invoice lines (officer count = line count, post count = distinct posts on lines).
    /// </summary>
    public static (decimal OfficerFeesTotal, decimal AgencyFeesTotal) CalculatePerOfficerFeeTotals(
        IEnumerable<DepartmentFeeDto> departmentFees,
        IReadOnlyCollection<InvoiceDetailDto> invoiceDetails,
        int requestCount = 1)
    {
        var officerCount = invoiceDetails.Count;
        var postCount = invoiceDetails
            .Select(d => d.PostID ?? d.ClosedPostPosition?.PostID)
            .Where(id => id.HasValue && id.Value != Guid.Empty)
            .Select(id => id!.Value)
            .Distinct()
            .Count();

        return CalculatePerOfficerFeeTotals(departmentFees, requestCount, officerCount, postCount);
    }
}
