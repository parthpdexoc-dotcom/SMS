using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Helpers;
using SMS.Generic.Common.Implementations.Paginations;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing invoice operations using the generic service pattern.
    /// </summary>
    public class InvoiceService : GenericService<InvoiceDto, Invoice>, IInvoiceService
    {
        private readonly IInvoiceRepository _repository;
        private readonly IPaymentService _paymentService;
        private readonly IPaymentAllocationService _paymentAllocationService;
        private readonly IMapper _mapper;
        private readonly IRequesterService _requesterService;
        private readonly IDepartmentFeeService _departmentFeeService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly SMSDbContext _dbContext;
        private readonly ITokenParser _tokenParser;
        private readonly IRequesterLedgerService _requesterLedgerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceService"/> class.
        /// </summary>
        /// <param name="repository">The invoice repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="tokenParser">The token parser for department/employee context.</param>
        public InvoiceService(
            IInvoiceRepository repository,
            IPaymentService paymentService,
            IPaymentAllocationService paymentAllocationService,
            IMapper mapper,
            ILogger<InvoiceService> logger,
            ITokenParser tokenParser,
            IRequesterService requesterService,
            IDepartmentFeeService departmentFeeService,
            IServiceScopeFactory scopeFactory,
            SMSDbContext dbContext,
            IRequesterLedgerService requesterLedgerService) : base(repository, mapper, "InvoicesID", tokenParser)
        {
            _repository = repository;
            _paymentService = paymentService;
            _paymentAllocationService = paymentAllocationService;
            _mapper = mapper;
            _requesterService = requesterService;
            _departmentFeeService = departmentFeeService;
            _scopeFactory = scopeFactory;
            _dbContext = dbContext;
            _tokenParser = tokenParser;
            _requesterLedgerService = requesterLedgerService;
        }

        public override async Task<InvoiceDto> UpsertAsync(InvoiceDto dto, bool isSuperUser = false)
        {
            decimal previousTotal = 0m;

            if (dto.InvoicesID == Guid.Empty)
            {
                var departmentId = dto.DepartmentID != Guid.Empty
                    ? dto.DepartmentID
                    : (_tokenParser.GetDepartmentID() ?? Guid.Empty);

                if (!string.IsNullOrWhiteSpace(dto.InvoiceNo))
                {
                    var prefix = InvoiceNumberFormat.GetPrefix(dto.InvoiceNo);
                    dto.InvoiceNo = await _repository
                        .GetNextInvoiceNumberForPrefixAsync(prefix, departmentId)
                        .ConfigureAwait(false);
                }
            }
            else if (await GetByIdAsync(dto.InvoicesID).ConfigureAwait(false) is { } existing)
            {
                previousTotal = existing.Total ?? 0m;
                dto.InvoiceNo = existing.InvoiceNo;
            }

            return await _dbContext.ExecuteInTransactionAsync(async () =>
            {
                var saved = await base.UpsertAsync(dto, isSuperUser).ConfigureAwait(false);

                var invoiceTotalChange = (saved.Total ?? 0m) - previousTotal;
                if (invoiceTotalChange != 0m
                    && saved.RequesterID is Guid requesterId && requesterId != Guid.Empty
                    && saved.DepartmentID != Guid.Empty)
                {
                    await _requesterLedgerService.AddTransactionOfRequesterLedgerAsync(new RequesterLedgerDto(
                        requesterId,
                        saved.DepartmentID,
                        Math.Abs(invoiceTotalChange),
                        invoiceTotalChange > 0m ? RequesterLedgerTransactionType.Debit : RequesterLedgerTransactionType.Credit,
                        RequesterLedgerSourceType.Invoice,
                        saved.InvoicesID,
                        saved.InvoiceNo,
                        RequesterLedgerDto.TransactionDateTimeFrom(saved),
                        saved.RequesterName)).ConfigureAwait(false);
                }

                return saved;
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves an invoice by its primary key (Guid).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The invoice DTO if found; otherwise, null.</returns>
        public async Task<InvoiceDto?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.ID_CANNOT_BE_EMPTY), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "InvoicesID");
            if (entity == null)
                return null;

            return _mapper.Map<InvoiceDto>(entity);
        }

        /// <summary>
        /// Soft-deletes an invoice and all of its invoice details.
        /// </summary>
        public override async Task<bool> DeleteAsync<TKey>(TKey id)
        {
            if (id is not Guid invoiceId || invoiceId == Guid.Empty)
            {
                return await base.DeleteAsync(id).ConfigureAwait(false);
            }

            var existing = await GetByIdAsync(invoiceId).ConfigureAwait(false);
            if (existing == null)
            {
                return false;
            }

            using var scope = _scopeFactory.CreateScope();
            var invoiceDetailService = scope.ServiceProvider.GetRequiredService<IInvoiceDetailService>();

            return await _dbContext.ExecuteInTransactionAsync(async () =>
            {
                var activeAllocations = await _paymentAllocationService.GetActivePaymentHistoryByInvoiceIdAsync(invoiceId).ConfigureAwait(false);

                foreach (var paymentId in activeAllocations.Select(a => a.PaymentID).Distinct())
                    await _paymentAllocationService.VoidActiveAllocationsByInvoiceAndPaymentAsync(invoiceId, paymentId).ConfigureAwait(false);

                await invoiceDetailService.UpdateAsync(x => x.InvoicesID == invoiceId, x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);

                await UpdateAsync(x => x.InvoicesID == invoiceId, x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);

                var total = existing.Total ?? 0m;
                if (total > 0m
                    && existing.RequesterID is Guid requesterId && requesterId != Guid.Empty
                    && existing.DepartmentID != Guid.Empty)
                {
                    await _requesterLedgerService.AddTransactionOfRequesterLedgerAsync(new RequesterLedgerDto(
                        requesterId,
                        existing.DepartmentID,
                        total,
                        RequesterLedgerTransactionType.Credit,
                        RequesterLedgerSourceType.Invoice,
                        existing.InvoicesID,
                        existing.InvoiceNo,
                        RequesterLedgerDto.TransactionDateTimeForDelete(),
                        existing.RequesterName)).ConfigureAwait(false);
                }

                return true;
            }).ConfigureAwait(false);
        }

        public async Task<bool> HasOverlappingInvoiceAsync(Guid requestId, DateOnly startDate, DateOnly endDate)
        {
            // Two ranges [a,b] and [c,d] overlap if a <= d and c <= b.
            var existing = await _repository.GetAllAsync(
                i => i.RequestId == requestId
                     && i.StartDate <= endDate
                     && startDate <= i.EndDate);

            return existing.Any();
        }

        public async Task RecalculateTotalsAsync(Guid invoiceId, IReadOnlyList<InvoiceDetailDto> details)
        {
            if (invoiceId == Guid.Empty)
                return;
            if (details == null)
                throw new ArgumentNullException(nameof(details));

            var invoice = await GetByIdAsync(invoiceId).ConfigureAwait(false);
            if (invoice == null)
                return;

            var officerHourlyTotal = details.Sum(d => (d.Rate ?? 0m) * (d.Hours ?? 0m));
            var vehicleFeesTotal = details.Sum(d => d.VehicleFeeAmount ?? 0m);
            var equipmentFeesTotal = details.Sum(d => d.EquipmentFeeAmount ?? 0m);

            invoice.OfficerHourlyTotal = officerHourlyTotal;
            invoice.VehicleFeesTotal = vehicleFeesTotal;
            invoice.EquipmentFeesTotal = equipmentFeesTotal;

            decimal officerFeesTotal = invoice.OfficerFeesTotal ?? 0m;
            decimal agencyFeesTotal = invoice.AgencyFeesTotal ?? 0m;

            if (invoice.DepartmentID != Guid.Empty && invoice.RequestId.HasValue && invoice.RequestId.Value != Guid.Empty)
            {
                var deptFeeEntities = await _departmentFeeService
                    .GetInvoiceEligibleFeesForRequestAsync(invoice.DepartmentID, invoice.RequestId.Value)
                    .ConfigureAwait(false);

                (officerFeesTotal, agencyFeesTotal) = DepartmentFeeInvoiceTotalsHelper.CalculatePerOfficerFeeTotals(
                    deptFeeEntities,
                    details);
            }

            invoice.OfficerFeesTotal = officerFeesTotal;
            invoice.AgencyFeesTotal = agencyFeesTotal;

            var newTotal = officerHourlyTotal + officerFeesTotal + agencyFeesTotal + vehicleFeesTotal + equipmentFeesTotal;
            invoice.ApplyRecalculatedTotal(newTotal);

            await UpsertAsync(invoice).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<PagedList<InvoiceDto>> GetAllByRequesterIncludingChildrenAsync(Guid? requesterId, PageParam? pagination = null, bool? isPaid = null, string? searchTerm = null, int? dueDateRangeDays = null)
        {
            Expression<Func<Invoice, bool>>? filter = null;

            if (requesterId.HasValue && requesterId.Value != Guid.Empty)
            {
                var childRequesters = await _requesterService.GetAllAsync(r => r.ParentID == requesterId.Value).ConfigureAwait(false);
                var requesterIds = childRequesters
                    .Where(r => r.RequesterID.HasValue)
                    .Select(r => r.RequesterID!.Value)
                    .ToList();
                requesterIds.Add(requesterId.Value);

                filter = x => x.RequesterID.HasValue && requesterIds.Contains(x.RequesterID.Value);
            }

            if (isPaid.HasValue)
            {
                var isPaidFilter = (Expression<Func<Invoice, bool>>)(x => x.IsPaid == isPaid.Value);
                filter = filter == null
                    ? isPaidFilter
                    : ExpressionCombiner.AndAlso(filter, isPaidFilter);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                var searchFilter = (Expression<Func<Invoice, bool>>)(x =>
                    (x.InvoiceNo != null && x.InvoiceNo.Contains(term))
                    || (x.Requester != null && x.Requester.Name != null && x.Requester.Name.Contains(term)));
                filter = filter == null
                    ? searchFilter
                    : ExpressionCombiner.AndAlso(filter, searchFilter);
            }

            if (dueDateRangeDays is 30 or 60 or 90)
            {
                var today = GetCurrentDepartmentDateTime().Date;
                var minDueDate = today.AddDays(-dueDateRangeDays.Value);
                var dueDateFilter = (Expression<Func<Invoice, bool>>)(x =>
                    x.DueDate.HasValue
                    && x.DueDate.Value.Date >= minDueDate
                    && x.DueDate.Value.Date <= today);
                filter = filter == null
                    ? dueDateFilter
                    : ExpressionCombiner.AndAlso(filter, dueDateFilter);
            }

            var includes = new Expression<Func<Invoice, object>>[]
            {
                x => x.InvoiceDetails!
            };

            var pagedInvoices = await GetAllPagedAsync(filter, pagination, includes).ConfigureAwait(false);

            foreach (var invoice in pagedInvoices.Data)
            {
                var workedDates = (invoice.InvoiceDetails ?? Enumerable.Empty<InvoiceDetailDto>())
                    .Where(d => d.JobDate.HasValue)
                    .Select(d => d.JobDate!.Value)
                    .OrderBy(d => d)
                    .ToList();

                if (workedDates.Count > 0)
                {
                    invoice.JobPositionStartDate = workedDates[0];
                    invoice.JobPositionEndDate = workedDates[^1];
                }
            }

            return pagedInvoices;
        }

        /// <inheritdoc />
        public async Task<PagedList<RequesterInvoiceCountDto>> GetRequesterInvoiceCountsAsync(
            PageParam pageParam,
            Guid? requesterId = null,
            bool? isPaid = null,
            string? searchTerm = null,
            int? dueDateRangeDays = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            pageParam ??= new PageParam();
            if (requesterId == Guid.Empty)
                requesterId = null;

            var (data, totalRecords) = await _repository.GetRequesterInvoiceCountsAsync(
                _tokenParser.GetDepartmentID(),
                requesterId,
                isPaid,
                searchTerm,
                dueDateRangeDays,
                GetCurrentDepartmentDateTime().Date,
                pageParam,
                cancellationToken).ConfigureAwait(false);

            var unpaidRequesterIds = await GetRequesterIdsWithUnpaidInvoicesDueTillDateAsync(
                data.Select(x => x.RequesterId),
                cancellationToken).ConfigureAwait(false);

            foreach (var row in data)
                row.HasUnpaidInvoice = unpaidRequesterIds.Contains(row.RequesterId);

            return new PagedList<RequesterInvoiceCountDto>(data, pageParam, totalRecords);
        }

        /// <inheritdoc />
        public async Task<IReadOnlySet<Guid>> GetRequesterIdsWithUnpaidInvoicesDueTillDateAsync(
            IEnumerable<Guid> requesterIds,
            CancellationToken cancellationToken = default)
        {
            var ids = requesterIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            if (ids.Count == 0)
                return new HashSet<Guid>();

            return await _repository.GetRequesterIdsWithUnpaidInvoicesDueTillDateAsync(
                ids,
                _tokenParser.GetDepartmentID(),
                GetCurrentDepartmentDateTime().Date,
                cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<bool> IsRequesterClearOfOverdueUnpaidInvoicesAsync(
            Guid requesterId,
            CancellationToken cancellationToken = default)
        {
            if (requesterId == Guid.Empty)
                return true;

            var unpaidRequesterIds = await GetRequesterIdsWithUnpaidInvoicesDueTillDateAsync(
                new[] { requesterId },
                cancellationToken).ConfigureAwait(false);

            return !unpaidRequesterIds.Contains(requesterId);
        }

        /// <inheritdoc />
        public async Task<RequesterInvoiceDetailsResponseDto> GetRequesterInvoiceDetailsAsync(Guid requesterId,bool isPaid,PageParam? pagination = null,CancellationToken cancellationToken = default)
        {
            if (requesterId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.ID_CANNOT_BE_EMPTY), nameof(requesterId));

            Expression<Func<Invoice, bool>> filter = x => x.RequesterID == requesterId && !x.IsDeleted;

            Expression<Func<Invoice, object>>[] includes = { x => x.Requester! };
            var allInvoices = (await GetAllAsync(filter, includes, null).ConfigureAwait(false)).ToList();
            var filteredInvoices = InvoiceDto.FilterInvoicesByPaidStatus(allInvoices, isPaid);

            var availableBalance = await _paymentService.GetAvailableBalanceByRequesterAsync(requesterId, cancellationToken).ConfigureAwait(false);

            return new RequesterInvoiceDetailsResponseDto
            {
                Summary = allInvoices.ToRequesterInvoiceDetailsSummary(availableBalance),
                Invoices = new PagedList<InvoiceDto>(filteredInvoices, pagination ?? new PageParam(), filteredInvoices.Count)
            };
        }

        public override Task<PagedList<InvoiceDto>> SearchAsync(
            string? searchTerm,
            PageParam? pageParam = null,
            Expression<Func<Invoice, bool>>? filter = null,
            Expression<Func<Invoice, object>>[]? includes = null,
            params string[] searchableProperties)
        {
            filter = filter ?? (Expression<Func<Invoice, bool>>)(x => true);

            var effectiveSearchableProperties =
                (searchableProperties?.Length > 0)
                    ? searchableProperties
                    : new[]
                    {
                        nameof(Invoice.InvoiceNo),
                        nameof(Invoice.BillingFromName),
                        nameof(Invoice.BillingFromAddress),
                        nameof(Invoice.BillingFromPhoneNumber),
                        nameof(Invoice.BillingFromEmail),
                        nameof(Invoice.BillingToName),
                        nameof(Invoice.BillingToAddress),
                        nameof(Invoice.BillingToPhoneNumber),
                        nameof(Invoice.BillingToEmail)
                    };

            return base.SearchAsync(
                searchTerm,
                pageParam,
                filter,
                includes,
                effectiveSearchableProperties
            );
        }

        /// <inheritdoc />
        public async Task<InvoicePaymentInformationResponseDto?> GetInvoicePaymentInformationAsync(Guid invoiceId,CancellationToken cancellationToken = default)
        {
            if (invoiceId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.ID_CANNOT_BE_EMPTY), nameof(invoiceId));

            var invoice = await GetByIdAsync(invoiceId).ConfigureAwait(false);
            if (invoice == null)
                return null;

            var paymentHistory = await _paymentAllocationService.GetActivePaymentHistoryByInvoiceIdAsync(invoiceId, cancellationToken).ConfigureAwait(false);

            var availableBalance = await _paymentService.GetAvailableBalanceByRequesterAsync(invoice.RequesterID, cancellationToken)
                .ConfigureAwait(false);

            return new InvoicePaymentInformationResponseDto
            {
                Invoice = invoice,
                PaymentHistory = paymentHistory,
                AvailableBalance = availableBalance,
                RemainingDue = invoice.BalanceDue ?? 0,
                AppliedPayment = invoice.PaymentsApplied ?? 0
            };
        }

        /// <inheritdoc />
        public async Task<InvoicePaymentSummaryResponseDto?> GetInvoicePaymentSummaryAsync(Guid invoiceId, CancellationToken cancellationToken = default)
        {
            if (invoiceId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.ID_CANNOT_BE_EMPTY), nameof(invoiceId));

            var invoice = await GetByIdAsync(invoiceId).ConfigureAwait(false);
            if (invoice == null)
                return null;

            var paymentHistory = await _paymentAllocationService.GetActivePaymentHistoryByInvoiceIdAsync(invoiceId, cancellationToken).ConfigureAwait(false);

            return new InvoicePaymentSummaryResponseDto
            {
                InvoiceId = invoice.InvoicesID,
                Total = invoice.Total ?? 0m,
                PaidAmount = invoice.PaymentsApplied ?? 0m,
                BalanceDue = invoice.BalanceDue ?? 0m,
                Payments = paymentHistory
                    .GroupBy(h => h.PaymentID)
                    .Select(g =>
                    {
                        var header = g.First();
                        return new PaymentInformationDto
                        {
                            PaymentID = g.Key,
                            InvoicesID = invoiceId,
                            PaymentNumber = header.PaymentNumber,
                            PaymentDate = header.PaymentDate,
                            PaymentTypeID = header.PaymentTypeID,
                            PaymentTypeName = header.PaymentTypeName,
                            ReferenceNumber = header.ReferenceNumber,
                            PaymentAmount = header.PaymentAmount,
                            AmountApplied = PaymentAllocationDto.SumNetAmountApplied(g),
                        };
                    })
                    .Where(p => p.AmountApplied > 0m)
                    .OrderByDescending(p => p.PaymentDate)
                    .ToList()
            };
        }

        /// <inheritdoc />
        public async Task<InvoicePaymentInformationResponseDto> VoidInvoicePaymentAsync(Guid invoiceId,Guid paymentId,string reason,CancellationToken cancellationToken = default)
        {
            if (invoiceId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.ID_CANNOT_BE_EMPTY), nameof(invoiceId));
            if (paymentId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_PAYMENT_ALLOCATION_PAYMENT_ID), nameof(paymentId));
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_ALLOCATION_DISCARD_REQUIRES_REASON), nameof(reason));

            var invoice = await GetByIdAsync(invoiceId).ConfigureAwait(false);
            if (invoice == null || invoice.IsDeleted)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVOICE_NOT_FOUND_OR_DELETED));

            await _dbContext.ExecuteInTransactionAsync(async () =>
            {
                var voidedCount = await _paymentAllocationService.VoidActiveAllocationsByInvoiceAndPaymentAsync(invoiceId, paymentId, reason.Trim(), cancellationToken).ConfigureAwait(false);

                if (voidedCount == 0)
                {
                    var existing = await _paymentAllocationService.GetByInvoiceAndPaymentAsync(invoiceId, paymentId, cancellationToken).ConfigureAwait(false);

                    if (existing.Count == 0)
                        throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_ALLOCATION_NOT_FOUND_FOR_INVOICE));

                    throw new InvalidOperationException(
                        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_ALLOCATION_ALREADY_VOIDED));
                }

                await UpdateInvoicePaymentTotalsAsync(invoice, cancellationToken).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);

            return await GetInvoicePaymentInformationAsync(invoiceId, cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVOICE_NOT_FOUND_OR_DELETED));
        }

        /// <inheritdoc />
        public async Task RecalculateInvoicePaymentTotalsAsync(Guid invoiceId, CancellationToken cancellationToken = default)
        {
            var invoice = await GetByIdAsync(invoiceId).ConfigureAwait(false);
            if (invoice == null || invoice.IsDeleted)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVOICE_NOT_FOUND_OR_DELETED));

            await UpdateInvoicePaymentTotalsAsync(invoice, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<InvoiceDto>> GetInvoiceByProjectNumberAsync(string projectNumber, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(projectNumber))
                throw new ArgumentException("Project number is required.", nameof(projectNumber));

            cancellationToken.ThrowIfCancellationRequested();

            var invoices = await _repository.GetByProjectNumberAsync(
                projectNumber.Trim(),
                _tokenParser.GetDepartmentID(),
                cancellationToken).ConfigureAwait(false);

            return _mapper.Map<List<InvoiceDto>>(invoices);
        }

        private static decimal GetInvoiceRemainingDue(InvoiceDto invoice, decimal paymentsApplied) =>Math.Max(0m, (invoice.Total ?? 0m) - paymentsApplied);

        private async Task UpdateInvoicePaymentTotalsAsync(InvoiceDto invoice, CancellationToken cancellationToken)
        {
            var payments = await _paymentAllocationService.GetActivePaymentHistoryByInvoiceIdAsync(invoice.InvoicesID, cancellationToken).ConfigureAwait(false);
            var paymentsApplied = PaymentAllocationDto.SumNetAmountApplied(payments);

            invoice.PaymentsApplied = paymentsApplied;
            invoice.BalanceDue = GetInvoiceRemainingDue(invoice, paymentsApplied);
            invoice.IsPaid = invoice.BalanceDue == 0m;

            await UpsertAsync(invoice).ConfigureAwait(false);
        }
    }
}

