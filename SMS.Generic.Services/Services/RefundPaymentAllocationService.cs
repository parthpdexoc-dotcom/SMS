using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    public class RefundPaymentAllocationService : GenericService<RefundPaymentAllocationDto, RefundPaymentAllocation>, IRefundPaymentAllocationService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public RefundPaymentAllocationService(
            IRefundPaymentAllocationRepository repository,
            IMapper mapper,
            ILogger<RefundPaymentAllocationService> logger,
            ITokenParser tokenParser,
            IServiceScopeFactory scopeFactory) : base(repository, mapper, "RefundPaymentAllocationID", tokenParser)
        {
            _scopeFactory = scopeFactory;
        }

        public override async Task<RefundPaymentAllocationDto> UpsertAsync(RefundPaymentAllocationDto dto, bool isSuperUser = false)
        {
            if (dto.PaymentID == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_PAYMENT_ALLOCATION_PAYMENT_ID), nameof(dto));

            using var scope = _scopeFactory.CreateScope();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
            var paymentAllocationService = scope.ServiceProvider.GetRequiredService<IPaymentAllocationService>();

            var payment = await paymentService.GetActiveByIdAsync(dto.PaymentID).ConfigureAwait(false);

            var invoiceAllocated = await paymentAllocationService.GetTotalActiveAmountAppliedByPaymentIdAsync(dto.PaymentID).ConfigureAwait(false);

            var existingRefunds = await GetByPaymentIdAsync(dto.PaymentID).ConfigureAwait(false);

            if (dto.TransactionType == RefundPaymentAllocationTransactionType.Credit)
                dto.ValidateCredit(existingRefunds.Where(a => a.RefundPaymentID == dto.RefundPaymentID));

            else
            {
                var refundedExcludingCurrent = RefundPaymentAllocationDto.SumNetAmountApplied(existingRefunds.Where(a => a.RefundPaymentAllocationID != dto.RefundPaymentAllocationID));
                RefundPaymentAllocationDto.ValidateAmountForPayment(dto.AmountApplied,payment,invoiceAllocated,refundedExcludingCurrent);
            }

            return await base.UpsertAsync(dto, isSuperUser).ConfigureAwait(false);
        }

        public async Task ValidateRefundAllocateItemsAsync(Guid refundRequesterId, IReadOnlyList<RefundPaymentAllocationItemDto> items, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
            var paymentAllocationService = scope.ServiceProvider.GetRequiredService<IPaymentAllocationService>();

            foreach (var item in items)
            {
                var payment = await paymentService.GetActiveByIdAsync(item.PaymentId, cancellationToken).ConfigureAwait(false);
                var invoiceAllocated = await paymentAllocationService.GetTotalActiveAmountAppliedByPaymentIdAsync(item.PaymentId, cancellationToken).ConfigureAwait(false);
                var existingRefunds = await GetByPaymentIdAsync(item.PaymentId, cancellationToken).ConfigureAwait(false);
                item.ValidateForAllocation(payment,refundRequesterId,invoiceAllocated,RefundPaymentAllocationDto.SumNetAmountApplied(existingRefunds));
            }
        }

        public async Task<List<RefundPaymentAllocationDto>> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            if (paymentId == Guid.Empty)
                return new List<RefundPaymentAllocationDto>();

            Expression<Func<RefundPaymentAllocation, bool>> filter = a =>
                a.PaymentID == paymentId && !a.IsDeleted;

            return (await GetAllAsync(filter, pageParam: null).ConfigureAwait(false)).ToList();
        }

        public async Task<decimal> GetTotalNetRefundedByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            var refunds = await GetByPaymentIdAsync(paymentId, cancellationToken).ConfigureAwait(false);
            return RefundPaymentAllocationDto.SumNetAmountApplied(refunds);
        }

        /// <inheritdoc />
        public async Task<(decimal TotalNetRefunded, IReadOnlyList<string> ActiveRefundNumbers)> GetTotalNetRefundedAmountWithRefundNumbersByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            if (paymentId == Guid.Empty)
                return (0m, []);

            var allocations = await GetByPaymentIdAsync(paymentId, cancellationToken).ConfigureAwait(false);
            var totalNetRefunded = RefundPaymentAllocationDto.SumNetAmountApplied(allocations);
            if (totalNetRefunded <= 0m)
                return (0m, []);

            var activeRefundPaymentIds = allocations
                .GroupBy(a => a.RefundPaymentID)
                .Where(g => RefundPaymentAllocationDto.SumNetAmountApplied(g) > 0m)
                .Select(g => g.Key);

            using var scope = _scopeFactory.CreateScope();
            var refundPaymentsById = (await scope.ServiceProvider.GetRequiredService<IRefundPaymentService>()
                .GetAllAsync(r => activeRefundPaymentIds.Contains(r.RefundPaymentID))
                .ConfigureAwait(false)).ToDictionary(r => r.RefundPaymentID);

            return (totalNetRefunded, RefundPaymentAllocationDto.GetActiveRefundDisplayNumbers(allocations, refundPaymentsById));
        }

        public async Task<List<RefundPaymentAllocationDto>> GetByRefundPaymentIdAsync(Guid refundPaymentId, CancellationToken cancellationToken = default)
        {
            if (refundPaymentId == Guid.Empty)
                return new List<RefundPaymentAllocationDto>();

            Expression<Func<RefundPaymentAllocation, bool>> filter = a =>
                a.RefundPaymentID == refundPaymentId && !a.IsDeleted;

            return (await GetAllAsync(filter, pageParam: null).ConfigureAwait(false)).ToList();
        }

        public async Task<List<RefundPaymentAllocationDetailDto>> GetRefundAllocationHistoryByRefundPaymentIdAsync(Guid refundPaymentId, CancellationToken cancellationToken = default)
        {
            if (refundPaymentId == Guid.Empty)
                return new List<RefundPaymentAllocationDetailDto>();

            var allocations = await GetByRefundPaymentIdAsync(refundPaymentId, cancellationToken).ConfigureAwait(false);
            var paymentIds = allocations.Select(a => a.PaymentID).Distinct().ToList();

            using var scope = _scopeFactory.CreateScope();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
            var payments = paymentIds.Count > 0
                ? (await paymentService.GetAllAsync(p => paymentIds.Contains(p.PaymentID)).ConfigureAwait(false)).ToList()
                : new List<PaymentDto>();

            return allocations
                .OrderByDescending(a => a.AllocationDate)
                .ThenByDescending(a => a.RefundPaymentAllocationID)
                .Select(allocation =>
                {
                    var payment = payments.FirstOrDefault(p => p.PaymentID == allocation.PaymentID) ?? new PaymentDto();
                    return new RefundPaymentAllocationDetailDto(allocation, payment);
                })
                .ToList();
        }

        /// <inheritdoc />
        public async Task<RefundPaymentAllocationDto> VoidRefundPaymentAllocationAsync(Guid refundPaymentAllocationId, string reason, CancellationToken cancellationToken = default)
        {
            if (refundPaymentAllocationId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REFUND_PAYMENT_ALLOCATION_NOT_FOUND_OR_DELETED), nameof(refundPaymentAllocationId));
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REFUND_PAYMENT_ALLOCATION_DISCARD_REQUIRES_REASON), nameof(reason));

            var allocation = await GetByIdAsync(refundPaymentAllocationId).ConfigureAwait(false);
            if (allocation == null || allocation.IsDeleted)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REFUND_PAYMENT_ALLOCATION_NOT_FOUND_OR_DELETED));

            if (allocation.TransactionType != RefundPaymentAllocationTransactionType.Debit || allocation.AmountApplied <= 0m)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REFUND_PAYMENT_ALLOCATION_ALREADY_VOIDED));

            var allForPayment = await GetByPaymentIdAsync(allocation.PaymentID, cancellationToken).ConfigureAwait(false);
            var netForPayment = RefundPaymentAllocationDto.SumNetAmountApplied(
                allForPayment.Where(a => a.RefundPaymentID == allocation.RefundPaymentID));

            if (netForPayment <= 0m)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REFUND_PAYMENT_ALLOCATION_ALREADY_VOIDED));

            var discardAmount = Math.Min(allocation.AmountApplied, netForPayment);

            var allForRefund = await GetByRefundPaymentIdAsync(allocation.RefundPaymentID, cancellationToken).ConfigureAwait(false);
            var totalNetForRefund = RefundPaymentAllocationDto.SumNetAmountApplied(allForRefund);

            using var scope = _scopeFactory.CreateScope();
            var refundPaymentService = scope.ServiceProvider.GetRequiredService<IRefundPaymentService>();

            if (totalNetForRefund - discardAmount <= 0.001m)
            {
                var refund = await refundPaymentService.GetByIdAsync(allocation.RefundPaymentID).ConfigureAwait(false);
                var refundNumber = refund?.RefundPaymentNumber?.Trim();
                if (string.IsNullOrEmpty(refundNumber))
                    refundNumber = allocation.RefundPaymentID.ToString();

                throw new InvalidOperationException(string.Format(
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REFUND_PAYMENT_ALLOCATION_VOID_WOULD_ZERO_REFUND),
                    refundNumber));
            }

            var dbContext = scope.ServiceProvider.GetRequiredService<SMSDbContext>();

            return await dbContext.ExecuteInTransactionAsync(async () =>
            {
                var discarded = await base.UpsertAsync(new RefundPaymentAllocationDto
                {
                    PaymentID = allocation.PaymentID,
                    RefundPaymentID = allocation.RefundPaymentID,
                    AmountApplied = discardAmount,
                    AllocationDate = DateTime.UtcNow,
                    TransactionType = RefundPaymentAllocationTransactionType.Discard,
                    Reason = reason.Trim(),
                }).ConfigureAwait(false);

                await refundPaymentService
                    .ReduceAmountForReleasedAllocationAsync(allocation.RefundPaymentID, discardAmount, cancellationToken)
                    .ConfigureAwait(false);

                return discarded;
            }, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<int> CreditAllAllocationsByRefundPaymentIdAsync(Guid refundPaymentId, CancellationToken cancellationToken = default)
        {
            if (refundPaymentId == Guid.Empty)
                return 0;

            var allocations = await GetByRefundPaymentIdAsync(refundPaymentId, cancellationToken).ConfigureAwait(false);
            var activePayments = RefundPaymentAllocationDto.GetActivePaymentNets(allocations);

            if (activePayments.Count == 0)
                return 0;

            var creditedCount = 0;
            foreach (var payment in activePayments)
            {
                await base.UpsertAsync(new RefundPaymentAllocationDto
                {
                    PaymentID = payment.PaymentID,
                    RefundPaymentID = refundPaymentId,
                    AmountApplied = payment.NetAmount,
                    AllocationDate = DateTime.UtcNow,
                    TransactionType = RefundPaymentAllocationTransactionType.Credit,
                }).ConfigureAwait(false);

                creditedCount++;
            }

            return creditedCount;
        }
    }
}
