using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    /// <summary>
    /// Service for managing payment allocation operations using the generic service pattern.
    /// </summary>
    public class PaymentAllocationService : GenericService<PaymentAllocationDto, PaymentAllocation>, IPaymentAllocationService
    {
        private readonly IPaymentAllocationRepository _repository;
        private readonly IMapper _mapper;
        private readonly SMSDbContext _dbContext;
        private readonly IServiceScopeFactory _scopeFactory;

        public PaymentAllocationService(
            IPaymentAllocationRepository repository,
            IMapper mapper,
            ILogger<PaymentAllocationService> logger,
            ITokenParser tokenParser,
            SMSDbContext dbContext,
            IServiceScopeFactory scopeFactory) : base(repository, mapper, "PaymentAllocationID", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _dbContext = dbContext;
            _scopeFactory = scopeFactory;
        }

        /// <inheritdoc />
        public override async Task<PaymentAllocationDto> UpsertAsync(PaymentAllocationDto dto, bool isSuperUser = false)
        {
            if (dto.TransactionType == PaymentAllocationTransactionType.Credit
                || dto.TransactionType == PaymentAllocationTransactionType.Discard)
            {
                var existing = await GetByInvoiceAndPaymentAsync(dto.InvoicesID, dto.PaymentID).ConfigureAwait(false);
                dto.ValidateCredit(existing);
            }

            return await base.UpsertAsync(dto, isSuperUser).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<List<PaymentInformationDto>> GetActivePaymentHistoryByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default)
        {
            Expression<Func<PaymentAllocation, bool>> filter = pa =>
                pa.InvoicesID == invoiceId
                && !pa.IsDeleted
                && pa.Payment != null
                && !pa.Payment.IsDeleted;

            Expression<Func<PaymentAllocation, object>>[] includes =
            {
                pa => pa.Payment!,
                pa => pa.Payment!.PaymentTypeMaster!
            };

            var entities = (await _repository.GetAllAsync(filter, includes, pageParam: null).ConfigureAwait(false)).ToList();

            return _mapper.Map<List<PaymentInformationDto>>(entities)
                .OrderByDescending(a => a.AllocationDate)
                .ToList();
        }

        /// <inheritdoc />
        public async Task<decimal> GetTotalActiveAmountAppliedByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            var (totalActiveAmountApplied, _) = await GetTotalActiveAmountAppliedWithInvoiceNumbersByPaymentIdAsync(paymentId, cancellationToken).ConfigureAwait(false);
            return totalActiveAmountApplied;
        }

        /// <inheritdoc />
        public async Task<(decimal TotalActiveAmountApplied, IReadOnlyList<string> ActiveInvoiceNumbers)> GetTotalActiveAmountAppliedWithInvoiceNumbersByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            if (paymentId == Guid.Empty)
                return (0m, []);

            var allocations = await GetByPaymentIdAsync(paymentId, cancellationToken).ConfigureAwait(false);
            var totalActiveAmountApplied = PaymentAllocationDto.SumNetAmountApplied(allocations);
            if (totalActiveAmountApplied <= 0m)
                return (0m, []);

            var activeInvoiceIds = allocations
                .GroupBy(a => a.InvoicesID)
                .Where(g => PaymentAllocationDto.SumNetAmountApplied(g) > 0m)
                .Select(g => g.Key);

            using var scope = _scopeFactory.CreateScope();
            var invoicesById = (await scope.ServiceProvider.GetRequiredService<IInvoiceService>()
                .GetAllAsync(i => activeInvoiceIds.Contains(i.InvoicesID))
                .ConfigureAwait(false)).ToDictionary(i => i.InvoicesID);

            return (totalActiveAmountApplied, PaymentAllocationDto.GetActiveInvoiceDisplayNumbers(allocations, invoicesById));
        }

        /// <inheritdoc />
        public async Task<List<PaymentAllocationDto>> GetByInvoiceAndPaymentAsync(Guid invoiceId, Guid paymentId, CancellationToken cancellationToken = default)
        {
            if (invoiceId == Guid.Empty || paymentId == Guid.Empty)
                return new List<PaymentAllocationDto>();

            Expression<Func<PaymentAllocation, bool>> filter = pa =>
                pa.InvoicesID == invoiceId
                && pa.PaymentID == paymentId
                && !pa.IsDeleted;

            return (await GetAllAsync(filter, pageParam: null).ConfigureAwait(false)).ToList();
        }

        /// <inheritdoc />
        public async Task<int> VoidActiveAllocationsByInvoiceAndPaymentAsync(Guid invoiceId, Guid paymentId, string? reason = null, CancellationToken cancellationToken = default)
        {
            if (invoiceId == Guid.Empty || paymentId == Guid.Empty)
                return 0;

            var useDiscard = !string.IsNullOrWhiteSpace(reason);
            var credited = await UpsertAsync(new PaymentAllocationDto
            {
                InvoicesID = invoiceId,
                PaymentID = paymentId,
                AllocationDate = DateTime.UtcNow,
                TransactionType = useDiscard ? PaymentAllocationTransactionType.Discard : PaymentAllocationTransactionType.Credit,
                Reason = useDiscard ? reason!.Trim() : null,
            }).ConfigureAwait(false);

            return credited.AmountApplied > 0m ? 1 : 0;
        }

        /// <inheritdoc />
        public async Task<List<PaymentAllocationDto>> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            if (paymentId == Guid.Empty)
                return new List<PaymentAllocationDto>();

            Expression<Func<PaymentAllocation, bool>> filter = pa =>
                pa.PaymentID == paymentId && !pa.IsDeleted;

            return (await GetAllAsync(filter, pageParam: null).ConfigureAwait(false)).ToList();
        }

        /// <inheritdoc />
        public async Task<(int VoidedCount, IReadOnlyList<Guid> InvoiceIds)> VoidActiveAllocationsByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            return await VoidActiveAllocationsByPaymentIdCoreAsync(paymentId, null, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<(int VoidedCount, IReadOnlyList<Guid> InvoiceIds)> VoidActiveAllocationsByPaymentIdAsync(Guid paymentId, string reason, CancellationToken cancellationToken = default)
        {
            return await VoidActiveAllocationsByPaymentIdCoreAsync(paymentId, reason, cancellationToken).ConfigureAwait(false);
        }

        private async Task<(int VoidedCount, IReadOnlyList<Guid> InvoiceIds)> VoidActiveAllocationsByPaymentIdCoreAsync(Guid paymentId, string? reason, CancellationToken cancellationToken)
        {
            if (paymentId == Guid.Empty)
                return (0, Array.Empty<Guid>());

            var allocations = await GetByPaymentIdAsync(paymentId, cancellationToken).ConfigureAwait(false);
            var invoiceIds = allocations.Select(a => a.InvoicesID).Distinct().ToList();
            var creditedCount = 0;
            var affectedInvoiceIds = new List<Guid>();

            foreach (var invoiceId in invoiceIds)
            {
                var voidedCount = await VoidActiveAllocationsByInvoiceAndPaymentAsync(invoiceId, paymentId, reason, cancellationToken).ConfigureAwait(false);

                if (voidedCount <= 0)
                    continue;

                creditedCount++;
                affectedInvoiceIds.Add(invoiceId);
            }

            return (creditedCount, affectedInvoiceIds);
        }

        /// <inheritdoc />
        public async Task<PaymentAllocateResponseDto> VoidPaymentAllocationAsync(Guid paymentAllocationId, string reason, CancellationToken cancellationToken = default)
        {
            if (paymentAllocationId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVALID_PAYMENT_ALLOCATION_AMOUNT_APPLIED), nameof(paymentAllocationId));
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_ALLOCATION_DISCARD_REQUIRES_REASON), nameof(reason));

            var allocation = await GetByIdAsync(paymentAllocationId).ConfigureAwait(false);
            if (allocation == null || allocation.IsDeleted)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_ALLOCATION_NOT_FOUND_FOR_INVOICE));

            var paymentId = allocation.PaymentID;
            var invoiceId = allocation.InvoicesID;

            await _dbContext.ExecuteInTransactionAsync(async () =>
            {
                var voidedCount = await VoidActiveAllocationsByInvoiceAndPaymentAsync(invoiceId, paymentId, reason.Trim(), cancellationToken).ConfigureAwait(false);

                if (voidedCount == 0)
                    throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_ALLOCATION_ALREADY_VOIDED));
            }, cancellationToken).ConfigureAwait(false);

            using var invoiceScope = _scopeFactory.CreateScope();
            var invoiceService = invoiceScope.ServiceProvider.GetRequiredService<IInvoiceService>();
            await invoiceService.RecalculateInvoicePaymentTotalsAsync(invoiceId, cancellationToken).ConfigureAwait(false);

            using var balanceScope = _scopeFactory.CreateScope();
            var paymentService = balanceScope.ServiceProvider.GetRequiredService<IPaymentService>();

            var payment = await paymentService.GetByIdAsync(paymentId).ConfigureAwait(false);
            if (payment == null || payment.IsDeleted)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_NOT_FOUND_OR_DELETED));

            var appliedAmount = await GetTotalActiveAmountAppliedByPaymentIdAsync(paymentId, cancellationToken).ConfigureAwait(false);
            var refundService = balanceScope.ServiceProvider.GetRequiredService<IRefundPaymentAllocationService>();
            var refundedAmount = await refundService
                .GetTotalNetRefundedByPaymentIdAsync(paymentId, cancellationToken)
                .ConfigureAwait(false);
            return new PaymentAllocateResponseDto(payment, appliedAmount, new[] { invoiceId }, refundedAmount);
        }

        /// <inheritdoc />
        public async Task<PaymentAllocationUpdateResultDto> UpdatePaymentAllocationAmountAsync(Guid paymentId, IReadOnlyList<PaymentAdjustAllocationItemDto> allocations, decimal? paymentAmount = null, bool validate = true, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(allocations);
            if (paymentId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_PAYMENT_ALLOCATION_PAYMENT_ID), nameof(paymentId));
            if (allocations.Count == 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVALID_PAYMENT_ALLOCATION_AMOUNT_APPLIED), nameof(allocations));

            foreach (var request in allocations)
            {
                if (request.PaymentAllocationId == Guid.Empty)
                    throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVALID_PAYMENT_ALLOCATION_AMOUNT_APPLIED), nameof(allocations));
            }

            using var scope = _scopeFactory.CreateScope();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
            var invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();
            var loaded = await paymentService.GetPaymentWithActiveAllocationsAsync(paymentId, cancellationToken).ConfigureAwait(false);
            PaymentDto.EnsureExists(loaded?.Payment);

            var invoiceAllocationsByInvoiceId = new Dictionary<Guid, IEnumerable<PaymentAllocationDto>>();
            foreach (var invoiceId in allocations.Select(a => a.InvoiceId).Distinct())
                invoiceAllocationsByInvoiceId[invoiceId] = await GetActivePaymentHistoryByInvoiceIdAsync(invoiceId, cancellationToken).ConfigureAwait(false);

            var payment = loaded!.Value.Payment;
            var paymentAllocations = loaded.Value.Allocations;
            var (apply, skipped, messages) = PaymentAllocationDto.ResolveAllocationEdits(paymentId, allocations, paymentAllocations, invoiceAllocationsByInvoiceId);
            if (validate && apply.Count > 0)
            {
                var refundService = scope.ServiceProvider.GetRequiredService<IRefundPaymentAllocationService>();
                var refundedAmount = await refundService
                    .GetTotalNetRefundedByPaymentIdAsync(paymentId, cancellationToken)
                    .ConfigureAwait(false);
                PaymentAllocationDto.ValidatePaymentTotalForEdits(
                    payment,
                    paymentAllocations,
                    apply,
                    paymentAmount ?? payment.Amount,
                    refundedAmount);
            }

            var result = new PaymentAllocationUpdateResultDto { SkippedAllocations = skipped, Messages = messages };
            var invoiceIds = new HashSet<Guid>();
            foreach (var request in apply)
            {
                var allocationDto = await GetByIdAsync(request.PaymentAllocationId).ConfigureAwait(false);
                if (allocationDto == null || allocationDto.IsDeleted)
                    throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_ALLOCATION_NOT_FOUND_FOR_INVOICE));

                allocationDto.AmountApplied = request.Amount;
                result.Updated.Add(await UpsertAsync(allocationDto).ConfigureAwait(false));
                invoiceIds.Add(request.InvoiceId);
            }

            foreach (var invoiceId in invoiceIds)
                await invoiceService.RecalculateInvoicePaymentTotalsAsync(invoiceId, cancellationToken).ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc />
        public async Task<InvoicePaymentSummaryResponseDto> ApplyInvoiceRefundAllocationsAsync(
            InvoiceRefundAllocationsDto request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            if (request.InvoiceId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_PAYMENT_ALLOCATION_INVOICE_ID), nameof(request));
            if (request.RequiredRefundAmount <= 0m)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVALID_PAYMENT_ALLOCATION_AMOUNT_APPLIED), nameof(request));
            if (request.Payments == null || request.Payments.Count == 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVALID_PAYMENT_ALLOCATION_AMOUNT_APPLIED), nameof(request));

            var items = request.Payments
                .Where(p => p.PaymentId != Guid.Empty && p.Amount > 0m)
                .GroupBy(p => p.PaymentId)
                .Select(g => new InvoiceRefundAllocationItemDto { PaymentId = g.Key, Amount = g.Sum(x => x.Amount) })
                .ToList();

            var totalRefund = items.Sum(i => i.Amount);
            if (Math.Abs(totalRefund - request.RequiredRefundAmount) > 0.001m)
                throw new InvalidOperationException(string.Format(
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVOICE_REFUND_TOTAL_MISMATCH),
                    totalRefund,
                    request.RequiredRefundAmount));

            using var scope = _scopeFactory.CreateScope();
            var invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();
            var invoice = await invoiceService.GetByIdAsync(request.InvoiceId).ConfigureAwait(false);
            if (invoice == null || invoice.IsDeleted)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVOICE_NOT_FOUND_OR_DELETED));

            var allocationDate = DateTime.UtcNow;
            await _dbContext.ExecuteInTransactionAsync(async () =>
            {
                foreach (var item in items)
                {
                    await UpsertAsync(new PaymentAllocationDto
                    {
                        InvoicesID = request.InvoiceId,
                        PaymentID = item.PaymentId,
                        AmountApplied = item.Amount,
                        AllocationDate = allocationDate,
                        TransactionType = PaymentAllocationTransactionType.Credit
                    }).ConfigureAwait(false);
                }
            }, cancellationToken).ConfigureAwait(false);

            await invoiceService.RecalculateInvoicePaymentTotalsAsync(request.InvoiceId, cancellationToken).ConfigureAwait(false);

            return await invoiceService.GetInvoicePaymentSummaryAsync(request.InvoiceId, cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVOICE_NOT_FOUND_OR_DELETED));
        }
    }
}
