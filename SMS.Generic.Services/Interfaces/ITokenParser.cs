using SMS.Generic.Dtos.Dtos;
using System.Security.Claims;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for parsing and extracting information from JWT tokens.
    /// </summary>
    public interface ITokenParser
    {
        /// <summary>
        /// Verifies and decodes a JWT token, returning all claims and information.
        /// </summary>
        /// <param name="token">The JWT token to verify and decode.</param>
        /// <returns>The token verification DTO containing all decoded claims and information.</returns>
        Task<TokenVerificationDto> ParseTokenAsync(string token);

        /// <summary>
        /// Validates a JWT token without parsing all claims.
        /// </summary>
        /// <param name="token">The JWT token to validate.</param>
        /// <returns>True if the token is valid; otherwise, false.</returns>
        Task<bool> ValidateTokenAsync(string token);

        /// <summary>
        /// Gets a claim value from the current HTTP context token.
        /// </summary>
        /// <param name="claimType">The type of claim to retrieve.</param>
        /// <returns>The claim value if found; otherwise, null.</returns>
        string? GetClaimValue(string claimType);

        /// <summary>
        /// Gets an integer claim value from the current HTTP context token.
        /// </summary>
        /// <param name="claimType">The type of claim to retrieve.</param>
        /// <returns>The integer claim value if found and valid; otherwise, null.</returns>
        int? GetIntClaim(string claimType);

        /// <summary>
        /// Gets a GUID claim value from the current HTTP context token.
        /// </summary>
        /// <param name="claimType">The type of claim to retrieve.</param>
        /// <returns>The GUID claim value if found and valid; otherwise, null.</returns>
        Guid? GetGuidClaim(string claimType);

        /// <summary>
        /// Gets the EmployeeID from the current HTTP context token.
        /// </summary>
        /// <returns>The EmployeeID if found; otherwise, null.</returns>
        Guid? GetEmployeeID();

        /// <summary>
        /// Gets the Username from the current HTTP context token.
        /// </summary>
        /// <returns>The Username if found; otherwise, null.</returns>
        string? GetUsername();

        /// <summary>
        /// Gets the primary email from the current HTTP context token.
        /// </summary>
        /// <returns>The trimmed primary email if present; otherwise, null.</returns>
        string? GetPrimaryEmail();

        /// <summary>
        /// Gets the DepartmentID from the current HTTP context token.
        /// </summary>
        /// <returns>The DepartmentID if found; otherwise, null.</returns>
        Guid? GetDepartmentID();

        /// <summary>
        /// Gets the AdminID from the current HTTP context token.
        /// </summary>
        /// <returns>The AdminID if found; otherwise, null.</returns>
        Guid? GetAdminID();
        
        /// <summary>
        /// Gets the TimeZoneId from the current HTTP context token.
        /// </summary>
        /// <returns>The TimeZoneId if found; otherwise, null.</returns>
        string? GetTimeZoneId();
        
        /// <summary>
        /// Gets the DepartmentPolicy configuration from the current HTTP context token.
        /// </summary>
        /// <returns>The DepartmentPolicyDto if found; otherwise, null.</returns>
        DepartmentPolicyDto? GetDepartmentPolicy();

        /// <summary>
        /// Checks whether current user has the specified role claim.
        /// </summary>
        /// <param name="roleName">Role name to check.</param>
        /// <returns>True when role is present; otherwise false.</returns>
        bool IsInRole(string roleName);

        /// <summary>
        /// Checks whether current user satisfies DepartmentAdmin policy scope.
        /// </summary>
        /// <returns>True when user is DepartmentAdmin or SuperUser; otherwise false.</returns>
        bool IsDepartmentAdmin();
    }
}

