using System.Collections.Generic;
using SMS.Generic.Common.Implementations.Responses;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for invoice detail service operations.
    /// </summary>
    public interface IInvoiceDetailService : IGenericService<InvoiceDetailDto, InvoiceDetail>
    {
        /// <summary>
        /// Retrieves an invoice detail by its primary key (Guid).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The invoice detail DTO if found; otherwise, null.</returns>
        Task<InvoiceDetailDto?> GetByIdAsync(Guid id);

        /// <summary>
        /// Appends new edit versions for one or more invoice details and returns all latest lines for the invoice.
        /// </summary>
        /// <param name="dtos">Line items to update (each must include <see cref="InvoiceDetailDto.InvoiceDetailID"/>).</param>
        /// <returns>Latest invoice detail line per closed post position after updates.</returns>
        Task<IReadOnlyList<InvoiceDetailDto>> UpdateInvoiceDetailsAsync(IReadOnlyList<InvoiceDetailDto> dtos);

        /// <summary>
        /// Soft-deletes one or more invoice detail rows and returns all latest lines for the invoice.
        /// </summary>
        /// <summary>
        /// Undoes all manual modifications for an invoice: sets <see cref="BaseEntity.IsDeleted"/> on active
        /// <see cref="FinancialImpactType.Modified"/> and superseded manual <see cref="FinancialImpactType.Debit"/> rows.
        /// </summary>
        /// <param name="invoicesId">Invoice primary key (not an invoice detail ID).</param>
        Task<IReadOnlyList<InvoiceDetailDto>> UndoInvoiceModificationsAsync(Guid invoicesId);

        /// <summary>
        /// Returns the prior billing version for a line by <see cref="BaseEntity.CreatedDate"/> (read-only; no database changes).
        /// </summary>
        /// <param name="invoiceDetailId">The invoice detail currently displayed (any version in the chain).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Previous version with <see cref="InvoiceDetailDto.CurrentInvoiceDetailId"/> set; null when missing.</returns>
        Task<InvoiceDetailDto?> ViewPreviousVersionAsync(Guid invoiceDetailId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the prior billing version for each current display line on the invoice (read-only; no database changes).
        /// </summary>
        /// <param name="invoicesId">Invoice ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>One <see cref="InvoiceDetailDto"/> per line that has a previous version (<see cref="InvoiceDetailDto.CurrentInvoiceDetailId"/> set).</returns>
        Task<IReadOnlyList<InvoiceDetailDto>> ViewPreviousInvoiceVersionsAsync(
            Guid invoicesId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates an invoice from request data: calls GetRequestDataForInvoice, then persists the result into Invoice and InvoiceDetails tables.
        /// </summary>
        /// <param name="projectNumber">Combined project number (e.g. 202600001).</param>
        /// <param name="startDate">Start date filter.</param>
        /// <param name="endDate">End date filter.</param>
        /// <returns>SuccessResult with created InvoiceDto and details on success; Status 404 and Message when no request or no details.</returns>
        Task<SuccessResult<InvoiceDto>> GenerateInvoiceFromRequestDataAsync(string projectNumber, DateOnly startDate, DateOnly endDate);

        /// <summary>
        /// Generates invoices for multiple project numbers using the same date range.
        /// </summary>
        /// <param name="projectNumbers">Combined project numbers (year + number, e.g. 202600001).</param>
        /// <param name="startDate">Start date filter.</param>
        /// <param name="endDate">End date filter.</param>
        /// <returns>SuccessResult with created invoices.</returns>
        Task<SuccessResult<List<InvoiceDto>>> GenerateInvoicesAsync(IReadOnlyCollection<string> projectNumbers, DateOnly startDate, DateOnly endDate);

        /// <summary>
        /// Gets all invoice details for a specific invoice ID and includes department logo from the current user's token (department ID).
        /// </summary>
        /// <param name="invoicesId">The invoice ID to filter by.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Response with Items (invoice details) and optional Logo (contentType + base64) when token has department ID.</returns>
        Task<InvoiceDetailsWithLogoResponseDto> GetByInvoiceIdWithLogoAsync(Guid invoicesId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Original billing line items for an invoice (financial impact None) with closed post position included for work date/time.
        /// Summary fields mirror the persisted invoice header totals.
        /// </summary>
        /// <param name="invoiceId">Invoice primary key.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Line items and summary, or null when the invoice does not exist.</returns>
        Task<ViewOriginalInvoiceDetailsResponseDto?> ViewOriginalDetailsByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Calculates the invoice total after unclosing the given closed post position and throws HTTP 409 when
        /// the reduced total would fall below <see cref="InvoiceDto.PaymentsApplied"/>.
        /// No-ops when the closed post position is not invoiced.
        /// </summary>
        /// <param name="closedPostPositionId">Closed post position ID linked to invoice detail(s).</param>
        Task EnsureUncloseInvoiceAllowedAsync(Guid closedPostPositionId);

        /// <summary>
        /// When a closed post position is unclosed, inserts a reversing (Debit) invoice detail line (negative amounts)
        /// for the same <see cref="InvoiceDetailDto.ClosePostPositionID"/>, then recalculates invoice totals.
        /// No-ops when no invoice detail exists for the given closed post position, or when a debit line already exists.
        /// </summary>
        /// <param name="closedPostPositionId">Closed post position ID linked to invoice detail(s).</param>
        /// <param name="deferRecalculation">
        /// When true (re-close edit flow), persist the debit line but skip invoice total recalculation until the new
        /// credit line is applied — avoids a false zero-total error on the intermediate unclose step.
        /// </param>
        Task ApplyDebitForUncloseAsync(Guid closedPostPositionId, bool deferRecalculation = false);

        /// <summary>
        /// When an unclosed post position is closed again (re-close), inserts a new invoice detail line marked as
        /// <see cref="SMS.Generic.Enums.FinancialImpactType.Credit"/> on the same invoice as the prior line(s),
        /// then recalculates invoice totals.
        /// No-ops when no invoice line exists for <paramref name="sourceClosedPostPositionId"/> (not invoiced yet),
        /// or when a credit line already exists for <paramref name="newClosedPostPositionId"/>.
        /// </summary>
        /// <param name="sourceClosedPostPositionId">The previous closed-post-position history row that the invoice is linked to.</param>
        /// <param name="newClosedPostPositionId">The new closed-post-position history row created by the re-close.</param>
        /// <param name="hours">Hours to bill for the new closed row (already derived from new ClosedPostPosition).</param>
        /// <param name="vehicleFeeAmount">Vehicle fee amount to bill for the new closed row.</param>
        /// <param name="equipmentFeeAmount">Equipment fee amount to bill for the new closed row.</param>
        /// <param name="validateOnly">When true, validates totals only (throws HTTP 409 when refund required); no database changes.</param>
        Task ApplyCreditForRecloseAsync(
            Guid sourceClosedPostPositionId,
            Guid newClosedPostPositionId,
            decimal? hours,
            decimal? vehicleFeeAmount,
            decimal? equipmentFeeAmount,
            bool validateOnly = false);
    }
}
