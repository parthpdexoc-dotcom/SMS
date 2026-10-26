using SMS.Generic.Dtos.Dtos;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Service responsible for secure superuser impersonation authentication flows.
    /// </summary>
    public interface IImpersonationAuthService
    {
        /// <summary>
        /// Issues a short-lived employee token for superuser impersonation.
        /// </summary>
        /// <param name="request">The impersonation request.</param>
        /// <returns>Short-lived bearer token for employee context.</returns>
        Task<TokenDto> ImpersonateEmployeeAsync(ImpersonateEmployeeRequestDto request);

        /// <summary>
        /// Generates a new impersonation access token from the employee's current persisted state.
        /// </summary>
        /// <param name="employeeId">The impersonated employee identifier.</param>
        /// <param name="impersonatedBySuperUserId">The superuser who initiated impersonation.</param>
        /// <returns>A renewed impersonation token, or null when the employee is ineligible.</returns>
        Task<TokenDto?> GenerateAccessTokenAsync(Guid employeeId, Guid impersonatedBySuperUserId);
    }
}
