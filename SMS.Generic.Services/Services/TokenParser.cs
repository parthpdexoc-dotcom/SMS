using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SMS.Generic.Common.Constants;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;
using SMS.Generic.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for parsing and extracting information from JWT tokens.
    /// </summary>
    public class TokenParser : ITokenParser
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TokenParser> _logger;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenParser"/> class.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor for accessing current request context.</param>
        /// <param name="logger">The logger instance.</param>
        public TokenParser(
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<TokenParser> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _secretKey = _configuration["JwtSettings:SecretKey"]
                ?? throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_JWT_SECRET_NOT_CONFIGURED));
            _issuer = _configuration["JwtSettings:Issuer"] ?? "SMS";
            _audience = _configuration["JwtSettings:Audience"] ?? "SMS-Users";
        }

        /// <summary>
        /// Verifies and decodes a JWT token, returning all claims and information.
        /// </summary>
        /// <param name="token">The JWT token to verify and decode.</param>
        /// <returns>The token verification DTO containing all decoded claims and information.</returns>
        public async Task<TokenVerificationDto> ParseTokenAsync(string token)
        {
            var result = new TokenVerificationDto();

            if (string.IsNullOrWhiteSpace(token))
            {
                result.IsValid = false;
                result.ErrorMessage = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.TOKEN_REQUIRED);
                return result;
            }

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

                // Validate token and get principal
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    result.IsValid = true;
                    result.Issuer = jwtToken.Issuer;
                    result.Audience = string.Join(",", jwtToken.Audiences);

                    // Extract expiration and issued at times
                    if (jwtToken.ValidTo != DateTime.MinValue)
                    {
                        result.ExpiresAt = jwtToken.ValidTo;
                    }

                    if (jwtToken.ValidFrom != DateTime.MinValue)
                    {
                        result.IssuedAt = jwtToken.ValidFrom;
                    }

                    // Extract all claims
                    foreach (var claim in jwtToken.Claims)
                    {
                        result.Claims[claim.Type] = claim.Value;
                    }

                    // Extract specific claims for easy access
                    // EmployeeID can be Guid or int - try both
                    var employeeIdGuid = GetGuidClaim(jwtToken, ClaimNames.EmployeeID);
                    var employeeIdInt = GetIntClaim(jwtToken, ClaimNames.EmployeeID);
                    result.EmployeeID = employeeIdInt; // TokenVerificationDto uses int, but we'll handle Guid in GetEmployeeID()
                    
                    result.Username = GetClaimValue(jwtToken, ClaimNames.Username) ?? GetClaimValue(jwtToken, ClaimTypes.Name);
                    result.BadgeNumber = GetClaimValue(jwtToken, ClaimNames.BadgeNumber);
                    result.FullName = GetClaimValue(jwtToken, ClaimNames.FullName);
                    result.FirstName = GetClaimValue(jwtToken, ClaimNames.FirstName);
                    result.LastName = GetClaimValue(jwtToken, ClaimNames.LastName);

                    // DepartmentID can be Guid or int - try both
                    var departmentIdGuid = GetGuidClaim(jwtToken, ClaimNames.DepartmentID);
                    var departmentIdInt = GetIntClaim(jwtToken, ClaimNames.DepartmentID);
                    result.DepartmentID = departmentIdInt; // TokenVerificationDto uses int, but we'll handle Guid in GetDepartmentID()
                    result.DepartmentName = GetClaimValue(jwtToken, ClaimNames.DepartmentName);
                    result.DepartmentCode = GetClaimValue(jwtToken, ClaimNames.DepartmentCode);

                    result.EmployeeTypeID = GetIntClaim(jwtToken, ClaimNames.EmployeeTypeID);
                    result.EmployeeTypeShort = GetClaimValue(jwtToken, ClaimNames.EmployeeTypeShort);
                    result.EmployeeTypeLong = GetClaimValue(jwtToken, ClaimNames.EmployeeTypeLong);

                    result.StatusID = GetIntClaim(jwtToken, ClaimNames.StatusID);
                    result.StatusShort = GetClaimValue(jwtToken, ClaimNames.StatusShort);
                    result.StatusLong = GetClaimValue(jwtToken, ClaimNames.StatusLong);

                    // Parse AdminTypes JSON array
                    var adminTypesJson = GetClaimValue(jwtToken, ClaimNames.AdminTypes);
                    if (!string.IsNullOrEmpty(adminTypesJson))
                    {
                        try
                        {
                            result.AdminTypes = JsonSerializer.Deserialize<List<string>>(adminTypesJson);
                        }
                        catch
                        {
                            result.AdminTypes = new List<string>();
                        }
                    }

                    // Parse AdminTypeIDs JSON array
                    var adminTypeIDsJson = GetClaimValue(jwtToken, ClaimNames.AdminTypeIDs);
                    if (!string.IsNullOrEmpty(adminTypeIDsJson))
                    {
                        try
                        {
                            result.AdminTypeIDs = JsonSerializer.Deserialize<List<int>>(adminTypeIDsJson);
                        }
                        catch
                        {
                            result.AdminTypeIDs = new List<int>();
                        }
                    }

                    // Extract roles - JWT tokens can have multiple role claims
                    var roleClaims = jwtToken.Claims
                        .Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value)
                        .ToList();
                    
                    if (roleClaims.Any())
                    {
                        result.Roles = roleClaims;
                    }
                    else
                    {
                        result.Roles = new List<string>();
                    }
                }
            }
            catch (SecurityTokenExpiredException)
            {
                result.IsValid = false;
                result.ErrorMessage = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.TOKEN_EXPIRED);
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                result.IsValid = false;
                result.ErrorMessage = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.TOKEN_SIGNATURE_INVALID);
            }
            catch (SecurityTokenException ex)
            {
                result.IsValid = false;
                result.ErrorMessage = $"Token validation failed: {ex.Message}";
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ErrorMessage = $"Error decoding token: {ex.Message}";
                _logger.LogWarning(ex, "Error parsing token");
            }

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Validates a JWT token without parsing all claims.
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
                _logger.LogWarning(ex, "Token validation failed");
                return false;
            }
        }

        /// <summary>
        /// Gets a claim value from the current HTTP context token.
        /// </summary>
        /// <param name="claimType">The type of claim to retrieve.</param>
        /// <returns>The claim value if found; otherwise, null.</returns>
        public string? GetClaimValue(string claimType)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
                return null;

            return user.FindFirst(claimType)?.Value;
        }

        /// <summary>
        /// Gets an integer claim value from the current HTTP context token.
        /// </summary>
        /// <param name="claimType">The type of claim to retrieve.</param>
        /// <returns>The integer claim value if found and valid; otherwise, null.</returns>
        public int? GetIntClaim(string claimType)
        {
            var value = GetClaimValue(claimType);
            if (string.IsNullOrEmpty(value))
                return null;

            if (int.TryParse(value, out int result))
                return result;

            return null;
        }

        /// <summary>
        /// Gets a GUID claim value from the current HTTP context token.
        /// </summary>
        /// <param name="claimType">The type of claim to retrieve.</param>
        /// <returns>The GUID claim value if found and valid; otherwise, null.</returns>
        public Guid? GetGuidClaim(string claimType)
        {
            var value = GetClaimValue(claimType);
            if (string.IsNullOrEmpty(value))
                return null;

            if (Guid.TryParse(value, out Guid result))
                return result;

            return null;
        }

        /// <summary>
        /// Gets the EmployeeID from the current HTTP context token.
        /// </summary>
        /// <returns>The EmployeeID if found; otherwise, null.</returns>
        public Guid? GetEmployeeID()
        {
            // Try to get as GUID first (new format)
            var guidValue = GetGuidClaim(ClaimNames.EmployeeID);
            if (guidValue.HasValue)
                return guidValue;

            return null;
        }

        /// <summary>
        /// Gets the Username from the current HTTP context token.
        /// </summary>
        /// <returns>The Username if found; otherwise, null.</returns>
        public string? GetUsername()
        {
            return GetClaimValue(ClaimNames.Username) ?? GetClaimValue(ClaimTypes.Name);
        }

        /// <summary>
        /// Gets the primary email from the current HTTP context token.
        /// </summary>
        /// <returns>The trimmed primary email if present; otherwise, null.</returns>
        public string? GetPrimaryEmail()
        {
            var email = GetClaimValue(ClaimNames.PrimaryEmail)?.Trim();
            return string.IsNullOrWhiteSpace(email) ? null : email;
        }

        /// <summary>
        /// Gets the DepartmentID from the current HTTP context token.
        /// </summary>
        /// <returns>The DepartmentID if found; otherwise, null.</returns>
        public Guid? GetDepartmentID()
        {
            // Try to get as GUID first (new format)
            var guidValue = GetGuidClaim(ClaimNames.DepartmentID);
            if (guidValue.HasValue)
                return guidValue;

            return null;
        }

        /// <summary>
        /// Gets the AdminID from the current HTTP context token.
        /// </summary>
        /// <returns>The AdminID if found; otherwise, null.</returns>
        public Guid? GetAdminID()
        {
            return GetGuidClaim(ClaimNames.AdminID);
        }
        public string? GetTimeZoneId(){
            return GetClaimValue(ClaimNames.TimeZoneId);
        }

        /// <summary>
        /// Gets the DepartmentPolicy configuration from the current HTTP context token.
        /// </summary>
        /// <returns>The DepartmentPolicyDto if found; otherwise, null.</returns>
        public DepartmentPolicyDto? GetDepartmentPolicy()
        {
            var policyJson = GetClaimValue(ClaimNames.DepartmentPolicyConfiguration);
            if (string.IsNullOrEmpty(policyJson))
                return null;

            try
            {
                return JsonSerializer.Deserialize<DepartmentPolicyDto>(policyJson);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize DepartmentPolicy from claim");
                return null;
            }
        }

        /// <summary>
        /// Checks whether current user has the specified role claim.
        /// </summary>
        public bool IsInRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return false;

            var user = _httpContextAccessor.HttpContext?.User;
            return user?.IsInRole(roleName) == true;
        }

        /// <summary>
        /// Checks whether current user satisfies DepartmentAdmin policy scope.
        /// </summary>
        public bool IsDepartmentAdmin()
        {
            var value = GetClaimValue(ClaimNames.IsDepartmentAdminPolicy);
            if (!string.IsNullOrWhiteSpace(value) && bool.TryParse(value, out var parsed))
                return parsed;

            // Backward-compatible fallback for tokens without this cached claim.
            return IsInRole(RoleTypeNames.DepartmentAdmin()) || IsInRole(RoleTypeNames.SuperUser());
        }

        /// <summary>
        /// Helper method to get a claim value from a JWT token.
        /// </summary>
        private string? GetClaimValue(JwtSecurityToken token, string claimType)
        {
            return token.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        }

        /// <summary>
        /// Helper method to get an integer claim value from a JWT token.
        /// </summary>
        private int? GetIntClaim(JwtSecurityToken token, string claimType)
        {
            var value = GetClaimValue(token, claimType);
            if (string.IsNullOrEmpty(value))
                return null;

            if (int.TryParse(value, out int result))
                return result;

            return null;
        }

        /// <summary>
        /// Helper method to get a GUID claim value from a JWT token.
        /// </summary>
        private Guid? GetGuidClaim(JwtSecurityToken token, string claimType)
        {
            var value = GetClaimValue(token, claimType);
            if (string.IsNullOrEmpty(value))
                return null;

            if (Guid.TryParse(value, out Guid result))
                return result;

            return null;
        }
    }
}

