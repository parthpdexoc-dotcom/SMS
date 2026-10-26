using System.Globalization;
using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Common;
using SMS.Generic.Common.Exceptions;
using SMS.Generic.Common.Implementations.Responses;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Helpers;
using SMS.Generic.Enums;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing invoice detail operations using the generic service pattern.
    /// </summary>
    public class InvoiceDetailService : GenericService<InvoiceDetailDto, InvoiceDetail>, IInvoiceDetailService
    {
        private readonly IInvoiceDetailRepository _repository;
        private readonly IMapper _mapper;
        private readonly IInvoiceService _invoiceService;
        private readonly IRequestService _requestService;
        private readonly IUploadFileService _uploadFileService;
        private readonly ITokenParser _tokenParser;
        private readonly IClosedPostPositionRepository _closedPostPositionRepository;
        private readonly IDepartmentFeeService _departmentFeeService;
        private readonly SMSDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceDetailService"/> class.
        /// </summary>
        /// <param name="repository">The invoice detail repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="invoiceService">The invoice service for foreign key validation and invoice creation.</param>
        /// <param name="requestService">The request service for request data for invoice.</param>
        /// <param name="uploadFileService">The upload file service (for department logo).</param>
        /// <param name="tokenParser">The token parser for department/employee context.</param>
        /// <param name="closedPostPositionRepository">Closed post repository (read-only enrichment; avoids circular service references).</param>
        /// <param name="departmentFeeService">Department fees (per-officer totals for view-original summary, same rules as invoice recalculation).</param>
        /// <param name="dbContext">Database context for transactional invoice detail updates.</param>
        public InvoiceDetailService(
            IInvoiceDetailRepository repository,
            IMapper mapper,
            ILogger<InvoiceDetailService> logger,
            IInvoiceService invoiceService,
            IRequestService requestService,
            IUploadFileService uploadFileService,
            ITokenParser tokenParser,
            IClosedPostPositionRepository closedPostPositionRepository,
            IDepartmentFeeService departmentFeeService,
            SMSDbContext dbContext) : base(repository, mapper, "InvoiceDetailID", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _invoiceService = invoiceService;
            _requestService = requestService;
            _uploadFileService = uploadFileService;
            _tokenParser = tokenParser;
            _closedPostPositionRepository = closedPostPositionRepository ?? throw new ArgumentNullException(nameof(closedPostPositionRepository));
            _departmentFeeService = departmentFeeService ?? throw new ArgumentNullException(nameof(departmentFeeService));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task ApplyDebitForUncloseAsync(Guid closedPostPositionId, bool deferRecalculation = false)
        {
            if (closedPostPositionId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.CLOSED_JOB_POSITION_ID_CANNOT_BE_EMPTY), nameof(closedPostPositionId));

            // Find the invoice line(s) created for this closed post position.
            // If none exists, there is nothing to reverse.
            var relatedLines = ((await GetAllAsync(
                    d => d.ClosePostPositionID == closedPostPositionId,
                    includes: null,
                    pageParam: null).ConfigureAwait(false)) ?? Enumerable.Empty<InvoiceDetailDto>())
                .ToList();

            if (relatedLines.Count == 0)
                return;

            var invoiceId = relatedLines.First().InvoicesID;
            if (invoiceId == Guid.Empty)
                return;

            // Idempotency: if we already inserted a debit reversal, don't insert another one.
            if (relatedLines.Any(l => l.FinancialImpactType == Enums.FinancialImpactType.Debit))
            {
                if (!deferRecalculation)
                    await RecalculateInvoiceTotalsAsync(invoiceId).ConfigureAwait(false);
                return;
            }

            // Prefer reversing the normal billing line (None). If not present, fall back to the first one we have.
            var source = relatedLines.FirstOrDefault(l => l.FinancialImpactType == Enums.FinancialImpactType.None)
                         ?? relatedLines.First();

            var hours = source.Hours ?? 0m;
            var vehicle = source.VehicleFeeAmount ?? 0m;
            var equipment = source.EquipmentFeeAmount ?? 0m;

            var debitLine = new InvoiceDetailDto(
                jobId: source.JobId,
                jobName: source.JobName,
                jobDate: source.JobDate,
                jobTime: source.JobTime,
                officerName: source.OfficerName,
                rate: source.Rate,
                hours: -hours,
                vehicleFeeAmount: -vehicle,
                equipmentFeeAmount: -equipment)
            {
                InvoicesID = invoiceId,
                ClosePostPositionID = closedPostPositionId,
                PostLength = source.PostLength,
                SpecialRate = source.SpecialRate,
                FinancialImpactType = Enums.FinancialImpactType.Debit,
                VehicleOwnedBy = source.VehicleOwnedBy,
                PayableDirectlyTo = source.PayableDirectlyTo,
                IsPayableToOfficer = source.IsPayableToOfficer
            };

            await UpsertAsync(debitLine).ConfigureAwait(false);
            if (!deferRecalculation)
                await RecalculateInvoiceTotalsAsync(invoiceId).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task EnsureUncloseInvoiceAllowedAsync(Guid closedPostPositionId)
        {
            if (closedPostPositionId == Guid.Empty)
                return;

            var relatedLines = ((await GetAllAsync(
                    d => d.ClosePostPositionID == closedPostPositionId,
                    includes: null,
                    pageParam: null).ConfigureAwait(false)) ?? Enumerable.Empty<InvoiceDetailDto>())
                .ToList();

            if (relatedLines.Count == 0)
                return;

            var invoiceId = relatedLines.First().InvoicesID;
            if (invoiceId == Guid.Empty)
                return;

            var invoice = await _invoiceService.GetByIdAsync(invoiceId).ConfigureAwait(false);
            if (invoice == null)
                return;

            var latestDtos = _mapper.Map<List<InvoiceDetailDto>>(
                    await _repository.GetLatestInvoiceDetailsByInvoiceIdAsync(invoiceId).ConfigureAwait(false))
                .Where(d => d.ClosePostPositionID != closedPostPositionId)
                .ToList();

            decimal officerFeesTotal = 0m;
            decimal agencyFeesTotal = 0m;
            if (invoice.DepartmentID != Guid.Empty)
            {
                (officerFeesTotal, agencyFeesTotal) = await CalculateDepartmentFeeTotalsAsync(
                    invoice.DepartmentID,
                    invoice.RequestId,
                    latestDtos).ConfigureAwait(false);
            }

            invoice.ValidateRecalculatedTotalFromDetails(latestDtos, officerFeesTotal, agencyFeesTotal, closedPostPositionId);
        }

        public async Task ApplyCreditForRecloseAsync(
            Guid sourceClosedPostPositionId,
            Guid newClosedPostPositionId,
            decimal? hours,
            decimal? vehicleFeeAmount,
            decimal? equipmentFeeAmount,
            bool validateOnly = false)
        {
            if (sourceClosedPostPositionId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.SOURCE_CLOSED_JOB_POSITION_ID_CANNOT_BE_EMPTY), nameof(sourceClosedPostPositionId));
            if (!validateOnly && newClosedPostPositionId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.NEW_CLOSED_JOB_POSITION_ID_CANNOT_BE_EMPTY), nameof(newClosedPostPositionId));

            var sourceLines = ((await GetAllAsync(
                    d => d.ClosePostPositionID == sourceClosedPostPositionId,
                    includes: null,
                    pageParam: null).ConfigureAwait(false)) ?? Enumerable.Empty<InvoiceDetailDto>())
                .ToList();

            if (sourceLines.Count == 0)
                return;

            var invoiceId = sourceLines.First().InvoicesID;
            if (invoiceId == Guid.Empty)
                return;

            if (!validateOnly)
            {
                var existingNewLines = ((await GetAllAsync(
                        d => d.ClosePostPositionID == newClosedPostPositionId,
                        includes: null,
                        pageParam: null).ConfigureAwait(false)) ?? Enumerable.Empty<InvoiceDetailDto>())
                    .ToList();

                if (existingNewLines.Any(l => l.FinancialImpactType == Enums.FinancialImpactType.Credit))
                {
                    await RecalculateInvoiceTotalsAsync(invoiceId).ConfigureAwait(false);
                    return;
                }
            }

            var source = sourceLines.FirstOrDefault(l => l.FinancialImpactType == Enums.FinancialImpactType.None)
                         ?? sourceLines.First();

            var normalizedHours = Math.Abs(hours ?? 0m);
            decimal normalizedVehicle;
            decimal normalizedEquipment;
            if (validateOnly && !vehicleFeeAmount.HasValue && !equipmentFeeAmount.HasValue)
            {
                normalizedVehicle = Math.Abs(source.VehicleFeeAmount ?? 0m);
                normalizedEquipment = Math.Abs(source.EquipmentFeeAmount ?? 0m);
            }
            else
            {
                normalizedVehicle = Math.Abs(vehicleFeeAmount ?? 0m);
                normalizedEquipment = Math.Abs(equipmentFeeAmount ?? 0m);
            }

            var creditLine = new InvoiceDetailDto(
                jobId: source.JobId,
                jobName: source.JobName,
                jobDate: source.JobDate,
                jobTime: source.JobTime,
                officerName: source.OfficerName,
                rate: source.Rate,
                hours: normalizedHours,
                vehicleFeeAmount: normalizedVehicle,
                equipmentFeeAmount: normalizedEquipment)
            {
                InvoicesID = invoiceId,
                ClosePostPositionID = newClosedPostPositionId,
                PostLength = source.PostLength,
                SpecialRate = source.SpecialRate,
                FinancialImpactType = Enums.FinancialImpactType.Credit,
                VehicleOwnedBy = source.VehicleOwnedBy,
                PayableDirectlyTo = source.PayableDirectlyTo,
                IsPayableToOfficer = source.IsPayableToOfficer
            };

            var invoice = await _invoiceService.GetByIdAsync(invoiceId).ConfigureAwait(false);
            if (invoice == null)
                return;

            await ValidateRecloseProjectedTotalsAsync(
                invoice,
                invoiceId,
                sourceClosedPostPositionId,
                creditLine).ConfigureAwait(false);

            if (validateOnly)
                return;

            await UpsertAsync(creditLine).ConfigureAwait(false);
            await RecalculateInvoiceTotalsAsync(invoiceId).ConfigureAwait(false);
        }

        private async Task ValidateRecloseProjectedTotalsAsync(
            InvoiceDto invoice,
            Guid invoiceId,
            Guid sourceClosedPostPositionId,
            InvoiceDetailDto creditLine)
        {
            var latestDtos = _mapper.Map<List<InvoiceDetailDto>>(
                await _repository.GetLatestInvoiceDetailsByInvoiceIdAsync(invoiceId).ConfigureAwait(false));

            // Re-close edit: validate the projected final invoice (old close removed + new credit line),
            // not the intermediate unclose-only state. Zero-total and refund checks apply to new form data only.
            var projectedDetails = latestDtos
                .Where(d => d.ClosePostPositionID != sourceClosedPostPositionId)
                .ToList();
            projectedDetails.Add(creditLine);

            decimal officerFeesTotal = 0m;
            decimal agencyFeesTotal = 0m;
            if (invoice.DepartmentID != Guid.Empty)
            {
                (officerFeesTotal, agencyFeesTotal) = await CalculateDepartmentFeeTotalsAsync(
                    invoice.DepartmentID,
                    invoice.RequestId,
                    projectedDetails).ConfigureAwait(false);
            }

            invoice.ValidateRecalculatedTotalFromDetails(projectedDetails, officerFeesTotal, agencyFeesTotal, sourceClosedPostPositionId);
        }

        /// <summary>
        /// Retrieves an invoice detail by its primary key (Guid).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The invoice detail DTO if found; otherwise, null.</returns>
        public async Task<InvoiceDetailDto?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.ID_CANNOT_BE_EMPTY), nameof(id));

            var entity = await _repository.GetLatestVersionByDetailIdAsync(id).ConfigureAwait(false);
            if (entity == null)
                return null;

            var dto = _mapper.Map<InvoiceDetailDto>(entity);
            await EnrichClosedPostPositionAsync(new List<InvoiceDetailDto> { dto }).ConfigureAwait(false);
            return dto;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<InvoiceDetailDto>> UndoInvoiceModificationsAsync(Guid invoicesId)
        {
            var invoice = await _invoiceService.GetByIdAsync(invoicesId).ConfigureAwait(false);
            if (invoice == null)
                throw new KeyNotFoundException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ENTITY_WITH_ID_NOT_FOUND, "Invoice", invoicesId));

            var postUndoLines = await _repository.GetLatestInvoiceDetailsByInvoiceIdAsync(invoicesId, excludeModifiedRows: true).ConfigureAwait(false);
            var postUndoDtos = _mapper.Map<List<InvoiceDetailDto>>(postUndoLines);
            await EnrichClosedPostPositionAsync(postUndoDtos).ConfigureAwait(false);

            decimal officerFeesTotal = 0m;
            decimal agencyFeesTotal = 0m;
            if (invoice.DepartmentID != Guid.Empty)
            {
                (officerFeesTotal, agencyFeesTotal) = await CalculateDepartmentFeeTotalsAsync(
                    invoice.DepartmentID,
                    invoice.RequestId,
                    postUndoDtos).ConfigureAwait(false);
            }

            invoice.ValidateRecalculatedTotalFromDetails(postUndoDtos, officerFeesTotal, agencyFeesTotal);

            await _dbContext.ExecuteInTransactionAsync(async () =>
            {
                // Soft-delete all active Modified rows and superseded manual Debit rows (OR — a row cannot be both types).
                await UpdateAsync(
                    d => d.InvoicesID == invoicesId
                         && !d.IsDeleted
                         && (d.FinancialImpactType == FinancialImpactType.Modified
                             || d.FinancialImpactType == FinancialImpactType.Debit),
                    PopulateSoftDeleteFields).ConfigureAwait(false);

                await _invoiceService.RecalculateTotalsAsync(invoicesId, postUndoDtos).ConfigureAwait(false);
            }).ConfigureAwait(false);

            return postUndoDtos;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<InvoiceDetailDto>> UpdateInvoiceDetailsAsync(IReadOnlyList<InvoiceDetailDto> dtos)
        {
            var invoiceId = dtos[0].InvoicesID;

            var invoice = await _invoiceService.GetByIdAsync(invoiceId).ConfigureAwait(false);
            if (invoice == null)
                throw new KeyNotFoundException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ENTITY_WITH_ID_NOT_FOUND, "Invoice", invoiceId));

            var details = _mapper.Map<List<InvoiceDetailDto>>(
                await _repository.GetLatestInvoiceDetailsByInvoiceIdAsync(invoiceId).ConfigureAwait(false));
            var detailsAfterEdits = InvoiceDto.ApplyDetailChanges(details, dtos);

            decimal officerFeesTotal = 0m;
            decimal agencyFeesTotal = 0m;
            if (invoice.DepartmentID != Guid.Empty)
            {
                (officerFeesTotal, agencyFeesTotal) = await CalculateDepartmentFeeTotalsAsync(
                    invoice.DepartmentID,
                    invoice.RequestId,
                    detailsAfterEdits).ConfigureAwait(false);
            }

            invoice.ValidateRecalculatedTotalFromDetails(detailsAfterEdits, officerFeesTotal, agencyFeesTotal);

            List<InvoiceDetailDto> updatedDetails = null!;
            await _dbContext.ExecuteInTransactionAsync(async () =>
            {
                foreach (var dto in dtos)
                {
                    await AppendInvoiceDetailVersionAsync(dto.InvoiceDetailID, dto).ConfigureAwait(false);
                }

                updatedDetails = _mapper.Map<List<InvoiceDetailDto>>(
                    await _repository.GetLatestInvoiceDetailsByInvoiceIdAsync(invoiceId).ConfigureAwait(false));

                var (savedOfficerFeesTotal, savedAgencyFeesTotal) = await CalculateDepartmentFeeTotalsAsync(
                    invoice.DepartmentID,
                    invoice.RequestId,
                    updatedDetails).ConfigureAwait(false);

                var invoiceForRecalculation = await _invoiceService.GetByIdAsync(invoiceId).ConfigureAwait(false)
                    ?? throw new KeyNotFoundException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ENTITY_WITH_ID_NOT_FOUND, "Invoice", invoiceId));

                invoiceForRecalculation.ValidateRecalculatedTotalFromDetails(
                    updatedDetails,
                    savedOfficerFeesTotal,
                    savedAgencyFeesTotal);

                await _invoiceService.RecalculateTotalsAsync(invoiceId, updatedDetails).ConfigureAwait(false);
            }).ConfigureAwait(false);

            await EnrichClosedPostPositionAsync(updatedDetails).ConfigureAwait(false);
            return updatedDetails;
        }

        /// <summary>
        /// Inserts a new <see cref="FinancialImpactType.Modified"/> version for one line; original <see cref="FinancialImpactType.None"/> is unchanged.
        /// </summary>
        private async Task AppendInvoiceDetailVersionAsync(Guid id, InvoiceDetailDto dto)
        {
            var existing = await _repository.GetByIdAsync(id, "InvoiceDetailID").ConfigureAwait(false);

            if (existing.FinancialImpactType == FinancialImpactType.Modified)
            {
                await UpdateAsync(
                    d => d.InvoiceDetailID == id && !d.IsDeleted,
                    d => d.FinancialImpactType = FinancialImpactType.Debit).ConfigureAwait(false);
            }

            var newVersion = new InvoiceDetailDto(
                dto.JobId ?? existing.JobId,
                dto.JobName ?? existing.JobName,
                dto.JobDate ?? existing.JobDate,
                dto.JobTime ?? existing.JobTime,
                dto.OfficerName ?? existing.OfficerName,
                dto.Rate ?? existing.Rate,
                dto.Hours ?? existing.Hours,
                dto.VehicleFeeAmount ?? existing.VehicleFeeAmount,
                dto.EquipmentFeeAmount ?? existing.EquipmentFeeAmount)
            {
                InvoiceDetailID = Guid.Empty,
                InvoicesID = dto.InvoicesID,
                ClosePostPositionID = dto.ClosePostPositionID ?? existing.ClosePostPositionID,
                PostLength = dto.PostLength ?? existing.PostLength,
                TotalAmount = dto.TotalAmount,
                SpecialRate = dto.SpecialRate ?? existing.SpecialRate,
                FinancialImpactType = FinancialImpactType.Modified,
                VehicleOwnedBy = dto.VehicleOwnedBy ?? existing.VehicleOwnedBy,
                PayableDirectlyTo = dto.PayableDirectlyTo ?? existing.PayableDirectlyTo,
                IsPayableToOfficer = dto.PayableDirectlyTo != null
                    ? dto.IsPayableToOfficer
                    : existing.IsPayableToOfficer
            };

            await UpsertAsync(newVersion).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates or updates an invoice detail with foreign key validation for InvoicesID.
        /// </summary>
        /// <param name="dto">The invoice detail DTO to create or update.</param>
        /// <returns>The created or updated invoice detail DTO.</returns>
        public override async Task<InvoiceDetailDto> UpsertAsync(InvoiceDetailDto dto, bool isSuperUser = false)
        {
            // Validate InvoicesID foreign key - must reference an existing invoice
            if (dto.InvoicesID == Guid.Empty)
            {
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVALID_REQUEST));
            }

            var invoice = await _invoiceService.GetByIdAsync(dto.InvoicesID);
            if (invoice == null)
            {
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUEST_NOT_FOUND, dto.InvoicesID));
            }

            return await base.UpsertAsync(dto, isSuperUser);
        }

        /// <inheritdoc />
        public async Task<SuccessResult<InvoiceDto>> GenerateInvoiceFromRequestDataAsync(string projectNumber, DateOnly startDate, DateOnly endDate)
        {
            var result = await _requestService.GetRequestDataForInvoiceAsync(projectNumber, startDate, endDate).ConfigureAwait(false);
            if (result.Status == (int)HttpStatusCode.NotFound || result.Data == null)
                return result;

            if (result.Data.InvoiceDetails == null || !result.Data.InvoiceDetails.Any())
                return new SuccessResult<InvoiceDto>
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = ModelValidationErrorCodes.GetErrorMessage(
                        ModelValidationErrorCodes.NO_EMPLOYEES_JOB_IN_REQUEST,
                        GetInvoiceRequestName(result.Data, projectNumber),
                        projectNumber,
                        result.Data.StartDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                        result.Data.EndDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)),
                    Data = result.Data
                };

            return await SaveInvoiceWithDetailsAsync(result.Data, GetInvoiceRequestName(result.Data, projectNumber)).ConfigureAwait(false);
        }

        private async Task<SuccessResult<InvoiceDto>> SaveInvoiceWithDetailsAsync(InvoiceDto data, string overlapCheckName)
        {
            if ((data.Total is null or < 0m))
            {
                return new SuccessResult<InvoiceDto>
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = ModelValidationErrorCodes.GetErrorMessage(
                        ModelValidationErrorCodes.INVOICE_TOTAL_MUST_BE_GREATER_THAN_ZERO,
                        overlapCheckName,
                        data.StartDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                        data.EndDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)),
                    Data = data
                };
            }

            if (data.RequestId.HasValue && data.RequestId.Value != Guid.Empty)
            {
                var hasOverlap = await _invoiceService.HasOverlappingInvoiceAsync(
                    data.RequestId.Value, data.StartDate, data.EndDate).ConfigureAwait(false);

                if (hasOverlap)
                {
                    throw new WebApiException(
                        HttpStatusCode.BadRequest,
                        ((int)HttpStatusCode.BadRequest).ToString(),
                        ModelValidationErrorCodes.GetErrorMessage(
                            ModelValidationErrorCodes.INVOICE_ALREADY_EXISTS_FOR_DATE_RANGE,
                            overlapCheckName,
                            data.StartDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                            data.EndDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)));
                }
            }

            var invoiceDto = new InvoiceDto(
                data.RequesterID,
                data.BillingToName,
                data.BillingToAddress,
                data.BillingToPhoneNumber,
                data.BillingToEmail,
                data.BillingToContact,
                data.BillingFromName,
                data.BillingFromAddress,
                data.BillingFromPhoneNumber,
                data.BillingFromEmail,
                data.BillingFromContact)
            {
                InvoiceNo = data.InvoiceNo,
                DateCreated = data.DateCreated ?? DateTime.UtcNow,
                DueDate = data.DueDate,
                StartDate = data.StartDate,
                EndDate = data.EndDate,
                RequestId = data.RequestId,
                PostLength = data.PostLength,
                OfficerHourlyTotal = data.OfficerHourlyTotal,
                OfficerFeesTotal = data.OfficerFeesTotal,
                AgencyFeesTotal = data.AgencyFeesTotal,
                VehicleFeesTotal = data.VehicleFeesTotal,
                EquipmentFeesTotal = data.EquipmentFeesTotal,
                Total = data.Total,
                PaymentsApplied = data.PaymentsApplied,
                BalanceDue = data.BalanceDue,
                RequestName = data.RequestName,
                InvoiceDetails = null
            };

            InvoiceDto savedInvoice = null!;
            var savedDetails = new List<InvoiceDetailDto>();

            await _dbContext.ExecuteInTransactionAsync(async () =>
            {
                savedInvoice = await _invoiceService.UpsertAsync(invoiceDto).ConfigureAwait(false);

                foreach (var detail in data.InvoiceDetails ?? Enumerable.Empty<InvoiceDetailDto>())
                {
                    var detailDto = new InvoiceDetailDto(
                        detail.JobId,
                        detail.JobName,
                        detail.JobDate,
                        detail.JobTime,
                        detail.OfficerName,
                        detail.Rate,
                        detail.Hours,
                        detail.VehicleFeeAmount,
                        detail.EquipmentFeeAmount)
                    {
                        InvoicesID = savedInvoice.InvoicesID,
                        TotalAmount = detail.TotalAmount,
                        PostLength = detail.PostLength,
                        ClosePostPositionID = detail.ClosePostPositionID,
                        VehicleOwnedBy = detail.VehicleOwnedBy,
                        SpecialRate = detail.SpecialRate,
                        PayableDirectlyTo = detail.PayableDirectlyTo ?? data.PayableDirectlyTo,
                        IsPayableToOfficer = detail.PayableDirectlyTo != null
                            ? detail.IsPayableToOfficer
                            : data.IsPayableToOfficer,
                        IsMinimumDue = detail.IsMinimumDue
                    };
                    var savedDetail = await UpsertAsync(detailDto).ConfigureAwait(false);
                    savedDetails.Add(savedDetail);
                }
            }).ConfigureAwait(false);

            await EnrichClosedPostPositionAsync(savedDetails).ConfigureAwait(false);
            savedInvoice.InvoiceDetails = savedDetails;
            return new SuccessResult<InvoiceDto> { Status = (int)HttpStatusCode.OK, Message = string.Empty, Data = savedInvoice };
        }

        private static string GetInvoiceRequestName(InvoiceDto invoice, string fallback)
        {
            if (!string.IsNullOrWhiteSpace(invoice.RequestName))
                return invoice.RequestName;

            if (!string.IsNullOrWhiteSpace(invoice.InvoiceNo))
                return invoice.InvoiceNo;

            return fallback;
        }

        /// <inheritdoc />
        public async Task<SuccessResult<List<InvoiceDto>>> GenerateInvoicesAsync(IReadOnlyCollection<string> projectNumbers, DateOnly startDate, DateOnly endDate)
        {
            InvoiceDto? invoiceToSave = null;
            var errors = new List<ErrorDetail>();
            var missingDataErrors = new List<ErrorDetail>();
            var responseStatus = (int)HttpStatusCode.OK;

            foreach (var projectNumber in projectNumbers)
            {
                try
                {
                    var result = await _requestService.GetRequestDataForInvoiceAsync(projectNumber, startDate, endDate).ConfigureAwait(false);
                    if (result.Data != null)
                    {
                        if (invoiceToSave is null)
                        {
                            invoiceToSave = result.Data;
                        }
                        else
                        {
                            invoiceToSave.MergeFrom(result.Data, includeLineItems: true);
                        }
                    }

                    if (result.Status == (int)HttpStatusCode.NotFound)
                    {
                        missingDataErrors.Add(new ErrorDetail(
                            string.IsNullOrWhiteSpace(result.Message)
                                ? ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.NO_INVOICE_DATA_FOUND)
                                : result.Message,
                            ErrorType.Error,
                            result.Status.ToString(CultureInfo.InvariantCulture),
                            projectNumber));
                        continue;
                    }

                    if (result.Status >= (int)HttpStatusCode.BadRequest && !string.IsNullOrWhiteSpace(result.Message))
                    {
                        responseStatus = responseStatus == (int)HttpStatusCode.BadRequest
                            ? responseStatus
                            : result.Status;

                        errors.Add(new ErrorDetail(
                            result.Message,
                            ErrorType.Error,
                            result.Status.ToString(CultureInfo.InvariantCulture),
                            projectNumber));
                    }
                }
                catch (WebApiException ex) when (ex.HttpStatusCode == HttpStatusCode.NotFound)
                {
                    missingDataErrors.Add(new ErrorDetail(
                        ex.ErrorMessage,
                        ErrorType.Error,
                        ex.ErrorCode.ToString(CultureInfo.InvariantCulture),
                        projectNumber));
                }
                catch (WebApiException ex)
                {
                    responseStatus = responseStatus == (int)HttpStatusCode.BadRequest
                        ? responseStatus
                        : ex.ErrorCode;

                    errors.Add(new ErrorDetail(
                        ex.ErrorMessage,
                        ErrorType.Error,
                        ex.ErrorCode.ToString(CultureInfo.InvariantCulture),
                        projectNumber));
                }
            }

            if (invoiceToSave is null)
            {
                errors.InsertRange(0, missingDataErrors);
                if (responseStatus == (int)HttpStatusCode.OK && missingDataErrors.Count > 0)
                    responseStatus = (int)HttpStatusCode.NotFound;
            }

            if (errors.Count > 0)
            {
                return new SuccessResult<List<InvoiceDto>>
                {
                    Status = responseStatus,
                    Message = string.Join('\n', errors.Select(error => error.ErrorMessage)),
                    Data = new List<InvoiceDto>(),
                    Errors = errors
                };
            }

            if (invoiceToSave is null)
            {
                return new SuccessResult<List<InvoiceDto>>
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.NO_INVOICE_DATA_FOUND),
                    Data = new List<InvoiceDto>()
                };
            }

            var saved = await SaveInvoiceWithDetailsAsync(
                invoiceToSave,
                GetInvoiceRequestName(invoiceToSave, projectNumbers.First())).ConfigureAwait(false);

            return new SuccessResult<List<InvoiceDto>>
            {
                Status = saved.Status,
                Message = saved.Message,
                Data = saved.Status == (int)HttpStatusCode.OK && saved.Data != null
                    ? new List<InvoiceDto> { saved.Data }
                    : new List<InvoiceDto>(),
                Errors = saved.Errors
            };
        }

        /// <inheritdoc />
        public async Task<InvoiceDetailsWithLogoResponseDto> GetByInvoiceIdWithLogoAsync(Guid invoicesId, CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetLatestInvoiceDetailsByInvoiceIdAsync(invoicesId);
            var updatedInvoice = _mapper.Map<List<InvoiceDetailDto>>(items);
            await EnrichClosedPostPositionAsync(updatedInvoice).ConfigureAwait(false);
            DepartmentLogoDto? logo = null;

            var departmentId = _tokenParser.GetDepartmentID();
            if (departmentId.HasValue && departmentId.Value != Guid.Empty)
            {
                var logoResult = await _uploadFileService.GetDepartmentLogoAsync(departmentId.Value, cancellationToken).ConfigureAwait(false);
                if (logoResult.HasValue)
                {
                    await using (logoResult.Value.Stream)
                    {
                        using var ms = new MemoryStream();
                        await logoResult.Value.Stream.CopyToAsync(ms, cancellationToken).ConfigureAwait(false);
                        logo = new DepartmentLogoDto
                        {
                            ContentType = logoResult.Value.ContentType,
                            Base64 = Convert.ToBase64String(ms.ToArray())
                        };
                    }
                }
            }

            return new InvoiceDetailsWithLogoResponseDto
            {
                Items = updatedInvoice,
                Logo = logo,
                IsUpdated = updatedInvoice.Any(i =>
                    i.FinancialImpactType == FinancialImpactType.Modified
                    || i.FinancialImpactType == FinancialImpactType.Credit)
            };
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<InvoiceDetailDto>> ViewPreviousInvoiceVersionsAsync(
            Guid invoicesId,
            CancellationToken cancellationToken = default)
        {
            var latestInvoiceDetails = await _repository.GetLatestInvoiceDetailsByInvoiceIdAsync(invoicesId).ConfigureAwait(false);
            var results = new List<InvoiceDetailDto>();

            foreach (var line in latestInvoiceDetails)
            {
                var previous = await ViewPreviousVersionAsync(line.InvoiceDetailID, cancellationToken).ConfigureAwait(false);
                if (previous != null)
                    results.Add(previous);
            }

            return results;
        }

        /// <inheritdoc />
        public async Task<InvoiceDetailDto?> ViewPreviousVersionAsync(Guid invoiceDetailId, CancellationToken cancellationToken = default)
        {
            var currentInvoice = await _repository.GetByIdAsync(invoiceDetailId, "InvoiceDetailID").ConfigureAwait(false);
            if (currentInvoice == null || currentInvoice.IsDeleted)
                return null;

            if (!currentInvoice.ClosePostPositionID.HasValue || currentInvoice.ClosePostPositionID.Value == Guid.Empty)
                return null;

            var currentInvoiceCreatedDate = currentInvoice.CreatedDate ?? DateTime.MinValue;

            var previous = await _repository.GetPreviousInvoiceVersionByCreatedDateAsync(
                currentInvoice.InvoicesID,
                currentInvoice.ClosePostPositionID.Value,
                currentInvoiceCreatedDate).ConfigureAwait(false);

            if (previous == null)
                return null;

            var item = _mapper.Map<InvoiceDetailDto>(previous);
            await EnrichClosedPostPositionAsync(new List<InvoiceDetailDto> { item }).ConfigureAwait(false);

            item.CurrentInvoiceDetailId = invoiceDetailId;
            return item;
        }

        /// <inheritdoc />
        public async Task<ViewOriginalInvoiceDetailsResponseDto?> ViewOriginalDetailsByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default)
        {
            if (invoiceId == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.ID_CANNOT_BE_EMPTY), nameof(invoiceId));

            var invoice = await _invoiceService.GetByIdAsync(invoiceId).ConfigureAwait(false);
            if (invoice == null)
                return null;

            // GetByIdAsync loads a single row by InvoiceDetailID only. For all lines on an invoice with includes,
            // use the generic GetAllAsync(filter, includes) from the same service base.
            Expression<Func<InvoiceDetail, bool>> filter = d =>
                d.InvoicesID == invoiceId &&
                !d.IsDeleted &&
                d.FinancialImpactType == Enums.FinancialImpactType.None &&
                d.ClosePostPosition != null &&
                !d.ClosePostPosition.IsDeleted;

            Expression<Func<InvoiceDetail, object>>[] includes = { d => d.ClosePostPosition! };

            var detailDtos = (await GetAllAsync(filter, includes, pageParam: null).ConfigureAwait(false))?.ToList() ?? new List<InvoiceDetailDto>();

            var items = _mapper.Map<List<ViewOriginalInvoiceLineItemDto>>(detailDtos)
                .OrderBy(x => x.JobDate ?? DateOnly.MaxValue)
                .ThenBy(x => x.JobTime ?? TimeOnly.MaxValue)
                .ThenBy(x => x.OfficerName ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Summary from the displayed (original-only) lines + same per-officer fee rules as RecalculateTotalsAsync — not persisted header totals.
            decimal officerFeesTotal = invoice.OfficerFeesTotal ?? 0m;
            decimal agencyFeesTotal = invoice.AgencyFeesTotal ?? 0m;
            if (invoice.DepartmentID != Guid.Empty && invoice.RequestId.HasValue && invoice.RequestId.Value != Guid.Empty)
            {
                var deptFeeEntities = await _departmentFeeService
                    .GetInvoiceEligibleFeesForRequestAsync(invoice.DepartmentID, invoice.RequestId.Value)
                    .ConfigureAwait(false);
                (officerFeesTotal, agencyFeesTotal) = DepartmentFeeInvoiceTotalsHelper.CalculatePerOfficerFeeTotals(
                    deptFeeEntities,
                    detailDtos);
            }

            var totals = new InvoiceDto { InvoiceDetails = detailDtos };
            totals.ComputeAndSetTotals(officerFeesTotal, agencyFeesTotal, updateCreatedAndDueDates: false, subtractPaymentsFromBalance: true, paymentsApplied: invoice.PaymentsApplied);

            return new ViewOriginalInvoiceDetailsResponseDto
            {
                Items = items,
                OfficerHourlyTotal = totals.OfficerHourlyTotal,
                OfficerFeesTotal = totals.OfficerFeesTotal,
                AgencyFeesTotal = totals.AgencyFeesTotal,
                VehicleFeesTotal = totals.VehicleFeesTotal,
                EquipmentFeesTotal = totals.EquipmentFeesTotal,
                Total = totals.Total,
                PaymentsApplied = invoice.PaymentsApplied,
                BalanceDue = totals.BalanceDue
            };
        }

        /// <summary>
        /// Sets <see cref="InvoiceDetailDto.ClosedPostPosition"/> for UI using <see cref="InvoiceDetailDto.ClosePostPositionID"/>.
        /// </summary>
        private async Task EnrichClosedPostPositionAsync(IList<InvoiceDetailDto> details)
        {
            if (details == null || details.Count == 0)
                return;

            var ids = details
                .Where(d => d.ClosePostPositionID.HasValue && d.ClosePostPositionID.Value != Guid.Empty)
                .Select(d => d.ClosePostPositionID!.Value)
                .Distinct()
                .ToList();
            if (ids.Count == 0)
                return;

            var closedPosts = await _closedPostPositionRepository
                .GetAllAsync(cpp => ids.Contains(cpp.ClosedPostPositionID), null)
                .ConfigureAwait(false);
            var closedDtoMap = closedPosts.ToDictionary(c => c.ClosedPostPositionID, c => _mapper.Map<ClosedPostPositionDto>(c));

            foreach (var d in details)
            {
                if (!d.ClosePostPositionID.HasValue)
                    continue;
                if (closedDtoMap.TryGetValue(d.ClosePostPositionID.Value, out var closedDto))
                {
                    d.ClosedPostPosition = closedDto;
                    d.IsMinimumDue = closedDto.IsMinimumDue == true;
                }
            }
        }

        private async Task RecalculateInvoiceTotalsAsync(Guid invoiceId)
        {
            if (invoiceId == Guid.Empty)
                return;

            var latestEntities = await _repository.GetLatestInvoiceDetailsByInvoiceIdAsync(invoiceId).ConfigureAwait(false);
            var details = _mapper.Map<List<InvoiceDetailDto>>(latestEntities);

            await _invoiceService.RecalculateTotalsAsync(invoiceId, details).ConfigureAwait(false);
        }

        private async Task<(decimal OfficerFeesTotal, decimal AgencyFeesTotal)> CalculateDepartmentFeeTotalsAsync(
            Guid departmentId,
            Guid? requestId,
            IReadOnlyList<InvoiceDetailDto> details)
        {
            if (departmentId == Guid.Empty || details.Count == 0)
                return (0m, 0m);

            IReadOnlyList<DepartmentFeeDto> deptFees;
            if (requestId.HasValue && requestId.Value != Guid.Empty)
            {
                deptFees = await _departmentFeeService
                    .GetInvoiceEligibleFeesForRequestAsync(departmentId, requestId.Value)
                    .ConfigureAwait(false);
            }
            else
            {
                deptFees = (await _departmentFeeService
                    .GetAllAsync(f => f.DepartmentID == departmentId, null)
                    .ConfigureAwait(false)).ToList();
            }

            return DepartmentFeeInvoiceTotalsHelper.CalculatePerOfficerFeeTotals(deptFees, details);
        }
    }
}
