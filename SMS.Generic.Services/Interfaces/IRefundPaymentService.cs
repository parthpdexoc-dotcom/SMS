using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    public interface IRefundPaymentService : IGenericService<RefundPaymentDto, RefundPayment>
    {
        Task<RefundPaymentAllocateResponseDto> AllocateRefundPaymentAsync(
            RefundPaymentAllocateDto request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Requester refund payments within a date range with overall totals.
        /// </summary>
        Task<RequesterRefundPaymentSummaryResponseDto> GetRequesterRefundHistoryByRequesterIdAndDateRange(
            Guid requesterId,
            DateOnly? startDate,
            DateOnly? endDate,
            PageParam? pagination = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Refund payment header and active allocations for the detail view.
        /// </summary>
        Task<RefundPaymentAllocationsResponseDto?> GetRefundPaymentAllocationsAsync(
            Guid refundPaymentId,
            PageParam? pagination = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Refund payment header and full allocation history for the detail view.
        /// </summary>
        Task<RefundPaymentAllocationHistoryResponseDto?> GetRefundPaymentAllocationHistoryAsync(
            Guid refundPaymentId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Soft-deletes a refund: credits active allocations, posts ledger reversal, requires reason.
        /// </summary>
        Task<bool> DeleteAsync(Guid refundPaymentId, string reason, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reduces refund header amount when allocation net is released (discard/credit).
        /// Posts a requester-ledger credit for the reduction via <see cref="UpsertAsync"/>.
        /// </summary>
        Task<RefundPaymentDto> ReduceAmountForReleasedAllocationAsync(
            Guid refundPaymentId,
            decimal releasedAmount,
            CancellationToken cancellationToken = default);
    }
}
