using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Common.Implementations.Paginations;

namespace SMS.Generic.Services.Interfaces
{
    public interface IPaymentService : IGenericService<PaymentDto, Payment>
    {
        /// <summary>
        /// Unapplied requester balance: MAX(0, total payments − total allocated). Returns 0 when requester is null or empty.
        /// </summary>
        Task<decimal> GetAvailableBalanceByRequesterAsync(Guid? requesterId,CancellationToken cancellationToken = default);

        /// <summary>
        /// Active payments for the requester that still have unallocated balance, oldest <see cref="PaymentDto.PaymentDate"/> first.
        /// </summary>
        Task<IReadOnlyList<(PaymentDto Payment, decimal RemainingBalance)>> GetUnallocatedPaymentsOrderedAsync(Guid requesterId,CancellationToken cancellationToken = default);

        /// <summary>
        /// Voids the payment and all active allocations on every linked invoice, then recalculates each affected invoice.
        /// </summary>
        Task<PaymentDto> VoidPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Allocates an existing payment to invoices in list order (full or partial per line).
        /// </summary>
        Task<PaymentAllocateResponseDto> AllocatePaymentAsync(PaymentAllocateDto request,CancellationToken cancellationToken = default);

        /// <summary>
        /// Payment amount, used/unused balances, and invoices that received allocations from this payment.
        /// </summary>
        Task<PaymentInvoicesResponseDto?> GetPaymentDetailAsync(Guid paymentId,PageParam? pagination = null,CancellationToken cancellationToken = default);

        /// <summary>
        /// Payment balances and unpaid requester invoices (not fully paid) for allocation.
        /// </summary>
        Task<PaymentInvoicesResponseDto?> GetPaymentUnpaidInvoicesAsync(Guid paymentId,Guid requesterId,PageParam? pagination = null,CancellationToken cancellationToken = default);

        /// <summary>
        /// Requester payments within a date range with per-payment balances, allocation status, and overall totals.
        /// </summary>
        Task<RequesterPaymentSummaryResponseDto> GetRequesterPaymentHistoryByRequesterIdAndDateRange(Guid requesterId,DateOnly? startDate,DateOnly? endDate,PageParam? pagination = null,CancellationToken cancellationToken = default);

        /// <summary>
        /// Requester payments that still have escrow (unallocated) balance, for recording refunds.
        /// </summary>
        Task<RequesterPaymentSummaryResponseDto> GetRequesterRefundablePaymentsAsync(Guid requesterId, PageParam? pagination = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Payment balances and active allocations for review before adjusting (same allocation payload as adjust conflict).
        /// </summary>
        Task<PaymentReviewAllocationsResponseDto?> GetPaymentReviewAllocationsAsync(Guid paymentId,PageParam? pagination = null,CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates allocation amounts, recalculates affected invoices, then updates the payment amount.
        /// Throws <see cref="WebApiException"/> with HTTP 409 when the payment amount conflicts with existing allocations.
        /// </summary>
        Task<PaymentAdjustAllocationsResultDto> AdjustPaymentAmountAgainstAllocationsAsync(PaymentAdjustAllocationsDto request,CancellationToken cancellationToken = default);

        /// <summary>
        /// Loads a payment with its active allocations and invoice display fields.
        /// </summary>
        Task<(PaymentDto Payment, List<PaymentInvoiceAllocationDto> Allocations)?> GetPaymentWithActiveAllocationsAsync(Guid paymentId,CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns an active (non-deleted) payment or throws when not found.
        /// </summary>
        Task<PaymentDto> GetActiveByIdAsync(Guid paymentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Soft-deletes a payment: blocks when active allocations exist, posts ledger reversal, requires reason.
        /// </summary>
        Task<bool> DeleteAsync(Guid paymentId, string reason, CancellationToken cancellationToken = default);
    }
}
