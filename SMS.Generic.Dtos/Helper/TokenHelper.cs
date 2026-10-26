using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SMS.Generic.Common.Constants;
using SMS.Generic.Common.Enums;
using SMS.Generic.Dtos.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using System.Linq;

namespace SMS.Generic.Dtos.Helper
{
    /// <summary>
    /// Helper class for JWT token generation operations.
    /// </summary>
    public static class TokenHelper
    {
        /// <summary>
        /// Generates a JWT token for an employee with comprehensive claims.
        /// </summary>
        /// <param name="employeeInfo">The employee login information DTO containing all necessary data.</param>
        /// <param name="configuration">The configuration instance to get JWT settings.</param>
        /// <param name="enableOpenAgencyWaitLists">DepartmentConfiguration.EnableOpenAgencyWaitLists at login.</param>
        /// <param name="allowEmployeeCancellation">DepartmentConfiguration.AllowEmployeeCancellation.</param>
        /// <param name="enableEmployeeSplittingPosts">DepartmentConfiguration.EnableEmployeeSplittingPosts.</param>
        /// <param name="enablePolicyManagement">DepartmentConfiguration.EnablePolicyManagement (default true when no dept config row).</param>
        /// <param name="useAgencyDefinedFields">DepartmentConfiguration.UseAgencyDefinedFields.</param>
        /// <param name="enableEmployeeLocation">DepartmentConfiguration.EnableEmployeeLocation.</param>
        /// <param name="releaseAsTabTier">Available Post Release-as tab tier for the SPA (from requester-manager resolution at login).</param>
        /// <param name="isLogo">True when a profile photo exists for the employee (JWT <see cref="ClaimNames.IsLogo"/> claim).</param>
        /// <returns>The generated JWT token string.</returns>
        public static string GenerateEmployeeToken(
            EmployeeLoginInfoDto employeeInfo,
            IConfiguration configuration,
            bool enableOpenAgencyWaitLists = false,
            bool allowEmployeeCancellation = false,
            bool enableEmployeeSplittingPosts = false,
            bool enablePolicyManagement = true,
            bool useAgencyDefinedFields = false,
            bool enableEmployeeLocation = false,
            AvailablePostReleaseTabTier releaseAsTabTier = AvailablePostReleaseTabTier.RegularEmployee,
            bool isLogo = false,
            bool isImpersonation = false,
            Guid? impersonatedBy = null,
            int? expirationInMinutesOverride = null,
            bool includeAdminTypeRoles = true,
            Guid? sessionId = null)
        {
            var secretKey = configuration["JwtSettings:SecretKey"]
                ?? throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_JWT_SECRET_NOT_CONFIGURED));
            var issuer = configuration["JwtSettings:Issuer"]
                ?? ModelErrorCodes.GetErrorMessage(ModelErrorCodes.JWT_DEFAULT_ISSUER);
            var audience = configuration["JwtSettings:Audience"]
                ?? ModelErrorCodes.GetErrorMessage(ModelErrorCodes.JWT_DEFAULT_AUDIENCE);
            var expirationInMinutes = expirationInMinutesOverride
                ?? int.Parse(configuration["JwtSettings:ExpirationInMinutes"] ?? "60");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var claims = new List<Claim>
            {
                // Standard claims
                new Claim(ClaimTypes.Name, employeeInfo.Username),
                new Claim(ClaimTypes.NameIdentifier, employeeInfo.EmployeeID.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, employeeInfo.EmployeeID.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                
                // UserType claim
                new Claim(ClaimNames.UserType, UserType.Employee.ToString()),
                
                // Employee Info
                new Claim(ClaimNames.EmployeeID, employeeInfo.EmployeeID.ToString()),
                new Claim(ClaimNames.Username, employeeInfo.Username ?? string.Empty),
                new Claim(ClaimNames.BadgeNumber, employeeInfo.BadgeNumber ?? string.Empty),
                new Claim(ClaimNames.FirstName, employeeInfo.FirstName ?? string.Empty),
                new Claim(ClaimNames.LastName, employeeInfo.LastName ?? string.Empty),
                new Claim(ClaimNames.FullName, $"{employeeInfo.FirstName} {employeeInfo.LastName}".Trim()),
                new Claim(ClaimNames.PrimaryEmail, employeeInfo.PrimaryEmail ?? string.Empty),
                
                // Department Info
                new Claim(ClaimNames.DepartmentID, employeeInfo.DepartmentID.ToString()),
                new Claim(ClaimNames.DepartmentName, employeeInfo.DepartmentName ?? string.Empty),
                new Claim(ClaimNames.DepartmentCode, employeeInfo.DepartmentCode ?? string.Empty),
                
                // EmployeeType Info
                new Claim(ClaimNames.EmployeeTypeID, employeeInfo.EmployeeTypeID?.ToString() ?? string.Empty),
                new Claim(ClaimNames.IsSupervisor, employeeInfo.IsSupervisor? "true" : "false"),
                new Claim(ClaimNames.EmployeeTypeShort, employeeInfo.EmployeeTypeShort ?? string.Empty),
                new Claim(ClaimNames.EmployeeTypeLong, employeeInfo.EmployeeTypeLong ?? string.Empty),
                
                // EmployeeStatus Info
                new Claim(ClaimNames.StatusID, employeeInfo.StatusID?.ToString() ?? string.Empty),
                new Claim(ClaimNames.StatusShort, employeeInfo.StatusShort ?? string.Empty),
                new Claim(ClaimNames.StatusLong, employeeInfo.StatusLong ?? string.Empty),
                
                // AdminTypes - Store as JSON array in claim
                new Claim(ClaimNames.AdminTypes, JsonSerializer.Serialize(employeeInfo.AdminTypes.Select(at => at.AdminTypeShort))),
                new Claim(ClaimNames.AdminTypeIDs, JsonSerializer.Serialize(employeeInfo.AdminTypes.Select(at => at.AdminTypeID)))
            };

            // Keep RegularEmployee role for authorization compatibility.
            // Add isSuspended role when login query found an active Suspended skill for current UTC date.
            claims.Add(new Claim(ClaimNames.Role, RoleTypeNames.RegularEmployee()));
            if (employeeInfo.HasActiveSuspendedPeriod)
            {
                claims.Add(new Claim(ClaimNames.Role, RoleTypeNames.IsSuspended()));
            }

            // Add each admin type as a separate role claim (normalized to match policy names)
            // unless caller explicitly requests an employee-only role set (impersonation flow).
            if (includeAdminTypeRoles)
            {
                foreach (var adminType in employeeInfo.AdminTypes)
                {
                    var policyRoleName = GetPolicyRoleNameFromAdminTypeShort(adminType.AdminTypeShort ?? string.Empty);
                    if (!string.IsNullOrEmpty(policyRoleName))
                        claims.Add(new Claim(ClaimNames.Role, policyRoleName));
                }
            }

            // Cache DepartmentAdmin policy scope in token for service-layer checks.
            var isDepartmentAdminPolicy = claims.Any(c =>
                c.Type == ClaimNames.Role &&
                (string.Equals(c.Value, RoleTypeNames.DepartmentAdmin(), StringComparison.OrdinalIgnoreCase)));
            claims.Add(new Claim(ClaimNames.IsDepartmentAdminPolicy, isDepartmentAdminPolicy ? "true" : "false"));

            claims.Add(new Claim(
                ClaimNames.EnableOpenWaitingList,
                enableOpenAgencyWaitLists ? "true" : "false"));
            claims.Add(new Claim(
                ClaimNames.AllowEmployeeCancellation,
                allowEmployeeCancellation ? "true" : "false"));
            claims.Add(new Claim(
                ClaimNames.EnableEmployeeSplittingPosts,
                enableEmployeeSplittingPosts ? "true" : "false"));
            claims.Add(new Claim(
                ClaimNames.EnablePolicyManagement,
                enablePolicyManagement ? "true" : "false"));
            claims.Add(new Claim(
                ClaimNames.UseAgencyDefinedFields,
                useAgencyDefinedFields ? "true" : "false"));
            claims.Add(new Claim(
                ClaimNames.EnableEmployeeLocation,
                enableEmployeeLocation ? "true" : "false"));

            var interval = configuration["Interval"]
                ?? configuration["JwtSettings:Interval"]
                ?? "30";

            claims.Add(new Claim(
                ClaimNames.Interval,
                interval,
                ClaimValueTypes.Integer32));

            claims.Add(new Claim(
                ClaimNames.ReleaseAsTabTier,
                ((int)releaseAsTabTier).ToString(),
                ClaimValueTypes.Integer32));

            claims.Add(new Claim(ClaimNames.IsLogo, isLogo ? "true" : "false"));

            if (sessionId.HasValue && sessionId.Value != Guid.Empty)
                claims.Add(new Claim(ClaimNames.SessionId, sessionId.Value.ToString()));

            if (isImpersonation)
            {
                claims.Add(new Claim(ClaimNames.IsImpersonation, "true"));
                if (impersonatedBy.HasValue && impersonatedBy.Value != Guid.Empty)
                {
                    claims.Add(new Claim(ClaimNames.ImpersonatedBy, impersonatedBy.Value.ToString()));
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme),
                Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            return WriteTokenWithRoleAlwaysJsonArray(tokenHandler, tokenDescriptor);
        }

        /// <summary>
        /// Generates a JWT token for an admin with comprehensive claims.
        /// </summary>
        /// <param name="adminInfo">The admin login information DTO containing all necessary data.</param>
        /// <param name="configuration">The configuration instance to get JWT settings.</param>
        /// <returns>The generated JWT token string.</returns>
        public static string GenerateAdminToken(AdminLoginInfoDto adminInfo, IConfiguration configuration)
        {
            var secretKey = configuration["JwtSettings:SecretKey"]
                ?? throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.AUTH_JWT_SECRET_NOT_CONFIGURED));
            var issuer = configuration["JwtSettings:Issuer"]
                ?? ModelErrorCodes.GetErrorMessage(ModelErrorCodes.JWT_DEFAULT_ISSUER);
            var audience = configuration["JwtSettings:Audience"]
                ?? ModelErrorCodes.GetErrorMessage(ModelErrorCodes.JWT_DEFAULT_AUDIENCE);
            var expirationInMinutes = int.Parse(configuration["JwtSettings:ExpirationInMinutes"] ?? "60");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var claims = new List<Claim>
            {
                // Standard claims
                new Claim(ClaimTypes.Name, adminInfo.UserName),
                new Claim(ClaimTypes.NameIdentifier, adminInfo.AdminID.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, adminInfo.AdminID.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                
                // UserType claim
                new Claim(ClaimNames.UserType, UserType.Admin.ToString()),
                
                // Admin Info
                new Claim(ClaimNames.AdminID, adminInfo.AdminID.ToString()),
                new Claim(ClaimNames.Username, adminInfo.UserName ?? string.Empty),
                new Claim(ClaimNames.FirstName, adminInfo.FirstName ?? string.Empty),
                new Claim(ClaimNames.LastName, adminInfo.LastName ?? string.Empty),
                new Claim(ClaimNames.FullName, $"{adminInfo.FirstName} {adminInfo.LastName}".Trim()),
                
                // AdminType Info
                new Claim(ClaimNames.AdminTypeID, adminInfo.AdminTypeID.ToString()),
                new Claim(ClaimNames.AdminTypeShort, adminInfo.AdminTypeShort ?? string.Empty),
                new Claim(ClaimNames.AdminTypeLong, adminInfo.AdminTypeLong ?? string.Empty),
                new Claim(ClaimNames.IsSuperUser, adminInfo.IsSuperUser ? "true" : "false"),
                
                // Status
                new Claim(ClaimNames.Status, adminInfo.Status.ToString()),
                
                // Role claim: use ClaimNames.Role ("role") so JWT payload matches RoleClaimType in AddJwtBearer
                new Claim(ClaimNames.Role, RoleTypeNames.Admin()) // Base role
            };

            // Add admin type as a role claim. Normalize so "Department Admin" -> "DepartmentAdmin" to match policy names.
            if (!string.IsNullOrWhiteSpace(adminInfo.AdminTypeShort))
            {
                var policyRoleName = GetPolicyRoleNameFromAdminTypeShort(adminInfo.AdminTypeShort);
                claims.Add(new Claim(ClaimNames.Role, policyRoleName));
            }

            // Add SuperUser role when admin's AdminType has IsSuperUser = true (from AdminType table).
            // This is what grants SuperUser-only policy access (e.g. Department CRUD, assign IA Admin).
            if (adminInfo.IsSuperUser)
            {
                claims.Add(new Claim(ClaimNames.Role, RoleTypeNames.SuperUser()));
            }

            // Cache DepartmentAdmin policy scope in token for service-layer checks.
            var isDepartmentAdminPolicy = claims.Any(c =>
                c.Type == ClaimNames.Role &&
                (string.Equals(c.Value, RoleTypeNames.DepartmentAdmin(), StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(c.Value, RoleTypeNames.SuperUser(), StringComparison.OrdinalIgnoreCase)));
            claims.Add(new Claim(ClaimNames.IsDepartmentAdminPolicy, isDepartmentAdminPolicy ? "true" : "false"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme),
                Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            return WriteTokenWithRoleAlwaysJsonArray(tokenHandler, tokenDescriptor);
        }

        /// <summary>
        /// True when the employee has a Department Admin admin type (same rule as <see cref="ClaimNames.IsDepartmentAdminPolicy"/> on employee tokens).
        /// </summary>
        public static bool EmployeeHasDepartmentAdminPolicyScope(IEnumerable<AdminTypeInfoDto>? adminTypes)
        {
            if (adminTypes == null)
                return false;
            foreach (var at in adminTypes)
            {
                var policyRoleName = GetPolicyRoleNameFromAdminTypeShort(at.AdminTypeShort ?? string.Empty);
                if (string.Equals(policyRoleName, RoleTypeNames.DepartmentAdmin(), StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the token expiration time in minutes from configuration.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The expiration time in minutes.</returns>
        public static int GetTokenExpirationInMinutes(IConfiguration configuration)
        {
            return int.Parse(configuration["JwtSettings:ExpirationInMinutes"] ?? "60");
        }

        /// <summary>
        /// Maps AdminType.AdminTypeShort (as stored in DB) to the role name used by authorization policies.
        /// Policies expect names matching <see cref="UserRoleType"/> / <see cref="RoleTypeNames"/> (e.g. DepartmentAdmin, SuperUser).
        /// Handles display names with spaces (e.g. "Department Admin" -> "DepartmentAdmin", "IA Admin" -> "IAAdmin").
        /// </summary>
        private static string GetPolicyRoleNameFromAdminTypeShort(string adminTypeShort)
        {
            if (string.IsNullOrWhiteSpace(adminTypeShort))
                return string.Empty;

            // Remove spaces so "Department Admin" -> "DepartmentAdmin", "IA Admin" -> "IAAdmin"
            var normalized = string.Concat(adminTypeShort.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            if (normalized.Length == 0)
                return adminTypeShort.Trim();

            switch (normalized.ToUpperInvariant())
            {
                case "SUPERUSER": return RoleTypeNames.SuperUser();
                case "DEPARTMENTADMIN": return RoleTypeNames.DepartmentAdmin();
                case "COMMCENTER": return RoleTypeNames.CommCenter();
                case "OPERATOR": return RoleTypeNames.Operator();
                case "REQUESTERMANAGER": return RoleTypeNames.RequesterManager();
                case "HRADMIN": return RoleTypeNames.HRAdmin();
                case "IAADMIN": return RoleTypeNames.IAAdmin();
                case "FISCALADMIN": return RoleTypeNames.FiscalAdmin();
                case "ADMIN": return RoleTypeNames.Admin();
                default: return normalized;
            }
        }

        /// <summary>
        /// <see cref="JwtSecurityTokenHandler"/> serializes a single <c>role</c> claim as a JSON string and multiple as a JSON array.
        /// Only the single-string case is rewritten so the payload always has a JSON array; multi-role payloads are left unchanged.
        /// </summary>
        private static string WriteTokenWithRoleAlwaysJsonArray(JwtSecurityTokenHandler handler, SecurityTokenDescriptor tokenDescriptor)
        {
            var jwt = (JwtSecurityToken)handler.CreateToken(tokenDescriptor);
            CoerceSingleJwtRoleStringToArray(jwt);
            return handler.WriteToken(jwt);
        }

        private static void CoerceSingleJwtRoleStringToArray(JwtSecurityToken jwt)
        {
            if (!jwt.Payload.TryGetValue(ClaimNames.Role, out var raw) || raw is not string s || string.IsNullOrEmpty(s))
                return;

            jwt.Payload.Remove(ClaimNames.Role);
            jwt.Payload.Add(ClaimNames.Role, new[] { s });
        }
    }
}

