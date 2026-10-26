using SMS.Generic.Common.Implementations.Paginations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Requester-level invoice statistics (all invoices for the requester, paid and unpaid).
    /// </summary>
    public class RequesterInvoiceDetailsSummaryDto
    {
        public string RequesterName { get; set; } = string.Empty;
        public int TotalInvoices { get; set; }
        public decimal TotalInvoiceAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalRemaining { get; set; }
        public decimal AvailableBalance { get; set; }
    }

    /// <summary>
    /// Requester invoice details: summary across all invoices plus a filtered invoice list.
    /// </summary>
    public class RequesterInvoiceDetailsResponseDto
    {
        public RequesterInvoiceDetailsSummaryDto Summary { get; set; } = new();
        public PagedList<InvoiceDto> Invoices { get; set; } = new();
    }

    /// <summary>
    /// Requester header row for the invoice list (name + invoice count only).
    /// </summary>
    public class RequesterInvoiceCountDto
    {
        public Guid RequesterId { get; set; }
        public string RequesterName { get; set; } = string.Empty;
        public int InvoicesCount { get; set; }
        public bool HasUnpaidInvoice { get; set; }
    }

    /// <summary>
    /// Body payload for requester invoice-count list filtering.
    /// </summary>
    public class RequesterInvoiceCountFilterDto
    {
        public PageParam? PageParam { get; set; }
        public Guid? RequesterId { get; set; }
        public bool? IsPaid { get; set; }
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Due-date lookback: 30, 60, or 90 days from today (inclusive).
        /// </summary>
        public int? DueDateRangeDays { get; set; }
    }
}
