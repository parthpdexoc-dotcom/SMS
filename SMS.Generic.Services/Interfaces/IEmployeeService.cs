using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for employee service operations.
    /// </summary>
    public interface IEmployeeService : IGenericService<EmployeeDto, Employee>
    {
        /// <summary>
        /// Retrieves an employee by badge number (employee number). Used by Excel import job to resolve existing employees for update.
        /// </summary>
        /// <param name="badgeNumber">The badge/employee number.</param>
        /// <returns>The employee DTO if found; otherwise, null.</returns>
        Task<EmployeeDto?> GetByBadgeNumberAsync(string badgeNumber);

        /// <summary>
        /// Retrieves an employee by employee number (EmployeeNumber field).
        /// </summary>
        /// <param name="employeeNumber">The employee number.</param>
        /// <returns>The employee DTO if found; otherwise, null.</returns>
        Task<EmployeeDto?> GetByEmployeeNumberAsync(string employeeNumber);

        /// <summary>
        /// Authenticates an employee and generates a JWT token.
        /// </summary>
        /// <param name="loginDto">The login credentials.</param>
        /// <returns>The token DTO if authentication is successful.</returns>
        /// <exception cref="WebApiException">Thrown when validation fails or authentication is denied.</exception>
        Task<TokenDto> LoginAsync(LoginDto loginDto);

        /// <summary>
        /// Generates a new access token from the employee's current persisted state.
        /// </summary>
        Task<TokenDto?> GenerateAccessTokenAsync(Guid employeeId, Guid? sessionId = null);

        /// <summary>
        /// Initiates password recovery process for an employee.
        /// </summary>
        /// <param name="forgotPasswordDto">The password recovery request.</param>
        /// <returns>The password recovery response with login ID and email confirmation.</returns>
        Task<PasswordRecoveryResponseDto?> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<PasswordRecoveryResponseDto> SendForgotPasswordOtpAsync(ForgotPasswordDto dto);
        Task<OtpVerificationResponseDto> VerifyForgotPasswordOtpAsync(ForgotPasswordDto dto);
        Task<PasswordRecoveryResponseDto> ResetForgotPasswordAsync(ForgotPasswordDto dto, bool isPasswordReset = false);

        /// <summary>
        /// Validates a JWT token.
        /// </summary>
        /// <param name="token">The JWT token to validate.</param>
        /// <returns>True if the token is valid; otherwise, false.</returns>
        Task<bool> ValidateTokenAsync(string token);

        /// <summary>
        /// Verifies and decodes a JWT token, returning all claims and information.
        /// </summary>
        /// <param name="token">The JWT token to verify and decode.</param>
        /// <returns>The token verification DTO containing all decoded claims and information.</returns>
        Task<TokenVerificationDto> VerifyTokenAsync(string token);
        Task<PagedList<EmployeeResponseDto>> GetAllEmployeeWithDetailsAsync(PageParam? pageParam, Guid? selectedId = null);
        Task<PagedList<EmployeeResponseDto>> GetAllEmployees(PageParam? pageParam, bool? isActive, Guid? departmentId = null);
        Task<PagedList<EmployeeResponseDto>> GetAllEmployeesByRankFilter(PageParam? pageParam, bool? isActive, Guid? departmentId = null, string? searchTerm = null, Guid? employeeTypeId = null);

        /// <summary>
        /// Searches employees across the full dataset (no pagination-based filtering of the search)
        /// and returns a paged list where TotalRecords reflects all matches.
        /// </summary>
        /// <param name="searchTerm">Free-text search term (name, badge, username, etc.).</param>
        /// <param name="employeeTypeId">Optional EmployeeType filter. When provided, only employees with this EmployeeTypeID are returned.</param>
        /// <param name="isActive">Optional active status filter. When provided, only employees with matching active status are returned.</param>
        /// <param name="departmentId">Optional department filter. When provided by SuperUser, only employees from this department are returned.</param>
        /// <param name="pageParam">Optional paging/sorting parameters for the result page.</param>
        /// <returns>Paged list of employees matching the search criteria.</returns>
        Task<PagedList<EmployeeResponseDto>> SearchEmployeesAsync(string? searchTerm, Guid? employeeTypeId, bool? isActive, Guid? departmentId, PageParam? pageParam);

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
        Task ValidateEmployeeEligibilityForPostAsync(List<Guid>? postSkillIds = null, bool? isRankEligible = null, Guid? requiredEmployeeTypeIDs = null, Guid? specificEmployeeId = null);
        Task<List<EmployeeDto>> GetAvailableRequesterManagers(Guid requesterId);
        Task<List<EmployeeDto>> GetAvailableRequesterEmployees(Guid requesterManagerId);

        /// <summary>
        /// Ensures a Master employee exists for the department. Creates one if not found. Used when department is created.
        /// </summary>
        /// <param name="departmentId">The department identifier.</param>
        /// <param name="departmentCode">The department code (used for username: DepartmentCode + "Master").</param>
        /// <param name="masterTypeId">The MasterType EmployeeTypeID.</param>
        /// <returns>The Master employee's ID and display name (FirstName + LastName), or (null, null) if master was not created.</returns>
        Task<(Guid? EmployeeId, string? CreatedByName)> UpsertMasterEmployeeAsync(Guid departmentId, string departmentCode, Guid masterTypeId);

        /// <summary>
        /// Gets the full Master employee for a department by department ID. Finds by EmployeeType with IsSystemUser. Does not create.
        /// </summary>
        /// <param name="departmentId">The department identifier.</param>
        /// <returns>The Master employee DTO if found (for audit fields and building user claims); otherwise, null.</returns>
        Task<EmployeeDto?> GetMasterEmployeeByDepartmentIdAsync(Guid departmentId);
        /// <returns>The Master employee's ID and display name (FirstName + LastName), or (null, null) if not found.</returns>
        /// <summary>
        /// Partially updates an employee using a JSON Patch document.
        /// Handles loading the current employee, applying the patch, and saving changes.
        /// Throws a WebApiException when the patch is invalid.
        /// </summary>
        /// <param name="id">The employee identifier from route.</param>
        /// <param name="patchDoc">The JSON Patch document describing the changes.</param>
        /// <returns>The updated employee DTO, or null if not found.</returns>
        Task<EmployeeDto?> PatchAsync(Guid id, Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<EmployeeDto> patchDoc);

        /// <summary>
        /// Sends employee password reset/new-account email using the admin template.
        /// </summary>
        /// <param name="isUpdate">True when this is an update flow; false for new-account flow.</param>
        /// <param name="upsertedEmployee">Employee details used to resolve recipient and template fields.</param>
        /// <param name="plainTextPassword">Plain-text password to include in the email template.</param>
        /// <param name="bypassRoleCheck">When true, skips admin-role check (used by trusted background jobs).</param>
        Task SendEmployeePasswordResetEmailByAdminAsync(
            bool isUpdate,
            EmployeeDto upsertedEmployee,
            string? plainTextPassword,
            bool bypassRoleCheck = false);

        /// <summary>
        /// Gets employee dropdown options with full-name label.
        /// </summary>
        Task<IEnumerable<SelectOptionDto>> GetOptionsAsync(
            string? searchTerm = null,
            IReadOnlyList<Guid>? excludeEmployeeIds = null);

    }
}



