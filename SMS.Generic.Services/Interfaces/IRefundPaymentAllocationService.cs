using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    public interface IRefundPaymentAllocationService : IGenericService<RefundPaymentAllocationDto, RefundPaymentAllocation>
    {
        Task<List<RefundPaymentAllocationDto>> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);

        Task<List<RefundPaymentAllocationDto>> GetByRefundPaymentIdAsync(Guid refundPaymentId, CancellationToken cancellationToken = default);

        Task<List<RefundPaymentAllocationDetailDto>> GetRefundAllocationHistoryByRefundPaymentIdAsync(Guid refundPaymentId, CancellationToken cancellationToken = default);

        Task ValidateRefundAllocateItemsAsync(Guid refundRequesterId, IReadOnlyList<RefundPaymentAllocationItemDto> items, CancellationToken cancellationToken = default);

        Task<decimal> GetTotalNetRefundedByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Net active refund allocation total and active refund numbers from a single allocation load.
        /// </summary>
        Task<(decimal TotalNetRefunded, IReadOnlyList<string> ActiveRefundNumbers)> GetTotalNetRefundedAmountWithRefundNumbersByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Credits (reverses) all active Debit allocations for a refund payment, restoring escrow on linked payments.
        /// </summary>
        Task<int> CreditAllAllocationsByRefundPaymentIdAsync(Guid refundPaymentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Voids (discards) a single refund payment allocation by inserting a Credit entry with reason,
        /// restoring escrow balance on the linked payment.
        /// </summary>
        Task<RefundPaymentAllocationDto> VoidRefundPaymentAllocationAsync(Guid refundPaymentAllocationId, string reason, CancellationToken cancellationToken = default);
    }
}
