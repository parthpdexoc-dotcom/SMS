using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    public class RefundPaymentService : GenericService<RefundPaymentDto, RefundPayment>, IRefundPaymentService
    {
        private readonly IRefundPaymentRepository _repository;
        private readonly IRefundPaymentAllocationService _refundPaymentAllocationService;
        private readonly IDepartmentInvoiceConfigService _departmentInvoiceConfigService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ITokenParser _tokenParser;
        private readonly SMSDbContext _dbContext;
        private readonly IRequesterLedgerService _requesterLedgerService;
        private readonly IPaymentTypeService _paymentTypeService;

        public RefundPaymentService(
            IRefundPaymentRepository repository,
            IRefundPaymentAllocationService refundPaymentAllocationService,
            IDepartmentInvoiceConfigService departmentInvoiceConfigService,
            IServiceScopeFactory scopeFactory,
            IMapper mapper,
            ILogger<RefundPaymentService> logger,
            ITokenParser tokenParser,
            SMSDbContext dbContext,
            IRequesterLedgerService requesterLedgerService,
            IPaymentTypeService paymentTypeService) : base(repository, mapper, "RefundPaymentID", tokenParser)
        {
            _repository = repository;
            _refundPaymentAllocationService = refundPaymentAllocationService;
            _departmentInvoiceConfigService = departmentInvoiceConfigService;
            _scopeFactory = scopeFactory;
            _tokenParser = tokenParser;
            _dbContext = dbContext;
            _requesterLedgerService = requesterLedgerService;
            _paymentTypeService = paymentTypeService;
        }

        public override async Task<RefundPaymentDto> UpsertAsync(RefundPaymentDto dto, bool isSuperUser = false)
        {
            decimal previousAmount = 0m;

            if (dto.RefundPaymentID == Guid.Empty)
            {
                var departmentId = dto.DepartmentID != Guid.Empty
                    ? dto.DepartmentID
                    : (_tokenParser.GetDepartmentID() ?? Guid.Empty);

                var fiscalYear = await _departmentInvoiceConfigService.GetFiscalYearForDepartmentAsync(departmentId).ConfigureAwait(false);
                dto.RefundPaymentNumber = await _repository.GetNextRefundPaymentNumberForDepartmentAndYearAsync(departmentId, fiscalYear).ConfigureAwait(false);
            }
            else if (await GetByIdAsync(dto.RefundPaymentID).ConfigureAwait(false) is { } existingRefundPayment)
            {
                previousAmount = existingRefundPayment.Amount;
                dto.RefundPaymentNumber = existingRefundPayment.RefundPaymentNumber;
            }

            return await _dbContext.ExecuteInTransactionAsync(async () =>
            {
                var saved = await base.UpsertAsync(dto, isSuperUser).ConfigureAwait(false);

                var refundAmountChange = saved.Amount - previousAmount;
                if (refundAmountChange != 0m && saved.RequesterID != Guid.Empty && saved.DepartmentID != Guid.Empty)
                {
                    var ledgerEntry = new RequesterLedgerDto(
                        saved.RequesterID,
                        saved.DepartmentID,
                        Math.Abs(refundAmountChange),
                        refundAmountChange > 0m ? RequesterLedgerTransactionType.Debit : RequesterLedgerTransactionType.Credit,
                        RequesterLedgerSourceType.Refund,
                        saved.RefundPaymentID,
                        saved.RefundPaymentNumber,
                        RequesterLedgerDto.TransactionDateTimeFrom(saved));

                    if (saved.PaymentTypeID is Guid paymentTypeId && paymentTypeId != Guid.Empty)
                    {
                        ledgerEntry.PaymentTypeName = await _paymentTypeService
                            .GetNameByIdAsync(paymentTypeId)
                            .ConfigureAwait(false);
                    }

                    await _requesterLedgerService.AddTransactionOfRequesterLedgerAsync(ledgerEntry).ConfigureAwait(false);
                }

                return saved;
            }).ConfigureAwait(false);
        }

        public override Task<bool> DeleteAsync<TKey>(TKey id) =>
            throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REFUND_PAYMENT_DELETE_REQUIRES_REASON), nameof(id));

        public async Task<bool> DeleteAsync(Guid refundPaymentId, string reason, CancellationToken cancellationToken = default)
        {
            if (refundPaymentId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.ID_CANNOT_BE_EMPTY), nameof(refundPaymentId));
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REFUND_PAYMENT_DELETE_REQUIRES_REASON), nameof(reason));

            var existing = await GetByIdAsync(refundPaymentId).ConfigureAwait(false);
            if (existing == null || existing.IsDeleted)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REFUND_PAYMENT_NOT_FOUND_OR_DELETED));

            return await _dbContext.ExecuteInTransactionAsync(async () =>
            {
                await _refundPaymentAllocationService
                    .CreditAllAllocationsByRefundPaymentIdAsync(refundPaymentId, cancellationToken)
                    .ConfigureAwait(false);

                if (existing.Amount > 0m && existing.RequesterID != Guid.Empty && existing.DepartmentID != Guid.Empty)
                {
                    var ledgerEntry = new RequesterLedgerDto(
                        existing.RequesterID,
                        existing.DepartmentID,
                        existing.Amount,
                        RequesterLedgerTransactionType.Credit,
                        RequesterLedgerSourceType.Refund,
                        existing.RefundPaymentID,
                        existing.RefundPaymentNumber,
                        RequesterLedgerDto.TransactionDateTimeForDelete());

                    if (existing.PaymentTypeID is Guid paymentTypeId && paymentTypeId != Guid.Empty)
                    {
                        ledgerEntry.PaymentTypeName = await _paymentTypeService
                            .GetNameByIdAsync(paymentTypeId, cancellationToken)
                            .ConfigureAwait(false);
                    }

                    await _requesterLedgerService.AddTransactionOfRequesterLedgerAsync(ledgerEntry).ConfigureAwait(false);
                }

                existing.Reason = reason.Trim();
                PopulateSoftDeleteFields(existing);
                await base.UpsertAsync(existing).ConfigureAwait(false);
                return true;
            }, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<RefundPaymentDto> ReduceAmountForReleasedAllocationAsync(Guid refundPaymentId,decimal releasedAmount,CancellationToken cancellationToken = default)
        {
            if (refundPaymentId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.ID_CANNOT_BE_EMPTY), nameof(refundPaymentId));
            if (releasedAmount <= 0m)
                return await GetByIdAsync(refundPaymentId).ConfigureAwait(false)
                    ?? throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REFUND_PAYMENT_NOT_FOUND_OR_DELETED));

            cancellationToken.ThrowIfCancellationRequested();

            var existing = await GetByIdAsync(refundPaymentId).ConfigureAwait(false);
            if (existing == null || existing.IsDeleted)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REFUND_PAYMENT_NOT_FOUND_OR_DELETED));

            if (releasedAmount > existing.Amount + 0.001m)
                throw new InvalidOperationException(string.Format(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REFUND_PAYMENT_RELEASE_EXCEEDS_AMOUNT),releasedAmount,existing.Amount));

            var newAmount = existing.Amount - releasedAmount;
            if (newAmount < 0m)
                newAmount = 0m;

            if (Math.Abs(newAmount - existing.Amount) <= 0.001m)
                return existing;

            existing.Amount = newAmount;
            return await UpsertAsync(existing).ConfigureAwait(false);
        }

        public async Task<RefundPaymentAllocateResponseDto> AllocateRefundPaymentAsync(RefundPaymentAllocateDto request,CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var refundPayment = request.RefundPayment;
            var items = request.Validate();

            await _refundPaymentAllocationService.ValidateRefundAllocateItemsAsync(refundPayment.RequesterID, items, cancellationToken).ConfigureAwait(false);

            var allocationDate = DateTime.UtcNow;
            return await _dbContext.ExecuteInTransactionAsync(async () =>
            {
                var savedRefundPayment = await UpsertAsync(refundPayment).ConfigureAwait(false);
                var savedAllocations = new List<RefundPaymentAllocationDto>();

                foreach (var item in items)
                {
                    savedAllocations.Add(await _refundPaymentAllocationService.UpsertAsync(new RefundPaymentAllocationDto
                    {
                        PaymentID = item.PaymentId,
                        RefundPaymentID = savedRefundPayment.RefundPaymentID,
                        AmountApplied = item.Amount,
                        AllocationDate = allocationDate,
                        TransactionType = RefundPaymentAllocationTransactionType.Debit
                    }).ConfigureAwait(false));
                }

                return new RefundPaymentAllocateResponseDto
                {
                    RefundPayment = savedRefundPayment,
                    Allocations = savedAllocations
                };
            }, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<RequesterRefundPaymentSummaryResponseDto> GetRequesterRefundHistoryByRequesterIdAndDateRange(Guid requesterId,DateOnly? startDate,DateOnly? endDate,PageParam? pagination = null,CancellationToken cancellationToken = default)
        {
            if (requesterId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.ID_CANNOT_BE_EMPTY), nameof(requesterId));

            if (startDate.HasValue && endDate.HasValue && endDate.Value < startDate.Value)
                throw new ArgumentException(
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.END_DATE_CANNOT_BE_LESS_THAN_START_DATE),
                    nameof(endDate));

            DateTime? start = startDate?.ToDateTime(TimeOnly.MinValue);
            DateTime? endExclusive = endDate?.AddDays(1).ToDateTime(TimeOnly.MinValue);

            Expression<Func<RefundPayment, bool>> filter = r =>
                r.RequesterID == requesterId
                && !r.IsDeleted
                && (start == null || r.PaymentDate >= start)
                && (endExclusive == null || r.PaymentDate < endExclusive);

            var page = pagination ?? new PageParam();
            if (string.IsNullOrWhiteSpace(page.SortBy))
            {
                page.SortBy = nameof(RefundPayment.PaymentDate);
                page.SortDirection = "desc";
            }

            var paged = await GetAllPagedAsync(
                filter,
                page,
                new Expression<Func<RefundPayment, object>>[] { r => r.PaymentTypeMaster! }).ConfigureAwait(false);
            var rows = paged.Data.Select(r => new RequesterRefundPaymentRowDto(r)).ToList();
            var totalAmount = (await GetAllAsync(
                filter,
                new Expression<Func<RefundPayment, object>>[] { r => r.PaymentTypeMaster! },
                pageParam: null).ConfigureAwait(false)).Sum(r => r.Amount);

            return new RequesterRefundPaymentSummaryResponseDto(
                requesterId,
                totalAmount,
                new PagedList<RequesterRefundPaymentRowDto>(rows, page, paged.TotalRecords));
        }

        /// <inheritdoc />
        public async Task<RefundPaymentAllocationsResponseDto?> GetRefundPaymentAllocationsAsync(Guid refundPaymentId,PageParam? pagination = null,CancellationToken cancellationToken = default)
        {
            if (refundPaymentId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.ID_CANNOT_BE_EMPTY), nameof(refundPaymentId));

            var refund = await GetByIdAsync(refundPaymentId).ConfigureAwait(false);
            if (refund == null)
                return null;

            var allocations = await _refundPaymentAllocationService.GetByRefundPaymentIdAsync(refundPaymentId, cancellationToken).ConfigureAwait(false);

            using var scope = _scopeFactory.CreateScope();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

            // Per payment: only include debit rows when that payment's net applied amount is > 0.
            var paymentIdsWithNet = RefundPaymentAllocationDto.GetActivePaymentNets(allocations)
                .Select(x => x.PaymentID)
                .ToHashSet();

            var activeDebits = allocations
                .Where(a => a.TransactionType == RefundPaymentAllocationTransactionType.Debit && a.AmountApplied > 0m)
                .Where(a => paymentIdsWithNet.Contains(a.PaymentID))
                .OrderByDescending(a => a.AllocationDate)
                .ThenByDescending(a => a.RefundPaymentAllocationID)
                .ToList();

            var paymentIds = activeDebits.Select(a => a.PaymentID).Distinct().ToList();
            var payments = paymentIds.Count > 0
                ? (await paymentService.GetAllAsync(p => paymentIds.Contains(p.PaymentID)).ConfigureAwait(false)).ToList()
                : new List<PaymentDto>();

            var detailRows = activeDebits.Select(debit =>
            {
                var payment = payments.FirstOrDefault(p => p.PaymentID == debit.PaymentID) ?? new PaymentDto();
                return new RefundPaymentAllocationDetailDto(debit, payment);
            }).ToList();

            var page = pagination ?? new PageParam();
            var pageNum = page.Page ?? 1;
            var pageSize = page.GetPageSize() ?? PageParam.MinPageSize;
            var pagedRows = detailRows.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();

            return new RefundPaymentAllocationsResponseDto(refund, new PagedList<RefundPaymentAllocationDetailDto>(pagedRows, page, detailRows.Count));
        }

        /// <inheritdoc />
        public async Task<RefundPaymentAllocationHistoryResponseDto?> GetRefundPaymentAllocationHistoryAsync(Guid refundPaymentId, CancellationToken cancellationToken = default)
        {
            if (refundPaymentId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.ID_CANNOT_BE_EMPTY), nameof(refundPaymentId));

            var refund = await GetByIdAsync(refundPaymentId).ConfigureAwait(false);
            if (refund == null)
                return null;

            var allocationHistory = await _refundPaymentAllocationService
                .GetRefundAllocationHistoryByRefundPaymentIdAsync(refundPaymentId, cancellationToken)
                .ConfigureAwait(false);

            return new RefundPaymentAllocationHistoryResponseDto(refund, allocationHistory);
        }
    }
}
