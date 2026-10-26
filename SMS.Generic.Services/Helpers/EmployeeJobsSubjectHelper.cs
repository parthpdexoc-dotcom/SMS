using SMS.Generic.Common.Constants;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Helpers
{
    /// <summary>
    /// Resolves the employee subject for Employee Dashboard / Available Jobs actions.
    /// JWT employee when omitted; privileged same-department override when targeting another employee.
    /// </summary>
    public static class EmployeeJobsSubjectHelper
    {
        /// <summary>
        /// Roles aligned with <see cref="AuthorizationPolicyType.EmployeesViewManage"/> for employee overrides.
        /// </summary>
        public static bool CanOverrideEmployee(ITokenParser tokenParser)
        {
            return tokenParser.IsInRole(RoleTypeNames.HRAdmin())
                || tokenParser.IsInRole(RoleTypeNames.Operator())
                || tokenParser.IsInRole(RoleTypeNames.DepartmentAdmin())
                || tokenParser.IsInRole(RoleTypeNames.SuperUser());
        }

        /// <summary>
        /// Resolves the employee id to act for.
        /// When <paramref name="skipPrivilegeCheck"/> is true (e.g. bulk assign already authorized),
        /// a supplied non-empty <paramref name="requestedEmployeeId"/> is returned without role/department checks.
        /// </summary>
        public static async Task<(Guid SubjectEmployeeId, bool ActingForOtherEmployee)> ResolveSubjectEmployeeAsync(
            Guid? requestedEmployeeId,
            ITokenParser tokenParser,
            IEmployeeService employeeService,
            bool skipPrivilegeCheck = false)
        {
            if (IsNullOrEmpty(requestedEmployeeId))
                return (tokenParser.GetEmployeeID() ?? Guid.Empty, false);

            var subjectId = requestedEmployeeId.Value;
            var tokenEmployeeId = tokenParser.GetEmployeeID();
            var actingForOther = IsNullOrEmpty(tokenEmployeeId) || subjectId != tokenEmployeeId.Value;

            if (!actingForOther || skipPrivilegeCheck)
                return (subjectId, actingForOther);

            if (!CanOverrideEmployee(tokenParser))
                throw new UnauthorizedAccessException("Not authorized to act on behalf of another employee.");

            var tokenDepartmentId = tokenParser.GetDepartmentID();
            if (IsNullOrEmpty(tokenDepartmentId))
                throw CreateDifferentDepartmentException();

            var target = await employeeService.GetByIdAsync(subjectId).ConfigureAwait(false);
            if (target is null || target.DepartmentID != tokenDepartmentId.Value)
                throw CreateDifferentDepartmentException();

            return (subjectId, actingForOther);
        }

        private static bool IsNullOrEmpty(Guid? id) =>
            !id.HasValue || id.Value == Guid.Empty;

        private static UnauthorizedAccessException CreateDifferentDepartmentException() =>
            new(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUEST_EMPLOYEE_DIFFERENT_DEPARTMENT));
    }
}
