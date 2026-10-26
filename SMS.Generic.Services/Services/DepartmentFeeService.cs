using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Helpers;
using System.Net;
using System.Linq.Expressions;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing department fee operations using the generic service pattern.
    /// </summary>
    public class DepartmentFeeService : GenericService<DepartmentFeeDto, DepartmentFee>, IDepartmentFeeService
    {
        private readonly IDepartmentFeeRepository _repository;
        private readonly IRequestDepartmentFeeRepository _requestDepartmentFeeRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;
        private readonly IDepartmentService _departmentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentFeeService"/> class.
        /// </summary>
        public DepartmentFeeService(
            IDepartmentFeeRepository repository,
            IRequestDepartmentFeeRepository requestDepartmentFeeRepository,
            IRequestRepository requestRepository,
            IMapper mapper,
            ITokenParser tokenParser,
            ILogger<DepartmentFeeService> logger,
            IDepartmentService departmentService) : base(repository, mapper, "FeeId", tokenParser)
        {
            _repository = repository;
            _requestDepartmentFeeRepository = requestDepartmentFeeRepository;
            _requestRepository = requestRepository;
            _mapper = mapper;
            _tokenParser = tokenParser;
            _departmentService = departmentService;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<ApplicableDepartmentFeeDto>> GetFeesForRequestSelectionAsync(
            Guid? requestId = null)
        {
            var departmentId = _tokenParser.GetDepartmentID() ?? Guid.Empty;
            if (departmentId == Guid.Empty)
                return Array.Empty<ApplicableDepartmentFeeDto>();

            var editRequestId = requestId is { } id && id != Guid.Empty ? id : (Guid?)null;
            var isEditMode = editRequestId.HasValue;

            HashSet<Guid> existingFeeIds = new();
            if (isEditMode)
            {
                var saved = await _requestDepartmentFeeRepository.GetAllAsync(
                    r => r.RequestID == editRequestId.Value,
                    null,
                    null).ConfigureAwait(false);
                existingFeeIds = saved.Select(x => x.DepartmentFeeID).ToHashSet();
            }

            var fees = await GetAllAsync(
                f => f.DepartmentID == departmentId && f.IsActive,
                null).ConfigureAwait(false);

            return fees
                .Where(f => f.FeeId.HasValue)
                .Select(f =>
                {
                    var feeId = f.FeeId.Value;
                    var isExisting = isEditMode && existingFeeIds.Contains(feeId);
                    return new ApplicableDepartmentFeeDto
                    {
                        DepartmentFeeId = feeId,
                        Title = f.FeeShort,
                        Description = f.FeeLong,
                        FeeType = f.FeeType,
                        DefaultFee = f.DefaultFee,
                        AdditionalFee = f.AdditionalFee,
                        ReimburseTo = f.ReimburseTo,
                        IsDefault = f.IsDefault,
                        IsSelected = isEditMode ? isExisting || f.IsDefault : f.IsDefault,
                        IsExisting = isExisting
                    };
                })
                .ToList();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<DepartmentFeeDto>> GetInvoiceEligibleFeesForRequestAsync(
            Guid departmentId,
            Guid requestId)
        {
            if (departmentId == Guid.Empty || requestId == Guid.Empty)
                return Array.Empty<DepartmentFeeDto>();

            var request = await _requestRepository.GetByIdAsync(requestId, "RequestID").ConfigureAwait(false);
            if (request == null || !request.EnableFees)
                return Array.Empty<DepartmentFeeDto>();

            var selected = await _requestDepartmentFeeRepository.GetAllAsync(
                r => r.RequestID == requestId,
                null,
                null).ConfigureAwait(false);
            var selectedIds = selected.Select(r => r.DepartmentFeeID).ToHashSet();
            if (selectedIds.Count == 0)
                return Array.Empty<DepartmentFeeDto>();

            var fees = await GetAllAsync(
                f => f.DepartmentID == departmentId && f.IsActive && selectedIds.Contains(f.FeeId),
                null).ConfigureAwait(false);

            return DepartmentFeeInvoiceTotalsHelper
                .FilterForInvoice(fees, selectedIds, request.StartDate, request.EnableFees)
                .ToList();
        }

        /// <summary>
        /// Inserts a new department fee or updates an existing one with duplicate validation.
        /// </summary>
        public override async Task<DepartmentFeeDto> UpsertAsync(DepartmentFeeDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            if (!dto.EffectiveStartDate.HasValue)
            {
                throw new WebApiException(HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(
                        ModelValidationErrorCodes.REQUIRED_DEPARTMENT_FEE_EFFECTIVE_START_DATE));
            }

            if (dto.EffectiveEndDate.HasValue && dto.EffectiveEndDate.Value < dto.EffectiveStartDate.Value)
            {
                throw new WebApiException(HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(
                        ModelValidationErrorCodes.INVALID_DEPARTMENT_FEE_EFFECTIVE_DATE_RANGE));
            }

            // DefaultFeeConditions is mandatory when IsAdditionalFees is true
            if (dto.IsAdditionalFees && !dto.DefaultFeeConditions.HasValue)
            {
              throw new WebApiException(HttpStatusCode.BadRequest,
                                          ((int)HttpStatusCode.BadRequest).ToString(),
                                           ModelValidationErrorCodes.GetErrorMessage(
         ModelValidationErrorCodes.REQUIRED_DEPARTMENT_FEE_DEFAULT_FEE_CONDITIONS_WHEN_ADDITIONAL_FEES)
);

            }

            // Validate DepartmentID foreign key exists
            await ValidateForeignKeyAsync<IDepartmentService, DepartmentDto>(_departmentService, dto.DepartmentID, "Department");

            // Validate for duplicate FeeShort within the same department
            if (!string.IsNullOrWhiteSpace(dto.FeeShort))
            {
                await ValidateDuplicateAsync(
                    propertyName: "FeeShort",
                    value: dto.FeeShort,
                    dto: dto,
                    isDepartmentScoped: false,
                    additionalFilter: e => e.DepartmentID == dto.DepartmentID,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_DEPARTMENT_FEE_FEE_SHORT);
            }

            return await base.UpsertAsync(dto);
        }

        public override Task<PagedList<DepartmentFeeDto>> SearchAsync(
            string? searchTerm,
            PageParam? pageParam = null,
            Expression<Func<DepartmentFee, bool>>? filter = null,
            Expression<Func<DepartmentFee, object>>[]? includes = null,
            params string[] searchableProperties)
        {
            filter = filter ?? (Expression<Func<DepartmentFee, bool>>)(x => true);

            var effectiveSearchableProperties =
                (searchableProperties?.Length > 0)
                    ? searchableProperties
                    : new[]
                    {
                        nameof(DepartmentFee.FeeShort),
                        nameof(DepartmentFee.FeeLong)
                    };

            return base.SearchAsync(
                searchTerm,
                pageParam,
                filter,
                includes,
                effectiveSearchableProperties
            );
        }
    }
}
