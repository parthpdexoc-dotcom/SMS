using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SMS.Generic.Common.Constants;
using SMS.Generic.Common.Enums;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SMS.Infrastructure.Extensions
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]
                ?? throw new InvalidOperationException("JWT SecretKey missing");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"] ?? "SMS",
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"] ?? "SMS-Users",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = "Username",
                    RoleClaimType = ClaimNames.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        // Get DepartmentID from token claims
                        var departmentIdClaim = context.Principal?.FindFirst(ClaimNames.DepartmentID);
                        if (departmentIdClaim != null && Guid.TryParse(departmentIdClaim.Value, out var departmentId))
                        {
                            // Get DepartmentService from service provider
                            var serviceProvider = context.HttpContext.RequestServices;
                            var departmentService = serviceProvider.GetRequiredService<IDepartmentService>();
                            var departmentPolicyService = serviceProvider.GetRequiredService<IDepartmentPolicyService>();

                            try
                            {
                                // Fetch department to get TimeZoneId (DepartmentPolicyDto doesn't include Department properties like TimeZoneId)
                                var department = await departmentService.GetByIdAsync(departmentId);
                                var departmentPolicy = await departmentPolicyService.GetDepartmentPolicyConfigurationByDepartmentId(departmentId);
                                
                                var identity = context.Principal?.Identity as ClaimsIdentity;
                                if (identity != null)
                                {
                                    if (department != null && !string.IsNullOrEmpty(department.TimeZoneId))
                                    {
                                        // Check if TimeZoneId claim already exists
                                        var existingTimeZoneClaim = context.Principal?.FindFirst(ClaimNames.TimeZoneId);
                                        if (existingTimeZoneClaim == null)
                                        {
                                            // Add TimeZoneId claim to the principal
                                            identity.AddClaim(new Claim(ClaimNames.TimeZoneId, department.TimeZoneId));
                                        }
                                    }

                                    if (departmentPolicy != null)
                                    {
                                        void AddBoolClaimIfMissing(string claimType, bool value)
                                        {
                                            if (context.Principal?.FindFirst(claimType) != null) return;
                                            identity.AddClaim(new Claim(claimType, value ? "true" : "false"));
                                        }

                                        AddBoolClaimIfMissing(ClaimNames.EnableOpenWaitingList, departmentPolicy.EnableOpenAgencyWaitLists);
                                        AddBoolClaimIfMissing(ClaimNames.AllowEmployeeCancellation, departmentPolicy.AllowEmployeeCancellation);
                                        AddBoolClaimIfMissing(ClaimNames.EnableEmployeeSplittingPosts, departmentPolicy.EnableEmployeeSplittingPosts);
                                        AddBoolClaimIfMissing(ClaimNames.EnablePolicyManagement, departmentPolicy.EnablePolicyManagement);
                                        AddBoolClaimIfMissing(ClaimNames.UseAgencyDefinedFields, departmentPolicy.UseAgencyDefinedFields);

                                        // Check if DepartmentPolicyConfiguration claim already exists
                                        var existingPolicyClaim = context.Principal?.FindFirst(ClaimNames.DepartmentPolicyConfiguration);
                                        if (existingPolicyClaim == null)
                                        {
                                            // Serialize DepartmentPolicy to JSON and add as claim
                                            var policyJson = JsonSerializer.Serialize(departmentPolicy);
                                            identity.AddClaim(new Claim(ClaimNames.DepartmentPolicyConfiguration, policyJson));
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                // If department fetch fails, silently continue without adding the claim
                                // This prevents authentication from failing if there's a database issue
                            }
                        }

                        if (await RejectReplacedEmployeeSessionAsync(context).ConfigureAwait(false))
                            return;

                        await TryRenewAccessTokenAsync(context).ConfigureAwait(false);
                    },
                    OnChallenge = WriteUnauthorizedResponseAsync
                };
            });

            return services;
        }

        private static async Task<bool> RejectReplacedEmployeeSessionAsync(TokenValidatedContext context)
        {
            var principal = context.Principal;
            if (principal == null
                || !Guid.TryParse(principal.FindFirst(ClaimNames.SessionId)?.Value, out var sessionId)
                || !Guid.TryParse(principal.FindFirst(ClaimNames.EmployeeID)?.Value, out var employeeId))
            {
                return false;
            }

            try
            {
                if (await context.HttpContext.RequestServices
                    .GetRequiredService<IEmployeeRepository>()
                    .IsActiveSessionAsync(employeeId, sessionId)
                    .ConfigureAwait(false))
                {
                    return false;
                }
            }
            catch
            {
                context.Fail(ModelErrorCodes.GetErrorMessage(ModelErrorCodes.AUTH_EMPLOYEE_SESSION_COULD_NOT_BE_VERIFIED));
                return true;
            }

            context.HttpContext.Items[ModelErrorCodes.SESSION_REPLACED] = true;
            context.Fail(ModelErrorCodes.SESSION_REPLACED);
            return true;
        }

        private static Task WriteUnauthorizedResponseAsync(JwtBearerChallengeContext context)
        {
            if (context.Response.HasStarted)
                return Task.CompletedTask;

            var code = context.HttpContext.Items.ContainsKey(ModelErrorCodes.SESSION_REPLACED)
                ? ModelErrorCodes.SESSION_REPLACED
                : ModelErrorCodes.UNAUTHORIZED;

            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                code,
                message = ModelErrorCodes.GetErrorMessage(code)
            }));
        }

        private static async Task TryRenewAccessTokenAsync(TokenValidatedContext context)
        {
            var principal = context.Principal;
            var issuedAtValue = principal?.FindFirst(JwtRegisteredClaimNames.Iat)?.Value;
            var expiresAtValue = principal?.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
            if (principal == null
                || !long.TryParse(issuedAtValue, out var issuedAtUnixSeconds)
                || !long.TryParse(expiresAtValue, out var expiresAtUnixSeconds))
            {
                return;
            }

            // SCRUM-944: use exp/iat from the JWT payload so total lifetime matches the configured expiration.
            // Mixing iat (UTC) with SecurityToken.ValidTo (UTC instant, often Unspecified kind) skews totalLifetime
            // on short ExpirationInMinutes test values and makes renewalWindow smaller than remainingLifetime.
            var issuedAt = DateTimeOffset.FromUnixTimeSeconds(issuedAtUnixSeconds).UtcDateTime;
            var validTo = DateTimeOffset.FromUnixTimeSeconds(expiresAtUnixSeconds).UtcDateTime;
            var totalLifetime = validTo - issuedAt;
            var remainingLifetime = validTo - DateTime.UtcNow;
            var renewalWindow = TimeSpan.FromTicks(totalLifetime.Ticks / 10);

            // Renew only during the final 10% of this token's configured lifetime.
            // This automatically follows JwtSettings:ExpirationInMinutes.
            if (totalLifetime <= TimeSpan.Zero || renewalWindow <= TimeSpan.Zero || remainingLifetime <= TimeSpan.Zero || remainingLifetime > renewalWindow)
            {
                return;
            }

            if (!Enum.TryParse<UserType>(
                    principal.FindFirst(ClaimNames.UserType)?.Value,
                    true,
                    out var userType))
            {
                return;
            }

            var isImpersonation = string.Equals(
                principal.FindFirst(ClaimNames.IsImpersonation)?.Value,
                "true",
                StringComparison.OrdinalIgnoreCase);

            try
            {
                var serviceProvider = context.HttpContext.RequestServices;
                TokenDto? renewedToken;

                if (isImpersonation)
                {
                    if (userType != UserType.Employee || !Guid.TryParse(principal.FindFirst(ClaimNames.EmployeeID)?.Value, out var employeeId) || !Guid.TryParse(principal.FindFirst(ClaimNames.ImpersonatedBy)?.Value, out var superUserId))
                    {
                        return;
                    }

                    renewedToken = await serviceProvider.GetRequiredService<IImpersonationAuthService>().GenerateAccessTokenAsync(employeeId, superUserId).ConfigureAwait(false);
                }
                else
                {
                    renewedToken = userType switch
                    {
                        UserType.Employee when Guid.TryParse(
                            principal.FindFirst(ClaimNames.EmployeeID)?.Value,
                            out var employeeId) =>
                            await serviceProvider.GetRequiredService<IEmployeeService>()
                                .GenerateAccessTokenAsync(
                                    employeeId,
                                    Guid.TryParse(principal.FindFirst(ClaimNames.SessionId)?.Value, out var sessionId) ? sessionId : null)
                                .ConfigureAwait(false),

                        UserType.Admin when Guid.TryParse(
                            principal.FindFirst(ClaimNames.AdminID)?.Value,
                            out var adminId) =>
                            await serviceProvider.GetRequiredService<IAdminService>()
                                .GenerateAccessTokenAsync(adminId)
                                .ConfigureAwait(false),

                        _ => null
                    };
                }

                if (!string.IsNullOrWhiteSpace(renewedToken?.Token))
                {
                    context.Response.Headers["X-New-Access-Token"] = renewedToken.Token;
                    context.Response.Headers["X-New-Access-Token-Expires-At"] =
                        renewedToken.ExpiresAt.ToUniversalTime().ToString("O");
                }
            }
            catch
            {
                // Renewal is best-effort; a failure must not reject an otherwise valid request.
            }
        }
    }
}
