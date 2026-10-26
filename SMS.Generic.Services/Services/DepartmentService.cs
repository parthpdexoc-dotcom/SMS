using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common;
using SMS.Generic.Common.Constants;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Net;
using SMS.Generic.Common.Implementations.Paginations;
using System.Linq.Expressions;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing department operations using the generic service pattern.
    /// </summary>
    public class DepartmentService : GenericService<DepartmentDto, Department>, IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IDepartmentConfigurationService _departmentConfigurationService;
        private readonly IDepartmentPolicyService _departmentPolicyService;
        private readonly IDepartmentDefinedFieldsService _departmentDefinedFieldsService;
        private readonly IEmployeeTypeService _employeeTypeService;
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentDivisionsService _departmentDivisionsService;
        private readonly IEmployeeStatusService _employeeStatusService;
        private readonly ISkillService _skillService;
        private readonly IPaymentTypeService _paymentTypeService;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;
        private readonly ILogger<DepartmentService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentService"/> class.
        /// </summary>
        /// <param name="repository">The department repository.</param>
        /// <param name="departmentConfigurationService">The department configuration service.</param>
        /// <param name="departmentDefinedFieldsService">The department defined fields service.</param>
        /// <param name="employeeTypeService">The employee type service for ensuring MasterType exists.</param>
        /// <param name="employeeService">The employee service for ensuring Master employee exists.</param>
        /// <param name="departmentDivisionsService">The department divisions service.</param>
        /// <param name="employeeStatusService">The employee status service.</param>
        /// <param name="skillService">The skill service.</param>
        /// <param name="paymentTypeService">The payment type service.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="tokenParser">The token parser.</param>
        public DepartmentService(
            IDepartmentRepository repository,
            IDepartmentConfigurationService departmentConfigurationService,
            IDepartmentPolicyService departmentPolicyService,
            IDepartmentDefinedFieldsService departmentDefinedFieldsService,
            IEmployeeTypeService employeeTypeService,
            IEmployeeService employeeService,
            IDepartmentDivisionsService departmentDivisionsService,
            IEmployeeStatusService employeeStatusService,
            ISkillService skillService,
            IPaymentTypeService paymentTypeService,
            IMapper mapper,
            ILogger<DepartmentService> logger,
            ITokenParser tokenParser) : base(repository, mapper, "DepartmentID", tokenParser)
        {
            _departmentRepository = repository;
            _departmentConfigurationService = departmentConfigurationService;
            _departmentPolicyService = departmentPolicyService;
            _departmentDefinedFieldsService = departmentDefinedFieldsService;
            _employeeTypeService = employeeTypeService;
            _employeeService = employeeService;
            _departmentDivisionsService = departmentDivisionsService;
            _employeeStatusService = employeeStatusService;
            _skillService = skillService;
            _paymentTypeService = paymentTypeService;
            _mapper = mapper;
            _tokenParser = tokenParser;
            _logger = logger;
        }

        /// <summary>
        /// Inserts a new DepartmentDto or updates an existing one with duplicate validation.
        /// Also ensures related DepartmentConfiguration and DepartmentDefinedFields records
        /// exist and are linked to the Department via DepartmentID.
        /// </summary>
        /// <param name="dto">The DepartmentDto to insert or update.</param>
        /// <returns>The upserted DepartmentDto.</returns>
        public override async Task<DepartmentDto> UpsertAsync(DepartmentDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);
            var isNewDepartment = dto.DepartmentID == Guid.Empty;
            // Validate for duplicate DepartmentCode (globally unique)
            if (!string.IsNullOrWhiteSpace(dto.DepartmentCode))
            {
                await ValidateDuplicateAsync(
                    propertyName: "DepartmentCode",
                    value: dto.DepartmentCode,
                    dto: dto,
                    isDepartmentScoped: false,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_DEPARTMENT_CODE);
            }

            // Validate for duplicate DepartmentName (globally unique)
            if (!string.IsNullOrWhiteSpace(dto.DepartmentName))
            {
                await ValidateDuplicateAsync(
                    propertyName: "DepartmentName",
                    value: dto.DepartmentName,
                    dto: dto,
                    isDepartmentScoped: false,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_DEPARTMENT_NAME);
            }

            // Validate for duplicate EmployeeDataFolderName (globally unique, case-sensitive handled at DB/query layer)
            if (!string.IsNullOrWhiteSpace(dto.EmployeeDataFolderName))
            {
                await ValidateDuplicateAsync(
                    propertyName: "EmployeeDataFolderName",
                    value: dto.EmployeeDataFolderName,
                    dto: dto,
                    isDepartmentScoped: false,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_EMPLOYEE_DATA_FOLDER_NAME);
            }

            bool isSuperUserFromToken =
                string.Equals(_tokenParser.GetClaimValue(ClaimNames.IsSuperUser), "true", StringComparison.OrdinalIgnoreCase);

            var department = await base.UpsertAsync(dto, !isSuperUserFromToken);
            var departmentId = department.DepartmentID;

            await _departmentConfigurationService.EnsureExistsForDepartmentAsync(departmentId).ConfigureAwait(false);
            await _departmentPolicyService.EnsureExistsForDepartmentAsync(departmentId).ConfigureAwait(false);
            await _departmentDefinedFieldsService.EnsureExistsForDepartmentAsync(departmentId).ConfigureAwait(false);
            var masterTypeId = await _employeeTypeService.UpsertMasterTypeAsync(departmentId).ConfigureAwait(false);
            var (masterEmployeeId, masterCreatedByName) = await _employeeService.UpsertMasterEmployeeAsync(departmentId, department.DepartmentCode ?? "", masterTypeId).ConfigureAwait(false);

            // Seed required master data for dropdowns when a new department is created.
            if (isNewDepartment)
            {
                await _employeeTypeService.EnsureLeoEmployeeTypeExistsAsync(departmentId).ConfigureAwait(false);
                await _employeeStatusService.EnsureActiveStatusExistsForDepartmentAsync(departmentId).ConfigureAwait(false);
                await _departmentDivisionsService.EnsureDefaultDepartmentDivisionExistsAsync(departmentId).ConfigureAwait(false);
                await _skillService.EnsureCprSkillExistsForDepartmentAsync(departmentId).ConfigureAwait(false);
                await _skillService.EnsureSuspendedSkillExistsForDepartmentAsync(departmentId).ConfigureAwait(false);
                await _paymentTypeService.EnsureDefaultPaymentTypesExistAsync(departmentId).ConfigureAwait(false);
            }

            return department;
        }

        /// <inheritdoc />
        public async Task<DepartmentDto?> GetByDepartmentNameAsync(string departmentName)
        {
            var trimmed = departmentName?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                return null;

            var lower = trimmed.ToLowerInvariant();
            var list = await GetAllAsync(
                d => d.DepartmentName != null && d.DepartmentName.ToLower() == lower,
                null,
                null).ConfigureAwait(false);
            return list.FirstOrDefault();
        }

        /// <inheritdoc />
        public async Task<DepartmentDto?> GetByEmployeeDataFolderNameAsync(string folderName)
        {
            try
            {
                var trimmed = folderName?.Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                    return null;

                var lower = trimmed.ToLowerInvariant();
                var list = await GetAllAsync(
                    d => d.EmployeeDataFolderName != null && d.EmployeeDataFolderName.Trim().ToLower() == lower,
                    null,
                    null).ConfigureAwait(false);
                return list.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteDepartmentAsync(Guid departmentId)
        {
            if (departmentId == Guid.Empty)
                throw new ArgumentNullException(nameof(departmentId));

            // Load department first so we can use a friendly name in any error messages
            var department = await GetByIdAsync(departmentId).ConfigureAwait(false);
            if (department == null)
            {
                // Let controller handle NotFound response based on false return
                return false;
            }

            try
            {
                // 1. Delete dependent records first (using generic DeleteByPropertyAsync via service)
                await _departmentConfigurationService.DeleteByDepartmentIdAsync(departmentId).ConfigureAwait(false);
                await _departmentDefinedFieldsService.DeleteByDepartmentIdAsync(departmentId).ConfigureAwait(false);

                // 2. Delete department itself
                var result = await _departmentRepository.DeleteAsync(departmentId).ConfigureAwait(false);
                return result;
            }
            catch (WebApiException)
            {
                // Preserve any domain-level WebApiException as-is
                throw;
            }
            catch (Exception ex)
            {
                // Detect foreign key / constraint violations and surface a friendly message
                var message = ex.InnerException?.Message ?? ex.Message;
                var isForeignKeyViolation =
                    ex.GetType().FullName?.Contains(ModelValidationErrorCodes.DbUpdateExceptionTypeName) == true
                    || message.Contains(ModelValidationErrorCodes.ForeignKeyMessageSubstring, StringComparison.OrdinalIgnoreCase)
                    || message.Contains(ModelValidationErrorCodes.ReferenceConstraintMessageSubstring, StringComparison.OrdinalIgnoreCase);

                if (isForeignKeyViolation)
                {
                    var name = string.IsNullOrWhiteSpace(department.DepartmentName)
                        ? ModelValidationErrorCodes.DefaultDepartmentEntityName
                        : department.DepartmentName;

                    throw new WebApiException(
                        HttpStatusCode.BadRequest,
                        ((int)HttpStatusCode.BadRequest).ToString(),
                        ModelValidationErrorCodes.GetErrorMessage(
                            ModelValidationErrorCodes.DELETE_DEPENDENT_RECORDS_FIRST,
                            name));
                }

                // Re-throw unexpected exceptions to be handled by global filters
                throw;
            }
        }

        public override Task<PagedList<DepartmentDto>> SearchAsync(
            string? searchTerm,
            PageParam? pageParam = null,
            Expression<Func<Department, bool>>? filter = null,
            Expression<Func<Department, object>>[]? includes = null,
            params string[] searchableProperties)
        {
            filter = filter ?? (Expression<Func<Department, bool>>)(x => x.Active == true);

            var effectiveSearchableProperties =
                (searchableProperties?.Length > 0)
                    ? searchableProperties
                    : new[]
                    {
                        nameof(Department.DepartmentCode),
                        nameof(Department.DepartmentName),
                        nameof(Department.EmployeeDataFolderName),
                        nameof(Department.Phone),
                        nameof(Department.City),
                        nameof(Department.State)
                    };

            return base.SearchAsync(
                searchTerm,
                pageParam,
                filter,
                includes,
                effectiveSearchableProperties
            );
        }

        public async Task<DepartmentLocationDto?> GetDepartmentLocationAsync(CancellationToken cancellationToken = default)
        {
            var targetId = _tokenParser.GetDepartmentID();
            if (!targetId.HasValue || targetId.Value == Guid.Empty)
            {
                return null;
            }

            var department = await _departmentRepository.GetByIdAsync(targetId.Value, "DepartmentID").ConfigureAwait(false);
            if (department == null)
            {
                return null;
            }

            // Check if a Department Home Location is configured in DepartmentConfiguration
            var deptConfig = await _departmentConfigurationService.GetByDepartmentIdAsync(targetId.Value).ConfigureAwait(false);
            decimal? effectiveLat = null;
            decimal? effectiveLon = null;
            string? effectiveAddress = department.Address;

            if (deptConfig?.EnableEmployeeLocation == true && deptConfig.DepartmentHomeLatitude.HasValue == true && deptConfig.DepartmentHomeLongitude.HasValue == true)
            {
                effectiveLat = deptConfig.DepartmentHomeLatitude;
                effectiveLon = deptConfig.DepartmentHomeLongitude;
                if (!string.IsNullOrWhiteSpace(deptConfig.DepartmentHomeLocation))
                {
                    effectiveAddress = deptConfig.DepartmentHomeLocation;
                }
            }

            return new DepartmentLocationDto
            {
                DepartmentID = department.DepartmentID,
                DepartmentCode = department.DepartmentCode,
                DepartmentName = department.DepartmentName,
                Latitude = effectiveLat,
                Longitude = effectiveLon,
                Address = effectiveAddress,
                City = department.City,
                State = department.State,
                Zip = department.Zip,
                Phone = department.Phone,
                Email = department.Email,
                TimeZoneId = department.TimeZoneId,
                Active = department.Active
            };
        }
    }
}

