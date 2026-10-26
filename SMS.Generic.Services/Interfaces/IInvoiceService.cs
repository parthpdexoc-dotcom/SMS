using System;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using System.Collections.Generic;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for invoice service operations.
    /// </summary>
    public interface IInvoiceService : IGenericService<InvoiceDto, Invoice>
    {
        /// <summary>
        /// Retrieves an invoice by its primary key (Guid).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The invoice DTO if found; otherwise, null.</returns>
        Task<InvoiceDto?> GetByIdAsync(Guid id);

        /// <summary>
        /// Checks whether an invoice already exists for the given request and overlapping date range.
        /// </summary>
        /// <param name="requestId">The related request identifier.</param>
        /// <param name="startDate">The start of the date range.</param>
        /// <param name="endDate">The end of the date range.</param>
        /// <returns>True if an overlapping invoice exists; otherwise, false.</returns>
        Task<bool> HasOverlappingInvoiceAsync(Guid requestId, DateOnly startDate, DateOnly endDate);

        /// <summary>
        /// Gets invoices filtered by requester and its direct child requesters (paged; <c>TotalRecords</c> is count before paging).
        /// </summary>
        /// <param name="requesterId">Parent requester ID filter. If null/empty, returns all invoices.</param>
        /// <param name="pagination">Paging and sort; when null, a default <see cref="PageParam"/> is used.</param>
        /// <param name="isPaid">When true, only paid invoices; when false, only unpaid; when null, no paid-status filter.</param>
        /// <param name="searchTerm">Optional filter on invoice number or requester name (partial match).</param>
        /// <param name="dueDateRangeDays">Due-date lookback: 30, 60, or 90 days from today (inclusive).</param>
        /// <returns>Paged invoices for the requester and its child requesters.</returns>
        Task<PagedList<InvoiceDto>> GetAllByRequesterIncludingChildrenAsync(Guid? requesterId, PageParam? pagination = null, bool? isPaid = null, string? searchTerm = null, int? dueDateRangeDays = null);

        /// <summary>
        /// Gets requester invoice summary (all invoices) and a paged list filtered by <paramref name="isPaid"/>.
        /// </summary>
        Task<RequesterInvoiceDetailsResponseDto> GetRequesterInvoiceDetailsAsync(Guid requesterId,bool isPaid,PageParam? pagination = null,CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets paged requesters that have invoices, with invoice count only (no invoice rows).
        /// </summary>
        Task<PagedList<RequesterInvoiceCountDto>> GetRequesterInvoiceCountsAsync(
            PageParam pageParam,
            Guid? requesterId = null,
            bool? isPaid = null,
            string? searchTerm = null,
            int? dueDateRangeDays = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns requester IDs that have at least one unpaid invoice with due date on or before department today.
        /// </summary>
        Task<IReadOnlySet<Guid>> GetRequesterIdsWithUnpaidInvoicesDueTillDateAsync(
            IEnumerable<Guid> requesterIds,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns false when the requester has any unpaid invoice with due date on or before department today; otherwise true.
        /// </summary>
        Task<bool> IsRequesterClearOfOverdueUnpaidInvoicesAsync(
            Guid requesterId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Recalculates invoice totals (OfficerHourlyTotal, VehicleFeesTotal, EquipmentFeesTotal, Total, BalanceDue)
        /// from the provided invoice details and persists the updated invoice.
        /// </summary>
        /// <param name="invoiceId">Invoice ID to update.</param>
        /// <param name="details">All invoice details for that invoice (including debit/credit adjustments).</param>
        Task RecalculateTotalsAsync(Guid invoiceId, IReadOnlyList<InvoiceDetailDto> details);

        /// <summary>
        /// Gets invoice pay summary, payment history for the invoice, and requester unapplied balance.
        /// </summary>
        /// <param name="invoiceId">The invoice identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Payment information when the invoice exists; otherwise, null.</returns>
        Task<InvoicePaymentInformationResponseDto?> GetInvoicePaymentInformationAsync(Guid invoiceId,CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets invoice total, paid, balance due, and net applied amount per payment.
        /// </summary>
        Task<InvoicePaymentSummaryResponseDto?> GetInvoicePaymentSummaryAsync(Guid invoiceId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Voids the active payment allocation(s) for an invoice and payment, then recalculates invoice paid amount, balance due, and paid flag.
        /// </summary>
        Task<InvoicePaymentInformationResponseDto> VoidInvoicePaymentAsync(Guid invoiceId,Guid paymentId,string reason,CancellationToken cancellationToken = default);

        /// <summary>
        /// Recalculates <c>PaymentsApplied</c>, <c>BalanceDue</c>, and <c>IsPaid</c> from active payment allocations.
        /// </summary>
        Task RecalculateInvoicePaymentTotalsAsync(Guid invoiceId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets invoice(s) and related line items for a request identified by its project number.
        /// </summary>
        /// <param name="projectNumber">Request project number (not fiscal year + project).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Invoices for the matching request, ordered by date created descending.</returns>
        Task<IReadOnlyList<InvoiceDto>> GetInvoiceByProjectNumberAsync(string projectNumber, CancellationToken cancellationToken = default);
    }
}

