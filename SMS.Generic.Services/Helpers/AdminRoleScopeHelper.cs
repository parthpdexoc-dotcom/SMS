using System.Text.Json;
using SMS.Generic.Common.Constants;
using SMS.Generic.Models.Constants;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Helpers
{
    public sealed record AdminScope(Guid? EmployeeId, bool IsRequesterManagerOnly);

    /// <summary>
    /// Helper methods for evaluating admin-role scope from token claims.
    /// </summary>
    public static class AdminRoleScopeHelper
    {
        /// <summary>
        /// Returns parsed admin scope information from token claims.
        /// </summary>
        public static AdminScope GetScope(ITokenParser tokenParser)
        {
            try
            {
                var adminTypesJson = tokenParser.GetClaimValue(ClaimNames.AdminTypes);
                var tokenEmployeeId = tokenParser.GetEmployeeID();

                if (string.IsNullOrWhiteSpace(adminTypesJson) || !tokenEmployeeId.HasValue)
                    return new AdminScope(tokenEmployeeId, IsRequesterManagerOnly: false);

                var adminTypes = JsonSerializer.Deserialize<List<string>>(adminTypesJson) ?? new List<string>();
                var normalizedRequesterManager = NormalizeRole(RoleTypeNames.RequesterManager());
                var hasRequesterManagerRole = adminTypes.Any(t => NormalizeRole(t) == normalizedRequesterManager);
                var hasOtherAdminRoles = adminTypes.Any(t => NormalizeRole(t) != normalizedRequesterManager && !string.IsNullOrEmpty(NormalizeRole(t)));

                return new AdminScope(
                    tokenEmployeeId,
                    IsRequesterManagerOnly: hasRequesterManagerRole && !hasOtherAdminRoles);
            }
            catch
            {
                return new AdminScope(tokenParser.GetEmployeeID(), IsRequesterManagerOnly: false);
            }
        }

        private static string NormalizeRole(string? value) =>
            string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : new string(value.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLowerInvariant();
    }
}
