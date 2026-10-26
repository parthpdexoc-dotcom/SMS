using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Common;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Common.Implementations.Paginations;

namespace SMS.Generic.Services.Services
{
    public class PaymentService : GenericService<PaymentDto, Payment>, IPaymentService
    {
        private readonly IPaymentRepository _repository;
        private readonly IPaymentAllocationService _paymentAllocationService;
        private readonly IRefundPaymentAllocationService _refundPaymentAllocationService;
        private readonly SMSDbContext _dbContext;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IDepartmentInvoiceConfigService _departmentInvoiceConfigService;
        private readonly ITokenParser _tokenParser;
        private readonly IMapper _mapper;
        private readonly IRequesterLedgerService _requesterLedgerService;
        private readonly IPaymentTypeService _paymentTypeService;

        public PaymentService(
            IPaymentRepository repository,
            IPaymentAllocationService paymentAllocationService,
            IRefundPaymentAllocationService refundPaymentAllocationService,
            IDepartmentInvoiceConfigService departmentInvoiceConfigService,
            IMapper mapper,
            ILogger<PaymentService> logger,
            ITokenParser tokenParser,
            SMSDbContext dbContext,
            IServiceScopeFactory scopeFactory,
            IRequesterLedgerService requesterLedgerService,
            IPaymentTypeService paymentTypeService) : base(repository, mapper, "PaymentID", tokenParser)
        {
            _repository = repository;
            _paymentAllocationService = paymentAllocationService;
            _refundPaymentAllocationService = refundPaymentAllocationService;
            _departmentInvoiceConfigService = departmentInvoiceConfigService;
            _tokenParser = tokenParser;
            _dbContext = dbContext;
            _scopeFactory = scopeFactory;
            _mapper = mapper;
            _requesterLedgerService = requesterLedgerService;
            _paymentTypeService = paymentTypeService;
        }

        private async Task<decimal> GetNetRefundedAmountByPaymentIdAsync(Guid paymentId,CancellationToken cancellationToken = default) =>
            await _refundPaymentAllocationService.GetTotalNetRefundedByPaymentIdAsync(paymentId, cancellationToken).ConfigureAwait(false);

        private async Task<(decimal AppliedAmount, decimal RefundedAmount)> GetPaymentUsageTotalsAsync(Guid paymentId,CancellationToken cancellationToken = default)
        {
            var appliedAmount = await _paymentAllocationService.GetTotalActiveAmountAppliedByPaymentIdAsync(paymentId, cancellationToken).ConfigureAwait(false);
            var refundedAmount = await GetNetRefundedAmountByPaymentIdAsync(paymentId, cancellationToken).ConfigureAwait(false);
            return (appliedAmount, refundedAmount);
        }

        /// <inheritdoc />
        public override async Task<PaymentDto> UpsertAsync(PaymentDto dto, bool isSuperUser = false)
        {
            decimal previousAmount = 0m;

            if (dto.PaymentID == Guid.Empty)
            {
                var departmentId = dto.DepartmentID != Guid.Empty
                    ? dto.DepartmentID
                    : (_tokenParser.GetDepartmentID() ?? Guid.Empty);

                var fiscalYear = await _departmentInvoiceConfigService
                    .GetFiscalYearForDepartmentAsync(departmentId)
                    .ConfigureAwait(false);

                dto.PaymentNumber = await _repository
                    .GetNextPaymentNumberForDepartmentAndYearAsync(departmentId, fiscalYear)
                    .ConfigureAwait(false);
            }
            else if (await GetByIdAsync(dto.PaymentID).ConfigureAwait(false) is { } existingPayment)
            {
                previousAmount = existingPayment.Amount;
                dto.PaymentNumber = existingPayment.PaymentNumber;

                var totalAllocated = await _paymentAllocationService
                    .GetTotalActiveAmountAppliedByPaymentIdAsync(dto.PaymentID)
                    .ConfigureAwait(false);
                var totalRefunded = await GetNetRefundedAmountByPaymentIdAsync(dto.PaymentID).ConfigureAwait(false);

                if (dto.Amount < totalAllocated + totalRefunded)
                {
                    if (dto.Amount < totalRefunded)
                    {
                        throw new WebApiException(
                            HttpStatusCode.BadRequest,
                            ((int)HttpStatusCode.BadRequest).ToString(),
                            string.Format(
                                ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_AMOUNT_LESS_THAN_REFUNDED),
                                totalRefunded));
                    }

                    if (dto.Amount < totalAllocated)
                    {
                        throw new WebApiException(
                            HttpStatusCode.Conflict,
                            ((int)HttpStatusCode.Conflict).ToString(),
                            ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_AMOUNT_REQUIRES_ALLOCATION_ADJUSTMENT));
                    }

                    var minimumAmount = totalAllocated + totalRefunded;
                    throw new WebApiException(
                        HttpStatusCode.BadRequest,
                        ((int)HttpStatusCode.BadRequest).ToString(),
                        string.Format(
                            ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_AMOUNT_LESS_THAN_APPLIED_AND_REFUNDED),
                            minimumAmount,
                            totalAllocated,
                            totalRefunded));
                }
            }

            return await _dbContext.ExecuteInTransactionAsync(async () =>
            {
                var saved = await base.UpsertAsync(dto, isSuperUser).ConfigureAwait(false);

                var paymentAmountChange = saved.Amount - previousAmount;
                if (paymentAmountChange != 0m && saved.RequesterID != Guid.Empty && saved.DepartmentID != Guid.Empty)
                {
                    var ledgerEntry = new RequesterLedgerDto(
                        saved.RequesterID,
                        saved.DepartmentID,
                        Math.Abs(paymentAmountChange),
                        paymentAmountChange > 0m ? RequesterLedgerTransactionType.Credit : RequesterLedgerTransactionType.Debit,
                        RequesterLedgerSourceType.Payment,
                        saved.PaymentID,
                        saved.PaymentNumber,
                        RequesterLedgerDto.TransactionDateTimeFrom(saved));

                    if (saved.PaymentTypeID is Guid paymentTypeId && paymentTypeId != Guid.Empty)
                        ledgerEntry.PaymentTypeName = await _paymentTypeService.GetNameByIdAsync(paymentTypeId).ConfigureAwait(false);

                    await _requesterLedgerService.AddTransactionOfRequesterLedgerAsync(ledgerEntry).ConfigureAwait(false);
                }

                return saved;
            }).ConfigureAwait(false);
        }

        public override Task<bool> DeleteAsync<TKey>(TKey id) =>
            throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_DELETE_REQUIRES_REASON), nameof(id));

        public async Task<bool> DeleteAsync(Guid paymentId, string reason, CancellationToken cancellationToken = default)
        {
            if (paymentId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_PAYMENT_ALLOCATION_PAYMENT_ID), nameof(paymentId));
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_DELETE_REQUIRES_REASON), nameof(reason));

            var existing = await GetByIdAsync(paymentId).ConfigureAwait(false);
            if (existing == null || existing.IsDeleted)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_NOT_FOUND_OR_DELETED));

            var (appliedAmount, invoiceNumbers) = await _paymentAllocationService.GetTotalActiveAmountAppliedWithInvoiceNumbersByPaymentIdAsync(paymentId, cancellationToken).ConfigureAwait(false);
            var (refundedAmount, refundNumbers) = await _refundPaymentAllocationService.GetTotalNetRefundedAmountWithRefundNumbersByPaymentIdAsync(paymentId, cancellationToken).ConfigureAwait(false);
            existing.EnsureCanDelete(appliedAmount, refundedAmount, invoiceNumbers, refundNumbers);

            return await _dbContext.ExecuteInTransactionAsync(async () =>
            {
                if (existing.Amount > 0m && existing.RequesterID != Guid.Empty && existing.DepartmentID != Guid.Empty)
                {
                    var ledgerEntry = new RequesterLedgerDto(
                        existing.RequesterID,
                        existing.DepartmentID,
                        existing.Amount,
                        RequesterLedgerTransactionType.Debit,
                        RequesterLedgerSourceType.Payment,
                        existing.PaymentID,
                        existing.PaymentNumber,
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
        public async Task<PaymentDto> GetActiveByIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            if (paymentId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_PAYMENT_ALLOCATION_PAYMENT_ID), nameof(paymentId));

            var payment = await GetByIdAsync(paymentId).ConfigureAwait(false);
            PaymentDto.EnsureExists(payment);
            return payment!;
        }

        /// <inheritdoc />
        public async Task<decimal> GetAvailableBalanceByRequesterAsync(Guid? requesterId,CancellationToken cancellationToken = default)
        {
            if (!requesterId.HasValue || requesterId.Value == Guid.Empty)
                return 0m;

            // Unapplied balance = sum of (payment amount − active allocations) per payment, same basis as settle-up.
            var unallocatedPayments = await GetUnallocatedPaymentsOrderedAsync(requesterId.Value, cancellationToken).ConfigureAwait(false);
            return unallocatedPayments.Sum(x => x.RemainingBalance);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<(PaymentDto Payment, decimal RemainingBalance)>> GetUnallocatedPaymentsOrderedAsync(Guid requesterId,CancellationToken cancellationToken = default)
        {
            if (requesterId == Guid.Empty)
                return Array.Empty<(PaymentDto, decimal)>();

            Expression<Func<Payment, bool>> paymentFilter = p =>
                p.RequesterID == requesterId
                && !p.IsDeleted;

            var payments = (await GetAllAsync(
                paymentFilter,
                new Expression<Func<Payment, object>>[] { p => p.PaymentTypeMaster! },
                pageParam: null).ConfigureAwait(false)).ToList();
            if (payments.Count == 0)
                return Array.Empty<(PaymentDto, decimal)>();

            var result = new List<(PaymentDto Payment, decimal RemainingBalance)>();
            foreach (var payment in payments)
            {
                var (allocated, refunded) = await GetPaymentUsageTotalsAsync(payment.PaymentID, cancellationToken)
                    .ConfigureAwait(false);
                var remaining = payment.RemainingBalanceFrom(allocated + refunded);
                if (remaining > 0m)
                    result.Add((payment, remaining));
            }
            return result.OrderBy(x => x.Payment.PaymentDate).ToList();
        }

        /// <inheritdoc />
        public async Task<PaymentDto> VoidPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            if (paymentId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_PAYMENT_ALLOCATION_PAYMENT_ID), nameof(paymentId));

            var payment = await GetByIdAsync(paymentId).ConfigureAwait(false);
            if (payment == null || payment.IsDeleted)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_NOT_FOUND_OR_DELETED));

            payment = await _dbContext.ExecuteInTransactionAsync(async () =>
            {
                var (voidedCount, invoiceIds) = await _paymentAllocationService.VoidActiveAllocationsByPaymentIdAsync(paymentId, cancellationToken).ConfigureAwait(false);
                if (voidedCount == 0)
                {
                    var allocations = await _paymentAllocationService.GetByPaymentIdAsync(paymentId, cancellationToken).ConfigureAwait(false);

                    if (allocations.Count > 0)
                    {
                        throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_ALLOCATION_ALREADY_VOIDED));
                    }
                }

                var updatedPayment = await UpsertAsync(payment).ConfigureAwait(false);

                using var scope = _scopeFactory.CreateScope();
                var invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();
                foreach (var invoiceId in invoiceIds)
                    await invoiceService.RecalculateInvoicePaymentTotalsAsync(invoiceId, cancellationToken).ConfigureAwait(false);

                return updatedPayment;
            }, cancellationToken).ConfigureAwait(false);

            return payment;
        }

        /// <inheritdoc />
        public async Task<PaymentAllocateResponseDto> AllocatePaymentAsync(PaymentAllocateDto request,CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            var payment = await GetByIdAsync(request.PaymentId).ConfigureAwait(false);
            if (payment == null || payment.IsDeleted)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_NOT_FOUND_OR_DELETED));

            if (payment.RequesterID != request.RequesterId)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVOICE_PAYMENT_REQUESTER_MISMATCH));

            var totalPaidBefore = await _paymentAllocationService.GetTotalActiveAmountAppliedByPaymentIdAsync(request.PaymentId, cancellationToken).ConfigureAwait(false);
            var refundedBefore = await GetNetRefundedAmountByPaymentIdAsync(request.PaymentId, cancellationToken).ConfigureAwait(false);
            var paymentRemaining = payment.RemainingBalanceFrom(totalPaidBefore + refundedBefore);

            using var scope = _scopeFactory.CreateScope();
            var invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();

            var toApply = new List<(Guid InvoiceId, decimal Amount)>();
            var totalRequested = 0m;

            foreach (var item in request.Invoices)
            {
                var invoice = await invoiceService.GetByIdAsync(item.InvoiceId).ConfigureAwait(false);
                if (invoice == null || invoice.IsDeleted)
                    throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVOICE_NOT_FOUND_OR_DELETED));

                if (!invoice.RequesterID.HasValue || invoice.RequesterID.Value != request.RequesterId)
                    throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_INVOICE_REQUESTER_MISMATCH));

                var activeOnInvoice = await _paymentAllocationService.GetActivePaymentHistoryByInvoiceIdAsync(item.InvoiceId, cancellationToken).ConfigureAwait(false);
                var balanceDue = Math.Max(0m, (invoice.Total ?? 0m) - PaymentAllocationDto.SumNetAmountApplied(activeOnInvoice));

                var amount = item.ApplyFullAmount ? balanceDue : item.Amount;
                if (amount <= 0m)
                    continue;

                if (!item.ApplyFullAmount && amount > balanceDue)
                    throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVOICE_PAYMENT_EXCEEDS_BALANCE_DUE));

                totalRequested += amount;
                toApply.Add((item.InvoiceId, amount));
            }

            if (toApply.Count == 0)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVOICE_NO_BALANCE_DUE));

            if (totalRequested > paymentRemaining)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_ALLOCATION_EXCEEDS_REMAINING_BALANCE));

            var affectedInvoiceIds = new HashSet<Guid>();

            await _dbContext.ExecuteInTransactionAsync(async () =>
            {
                foreach (var (invoiceId, amount) in toApply)
                {
                    await _paymentAllocationService.UpsertAsync(new PaymentAllocationDto
                    {
                        InvoicesID = invoiceId,
                        PaymentID = request.PaymentId,
                        AmountApplied = amount,
                        AllocationDate = request.AllocationDate,
                        TransactionType = PaymentAllocationTransactionType.Debit
                    }).ConfigureAwait(false);

                    affectedInvoiceIds.Add(invoiceId);
                }
            }, cancellationToken).ConfigureAwait(false);

            foreach (var invoiceId in affectedInvoiceIds)
                await invoiceService.RecalculateInvoicePaymentTotalsAsync(invoiceId, cancellationToken).ConfigureAwait(false);

            var totalPaid = totalPaidBefore + totalRequested;

            return new PaymentAllocateResponseDto(payment, totalPaid, affectedInvoiceIds.ToList(), refundedBefore);
        }

        /// <inheritdoc />
        public async Task<PaymentInvoicesResponseDto?> GetPaymentDetailAsync(Guid paymentId,PageParam? pagination = null,CancellationToken cancellationToken = default)
        {
            if (paymentId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_PAYMENT_ALLOCATION_PAYMENT_ID), nameof(paymentId));

            var payment = await GetByIdAsync(paymentId).ConfigureAwait(false);
            if (payment == null || payment.IsDeleted)
                return null;

            var allocations = await _paymentAllocationService.GetByPaymentIdAsync(paymentId, cancellationToken).ConfigureAwait(false);
            var (usedAmount, refundedAmount) = await GetPaymentUsageTotalsAsync(paymentId, cancellationToken).ConfigureAwait(false);

            using var scope = _scopeFactory.CreateScope();
            var invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();

            var invoices = new List<PaymentInvoiceRowDto>();
            foreach (var invoiceId in allocations.Select(a => a.InvoicesID).Distinct())
            {
                var invoice = await invoiceService.GetByIdAsync(invoiceId).ConfigureAwait(false);
                if (invoice == null || invoice.IsDeleted)
                    continue;

                var paidFromPayment = PaymentAllocationDto.SumNetAmountApplied(allocations.Where(a => a.InvoicesID == invoiceId));if (paidFromPayment <= 0m)
                    continue;

                invoices.Add(new PaymentInvoiceRowDto(invoice, paidFromPayment));
            }

            var orderedInvoices = invoices.OrderByDescending(i => i.InvoiceDate).ToList();
            var effectivePage = pagination ?? new PageParam();
            var page = effectivePage.Page ?? 1;
            var pageSize = effectivePage.GetPageSize() ?? PageParam.MinPageSize;
            var pagedInvoices = orderedInvoices
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            Expression<Func<Invoice, bool>> unpaidFilter = x =>
                x.RequesterID == payment.RequesterID
                && !x.IsDeleted
                && !x.IsPaid;
            var totalInvoicesDueAmount = (await invoiceService.GetAllAsync(unpaidFilter, pageParam: null).ConfigureAwait(false))
                .Sum(i => i.BalanceDue ?? 0m);

            return new PaymentInvoicesResponseDto(
                payment,
                usedAmount,
                totalInvoicesDueAmount,
                new PagedList<PaymentInvoiceRowDto>(pagedInvoices, effectivePage, orderedInvoices.Count),
                refundedAmount: refundedAmount);
        }

        /// <inheritdoc />
        public async Task<PaymentInvoicesResponseDto?> GetPaymentUnpaidInvoicesAsync(Guid paymentId,Guid requesterId,PageParam? pagination = null,CancellationToken cancellationToken = default)
        {
            if (paymentId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_PAYMENT_ALLOCATION_PAYMENT_ID), nameof(paymentId));
            if (requesterId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_INVOICE_PAYMENT_REQUESTER_ID), nameof(requesterId));

            var payment = await GetByIdAsync(paymentId).ConfigureAwait(false);
            if (payment == null || payment.IsDeleted)
                return null;

            if (payment.RequesterID != requesterId)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_INVOICE_REQUESTER_MISMATCH));

            var (usedAmount, refundedAmount) = await GetPaymentUsageTotalsAsync(paymentId, cancellationToken).ConfigureAwait(false);

            using var scope = _scopeFactory.CreateScope();
            var invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();

            Expression<Func<Invoice, bool>> filter = x =>
                x.RequesterID == requesterId
                && !x.IsDeleted
                && !x.IsPaid
                && (x.Total ?? 0m) > 0m
                && (x.BalanceDue ?? 0m) > 0m;

            var unpaidInvoices = (await invoiceService.GetAllAsync(filter, pageParam: null).ConfigureAwait(false)).ToList();

            var invoiceLines = unpaidInvoices
                .Select(invoice => new PaymentInvoiceRowDto(invoice))
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();

            var effectivePage = pagination ?? new PageParam();
            var page = effectivePage.Page ?? 1;
            var pageSize = effectivePage.GetPageSize() ?? PageParam.MinPageSize;
            var pagedInvoiceLines = invoiceLines
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaymentInvoicesResponseDto(
                payment,
                usedAmount,
                invoiceLines.Sum(i => i.DueAmount),
                new PagedList<PaymentInvoiceRowDto>(pagedInvoiceLines, effectivePage, invoiceLines.Count),
                requesterId,
                refundedAmount);
        }

        /// <inheritdoc />
        public async Task<RequesterPaymentSummaryResponseDto> GetRequesterPaymentHistoryByRequesterIdAndDateRange(Guid requesterId,DateOnly? startDate,DateOnly? endDate, PageParam? pagination = null,CancellationToken cancellationToken = default)
        {
            if (requesterId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.ID_CANNOT_BE_EMPTY), nameof(requesterId));

            if (startDate.HasValue && endDate.HasValue && endDate.Value < startDate.Value)
                throw new ArgumentException(
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.END_DATE_CANNOT_BE_LESS_THAN_START_DATE),
                    nameof(endDate));

            Expression<Func<Payment, bool>> allPaymentsFilter = p =>
                p.RequesterID == requesterId
                && !p.IsDeleted;

            var allPayments = (await GetAllAsync(
                allPaymentsFilter,
                new Expression<Func<Payment, object>>[] { p => p.PaymentTypeMaster! },
                pageParam: null).ConfigureAwait(false))
                .OrderByDescending(p => p.PaymentDate)
                .ToList();

            var allRows = new List<RequesterPaymentRowDto>();
            foreach (var payment in allPayments)
            {
                var (used, refunded) = await GetPaymentUsageTotalsAsync(payment.PaymentID, cancellationToken)
                    .ConfigureAwait(false);
                allRows.Add(new RequesterPaymentRowDto(payment, used, refunded));
            }

            var rows = allRows
                .Where(r =>
                {
                    var paymentDateOnly = DateOnly.FromDateTime(r.PaymentDate);
                    if (startDate.HasValue && paymentDateOnly < startDate.Value)
                        return false;
                    if (endDate.HasValue && paymentDateOnly > endDate.Value)
                        return false;
                    return true;
                })
                .OrderByDescending(r => r.PaymentDate)
                .ToList();

            using var scope = _scopeFactory.CreateScope();
            var invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();

            Expression<Func<Invoice, bool>> invoiceFilter = x =>
                x.RequesterID == requesterId
                && !x.IsDeleted
                && !x.IsPaid;

            var unpaidInvoices = (await invoiceService.GetAllAsync(invoiceFilter, pageParam: null).ConfigureAwait(false)).ToList();
            var totalInvoiceDue = unpaidInvoices.Sum(i => i.BalanceDue ?? 0m);

            var totalAvailableBalance = await GetAvailableBalanceByRequesterAsync(requesterId, cancellationToken).ConfigureAwait(false);

            var effectivePaymentsPage = pagination ?? new PageParam();
            var page = effectivePaymentsPage.Page ?? 1;
            var pageSize = effectivePaymentsPage.GetPageSize() ?? PageParam.MinPageSize;
            var pagedPayments = rows
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new RequesterPaymentSummaryResponseDto(requesterId,allRows,totalInvoiceDue,totalAvailableBalance,new PagedList<RequesterPaymentRowDto>(pagedPayments, effectivePaymentsPage, rows.Count));
        }

        /// <inheritdoc />
        public async Task<RequesterPaymentSummaryResponseDto> GetRequesterRefundablePaymentsAsync(
            Guid requesterId,
            PageParam? pagination = null,
            CancellationToken cancellationToken = default)
        {
            if (requesterId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.ID_CANNOT_BE_EMPTY), nameof(requesterId));

            var unallocatedPayments = await GetUnallocatedPaymentsOrderedAsync(requesterId, cancellationToken).ConfigureAwait(false);

            var allRows = new List<RequesterPaymentRowDto>();
            foreach (var (payment, _) in unallocatedPayments)
            {
                var (used, refunded) = await GetPaymentUsageTotalsAsync(payment.PaymentID, cancellationToken)
                    .ConfigureAwait(false);
                allRows.Add(new RequesterPaymentRowDto(payment, used, refunded));
            }

            var rows = allRows
                .OrderByDescending(r => r.PaymentDate)
                .ToList();

            using var scope = _scopeFactory.CreateScope();
            var invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();

            Expression<Func<Invoice, bool>> invoiceFilter = x =>
                x.RequesterID == requesterId
                && !x.IsDeleted
                && !x.IsPaid;

            var unpaidInvoices = (await invoiceService.GetAllAsync(invoiceFilter, pageParam: null).ConfigureAwait(false)).ToList();
            var totalInvoiceDue = unpaidInvoices.Sum(i => i.BalanceDue ?? 0m);

            var totalAvailableBalance = await GetAvailableBalanceByRequesterAsync(requesterId, cancellationToken).ConfigureAwait(false);

            var effectivePage = pagination ?? new PageParam();
            var page = effectivePage.Page ?? 1;
            var pageSize = effectivePage.GetPageSize() ?? PageParam.MinPageSize;
            var pagedPayments = rows
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new RequesterPaymentSummaryResponseDto(
                requesterId,
                rows,
                totalInvoiceDue,
                totalAvailableBalance,
                new PagedList<RequesterPaymentRowDto>(pagedPayments, effectivePage, rows.Count));
        }

        /// <inheritdoc />
        public async Task<PaymentReviewAllocationsResponseDto?> GetPaymentReviewAllocationsAsync(Guid paymentId,PageParam? pagination = null,CancellationToken cancellationToken = default)
        {
            if (paymentId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_PAYMENT_ALLOCATION_PAYMENT_ID), nameof(paymentId));

            var payment = await GetByIdAsync(paymentId).ConfigureAwait(false);
            if (payment == null || payment.IsDeleted)
                return null;

            var detail = await GetPaymentDetailAsync(paymentId, pagination, cancellationToken).ConfigureAwait(false);
            if (detail == null)
                return null;

            var rawAllocations = await _paymentAllocationService.GetByPaymentIdAsync(paymentId, cancellationToken).ConfigureAwait(false);
            var allocations = detail.Invoices.Data
                .Select(invoice =>
                {
                    var header = rawAllocations
                        .Where(a => a.InvoicesID == invoice.InvoiceId)
                        .OrderByDescending(a => a.AllocationDate)
                        .First();
                    return new PaymentInvoiceAllocationDto
                    {
                        PaymentAllocationID = header.PaymentAllocationID,
                        InvoicesID = invoice.InvoiceId,
                        PaymentID = paymentId,
                        AmountApplied = invoice.PaidAmount,
                        AllocationDate = header.AllocationDate,
                        InvoiceNo = invoice.InvoiceNo,
                        InvoiceDate = invoice.InvoiceDate,
                        DueDate = invoice.DueDate,
                        InvoiceAmount = invoice.InvoiceAmount,
                        BalanceDue = invoice.DueAmount,
                        PaymentNumber = payment.PaymentNumber,
                        PaymentDate = payment.PaymentDate,
                        PaymentTypeID = payment.PaymentTypeID,
                        PaymentTypeName = payment.PaymentTypeName,
                        ReferenceNumber = payment.ReferenceNumber,
                        PaymentAmount = payment.Amount,
                    };
                })
                .ToList();

            return new PaymentReviewAllocationsResponseDto(
                payment,
                detail.AppliedAmount,
                new PagedList<PaymentInvoiceAllocationDto>(allocations, detail.Invoices, detail.Invoices.TotalRecords),
                detail.RefundedAmount);
        }

        private async Task<(PaymentDto Payment, List<PaymentInvoiceAllocationDto> Allocations)?> LoadPaymentWithActiveAllocationsAsync(
            Guid paymentId,
            CancellationToken cancellationToken)
        {
            var paymentEntity = await _repository.GetFullPaymentInformationByPaymentId(paymentId, cancellationToken).ConfigureAwait(false);
            if (paymentEntity == null)
                return null;

            var payment = _mapper.Map<PaymentDto>(paymentEntity);
            var allocations = _mapper.Map<List<PaymentInvoiceAllocationDto>>(paymentEntity.PaymentAllocations);
            return (payment, allocations);
        }

        /// <inheritdoc />
        public Task<(PaymentDto Payment, List<PaymentInvoiceAllocationDto> Allocations)?> GetPaymentWithActiveAllocationsAsync(Guid paymentId,CancellationToken cancellationToken = default) =>LoadPaymentWithActiveAllocationsAsync(paymentId, cancellationToken);

        /// <inheritdoc />
        public async Task<PaymentAdjustAllocationsResultDto> AdjustPaymentAmountAgainstAllocationsAsync(PaymentAdjustAllocationsDto request,CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (request.PaymentId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_PAYMENT_ALLOCATION_PAYMENT_ID), nameof(request));

            var loaded = await LoadPaymentWithActiveAllocationsAsync(request.PaymentId, cancellationToken).ConfigureAwait(false);
            if (loaded == null)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_NOT_FOUND_OR_DELETED));

            var payment = loaded.Value.Payment;
            var paymentAllocations = loaded.Value.Allocations;
            var originalPaymentAllocations = loaded.Value.Allocations;

            if (request.PaymentAmount <= 0m)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVALID_INVOICE_PAYMENT_AMOUNT));

            var allocations = request.Allocations ?? new List<PaymentAdjustAllocationItemDto>();

            IReadOnlyList<PaymentAllocationDto>? existingActiveAllocations = !request.HasPaymentFieldChanges(payment) && allocations.Count > 0 ? paymentAllocations  : null;

            if (!request.HasChanges(payment, existingActiveAllocations))
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_ADJUST_NO_CHANGES));

            var oldPaymentAmount = payment.Amount;
            request.ApplyTo(payment);

            if (allocations.Count == 0)
            {
                try
                {
                    return new PaymentAdjustAllocationsResultDto(await UpsertAsync(payment).ConfigureAwait(false), oldPaymentAmount);
                }
                catch (WebApiException ex) when (ex.HttpStatusCode == HttpStatusCode.Conflict)
                {
                    request.RevertAmount(payment, oldPaymentAmount);
                    throw;
                }
            }

            PaymentDto? updatedPayment = null;
            PaymentAllocationUpdateResultDto? allocationUpdate = null;

            try
            {
                allocationUpdate = await _paymentAllocationService.UpdatePaymentAllocationAmountAsync(request.PaymentId, allocations, request.PaymentAmount, true, cancellationToken).ConfigureAwait(false);
            }
            catch (WebApiException ex) when (ex.HttpStatusCode == HttpStatusCode.Conflict)
            {
                request.RevertAmount(payment, oldPaymentAmount);
                throw;
            }
            catch (InvalidOperationException)
            {
                request.RevertAmount(payment, oldPaymentAmount);
                throw;
            }

            try
            {
                updatedPayment = await _dbContext.ExecuteInTransactionAsync(
                    () => UpsertAsync(payment),
                    cancellationToken).ConfigureAwait(false);
            }
            catch (WebApiException ex) when (ex.HttpStatusCode == HttpStatusCode.Conflict)
            {
                request.RevertAmount(payment, oldPaymentAmount);
                throw;
            }
            catch (InvalidOperationException)
            {
                request.RevertAmount(payment, oldPaymentAmount);
                throw;
            }

            return new PaymentAdjustAllocationsResultDto(updatedPayment!, oldPaymentAmount, allocationUpdate!);
        }
    }
}
