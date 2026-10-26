using SMS.Generic.Dtos.Dtos;

namespace SMS.Generic.Dtos.Extensions;

/// <summary>
/// Invoice DTO helpers for requester-level summaries.
/// </summary>
public static class InvoiceDtoExtensions
{
    /// <summary>
    /// Builds requester invoice summary statistics from all invoices and available payment balance.
    /// </summary>
    public static RequesterInvoiceDetailsSummaryDto ToRequesterInvoiceDetailsSummary(this IEnumerable<InvoiceDto> invoices,decimal availableBalance)
    {
        var list = invoices as IReadOnlyCollection<InvoiceDto> ?? invoices.ToList();
        var requesterName = list.Select(i => i.RequesterName).FirstOrDefault(n => !string.IsNullOrWhiteSpace(n))?.Trim() ?? string.Empty;
        return new RequesterInvoiceDetailsSummaryDto
        {
            RequesterName = requesterName,
            TotalInvoices = list.Count,
            TotalInvoiceAmount = InvoiceDto.SumTotal(list),
            TotalPaid = InvoiceDto.SumPaymentsApplied(list),
            TotalRemaining = InvoiceDto.SumBalanceDue(list),
            AvailableBalance = availableBalance,
        };
    }
}
