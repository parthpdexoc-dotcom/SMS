using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Constants;
using SMS.Generic.Common.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;
using SMS.Generic.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing admin operations and authentication.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminController"/> class.
        /// </summary>
        /// <param name="service">The admin service.</param>
        public AdminController(IAdminService service)
        {
            _service = service;
        }

        /// <summary>
        /// Authenticates an admin and returns a JWT token.
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
            var tokenDto = await _service.LoginAsync(loginDto);
            if (tokenDto == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok(tokenDto);
        }

        /// <summary>
        /// Generates and returns a fresh JWT access token for the currently authenticated admin session.
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
                    out var userType) || userType != UserType.Admin)
            {
                return Unauthorized();
            }

            if (!Guid.TryParse(principal.FindFirst(ClaimNames.AdminID)?.Value, out var adminId))
            {
                return Unauthorized();
            }

            var renewedToken = await _service
                .GenerateAccessTokenAsync(adminId)
                .ConfigureAwait(false);

            if (renewedToken == null || string.IsNullOrWhiteSpace(renewedToken.Token))
            {
                return Unauthorized();
            }

            return Ok(renewedToken);
        }

        /// <summary>
        /// Initiates password recovery process for an admin.
        /// Accepts a new password, validates it, hashes it, and updates the admin's password.
        /// </summary>
        /// <param name="forgotPasswordDto">The password recovery request containing username and new password.</param>
        /// <returns>The password recovery response with login ID and confirmation message.</returns>
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(PasswordRecoveryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PasswordRecoveryResponseDto>> ForgotPasswordAsync([FromBody] AdminForgotPasswordDto forgotPasswordDto)
        {
            var result = await _service.ForgotPasswordAsync(forgotPasswordDto);
            if (result == null)
            {
                return NotFound("Admin not found or inactive. Please verify your username.");
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets all token details from the current authenticated user's token.
        /// This endpoint is used for debugging token issues.
        /// </summary>
        /// <returns>All token details including claims, authentication status, and user information.</returns>
        [HttpGet("token-details")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult GetTokenDetails()
        {
            if (User?.Identity == null)
            {
                return Unauthorized("User identity is null.");
            }

            var tokenDetails = new
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                AuthenticationType = User.Identity.AuthenticationType,
                Name = User.Identity.Name,
                NameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                
                // UserType
                UserType = User.FindFirst(ClaimNames.UserType)?.Value,
                
                // Admin Claims
                AdminID = User.FindFirst(ClaimNames.AdminID)?.Value,
                Username = User.FindFirst(ClaimNames.Username)?.Value ?? User.FindFirst(ClaimTypes.Name)?.Value,
                FirstName = User.FindFirst(ClaimNames.FirstName)?.Value,
                LastName = User.FindFirst(ClaimNames.LastName)?.Value,
                FullName = User.FindFirst(ClaimNames.FullName)?.Value,
                AdminTypeID = User.FindFirst(ClaimNames.AdminTypeID)?.Value,
                AdminTypeShort = User.FindFirst(ClaimNames.AdminTypeShort)?.Value,
                AdminTypeLong = User.FindFirst(ClaimNames.AdminTypeLong)?.Value,
                Status = User.FindFirst(ClaimNames.Status)?.Value,
                
                // Employee Claims (if any)
                EmployeeID = User.FindFirst(ClaimNames.EmployeeID)?.Value,
                DepartmentID = User.FindFirst(ClaimNames.DepartmentID)?.Value,
                
                // Roles
                Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList(),
                
                // Standard JWT Claims
                Subject = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Jti = User.FindFirst("jti")?.Value ?? User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value,
                IssuedAt = User.FindFirst("iat")?.Value ?? User.FindFirst(JwtRegisteredClaimNames.Iat)?.Value,
                Expiration = User.FindFirst("exp")?.Value ?? User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value,
                
                // All Claims (for complete debugging)
                AllClaims = User.Claims.Select(c => new
                {
                    Type = c.Type,
                    Value = c.Value,
                    ValueType = c.ValueType
                }).ToList()
            };

            return Ok(tokenDetails);
        }

        /// <summary>
        /// Gets all admins.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of admins.</returns>
        [HttpGet]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [ProducesResponseType(typeof(IEnumerable<AdminDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AdminDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an admin by ID.
        /// </summary>
        /// <param name="id">The admin ID (AdminID as Guid).</param>
        /// <returns>The admin if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [ProducesResponseType(typeof(AdminDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdminDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Admin with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Partially updates an admin using JSON Patch (RFC 6902). Only "replace" operations are allowed.
        /// The patched <see cref="AdminDto"/> is validated with FluentValidation using <see cref="SMS.Validators.AdminValidator"/> (same rules as POST).
        /// A new plaintext password may be set via <c>/password</c>; it is hashed on upsert.
        /// </summary>
        [HttpPatch("{id:guid}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [ProducesResponseType(typeof(AdminDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdminDto>> PatchAsync(Guid id, [FromBody] JsonPatchDocument<AdminDto> adminOprerations)
        {
            if (adminOprerations == null)
                return BadRequest(ModelErrorCodes.GetErrorMessage(ModelErrorCodes.REQUIRED_ADMIN_PATCH_DOCUMENT));

            var patched = await _service.BuildPatchedAdminAsync(id, adminOprerations);
            if (patched == null)
                return NotFound($"Admin with ID {id} not found.");

            var updated = await _service.UpsertAsync(patched);
            return Ok(updated);
        }

        /// <summary>
        /// Creates or updates an admin.
        /// If AdminID is provided and exists, updates the record.
        /// If AdminID is not provided or empty, creates a new record.
        /// On update, omit password or send it empty to keep the existing hash; send a new plaintext password to change it (stored hashed).
        /// </summary>
        /// <param name="dto">The admin data. Include AdminID to update, omit it to create.</param>
        /// <returns>The created or updated admin.</returns>
        [HttpPost]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [ProducesResponseType(typeof(AdminDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdminDto>> UpsertAsync([FromBody] AdminDto dto)
        {
            var result = await _service.UpsertAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Deletes an admin by ID.
        /// </summary>
        /// <param name="id">The admin ID (AdminID as Guid).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
                return NotFound($"Admin with ID {id} not found.");

            return NoContent();
        }
    }
}

