using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SMS.Filters;
using SMS.Generic.Common;
using SMS.Generic.Common.Constants;
using SMS.Generic.Common.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Models.Constants;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing employee operations. HRAdmin can view/manage employees; Operator and above have full control.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;
        private readonly IValidator<EmployeeDto> _validator;
        private readonly IImpersonationAuthService _impersonationAuthService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeController"/> class.
        /// </summary>
        /// <param name="service">The employee service.</param>
        /// <param name="validator">The FluentValidation validator for EmployeeDto.</param>
        /// <param name="impersonationAuthService">The impersonation authentication service.</param>
        public EmployeeController(
            IEmployeeService service,
            IValidator<EmployeeDto> validator,
            IImpersonationAuthService impersonationAuthService)
        {
            _service = service;
            _validator = validator;
            _impersonationAuthService = impersonationAuthService;
        }

        /// <summary>
        /// Authenticates an employee and returns a JWT token.
        /// </summary>
        /// <param name="loginDto">The login credentials.</param>
        /// <returns>The JWT token if authentication is successful; otherwise, Unauthorized.</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TokenDto>> LoginAsync([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var tokenDto = await _service.LoginAsync(loginDto);
                return Ok(tokenDto);
            }
            catch (WebApiException ex)
            {
                return BadRequest(ex.ErrorMessage);
            }
        }

        /// <summary>
        /// Generates and returns a fresh JWT access token for the currently authenticated employee session.
        /// Supports both standard employee sessions and superuser-impersonated employee sessions.
        /// </summary>
        /// <returns>A fresh TokenDto containing token, expiresAt, and tokenType.</returns>
        [HttpPost("renew-token")]
        [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TokenDto>> RenewTokenAsync()
        {
            var principal = HttpContext.User;
            if (principal == null || !principal.Identity?.IsAuthenticated == true)
            {
                return Unauthorized();
            }

            if (!Enum.TryParse<UserType>(
                    principal.FindFirst(ClaimNames.UserType)?.Value,
                    true,
                    out var userType) || userType != UserType.Employee)
            {
                return Unauthorized();
            }

            var isImpersonation = string.Equals(
                principal.FindFirst(ClaimNames.IsImpersonation)?.Value,
                "true",
                StringComparison.OrdinalIgnoreCase);

            TokenDto? renewedToken = null;

            if (isImpersonation)
            {
                if (Guid.TryParse(principal.FindFirst(ClaimNames.EmployeeID)?.Value, out var employeeId) &&
                    Guid.TryParse(principal.FindFirst(ClaimNames.ImpersonatedBy)?.Value, out var superUserId))
                {
                    renewedToken = await _impersonationAuthService
                        .GenerateAccessTokenAsync(employeeId, superUserId)
                        .ConfigureAwait(false);
                }
            }
            else
            {
                if (Guid.TryParse(principal.FindFirst(ClaimNames.EmployeeID)?.Value, out var employeeId))
                {
                    renewedToken = await _service
                        .GenerateAccessTokenAsync(
                            employeeId,
                            Guid.TryParse(principal.FindFirst(ClaimNames.SessionId)?.Value, out var sessionId) ? sessionId : null)
                        .ConfigureAwait(false);
                }
            }

            if (renewedToken == null || string.IsNullOrWhiteSpace(renewedToken.Token))
            {
                return Unauthorized();
            }

            return Ok(renewedToken);
        }

        /// <summary>
        /// Forgot password (single API). Step 1: send only Email – OTP is sent to that email.
        /// Step 2: send Email, Otp, and NewPassword – OTP is verified and password is reset.
        /// </summary>
        /// <param name="forgotPasswordDto">Email (required); Otp and NewPassword required only for step 2.</param>
        /// <returns>The password recovery response (OTP sent or password reset success).</returns>
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(PasswordRecoveryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PasswordRecoveryResponseDto>> ForgotPasswordAsync([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.ForgotPasswordAsync(forgotPasswordDto);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("forgot-password/send-otp")]
        [ProducesResponseType(typeof(PasswordRecoveryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PasswordRecoveryResponseDto>> SendForgotPasswordOtpAsync([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.SendForgotPasswordOtpAsync(dto);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("forgot-password/verify-otp")]
        [ProducesResponseType(typeof(OtpVerificationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OtpVerificationResponseDto>> VerifyForgotPasswordOtpAsync([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.VerifyForgotPasswordOtpAsync(dto);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("forgot-password/reset")]
        [ProducesResponseType(typeof(PasswordRecoveryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PasswordRecoveryResponseDto>> ResetForgotPasswordAsync(
            [FromBody] ForgotPasswordDto dto,
            [FromQuery] bool isPasswordReset = false)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.ResetForgotPasswordAsync(dto, isPasswordReset);
            return Ok(result);
        }

        /// <summary>
        /// Gets all employees.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of employees.</returns>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.IAAuditViewOnly))]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.IAAuditViewOnly),nameof(AuthorizationPolicyType.RequesterManagerOnly),nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpGet("getallemployeewithdetails")]
        public async Task<IActionResult> GetAllEmployeeWithDetails([FromQuery] PageParam? pageParam = null, [FromQuery] Guid? selectedId = null)
        {
            pageParam ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
            {
                pageParam.SortBy = nameof(EmployeeDto.LastName);
            }

            var result = await _service.GetAllEmployeeWithDetailsAsync(pageParam, selectedId);
            return Ok(result);
        }

        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.IAAuditViewOnly), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpGet("getallemployees")]
        public async Task<IActionResult> GetAllEmployees(
            [FromQuery] PageParam? pageParam = null,
            [FromQuery] bool? isactive = null,
            [FromQuery] Guid? departmentId = null)
        {
            pageParam ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
            {   
                pageParam.SortBy = nameof(EmployeeDto.LastName);
            }

            // SuperUser can pass departmentId to get department-wise employee data.
            var result = await _service.GetAllEmployees(pageParam, isactive, departmentId);
            return Ok(result);
        }

        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.IAAuditViewOnly), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpGet("getallemployeesbyrankfilter")]
        public async Task<IActionResult> GetAllEmployeesByRankFilter(
            [FromQuery] string? searchTerm = null,
            [FromQuery] Guid? employeeTypeId = null,
            [FromQuery] PageParam? pageParam = null,
            [FromQuery] bool? isactive = null,
            [FromQuery] Guid? departmentId = null)
        {
            pageParam ??= new PageParam();

            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
            {
                pageParam.SortBy = nameof(EmployeeDto.LastName);
            }

            // SuperUser can pass departmentId to get department-wise employee data.
            var result = await _service.GetAllEmployeesByRankFilter(pageParam, isactive, departmentId, searchTerm, employeeTypeId);

            return Ok(result);
        }

        /// <summary>
        /// Gets lightweight employee options for dropdowns (employeeId and fullName only).
        /// </summary>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.IAAuditViewOnly), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpPost("employeeoptions")]
        [ProducesResponseType(typeof(IReadOnlyList<SelectOptionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<SelectOptionDto>>> GetEmployeeOptionsAsync([FromBody] SelectOptionsRequestDto? body)
        {
            var request = SelectOptionsRequestDto.OrDefault(body);
            return Ok(await _service.GetOptionsAsync(request.SearchTerm));
        }

        /// <summary>
        /// Searches employees with optional filters for search term, employee type, and active status.
        /// Returns paged results where TotalRecords reflects all matches before paging.
        /// </summary>
        /// <param name="searchTerm">Free-text search term (name, badge, username, etc.).</param>
        /// <param name="employeeTypeId">Optional EmployeeType filter; when provided, only employees of this type are returned.</param>
        /// <param name="isActive">Optional active status filter; when provided, only employees with matching active status are returned.</param>
        /// <param name="departmentId">Optional department filter; when provided by SuperUser, only employees from this department are returned.</param>
        /// <param name="pageParam">Optional paging/sorting parameters for the result page.</param>
        [AuthorizeAny(
            nameof(AuthorizationPolicyType.EmployeesViewManage),
            nameof(AuthorizationPolicyType.IAAuditViewOnly),
            nameof(AuthorizationPolicyType.RequesterManagerOnly),
            nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<EmployeeResponseDto>), StatusCodes.Status200OK)]
            public async Task<IActionResult> SearchEmployees(
            [FromQuery] string? searchTerm,
            [FromQuery] Guid? employeeTypeId,
            [FromQuery] bool? isActive,
            [FromQuery] Guid? departmentId,
            [FromQuery] PageParam? pageParam = null)
            {
                pageParam ??= new PageParam();

                if (string.IsNullOrWhiteSpace(pageParam.SortBy))
                {
                    pageParam.SortBy = nameof(EmployeeResponseDto.LastName);
                }

                var result = await _service.SearchEmployeesAsync(
                    searchTerm,
                    employeeTypeId,
                    isActive,
                    departmentId,
                    pageParam);

                return Ok(result);
            }

        /// <summary>
        /// Partially updates an employee using JSON Patch (RFC 6902). Only "replace" operations are allowed.
        /// Existing employee is loaded, patch is applied to a DTO, and validation/business rules run in the service layer.
        /// </summary>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpPatch("{id:guid}")]
        [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeDto>> PatchAsync(Guid id, [FromBody] JsonPatchDocument<EmployeeDto> employeeOperations)
        {
            if (employeeOperations == null)
                return BadRequest(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_PATCH_DOCUMENT));

            var updated = await _service.PatchAsync(id, employeeOperations);
            if (updated == null)
                return NotFound($"Employee with ID {id} not found.");

            return Ok(updated);
        }

        /// <summary>
        /// Gets an employee by ID.
        /// </summary>
        /// <param name="id">The employee ID (EmployeeID as Guid).</param>
        /// <returns>The employee if found; otherwise, NotFound.</returns>

        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.IAAuditViewOnly), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Gets an employee by employee number (badge number).
        /// </summary>
        /// <param name="employeeNumber">The employee/badge number.</param>
        /// <returns>The employee if found; otherwise, NotFound.</returns>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpGet("by-employee-number/{employeeNumber}")]
        [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeDto>> GetByEmployeeNumberAsync(string employeeNumber)
        {
            var employee = await _service.GetByEmployeeNumberAsync(employeeNumber);
            if (employee == null)
            {
                return NotFound($"Employee with number {employeeNumber} not found.");
            }

            return Ok(employee);
        }

        /// <summary>
        /// Creates a new employee.
        /// </summary>
        /// <param name="dto">The employee data to create.</param>
        /// <returns>The created employee.</returns>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]

        [HttpPost]
        [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeeDto>> UpsertAsync([FromBody] EmployeeDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Deletes an employee by ID.
        /// </summary>
        /// <param name="id">The employee ID (EmployeeID as Guid).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Employee with ID {id} not found.");
            }
            return NoContent();
        }
        /// <summary>
        /// Gets employees who are NOT already assigned as managers for a requester.
        /// </summary>
        /// <param name="requesterId">Requester ID</param>
        /// <returns>Available employees</returns>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly))]

        [HttpGet("available-managers/{requesterId}")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAvailableRequesterManagers(Guid requesterId)
        {
            var employees = await _service.GetAvailableRequesterManagers(requesterId);
            return Ok(employees);
        }
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpGet("available-requester-employee/{requesterManagerId}")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAvailableRequesterEmployees(Guid requesterManagerId)
        {
            var employees = await _service.GetAvailableRequesterEmployees(requesterManagerId);
            return Ok(employees);
        }
    }
}

