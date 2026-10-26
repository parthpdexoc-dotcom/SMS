using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SMS.Data.Migrations;
using SMS.Generic.Common.Constants;
using SMS.Generic.Common;
using SMS.Generic.Common.Enums;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Helper;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Helpers;
using SMS.Generic.Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing employee operations using the generic service pattern.
    /// </summary>
    public class EmployeeService : GenericService<EmployeeDto, Employee>, IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly IEmployeeDeviceTokenService _employeeDeviceTokenService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmployeeService> _logger;
        private readonly ITokenParser _tokenParser;
        private readonly IEmployeeTypeService _employeeTypeService;
        private readonly ISkillService _skillService;
        private readonly IEmployeeTenureService _employeeTenureService;
        private readonly IRequesterManagerService _requesterManagerService;
        private readonly IDepartmentDivisionsService _departmentDivisionsService;
        private readonly IEmployeeStatusService _employeeStatusService;
        private readonly IValidator<EmployeeDto> _employeeValidator;
        private readonly IEmployeeSkillRepository _employeeSkillRepository;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmployeeValidationService _employeeValidationService;
        private readonly IDepartmentConfigurationService _departmentConfigurationService;
        private readonly IPostPositionRepository _postPositionRepository;
        private readonly IPostWorkflowAuditService _postWorkflowAuditService;
        private readonly IUploadFileService _uploadFileService;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly byte[] _loginClientKey;
        private readonly byte[] _loginClientIv;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeService"/> class.
        /// </summary>
        /// <param name="repository">The employee repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="tokenParser">The token parser service.</param>
        public EmployeeService(
            IEmployeeRepository repository,
            IEmployeeDeviceTokenService employeeDeviceTokenService,
            IMapper mapper,
            ILogger<EmployeeService> logger,
            IConfiguration configuration,
            ITokenParser tokenParser,
            IEmployeeTypeService employeeTypeService,
            ISkillService skillService,
            IEmployeeTenureService employeeTenureService,
            IRequesterManagerService requesterManagerService,
            IDepartmentDivisionsService departmentDivisionsService,
            IEmployeeStatusService employeeStatusService,
            IEmployeeSkillRepository employeeSkillRepository,
            IValidator<EmployeeDto> employeeValidator,
            IEmailTemplateService emailTemplateService,
            IEmployeeValidationService employeeValidationService,
            IDepartmentConfigurationService departmentConfigurationService,
            IPostPositionRepository postPositionRepository,
            IPostWorkflowAuditService postWorkflowAuditService,
            IUploadFileService uploadFileService) : base(repository, mapper, "EmployeeID", tokenParser)
        {
            _repository = repository;
            _employeeDeviceTokenService = employeeDeviceTokenService;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _tokenParser = tokenParser ?? throw new ArgumentNullException(nameof(tokenParser));
            _employeeTypeService = employeeTypeService;
            _skillService = skillService;
            _employeeTenureService = employeeTenureService;
            _requesterManagerService = requesterManagerService;
            _departmentDivisionsService = departmentDivisionsService;
            _employeeStatusService = employeeStatusService;
            _employeeValidator = employeeValidator;
            _employeeSkillRepository = employeeSkillRepository;
            _emailTemplateService = emailTemplateService ?? throw new ArgumentNullException(nameof(emailTemplateService));
            _employeeValidationService = employeeValidationService ?? throw new ArgumentNullException(nameof(employeeValidationService));
            _departmentConfigurationService = departmentConfigurationService ?? throw new ArgumentNullException(nameof(departmentConfigurationService));
            _postPositionRepository = postPositionRepository ?? throw new ArgumentNullException(nameof(postPositionRepository));
            _postWorkflowAuditService = postWorkflowAuditService ?? throw new ArgumentNullException(nameof(postWorkflowAuditService));
            _uploadFileService = uploadFileService ?? throw new ArgumentNullException(nameof(uploadFileService));
            _secretKey = _configuration["JwtSettings:SecretKey"]
                ?? throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_JWT_SECRET_NOT_CONFIGURED));
            _issuer = _configuration["JwtSettings:Issuer"]
                ?? ModelErrorCodes.GetErrorMessage(ModelErrorCodes.JWT_DEFAULT_ISSUER);
            _audience = _configuration["JwtSettings:Audience"]
                ?? ModelErrorCodes.GetErrorMessage(ModelErrorCodes.JWT_DEFAULT_AUDIENCE);
            _loginClientKey = Encoding.UTF8.GetBytes(_configuration["LoginClient:Key"] ?? string.Empty);
            _loginClientIv = Encoding.UTF8.GetBytes(_configuration["LoginClient:Iv"] ?? string.Empty);
        }

        /// <summary>
        /// Partially updates an employee using JSON Patch. All patch application logic is handled
        /// here so the controller stays thin. Validation errors are thrown as WebApiException.
        /// </summary>
        /// <param name="id">The employee identifier from the route.</param>
        /// <param name="employeeOperations">The JSON Patch document describing the changes.</param>
        /// <returns>The updated employee DTO, or null if not found.</returns>
        public async Task<EmployeeDto?> PatchAsync(
            Guid id,
            Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<EmployeeDto> employeeOperations)
        {
            if (employeeOperations == null)
                throw new ArgumentNullException(nameof(employeeOperations));

            // Only allow "replace" operations for safe partial updates
            var invalidOp = employeeOperations.Operations
                .FirstOrDefault(op => op.OperationType != OperationType.Replace);
            if (invalidOp != null)
            {
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    "400",
                    ModelErrorCodes.GetErrorMessage(ModelErrorCodes.EMPLOYEE_PATCH_ONLY_REPLACE_ALLOWED));
            }

            var existing = await GetByIdAsync(id);
            if (existing == null)
                return null;

            var dto = new EmployeeDto();
            dto.CloneEmployeeForPatchMerge(existing);

            // Apply patch to DTO; wrap any patch errors in a WebApiException understood by the global filter
            try
            {
                employeeOperations.ApplyTo(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to apply JSON patch to employee {EmployeeId}", id);
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    "400",
                    ModelErrorCodes.GetErrorMessage(ModelErrorCodes.EMPLOYEE_PATCH_INVALID_DOCUMENT));
            }

            // Run FluentValidation on the patched DTO so rules like EmailAddress, NotEmpty, MaxLength are enforced.
            var validationResult = await _employeeValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var firstError = validationResult.Errors.First();
                var errorKey = firstError.ErrorMessage; // validators use ErrorMessage as error *key*
                var userMessage = ModelErrorCodes.GetErrorMessage(errorKey);

                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    "400",
                    userMessage);
            }

            // When PATCH deactivates an employee, block if the employee is used in ongoing post positions.
            if (existing.IsApproved && !dto.IsApproved && dto.EmployeeID.HasValue)
                await _employeeValidationService.ValidateCanDeactivateEmployeeAsync(dto.EmployeeID.Value).ConfigureAwait(false);

            var result = await UpsertAsync(dto);

            if (dto.EmployeeID.HasValue)
                await CancelScheduledPostPositionsForSuspendedEmployeeAsync(dto.EmployeeID.Value, _tokenParser.GetDepartmentID()).ConfigureAwait(false);

            return result;
        }

        private async Task<HashSet<Guid>> GetSuspendedSkillIdsForDepartmentAsync(Guid departmentId)
        {
            var suspendedSkills = await _skillService.GetAllAsync(
                    s => s.DepartmentID == departmentId &&
                         ((s.SkillShort != null && s.SkillShort.ToLower() == "suspended") ||
                          (s.SkillLong != null && s.SkillLong.ToLower() == "suspended")),
                    null,
                    null)
                .ConfigureAwait(false);

            return suspendedSkills
                .Select(s => s.SkillID.GetValueOrDefault())
                .Where(skillId => skillId != Guid.Empty)
                .ToHashSet();
        }

        private async Task CancelScheduledPostPositionsForSuspendedEmployeeAsync(Guid employeeId, Guid? effectiveDepartmentId)
        {
            var now = GetCurrentDepartmentDateTime();

            var employee = await base.GetByIdAsync(employeeId, e => e.EmployeeSkills).ConfigureAwait(false);
            if (employee?.EmployeeSkills is not { Count: > 0 })
                return;

            var departmentIdForSuspendedSkills = employee.GetDepartmentIdForSuspendedSkillLookup(effectiveDepartmentId);
            if (departmentIdForSuspendedSkills == Guid.Empty)
                return;

            var suspendedSkillIds = await GetSuspendedSkillIdsForDepartmentAsync(departmentIdForSuspendedSkills).ConfigureAwait(false);
            if (suspendedSkillIds.Count == 0)
                return;

            employee.PopulateSuspendedSkillPeriods(suspendedSkillIds);
            if (employee.SuspendedSkillPeriods.Count == 0)
                return;

            Expression<Func<PostPosition, object>>[] includes =
            {
                x => x.Post,
                x => x.Post.Request
            };

            var entities = await _postPositionRepository.GetAssignedWithDateOfWorkAsync(
                employeeId,
                restrictToEmployee: true,
                effectiveDepartmentId,
                new[]
                {
                    PostPositionStatus.Assigned,
                    PostPositionStatus.SplitDecisionPending,
                    PostPositionStatus.Unclosed,
                    PostPositionStatus.CancellationPending
                },
                includes).ConfigureAwait(false);

            var toCancel = employee.SelectPostPositionsToCancelForSuspendedSchedule(
                entities,
                now,
                JobDateHelper.IsPostPositionDateTimePassed);

            if (toCancel.Count == 0)
                return;

            foreach (var e in toCancel)
                e.PostPositionStatus = PostPositionStatus.Cancelled;

            try
            {
                await _postPositionRepository.BulkUpsertAsync(toCancel, "PostPositionID").ConfigureAwait(false);

                foreach (var e in toCancel)
                {
                    await _postWorkflowAuditService.LogSuccessAsync(
                        PostPositionStatus.Cancelled,
                        new PostWorkflowAuditContext
                        {
                            PostPositionId = e.PostPositionID,
                            PostId = e.PostID
                        }).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                await _postWorkflowAuditService.LogFailureAsync(
                    PostPositionStatus.Cancelled,
                    ex,
                    new PostWorkflowAuditContext
                    {
                        EmployeeId = employeeId,
                        FailureReason = ModelValidationErrorCodes.GetErrorMessage(
                            ModelValidationErrorCodes.EMPLOYEE_SUSPENDED_BULK_CANCEL_SCHEDULED_JOB_POSITIONS_FAILED,
                            toCancel.Count)
                    }).ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Retrieves a single employee by ID and populates SkillIDs from the EmployeeSkill table.
        /// </summary>
        public override async Task<EmployeeDto> GetByIdAsync(Guid id)
        {
            var employee = await _repository.GetByIdWithSkillsAsync(id).ConfigureAwait(false);
            var dto = _mapper.Map<EmployeeDto>(employee);
            if (dto == null || !dto.EmployeeID.HasValue)
            {
                return dto;
            }

            var suspendedSkillIds = await GetSuspendedSkillIdsForDepartmentAsync(dto.DepartmentID);
            dto.PopulateSuspendedSkillPeriods(suspendedSkillIds);
            return dto;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SelectOptionDto>> GetOptionsAsync(
            string? searchTerm = null,
            IReadOnlyList<Guid>? excludeEmployeeIds = null)
        {
            Expression<Func<Employee, bool>> filter = e => !e.EmployeeType!.IsSystemUser;
            if (excludeEmployeeIds is { Count: > 0 })
            {
                var excluded = excludeEmployeeIds;
                Expression<Func<Employee, bool>> excludeFilter = e => !excluded.Contains(e.EmployeeID);
                filter = CombineFilterWithExcludeMaster(filter, excludeFilter);
            }

            return await GetOptionsAsync(
                e => new SelectOptionDto
                {
                    Id = e.EmployeeID,
                    Label = (e.LastName ?? string.Empty) + ", " + (e.FirstName ?? string.Empty)
                },
                filter,
                searchTerm,
                e => e.FirstName,
                e => e.LastName).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<EmployeeDto>> GetAllAsync(Expression<Func<Employee, bool>>? filter = null, Expression<Func<Employee, object>>[] includes = null, PageParam pageParam = null)
        {
            // Exclude Master (system) employees from GetAll - they are hidden and cannot login
            Expression<Func<Employee, bool>> excludeMaster = e => !e.EmployeeType!.IsSystemUser;
            var effectiveFilter = filter == null
                ? excludeMaster
                : CombineFilterWithExcludeMaster(filter, excludeMaster);
            return await base.GetAllAsync(effectiveFilter, includes, pageParam);
        }

        private static Expression<Func<Employee, bool>> CombineFilterWithExcludeMaster(
            Expression<Func<Employee, bool>> filter,
            Expression<Func<Employee, bool>> excludeMaster)
        {
            var parameter = filter.Parameters[0];
            var invokedMaster = Expression.Invoke(excludeMaster, parameter);
            var combined = Expression.AndAlso(filter.Body, invokedMaster);
            return Expression.Lambda<Func<Employee, bool>>(combined, parameter);
        }

        public override async Task<EmployeeDto> UpsertAsync(EmployeeDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);
            var plainTextPasswordForEmail = dto.Password;

            // Validate EmployeeTypeID exists in database if provided
            await ValidateForeignKeyAsync<IEmployeeTypeService, EmployeeTypeDto>(_employeeTypeService, dto.EmployeeTypeID, "EmployeeType");

            if (dto.EmployeeSkills != null && dto.EmployeeSkills.Any())
            {
                await _skillService.ValidateSkillIdsAsync(dto.EmployeeSkills.Select(s => s.SkillID).ToList());
            }

            bool isUpdate = dto.EmployeeID.HasValue && dto.EmployeeID != Guid.Empty;
            Guid? originalEmployeeTypeID = null;

            // 🔹 RANK VALIDATION - Check if new EmployeeTypeID has higher rank than current (only for updates with EmployeeTypeID change)
            if (isUpdate && dto.EmployeeTypeID.HasValue)
            {
                // Get current employee to check existing EmployeeTypeID
                var currentEmployee = await GetByIdAsync(dto.EmployeeID.Value);
                if (currentEmployee != null)
                {
                    originalEmployeeTypeID = currentEmployee.EmployeeTypeID;

                    // Validate rank promotion if EmployeeTypeID is changing
                    if (currentEmployee.EmployeeTypeID != dto.EmployeeTypeID.Value)
                    {
                        await _employeeTypeService.ValidateRankPromotionAsync(currentEmployee.EmployeeTypeID, dto.EmployeeTypeID.Value);
                    }
                }
            }

            // 🔹 DUPLICATE CHECK (CREATE + UPDATE SAFE)
            await ValidateEmployeeUniqueAsync(
                dto.Username,
                dto.BadgeNumber,
                dto.EmployeeNumber,
                isUpdate ? dto.EmployeeID : null,
                dto.DepartmentID);

            // 🔹 PASSWORD RULES
            if (!isUpdate)
            {
                if (!string.IsNullOrWhiteSpace(dto.Password))
                    dto.Password = PasswordHelper.HashPassword(dto.Password);

                var existingEmployeesInDepartment = await GetAllAsync(filter: e => e.DepartmentID == dto.DepartmentID, includes: null, pageParam: null);

                bool isFirstEmployeeInDepartment = !existingEmployeesInDepartment.Any();

                if (isFirstEmployeeInDepartment)
                {
                    // For the first employee, set audit fields to null to avoid self-referencing FK issues
                    dto.CreatedDate = DateTime.UtcNow;
                    dto.CreatedUserID = null; // No creator for the first employee
                    dto.CreatedBy = "Super User"; // Or get admin name from token if available
                    dto.EditedDate = null;
                    dto.EditedUserID = null;
                    dto.EditedBy = null;
                    var entity = _mapper.Map<Employee>(dto);
                    await _repository.UpsertAsync(entity, "EmployeeID");
                    // entity now contains generated EmployeeID
                    var result = _mapper.Map<EmployeeDto>(entity);

                    // Create EmployeeTenure record for the new employee
                    if (dto.EmployeeTypeID.HasValue)
                    {
                        await CreateEmployeeTenureAsync(
                            result.EmployeeID.Value,
                            dto.DepartmentID,
                            dto.EmployeeTypeID.Value,
                            dto.ImportedDateOfHire,
                            dto.ImportedDateOfPromotion,
                            true);
                    }

                    return result;

                }
            }
            else if (string.IsNullOrEmpty(dto.Password))
            {
                var existingEntity = await GetByIdAsync(dto.EmployeeID.Value);
                if (existingEntity == null)
                    throw new KeyNotFoundException(ModelErrorCodes.GetErrorMessage(
                        ModelErrorCodes.GENERIC_ENTITY_WITH_ID_NOT_FOUND,
                        "Employee",
                        dto.EmployeeID));

                dto.Password = existingEntity.Password;
            }
            else if (!PasswordHelper.IsPasswordHashed(dto.Password))
            {
                dto.Password = PasswordHelper.HashPassword(dto.Password);
            }

            // Updates: tenure first so promotion-date/duplicate rules fail before EmployeeTypeID is saved.
            await ValidateAndCreateEmployeeTenureIfNeededAsync(
                isUpdate,
                dto,
                dto.EmployeeID,
                originalEmployeeTypeID);

            var upsertedEmployee = await base.UpsertAsync(dto);

            await ValidateAndCreateEmployeeTenureIfNeededAsync(
                !isUpdate,
                dto,
                upsertedEmployee.EmployeeID,
                originalEmployeeTypeID);

            await SendEmployeePasswordResetEmailByAdminAsync(isUpdate, upsertedEmployee, plainTextPasswordForEmail);

            return upsertedEmployee;
        }

        public async Task SendEmployeePasswordResetEmailByAdminAsync(
            bool isUpdate,
            EmployeeDto upsertedEmployee,
            string? plainTextPassword,
            bool bypassRoleCheck = false)
        {
            if (string.IsNullOrWhiteSpace(plainTextPassword))
                return;

            if (PasswordHelper.IsPasswordHashed(plainTextPassword))
                return;

            if (!bypassRoleCheck && !IsCurrentUserSuperUserOrDepartmentAdmin())
                return;

            var recipientEmail = !string.IsNullOrWhiteSpace(upsertedEmployee.OffDutyCallBackEmail) ? upsertedEmployee.OffDutyCallBackEmail!.Trim() : upsertedEmployee.PrimaryEmail?.Trim();

            if (string.IsNullOrEmpty(recipientEmail))
            {
                await _emailTemplateService.SendEmployeeEmailUnavailableNotificationAsync(
                    upsertedEmployee,
                    EmailType.EmployeePasswordResetByAdminEmail,
                    CancellationToken.None).ConfigureAwait(false);
                return;
            }

            var fullName = PersonNameFormatHelper.FormatLastNameCommaFirstName(upsertedEmployee.LastName, upsertedEmployee.FirstName) ?? string.Empty;

            var templateParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // Keep both legacy and new keys so existing templates continue to work.
                { EnumHelper.ToStringValue(EmailKeyword.NewPassword), plainTextPassword.Trim() },
                { EnumHelper.ToStringValue(EmailKeyword.Password), plainTextPassword.Trim() },
                { EnumHelper.ToStringValue(EmailKeyword.Employee), fullName },
                { EnumHelper.ToStringValue(EmailKeyword.FullName), fullName },
                { EnumHelper.ToStringValue(EmailKeyword.UserName), upsertedEmployee.Username ?? string.Empty },
                { EnumHelper.ToStringValue(EmailKeyword.DepartmentName), string.Empty }
            };

            var sendMailRequest = new SendMailRequestDto(
                EmailType.EmployeePasswordResetByAdminEmail,
                recipientEmail,
                fromEmail: null,
                templateParams);

            await _emailTemplateService.SendMailAsync(sendMailRequest, CancellationToken.None);
        }

        private bool IsCurrentUserSuperUserOrDepartmentAdmin()
        {
            if (string.Equals(_tokenParser.GetClaimValue(ClaimNames.IsSuperUser), "true", StringComparison.OrdinalIgnoreCase))
                return true;

            var adminTypeShort = _tokenParser.GetClaimValue(ClaimNames.AdminTypeShort);
            if (NormalizeRole(adminTypeShort) == NormalizeRole(RoleTypeNames.DepartmentAdmin()))
                return true;

            var adminTypesJson = _tokenParser.GetClaimValue(ClaimNames.AdminTypes);
            if (string.IsNullOrWhiteSpace(adminTypesJson))
                return false;

            try
            {
                var adminTypes = JsonSerializer.Deserialize<List<string>>(adminTypesJson) ?? new List<string>();
                var normalizedDepartmentAdmin = NormalizeRole(RoleTypeNames.DepartmentAdmin());
                return adminTypes.Any(role => NormalizeRole(role) == normalizedDepartmentAdmin);
            }
            catch
            {
                return false;
            }
        }

        private static string NormalizeRole(string? value) =>
            string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : new string(value.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLowerInvariant();

        public override async Task<IEnumerable<EmployeeDto>> BulkUpsertAsync(IEnumerable<EmployeeDto> dtos, bool isSuperUser = false)
        {
            if (dtos == null)
                throw new ArgumentNullException(nameof(dtos));

            var dtosList = dtos.ToList();
            if (!dtosList.Any())
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_DTOS_COLLECTION_EMPTY), nameof(dtos));

            var validDtos = new List<EmployeeDto>();
            var metadata = new List<(bool IsUpdate, Guid? OriginalEmployeeTypeID)>();

            foreach (var dto in dtosList)
            {
                try
                {
                    ValidateDtoParameter(dto);

                    await ValidateForeignKeyAsync<IEmployeeTypeService, EmployeeTypeDto>(_employeeTypeService, dto.EmployeeTypeID, "EmployeeType");

                    if (dto.EmployeeSkills != null && dto.EmployeeSkills.Any())
                    {
                        await _skillService.ValidateSkillIdsAsync(dto.EmployeeSkills.Select(e => e.SkillID).ToList());
                    }

                    bool isUpdate = dto.EmployeeID.HasValue && dto.EmployeeID != Guid.Empty;
                    Guid? originalEmployeeTypeID = null;

                    if (isUpdate && dto.EmployeeTypeID.HasValue)
                    {
                        var currentEmployee = await GetByIdAsync(dto.EmployeeID.Value);
                        if (currentEmployee != null)
                        {
                            originalEmployeeTypeID = currentEmployee.EmployeeTypeID;

                            if (currentEmployee.EmployeeTypeID != dto.EmployeeTypeID.Value)
                            {
                                await _employeeTypeService.ValidateRankPromotionAsync(currentEmployee.EmployeeTypeID, dto.EmployeeTypeID.Value);
                            }
                        }
                    }

                    await ValidateEmployeeUniqueAsync(
                        dto.Username,
                        dto.BadgeNumber,
                        dto.EmployeeNumber,
                        isUpdate ? dto.EmployeeID : null,
                        dto.DepartmentID);

                    if (!isUpdate)
                    {
                        if (!string.IsNullOrWhiteSpace(dto.Password))
                            dto.Password = PasswordHelper.HashPassword(dto.Password);
                    }
                    else if (string.IsNullOrEmpty(dto.Password))
                    {
                        var existingEntity = await GetByIdAsync(dto.EmployeeID.Value);
                        if (existingEntity == null)
                            throw new KeyNotFoundException(ModelErrorCodes.GetErrorMessage(
                                ModelErrorCodes.GENERIC_ENTITY_WITH_ID_NOT_FOUND,
                                "Employee",
                                dto.EmployeeID));

                        dto.Password = existingEntity.Password;
                    }

                    validDtos.Add(dto);
                    metadata.Add((isUpdate, originalEmployeeTypeID));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "{Message}",
                        ModelErrorCodes.GetErrorMessage(ModelErrorCodes.LOG_EMPLOYEE_BULK_UPSERT_RECORD_SKIPPED));
                }
            }

            if (!validDtos.Any())
            {
                _logger.LogWarning(
                    "{Message}",
                    ModelErrorCodes.GetErrorMessage(ModelErrorCodes.LOG_EMPLOYEE_BULK_UPSERT_NO_VALID_RECORDS));
                return Array.Empty<EmployeeDto>();
            }

            var upsertedEmployees = (await base.BulkUpsertAsync(validDtos, isSuperUser)).ToList();
            var tenureDtos = new List<EmployeeTenureDto>();

            for (int i = 0; i < upsertedEmployees.Count; i++)
            {
                var upsertedEmployee = upsertedEmployees[i];
                var dto = validDtos[i];
                var meta = metadata[i];

                if (!upsertedEmployee.EmployeeID.HasValue || !dto.EmployeeTypeID.HasValue)
                    continue;

                if (await ShouldCreateEmployeeTenureAsync(
                    upsertedEmployee.EmployeeID.Value,
                    meta.IsUpdate,
                    meta.OriginalEmployeeTypeID,
                    dto.EmployeeTypeID.Value))
                {
                    tenureDtos.Add(new EmployeeTenureDto(
                                            null,
                                            dto.DepartmentID,
                                            upsertedEmployee.EmployeeID.Value,
                                            dto.EmployeeTypeID.Value,
                                            dto.ImportedDateOfHire,
                                            dto.ImportedDateOfPromotion,
                                            true));
                }
            }

            if (tenureDtos.Any())
            {
                await _employeeTenureService.BulkUpsertAsync(tenureDtos);
            }

            return upsertedEmployees;
        }

        /// <summary>
        /// Validates that an employee with the same username, badge number, or employee number doesn't already exist.
        /// Throws an exception if a duplicate is found.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <param name="badgeNumber">The badge number to check.</param>
        /// <param name="employeeNumber">The employee number to check.</param>
        /// <param name="excludeEmployeeId">The employee ID to exclude from the check (for updates).</param>
        /// <param name="departmentId">Optional department ID to scope the duplicate check.</param>
        /// <exception cref="InvalidOperationException">Thrown when a duplicate employee is found.</exception>
        private async Task ValidateEmployeeUniqueAsync(
            string username,
            string badgeNumber,
            string? employeeNumber,
            Guid? excludeEmployeeId = null,
            Guid? departmentId = null)
        {
            var existingEmployees = await GetAllAsync(filter: e =>
                (e.Username == username || e.BadgeNumber == badgeNumber) &&
                (!departmentId.HasValue || e.DepartmentID == departmentId.Value),
                includes: null,
                pageParam: null);

            bool exists = existingEmployees.Any(e => !excludeEmployeeId.HasValue || e.EmployeeID != excludeEmployeeId.Value);

            if (exists)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.EMPLOYEE_DUPLICATE_BY_USERNAME_BADGE_OR_NUMBER));
        }

        private async Task<bool> ShouldCreateEmployeeTenureAsync(
            Guid employeeID,
            bool isUpdate,
            Guid? originalEmployeeTypeID,
            Guid employeeTypeID)
        {
            if (!isUpdate)
                return true;

            var existingTenures = (await _employeeTenureService.GetAllAsync(
                x => x.EmployeeID == employeeID,
                null,
                null)).ToList();

            return !existingTenures.Any() || originalEmployeeTypeID != employeeTypeID;
        }

        private async Task ValidateAndCreateEmployeeTenureIfNeededAsync(
            bool shouldRun,
            EmployeeDto dto,
            Guid? employeeId,
            Guid? originalEmployeeTypeID)
        {
            if (!shouldRun || !employeeId.HasValue || !dto.EmployeeTypeID.HasValue)
                return;

            var isUpdate = dto.EmployeeID.HasValue && dto.EmployeeID != Guid.Empty;
            if (await ShouldCreateEmployeeTenureAsync(
                employeeId.Value,
                isUpdate,
                originalEmployeeTypeID,
                dto.EmployeeTypeID.Value))
            {
                await CreateEmployeeTenureAsync(
                    employeeId.Value,
                    dto.DepartmentID,
                    dto.EmployeeTypeID.Value,
                    dto.ImportedDateOfHire,
                    dto.ImportedDateOfPromotion);
            }
        }

        /// <summary>
        /// Creates or updates EmployeeTenure record for an employee.
        /// </summary>
        private async Task CreateEmployeeTenureAsync(
            Guid employeeID,
            Guid departmentID,
            Guid employeeTypeID,
            DateOnly? dateOfHire = null,
            DateOnly? dateOfPromotion = null,
            bool isSuperUser = false)
        {
            var tenureDto = new EmployeeTenureDto(
                null,
                departmentID,
                employeeID,
                employeeTypeID,
                dateOfHire,
                dateOfPromotion,
                true);

            // Create the tenure record (EmployeeTenureService will handle setting DateOfHire, DateOfPromotion, and existing IsCurrent records to false)
            await _employeeTenureService.UpsertAsync(tenureDto, isSuperUser);
        }
        public async Task<PagedList<EmployeeResponseDto>> GetAllEmployeeWithDetailsAsync(PageParam? pageParam, Guid? selectedId = null)
        {
            // Exclude Master (system) employees - hidden, cannot login
            Expression<Func<Employee, bool>> filter = x =>
                !x.EmployeeType!.IsSystemUser &&
                (x.Status.Status == true || (selectedId.HasValue && x.EmployeeID == selectedId.Value));
            return await GetEmployeesWithStatusAndTypeByFilterAsync(filter, pageParam);
        }

        public async Task<PagedList<EmployeeResponseDto>> GetAllEmployees(PageParam? pageParam, bool? isActive, Guid? departmentId = null)
        {
            var applyDepartmentWiseFilter = departmentId.HasValue && _tokenParser.IsInRole(RoleTypeNames.SuperUser());
            var effectiveDepartmentId = departmentId.GetValueOrDefault();

            Expression<Func<Employee, bool>> filter = x =>
                !x.EmployeeType!.IsSystemUser &&
                (!isActive.HasValue || x.IsApproved == isActive.Value) &&
                (!applyDepartmentWiseFilter || x.DepartmentID == effectiveDepartmentId);

            return await GetEmployeesWithStatusAndTypeByFilterAsync(filter, pageParam);
        }

        public async Task<PagedList<EmployeeResponseDto>> GetAllEmployeesByRankFilter(
            PageParam? pageParam,
            bool? isActive,
            Guid? departmentId = null,
            string? searchTerm = null,
            Guid? employeeTypeId = null)
        {
            pageParam ??= new PageParam();

            searchTerm = searchTerm?.Trim();

                var isDepartmentAdmin = _tokenParser.IsDepartmentAdmin();
                var currentEmployeeId = _tokenParser.GetEmployeeID();
            int? employeeRank = null;
                if (!isDepartmentAdmin)
                    {

                    var currentEmployee = await GetByIdAsync(currentEmployeeId.Value);
                    if (currentEmployee?.EmployeeTypeID is not Guid currentEmployeeTypeId || currentEmployeeTypeId == Guid.Empty)
                        return new PagedList<EmployeeResponseDto>(Enumerable.Empty<EmployeeResponseDto>(), pageParam, 0);

                    var currentEmployeeType = await _employeeTypeService.GetByIdAsync(currentEmployeeTypeId);
                    employeeRank = currentEmployeeType.Rank;
                }

                Expression<Func<Employee, bool>> filter = x =>
                    !x.EmployeeType!.IsSystemUser &&
                    x.EmployeeID != currentEmployeeId.Value &&
                    (isDepartmentAdmin || x.EmployeeType.Rank > employeeRank!.Value) &&
                    (!isActive.HasValue || x.IsApproved == isActive.Value) &&
                    (!employeeTypeId.HasValue || x.EmployeeTypeID == employeeTypeId.Value) &&
                    (string.IsNullOrWhiteSpace(searchTerm) ||
                        x.FirstName!.Contains(searchTerm) ||
                        x.LastName!.Contains(searchTerm) ||
                        (x.FirstName + " " + x.LastName).Contains(searchTerm));

                return await GetEmployeesWithStatusAndTypeByFilterAsync(filter, pageParam);
            }

        private async Task<PagedList<EmployeeResponseDto>> GetEmployeesWithStatusAndTypeByFilterAsync(
            Expression<Func<Employee, bool>> filter,
            PageParam? pageParam)
        {
            Expression<Func<Employee, object>>[] includes = { e => e.Status, e => e.EmployeeType };
            var paged = await GetAllPagedAsync(filter, pageParam, includes).ConfigureAwait(false);
            var dtos = _mapper.Map<IEnumerable<EmployeeResponseDto>>(paged.Data);
            return new PagedList<EmployeeResponseDto>(dtos, paged, paged.TotalRecords);
        }
        /// <summary>
        /// Searches employees across the full dataset (no pagination-based filtering of the search)
        /// and returns a paged list where TotalRecords reflects all matches.
        /// </summary>
        /// <param name="searchTerm">Free-text search term (name, badge, username, etc.).</param>
        /// <param name="employeeTypeId">Optional EmployeeType filter. When provided, only employees with this EmployeeTypeID are returned.</param>
        /// <param name="pageParam">Optional paging/sorting parameters for the result page.</param>
        public async Task<PagedList<EmployeeResponseDto>> SearchEmployeesAsync(
            string? searchTerm,
            Guid? employeeTypeId,
            bool? isActive,
            Guid? departmentId,
            PageParam? pageParam)
        {
            var applyDepartmentWiseFilter = departmentId.HasValue && _tokenParser.IsInRole(RoleTypeNames.SuperUser());
            var effectiveDepartmentId = departmentId.GetValueOrDefault();

            Expression<Func<Employee, object>>[] includes =
                { e => e.Status, e => e.EmployeeType };

            Expression<Func<Employee, bool>> filter = x =>
                !x.EmployeeType!.IsSystemUser &&
                (!employeeTypeId.HasValue || x.EmployeeTypeID == employeeTypeId.Value) &&
                (!isActive.HasValue || x.IsApproved == isActive.Value) &&
                (!applyDepartmentWiseFilter || x.DepartmentID == effectiveDepartmentId);

            var searchResult = await SearchAsync(
                searchTerm,
                pageParam,
                filter,
                includes,
                nameof(Employee.FirstName),
                nameof(Employee.LastName),
                nameof(Employee.BadgeNumber),
                nameof(Employee.Username)
            );

            var employeeResponseDtos =
                _mapper.Map<IEnumerable<EmployeeResponseDto>>(searchResult.Data);

            return new PagedList<EmployeeResponseDto>(
                employeeResponseDtos,
                searchResult,
                searchResult.TotalRecords);
        }

        /// <summary>
        /// Validates employee eligibility for a post position.
        /// Validates skills (combines request-level and post-level skills) and rank eligibility.
        /// Note: Date conflict validation should be handled separately by PostPositionService.
        /// </summary>
        /// <param name="requestSkillIds">The list of skill IDs from the request (optional).</param>
        /// <param name="postSkillIds">The list of skill IDs from the post (optional).</param>
        /// <param name="isRankEligible">Whether rank validation is required for this post (optional).</param>
        /// <param name="requiredEmployeeTypeIDs">The list of required employee type IDs for rank matching (optional).</param>
        /// <param name="specificEmployeeId">The employee ID to validate (optional, if not provided, uses employee from token).</param>
        public async Task ValidateEmployeeEligibilityForPostAsync(
            List<Guid>? postSkillIds = null,
            bool? isRankEligible = null,
            Guid? requiredEmployeeTypeIDs = null,
            Guid? specificEmployeeId = null)
        {
            // Get employee ID - either from parameter or from token
            Guid employeeIdToValidate;
            if (specificEmployeeId.HasValue && specificEmployeeId.Value != Guid.Empty)
            {
                employeeIdToValidate = specificEmployeeId.Value;
            }
            else
            {
                var employeeId = _tokenParser.GetEmployeeID();
                if (employeeId == null || employeeId == Guid.Empty)
                    throw new UnauthorizedAccessException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.EMPLOYEE_ID_NOT_FOUND_IN_TOKEN));
                employeeIdToValidate = employeeId.Value;
            }

            // Combine request skills and post skills (union, remove duplicates)
            var combinedRequiredSkillIds = new List<Guid>();
            if (postSkillIds != null && postSkillIds.Any())
            {
                combinedRequiredSkillIds.AddRange(postSkillIds);
            }
            combinedRequiredSkillIds = combinedRequiredSkillIds.Distinct().ToList();

            // Get the employee and their skills only if skill validation is required
            var employee = await GetByIdAsync(employeeIdToValidate);
            if (employee == null)
                throw new KeyNotFoundException(ModelErrorCodes.GetErrorMessage(
                    ModelErrorCodes.GENERIC_ENTITY_WITH_ID_NOT_FOUND,
                    "Employee",
                    employeeIdToValidate));

            var employeeSkills = await _employeeSkillRepository.GetAllAsync(
                es => es.EmployeeID == employeeIdToValidate,
                null,
                null);
            var employeeSkillIds = employeeSkills.Select(es => es.SkillID).ToList();

            // Validate skills only if there are required skills (optional validation)
            if (combinedRequiredSkillIds.Any())
            {
                // Check if employee has skills
                if (employeeSkillIds == null || !employeeSkillIds.Any())
                {
                    throw new WebApiException(
                        HttpStatusCode.BadRequest,
                        ((int)HttpStatusCode.BadRequest).ToString(),
                        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.EMPLOYEE_JOB_ELIGIBILITY_MISSING_SKILLS));
                }

                // Check if employee has all required skills
                var missingSkills = combinedRequiredSkillIds.Except(employeeSkillIds).ToList();

                if (missingSkills.Any())
                {
                    throw new WebApiException(
                        HttpStatusCode.BadRequest,
                        ((int)HttpStatusCode.BadRequest).ToString(),
                        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.EMPLOYEE_JOB_ELIGIBILITY_MISSING_REQUIRED_SKILLS));
                }
            }

            // Validate rank if rank eligibility is required
            // If rank 5 is selected, employees with rank 5 or higher (6, 7, 8, etc.) are eligible
            if (isRankEligible == true && requiredEmployeeTypeIDs.HasValue)
            {
                if (!employee.EmployeeTypeID.HasValue)
                {
                    throw new WebApiException(
                        HttpStatusCode.BadRequest,
                        ((int)HttpStatusCode.BadRequest).ToString(),
                        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.EMPLOYEE_JOB_ELIGIBILITY_MISSING_RANK));
                }

                // Get employee's employee type to check rank
                var employeeEmployeeType = await _employeeTypeService.GetByIdAsync(employee.EmployeeTypeID.Value);
                if (employeeEmployeeType == null || employeeEmployeeType.Rank == 0)
                {
                    throw new WebApiException(
                        HttpStatusCode.BadRequest,
                        ((int)HttpStatusCode.BadRequest).ToString(),
                        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.EMPLOYEE_JOB_ELIGIBILITY_MISSING_RANK));
                }

                // Get all required employee types to find minimum rank
                var requiredEmployeeType = await _employeeTypeService.GetByIdAsync(requiredEmployeeTypeIDs.Value);

                if (requiredEmployeeType == null || requiredEmployeeType.Rank == 0)
                {
                    return;
                }

                var employeeRank = employeeEmployeeType.Rank;
                var requiredRank = requiredEmployeeType.Rank;

                // Employee's rank must be >= minimum required rank (higher rank number = higher rank)
                if (employeeRank > requiredRank)
                {
                    throw new WebApiException(
                        HttpStatusCode.BadRequest,
                        ((int)HttpStatusCode.BadRequest).ToString(),
                        ModelValidationErrorCodes.GetErrorMessage(
                            ModelValidationErrorCodes.EMPLOYEE_JOB_ELIGIBILITY_INSUFFICIENT_RANK,
                            requiredRank,
                            employeeRank));
                }
            }
        }

        /// <summary>
        /// True when loginClient decrypts to <see cref="ClaimNames.WebLoginClient"/>.
        /// A failed decrypt is not a web login.
        /// </summary>
        private bool IsWebLoginClient(string? loginClient)
        {
            if (string.IsNullOrWhiteSpace(loginClient) || _loginClientKey.Length == 0 || _loginClientIv.Length == 0)
                return false;

            try
            {
                using var aes = Aes.Create();
                aes.Key = _loginClientKey;
                return Encoding.UTF8.GetString(aes.DecryptCbc(Convert.FromBase64String(loginClient.Trim()), _loginClientIv)) == ClaimNames.WebLoginClient;
            }
            catch (Exception ex) when (ex is FormatException or CryptographicException or ArgumentException)
            {
                return false;
            }
        }

        /// <summary>
        /// Authenticates an employee and generates a JWT token.
        /// </summary>
        /// <param name="loginDto">The login credentials.</param>
        /// <returns>The token DTO if authentication is successful.</returns>
        /// <exception cref="WebApiException">Thrown when validation fails or authentication is denied.</exception>
        public async Task<TokenDto> LoginAsync(LoginDto loginDto)
        {
            // 1. Validate input
            if (loginDto == null ||
                string.IsNullOrWhiteSpace(loginDto.DepartmentCode) ||
                string.IsNullOrWhiteSpace(loginDto.Username) ||
                string.IsNullOrWhiteSpace(loginDto.Password))
            {
                _logger.LogWarning(
                    "{Message}",
                    ModelErrorCodes.GetErrorMessage(ModelErrorCodes.LOG_AUTH_LOGIN_CREDENTIALS_EMPTY));
                throw new WebApiException(HttpStatusCode.BadRequest, "400", ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_DEPARTMENT_CODE_USERNAME_PASSWORD_REQUIRED));
            }

            // 2. Get employee with all details (single query)
            var employeeInfo = await _repository.GetEmployeeWithDetailsForLoginAsync(loginDto.DepartmentCode, loginDto.Username);
            if (employeeInfo == null)
            {
                _logger.LogWarning(
                    "{Message}",
                    ModelErrorCodes.GetErrorMessage(ModelErrorCodes.LOG_AUTH_LOGIN_NO_MATCHING_EMPLOYEE));
                throw new WebApiException(HttpStatusCode.BadRequest, "400", ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_INVALID_DEPARTMENT_CODE_USERNAME_PASSWORD));
            }

            // 2a. Validate password and hash if needed (for backward compatibility)
            if (string.IsNullOrWhiteSpace(employeeInfo.Password))
            {
                _logger.LogWarning(
                    "{Message}",
                    ModelErrorCodes.GetErrorMessage(ModelErrorCodes.LOG_AUTH_LOGIN_PASSWORD_VERIFICATION_FAILED));
                throw new WebApiException(HttpStatusCode.BadRequest, "400", ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_INVALID_USERNAME_OR_PASSWORD));
            }

            // 2b. Reject Master (system) employees - they cannot login via UI
            if (employeeInfo.IsSystemUser)
            {
                _logger.LogWarning(
                    "{Message}",
                    ModelErrorCodes.GetErrorMessage(ModelErrorCodes.LOG_AUTH_LOGIN_MASTER_ACCOUNT_FORBIDDEN));
                throw new WebApiException(HttpStatusCode.BadRequest, "400", ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_INVALID_USERNAME_OR_PASSWORD));
            }

            // Use common authentication helper for password validation
            var isPasswordValid = await AuthenticationHelper.ValidateAndHashPasswordIfNeededAsync(
                loginDto.Password,
                employeeInfo.Password,
                employeeInfo.EmployeeID,
                async (id, hashedPassword) => await _repository.UpdatePasswordAsync(id, hashedPassword),
                _logger,
                UserType.Employee);

            if (!isPasswordValid)
            {
                _logger.LogWarning(
                    "{Message}",
                    ModelErrorCodes.GetErrorMessage(ModelErrorCodes.LOG_AUTH_LOGIN_PASSWORD_VERIFICATION_FAILED));
                throw new WebApiException(HttpStatusCode.BadRequest, "400", ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_INVALID_USERNAME_OR_PASSWORD));
            }

            // 3. Check if employee is approved for login
            if (!employeeInfo.IsApproved)
            {
                _logger.LogWarning(
                    "{Message}",
                    ModelErrorCodes.GetErrorMessage(ModelErrorCodes.LOG_AUTH_LOGIN_ACCOUNT_NOT_APPROVED));
                throw new WebApiException(HttpStatusCode.BadRequest, "400", ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_LOGIN_NOT_ALLOWED));
            }

            if (employeeInfo.RestrictWebLoginForNonAdminEmployee &&
                (string.IsNullOrWhiteSpace(loginDto.LoginClient) || IsWebLoginClient(loginDto.LoginClient)) &&
                (employeeInfo.AdminTypes == null || employeeInfo.AdminTypes.Count == 0))
            {
                _logger.LogWarning(
                    "{Message}",
                    ModelErrorCodes.GetErrorMessage(ModelErrorCodes.LOG_AUTH_LOGIN_WEB_WITHOUT_ADMIN_TYPE));
                throw new WebApiException(HttpStatusCode.BadRequest, "400", ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_WEB_LOGIN_REQUIRES_ADMIN_TYPE));
            }

            Guid? sessionId = null;
            if (employeeInfo.EnableNormalEmployeeOneSession &&
                (employeeInfo.AdminTypes == null || employeeInfo.AdminTypes.Count == 0))
            {
                sessionId = Guid.NewGuid();
                if (!await _repository.SetActiveSessionIdAsync(employeeInfo.EmployeeID, sessionId.Value).ConfigureAwait(false))
                    throw new WebApiException(HttpStatusCode.BadRequest, "400", ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_SESSION_START_FAILED));
            }

            var tokenDto = await CreateAccessTokenAsync(employeeInfo, loginDto.DeviceToken, sessionId).ConfigureAwait(false);

            _logger.LogInformation(
                "{Message}",
                ModelErrorCodes.GetErrorMessage(ModelErrorCodes.LOG_AUTH_EMPLOYEE_AUTHENTICATED_SUCCESS));

            if (!string.IsNullOrWhiteSpace(loginDto.DeviceToken))
                await _employeeDeviceTokenService.UpsertDeviceTokenAsync(employeeInfo.EmployeeID, loginDto.DeviceToken);

            return tokenDto;
        }

        /// <inheritdoc />
        public async Task<TokenDto?> GenerateAccessTokenAsync(Guid employeeId, Guid? sessionId = null)
        {
            var employeeInfo = await _repository
                .GetEmployeeWithDetailsByIdAsync(employeeId)
                .ConfigureAwait(false);

            if (employeeInfo == null ||
                employeeInfo.IsSystemUser ||
                !employeeInfo.IsApproved ||
                employeeInfo.IsPasswordReset)
            {
                return null;
            }

            return await CreateAccessTokenAsync(employeeInfo, null, sessionId).ConfigureAwait(false);
        }

        private async Task<TokenDto> CreateAccessTokenAsync(
            EmployeeLoginInfoDto employeeInfo,
            string? deviceToken,
            Guid? sessionId)
        {
            // Generate JWT (department configuration flags for SPA — avoids extra DepartmentConfiguration API calls)
            var enableOpenAgencyWaitLists = false;
            var allowEmployeeCancellation = false;
            var enableEmployeeSplittingPosts = false;
            var enablePolicyManagement = true;
            var useAgencyDefinedFields = false;
            var enableEmployeeLocation = false;
            try
            {
                var deptConfig = await _departmentConfigurationService.GetByDepartmentIdAsync(employeeInfo.DepartmentID).ConfigureAwait(false);
                if (deptConfig != null)
                {
                    enableOpenAgencyWaitLists = deptConfig.EnableOpenAgencyWaitLists;
                    allowEmployeeCancellation = deptConfig.AllowEmployeeCancellation;
                    enableEmployeeSplittingPosts = deptConfig.EnableEmployeeSplittingPosts;
                    enablePolicyManagement = deptConfig.EnablePolicyManagement;
                    useAgencyDefinedFields = deptConfig.UseAgencyDefinedFields;
                    enableEmployeeLocation = deptConfig.EnableEmployeeLocation;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{Message}",
                    ModelErrorCodes.GetErrorMessage(ModelErrorCodes.LOG_AUTH_JWT_DEPT_CONFIG_LOAD_FAILED));
            }

            var releaseAsTabTier = AvailablePostReleaseTabTier.RegularEmployee;
            try
            {
                // Must use login path: no JWT exists yet, so ForCurrentUserAsync would see no EmployeeID and return None (0).
                releaseAsTabTier = await _requesterManagerService
                    .GetAvailablePostReleaseTabTierForLoginAsync(employeeInfo)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{Message}",
                    ModelErrorCodes.GetErrorMessage(ModelErrorCodes.LOG_AUTH_JWT_RELEASE_TAB_TIER_DEFAULTED));
            }

            var isLogo = false;
            try
            {
                isLogo = await _uploadFileService.HasEmployeePhotoAsync(employeeInfo.EmployeeID, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "{Message}",
                    ModelErrorCodes.GetErrorMessage(ModelErrorCodes.LOG_AUTH_JWT_IS_LOGO_CHECK_FAILED, employeeInfo.EmployeeID));
            }

            var token = TokenHelper.GenerateEmployeeToken(
                employeeInfo,
                _configuration,
                enableOpenAgencyWaitLists,
                allowEmployeeCancellation,
                enableEmployeeSplittingPosts,
                enablePolicyManagement,
                useAgencyDefinedFields,
                enableEmployeeLocation,
                releaseAsTabTier,
                isLogo,
                sessionId: sessionId);
            var expiresAt = DateTime.UtcNow.AddMinutes(TokenHelper.GetTokenExpirationInMinutes(_configuration));
            if (employeeInfo.IsPasswordReset)
            {
                return new TokenDto(
                    token: token,
                    expiresAt: expiresAt,
                    tokenType: ModelErrorCodes.GetErrorMessage(ModelErrorCodes.JWT_ACCESS_TOKEN_TYPE_BEARER),
                    deviceToken: deviceToken,
                    isPasswordReset: true);
            }

            return new TokenDto(
                token,
                expiresAt,
                ModelErrorCodes.GetErrorMessage(ModelErrorCodes.JWT_ACCESS_TOKEN_TYPE_BEARER),
                deviceToken);
        }

        /// <summary>
        /// Backward-compatible single endpoint flow.
        /// </summary>
        public async Task<PasswordRecoveryResponseDto?> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            if (forgotPasswordDto == null)
                throw new WebApiException(HttpStatusCode.BadRequest, "400", ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_REQUEST_REQUIRED));

            var hasOtp = !string.IsNullOrWhiteSpace(forgotPasswordDto.Otp);
            var hasPassword = !string.IsNullOrWhiteSpace(forgotPasswordDto.NewPassword);

            if (!hasOtp && !hasPassword)
            {
                return await SendForgotPasswordOtpAsync(forgotPasswordDto);
            }

            if (hasOtp && hasPassword)
            {
                return await ResetForgotPasswordAsync(forgotPasswordDto, false);
            }

            throw new WebApiException(HttpStatusCode.BadRequest, "400", ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_OTP_AND_NEW_PASSWORD_TOGETHER_REQUIRED));
        }

        public async Task<PasswordRecoveryResponseDto> SendForgotPasswordOtpAsync(ForgotPasswordDto dto)
        {
            try
            {
                var employee = await GetForgotPasswordEmployeeByUsernameAsync(dto.Username, dto.DepartmentCode);

                var otp = Random.Shared.Next(100000, 999999).ToString();
                employee.Otp = otp;
                await base.UpsertAsync(employee);

                const string otpExpiryMinutes = "10";
                var templateParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { EnumHelper.ToStringValue(EmailKeyword.Otp), otp },
                    { EnumHelper.ToStringValue(EmailKeyword.FullName), PersonNameFormatHelper.FormatLastNameCommaFirstName(employee.LastName, employee.FirstName) ?? string.Empty },
                    { EnumHelper.ToStringValue(EmailKeyword.UserName), employee.Username ?? string.Empty },
                    { EnumHelper.ToStringValue(EmailKeyword.ExpiryMinutes), otpExpiryMinutes },
                    { EnumHelper.ToStringValue(EmailKeyword.DepartmentName), string.Empty }
                };
                var recipientEmail = !string.IsNullOrWhiteSpace(employee.OffDutyCallBackEmail) ? employee.OffDutyCallBackEmail!.Trim() : employee.PrimaryEmail?.Trim();

                if (string.IsNullOrEmpty(recipientEmail))
                {
                    await _emailTemplateService.SendEmployeeEmailUnavailableNotificationAsync(
                        employee,
                        EmailType.ForgotPasswordOtpEmail,
                        CancellationToken.None).ConfigureAwait(false);
                    throw new WebApiException(
                        HttpStatusCode.BadRequest,
                        ((int)HttpStatusCode.BadRequest).ToString(),
                        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_EMAIL_REQUIRED));
                }

                var sendMailRequest = new SendMailRequestDto(
                    EmailType.ForgotPasswordOtpEmail,
                    recipientEmail,
                    fromEmail: null,
                    templateParams);
                await _emailTemplateService.SendMailAsync(sendMailRequest, CancellationToken.None);

                return new PasswordRecoveryResponseDto
                {
                    LoginId = employee.BadgeNumber ?? employee.Username ?? string.Empty,
                    Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_OTP_SENT_TO_EMAIL),
                    EmailAddress = recipientEmail
                };
            }
            catch (WebApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending forgot password OTP for Username: {Username}, DepartmentCode: {DepartmentCode}", dto?.Username, dto?.DepartmentCode);
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    ex.Message);
            }
        }

        public async Task<OtpVerificationResponseDto> VerifyForgotPasswordOtpAsync(ForgotPasswordDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Otp))
                {
                    throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(), ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_OTP_REQUIRED));
                }
                var employee = await GetForgotPasswordEmployeeByUsernameAsync(dto.Username, dto.DepartmentCode);
                var isValid = !string.IsNullOrWhiteSpace(employee.Otp) &&
                              employee.Otp.Trim() == dto.Otp.Trim();

                if (!isValid)
                {
                    throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(), ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_INVALID_OR_EXPIRED_OTP));
                }

                return new OtpVerificationResponseDto
                {
                    IsValid = true,
                    Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_OTP_VERIFIED_SUCCESS)
                };
            }
            catch (WebApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while verifying forgot password OTP for Username: {Username}, DepartmentCode: {DepartmentCode}", dto?.Username, dto?.DepartmentCode);
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    ex.Message);
            }
        }

        public async Task<PasswordRecoveryResponseDto> ResetForgotPasswordAsync(ForgotPasswordDto dto, bool isPasswordReset = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.NewPassword))
                {
                    throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(), ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_OTP_AND_NEW_PASSWORD_REQUIRED));
                }
                var employee = await GetForgotPasswordEmployeeByUsernameAsync(dto.Username, dto.DepartmentCode);

                if (!isPasswordReset)
                {
                    var isValidOtp = !string.IsNullOrWhiteSpace(employee.Otp) &&
                                     employee.Otp.Trim() == dto.Otp?.Trim();
                    if (!isValidOtp)
                    {
                        throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(), ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_INVALID_OR_EXPIRED_OTP));
                    }
                }

                employee.Password = PasswordHelper.HashPassword(dto.NewPassword);
                employee.Otp = null;
                if (isPasswordReset) employee.IsPasswordReset = false;
                await base.UpsertAsync(employee);

                return new PasswordRecoveryResponseDto
                {
                    LoginId = employee.BadgeNumber ?? employee.Username ?? string.Empty,
                    Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_PASSWORD_RESET_SUCCESS),
                    EmailAddress = employee.PrimaryEmail
                };
            }
            catch (WebApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while resetting forgot password for Username: {Username}, DepartmentCode: {DepartmentCode}", dto?.Username, dto?.DepartmentCode);
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    ex.Message);
            }
        }

        private async Task<EmployeeDto> GetForgotPasswordEmployeeByUsernameAsync(string username, string departmentCode)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(departmentCode))
            {
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_DEPARTMENT_CODE_USERNAME_REQUIRED));
            }

            var usernameTrimmed = username.Trim();
            var usernameLower = usernameTrimmed.ToLower();
            var departmentCodeLower = (departmentCode ?? string.Empty).Trim().ToLower();
            Expression<Func<Employee, bool>> usernameTrimmedMatch = e =>
                e.Username != null &&
                e.Department != null &&
                e.Department.DepartmentCode != null &&
                e.Username.Trim().ToLower() == usernameLower &&
                e.Department.DepartmentCode.Trim().ToLower() == departmentCodeLower;

            var employees = await GetAllAsync(usernameTrimmedMatch, null, null);
            if (employees.Count() > 1)
            {
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.EMPLOYEE_DUPLICATE_BY_USERNAME_BADGE_OR_NUMBER));
            }
            var employee = employees.FirstOrDefault();

            if (employee == null)
            {
                _logger.LogWarning(
                    "{Message}",
                    ModelErrorCodes.GetErrorMessage(ModelErrorCodes.LOG_AUTH_FORGOT_PASSWORD_EMAIL_NOT_FOUND));
                throw new WebApiException(HttpStatusCode.NotFound, ((int)HttpStatusCode.NotFound).ToString(), ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_NO_ACCOUNT_FOUND_WITH_USERNAME));
            }

            return employee;
        }

        /// <summary>
        /// Retrieves an employee by badge number (employee number). Used by Excel import job to resolve existing employees for update.
        /// </summary>
        public async Task<EmployeeDto?> GetByBadgeNumberAsync(string badgeNumber)
        {
            if (string.IsNullOrWhiteSpace(badgeNumber))
                return null;
            var employee = await _repository.GetByEmployeeNoAsync(badgeNumber.Trim()).ConfigureAwait(false);
            return employee == null ? null : _mapper.Map<EmployeeDto>(employee);
        }

        /// <summary>
        /// Retrieves an employee by employee number (EmployeeNumber field), excluding the current user.
        /// If the requested employee number belongs to the current user, a validation error is thrown
        /// to indicate that self-split is not allowed.
        /// </summary>
        public async Task<EmployeeDto?> GetByEmployeeNumberAsync(string employeeNumber)
        {
            if (string.IsNullOrWhiteSpace(employeeNumber))
                return null;

            var normalized = employeeNumber.Trim();

            try
            {
                // Get current employee ID from token (if available)
                var currentEmployeeId = _tokenParser.GetEmployeeID();

                // If we have a current employee and the requested number matches their own,
                // explicitly block this to prevent self-split.
                if (currentEmployeeId.HasValue && currentEmployeeId.Value != Guid.Empty)
                {
                    var currentEmployee = await GetByIdAsync(currentEmployeeId.Value);
                    if (currentEmployee != null && string.Equals(currentEmployee.EmployeeNumber, normalized, StringComparison.Ordinal))
                    {
                        throw new WebApiException(
                            HttpStatusCode.BadRequest,
                            ((int)HttpStatusCode.BadRequest).ToString(),
                            ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.EMPLOYEE_SPLIT_WITH_SELF_NOT_ALLOWED));
                    }
                }

                // Fetch employees matching the employee number, but exclude the current user's own record
                var employees = await GetAllAsync(e =>
                    e.EmployeeNumber == normalized &&
                    (!currentEmployeeId.HasValue || e.EmployeeID != currentEmployeeId.Value));

                var employee = employees.FirstOrDefault();
                return employee;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Validates a JWT token.
        /// </summary>
        /// <param name="token">The JWT token to validate.</param>
        /// <returns>True if the token is valid; otherwise, false.</returns>
        public async Task<bool> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{Message}",
                    ModelErrorCodes.GetErrorMessage(ModelErrorCodes.LOG_AUTH_TOKEN_VALIDATION_FAILED));
                return false;
            }
        }

        /// <summary>
        /// Verifies and decodes a JWT token, returning all claims and information.
        /// </summary>
        /// <param name="token">The JWT token to verify and decode.</param>
        /// <returns>The token verification DTO containing all decoded claims and information.</returns>
        public async Task<TokenVerificationDto> VerifyTokenAsync(string token)
        {
            // Delegate to TokenParser for consistency
            return await _tokenParser.ParseTokenAsync(token);
        }
        public async Task<List<EmployeeDto>> GetAvailableRequesterManagers(Guid requesterId)
        {
            // Step 1: get already assigned manager IDs
            var assignedRequesterManagerIds = await _requesterManagerService.GetAssignedRequesterManagerEmployeesByRequesterId(requesterId);
            return await ExcludeEmployees(assignedRequesterManagerIds);
        }
        public async Task<List<EmployeeDto>> GetAvailableRequesterEmployees(Guid requesterManagerId)
        {
            var assignedRequesterEmployeeIds = await _requesterManagerService.GetAssignedRequesterEmployeesByRequesterManagerId(requesterManagerId);
            return await ExcludeEmployees(assignedRequesterEmployeeIds);
        }
        private async Task<List<EmployeeDto>> ExcludeEmployees(List<Guid> employeeIds)
        {
            var employees = await GetAllAsync(filter: e => !employeeIds.Contains(e.EmployeeID));
            return employees.ToList();
        }

        /// <inheritdoc />
        public async Task<(Guid? EmployeeId, string? CreatedByName)> UpsertMasterEmployeeAsync(Guid departmentId, string departmentCode, Guid masterTypeId)
        {
            // Use repository directly here because service-level GetAllAsync appends token-based department filtering,
            // and this method must work even when there is no department in token context.
            var existingMasters = (await _repository.GetAllAsync(
                filter: e => e.DepartmentID == departmentId && e.CreatedBy == "System",
                null,
                null
            )).Select(_mapper.Map<EmployeeDto>).ToList();
            var existingMaster = existingMasters.FirstOrDefault();
            if (existingMaster != null)
            {
                var createdByName = $"{existingMaster.FirstName ?? ""} {existingMaster.LastName ?? ""}".Trim();
                if (string.IsNullOrWhiteSpace(createdByName))
                    createdByName = "Master System";
                return (existingMaster.EmployeeID, createdByName);
            }

            const string masterSuffix = "Master";
            const int usernameMaxLength = 25;
            int prefixMaxLen = usernameMaxLength - masterSuffix.Length; // 20
            var code = (departmentCode ?? "").Trim();
            var masterUsername = (code.Length <= prefixMaxLen ? code : code.Substring(0, prefixMaxLen)) + masterSuffix;

            var plainPassword = PasswordHelper.GenerateSecureRandomPassword(72);
            var hashedPassword = PasswordHelper.HashPassword(plainPassword);

            var entity = new Employee
            {
                EmployeeID = Guid.Empty,
                EmployeeNumber = masterUsername,
                DepartmentID = departmentId,
                EmployeeTypeID = masterTypeId,
                Username = masterUsername,
                Password = hashedPassword,
                BadgeNumber = masterUsername,
                FirstName = "Master",
                LastName = "System",
                IsApproved = false,
                CreatedDate = DateTime.UtcNow,
                CreatedUserID = null,
                CreatedBy = "System",
                EditedDate = null,
                EditedUserID = null,
                EditedBy = null
            };

            await _repository.UpsertAsync(entity, "EmployeeID");
            var createdByNameNew = $"{entity.FirstName} {entity.LastName}".Trim();
            return (entity.EmployeeID, createdByNameNew);
        }

        /// <inheritdoc />
        public async Task<EmployeeDto?> GetMasterEmployeeByDepartmentIdAsync(Guid departmentId)
        {
            var masterTypes = await _employeeTypeService.GetAllAsync(
                et => et.IsSystemUser && et.DepartmentID == departmentId,
                null,
                null).ConfigureAwait(false);
            var masterType = masterTypes.FirstOrDefault();

            if (masterType == null)
                return null;

            // Use repository directly here because service-level GetAllAsync appends token-based department filtering,
            // and cron jobs/background work may not have a department in token context.
            var employees = (await _repository.GetAllAsync(
                filter: e => e.DepartmentID == departmentId && e.EmployeeTypeID == masterType.EmployeeTypeID,
                null,
                null
            )).Select(_mapper.Map<EmployeeDto>).ToList();
            return employees.FirstOrDefault();
        }
    }
}



