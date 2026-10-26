using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Authentication endpoints for secure impersonation and bootstrap flows.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
    public class AuthController : ControllerBase
    {
        private readonly IImpersonationAuthService _impersonationAuthService;

        public AuthController(IImpersonationAuthService impersonationAuthService)
        {
            _impersonationAuthService = impersonationAuthService;
        }

        /// <summary>
        /// Issues a short-lived employee JWT for superuser impersonation.
        /// </summary>
        /// <param name="request">Impersonation request containing employeeId.</param>
        /// <returns>Short-lived bearer token for employee context.</returns>
        [HttpPost("impersonate")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.SuperUserOnly))]
        [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TokenDto>> ImpersonateAsync([FromBody] ImpersonateEmployeeRequestDto request)
        {
            var token = await _impersonationAuthService.ImpersonateEmployeeAsync(request).ConfigureAwait(false);
            return Ok(token);
        }
    }
}

