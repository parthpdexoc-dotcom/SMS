using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for payment allocation service operations.
    /// </summary>
    public interface IPaymentAllocationService : IGenericService<PaymentAllocationDto, PaymentAllocation>
    {
        /// <summary>
        /// Gets active payment allocations applied to an invoice, joined with active payment details.
        /// </summary>
        Task<List<PaymentInformationDto>> GetActivePaymentHistoryByInvoiceIdAsync(Guid invoiceId,CancellationToken cancellationToken = default);

        /// <summary>
        /// Sums active <see cref="PaymentAllocationDto.AmountApplied"/> for a payment across all invoices.
        /// </summary>
        Task<decimal> GetTotalActiveAmountAppliedByPaymentIdAsync(Guid paymentId,CancellationToken cancellationToken = default);

        /// <summary>
        /// Net active invoice allocation total and active invoice numbers from a single allocation load.
        /// </summary>
        Task<(decimal TotalActiveAmountApplied, IReadOnlyList<string> ActiveInvoiceNumbers)> GetTotalActiveAmountAppliedWithInvoiceNumbersByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Voids all active allocations on the given invoice and payment by inserting credit or discard entries.
        /// </summary>
        /// <returns>The number of allocations voided.</returns>
        Task<int> VoidActiveAllocationsByInvoiceAndPaymentAsync(Guid invoiceId,Guid paymentId,string? reason = null,CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns non-deleted allocations for an invoice and payment (any status).
        /// </summary>
        Task<List<PaymentAllocationDto>> GetByInvoiceAndPaymentAsync(Guid invoiceId,Guid paymentId,CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns non-deleted allocations for a payment (any status).
        /// </summary>
        Task<List<PaymentAllocationDto>> GetByPaymentIdAsync(Guid paymentId,CancellationToken cancellationToken = default);

        /// <summary>
        /// Voids all active allocations for a payment. Returns voided count and distinct affected invoice ids.
        /// </summary>
        Task<(int VoidedCount, IReadOnlyList<Guid> InvoiceIds)> VoidActiveAllocationsByPaymentIdAsync(Guid paymentId,CancellationToken cancellationToken = default);

        /// <summary>
        /// Voids all active allocations for a payment with a discard reason. Returns voided count and distinct affected invoice ids.
        /// </summary>
        Task<(int VoidedCount, IReadOnlyList<Guid> InvoiceIds)> VoidActiveAllocationsByPaymentIdAsync(Guid paymentId,string reason,CancellationToken cancellationToken = default);

        /// <summary>
        /// Voids a single active allocation, recalculates the affected invoice, and returns updated payment balances.
        /// </summary>
        Task<PaymentAllocateResponseDto> VoidPaymentAllocationAsync(Guid paymentAllocationId,string reason,CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates allocation amounts after validation and optionally recalculates affected invoices.
        /// </summary>
        Task<PaymentAllocationUpdateResultDto> UpdatePaymentAllocationAmountAsync(Guid paymentId,IReadOnlyList<PaymentAdjustAllocationItemDto> allocations,decimal? paymentAmount = null,bool validate = true,CancellationToken cancellationToken = default);

        /// <summary>
        /// Inserts credit payment allocations for an invoice refund and recalculates invoice payment totals.
        /// </summary>
        Task<InvoicePaymentSummaryResponseDto> ApplyInvoiceRefundAllocationsAsync(InvoiceRefundAllocationsDto request,CancellationToken cancellationToken = default);
    }
}
