using Microsoft.AspNetCore.Authorization;
using SMS.Generic.Common.Constants;
using SMS.Generic.Enums;

namespace SMS.Infrastructure.Extensions
{
    public static class AuthPolicyExtensions
    {
        public static IServiceCollection AddRolePolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Require authentication by default
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                // ===== ROLE-BASED POLICIES =====

                // SuperUser only: AdminType, Department CRUD, assign IA Admin, etc.
                options.AddPolicy(AuthorizationPolicyType.SuperUserOnly.ToString(),
                    policy => policy.RequireRole(UserRoleType.SuperUser.ToString()));

                // Full portal: Department Admin + SuperUser (includes department settings/config)
                options.AddPolicy(AuthorizationPolicyType.DepartmentAdminOnly.ToString(),
                    policy => policy.RequireRole(
                        UserRoleType.DepartmentAdmin.ToString(),
                        UserRoleType.SuperUser.ToString()));

                // Portal operations: full control of Employees, Requesters, Requests, Invoices. No Department Settings.
                options.AddPolicy(AuthorizationPolicyType.OperatorOnly.ToString(),
                    policy => policy.RequireRole(
                        UserRoleType.Operator.ToString(),
                        UserRoleType.DepartmentAdmin.ToString()));

                // Requester Manager-scoped: Posts, Post Assignments, Rosters, Managed Lists (designated requesters only; resource scoping elsewhere).
                options.AddPolicy(AuthorizationPolicyType.RequesterManagerOnly.ToString(),
                    policy => policy.RequireRole(
                        UserRoleType.RequesterManager.ToString(),
                        UserRoleType.Operator.ToString(),
                        UserRoleType.DepartmentAdmin.ToString()));

                // View and manage employees (and lookup data: Employee type, Divisions, etc.). HRAdmin gets only this area.
                options.AddPolicy(AuthorizationPolicyType.EmployeesViewManage.ToString(),
                    policy => policy.RequireRole(
                        UserRoleType.HRAdmin.ToString(),
                        UserRoleType.Operator.ToString(),
                        UserRoleType.DepartmentAdmin.ToString(),
                        UserRoleType.SuperUser.ToString()));

                // View-only audit: employee, requestor, invoice history. IA Admin only; assignable by SuperUser only.
                options.AddPolicy(AuthorizationPolicyType.IAAuditViewOnly.ToString(),
                    policy => policy.RequireRole(
                        UserRoleType.IAAdmin.ToString(),
                        UserRoleType.SuperUser.ToString()));

                options.AddPolicy(AuthorizationPolicyType.RegularEmployeeOnly.ToString(),
                    policy => policy.RequireRole(
                        UserRoleType.RegularEmployee.ToString()));

                // Fiscal admin. Rights TBD with stakeholder.
                options.AddPolicy(AuthorizationPolicyType.FiscalAdminOnly.ToString(),
                    policy => policy.RequireRole(
                        UserRoleType.FiscalAdmin.ToString(),
                        UserRoleType.SuperUser.ToString()));

                // Any authenticated portal role (all UserRoleType values used in JWT policies)
                options.AddPolicy(AuthorizationPolicyType.Authenticated.ToString(),
                    policy => policy.RequireRole(
                        UserRoleType.SuperUser.ToString(),
                        UserRoleType.Admin.ToString(),
                        UserRoleType.DepartmentAdmin.ToString(),
                        UserRoleType.RegularEmployee.ToString(),
                        UserRoleType.Operator.ToString(),
                        UserRoleType.RequesterManager.ToString(),
                        UserRoleType.HRAdmin.ToString(),
                        UserRoleType.IAAdmin.ToString(),
                        UserRoleType.FiscalAdmin.ToString(),
                        UserRoleType.CommCenter.ToString()));

                // Legacy: same as DepartmentAdminOnly (Admin treated as DepartmentAdmin)
                options.AddPolicy(AuthorizationPolicyType.AdminOnly.ToString(),
                    policy => policy.RequireRole(
                        UserRoleType.Admin.ToString(),
                        UserRoleType.DepartmentAdmin.ToString(),
                        UserRoleType.SuperUser.ToString()));

                // Read-only for department divisions (GET list/ById): DepartmentAdmin, HRAdmin, SuperUser. Do not add HRAdmin to DepartmentAdminOnly.
                options.AddPolicy(AuthorizationPolicyType.DepartmentAdminOrHRAdmin.ToString(),
                    policy => policy.RequireRole(
                        UserRoleType.DepartmentAdmin.ToString(),
                        UserRoleType.HRAdmin.ToString(),
                        UserRoleType.SuperUser.ToString()));

                // Watch commander report access: CommCenter and DepartmentAdmin only.
                options.AddPolicy(AuthorizationPolicyType.CommCenterOrDepartmentAdmin.ToString(),
                    policy => policy.RequireRole(
                        UserRoleType.CommCenter.ToString(),
                        UserRoleType.DepartmentAdmin.ToString()));
            });

            return services;
        }
    }
}
