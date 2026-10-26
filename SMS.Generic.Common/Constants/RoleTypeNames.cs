using SMS.Generic.Enums;

namespace SMS.Generic.Common.Constants
{
    /// <summary>
    /// Maps <see cref="UserRoleType"/> enum to the exact policy role strings used in JWT claims and authorization policies.
    /// Use these instead of hard-coded strings so role names stay in sync. Small functions per role.
    /// </summary>
    public static class RoleTypeNames
    {
        /// <summary>Policy role string for <see cref="UserRoleType.SuperUser"/>.</summary>
        public static string SuperUser() => UserRoleType.SuperUser.ToString();

        /// <summary>Policy role string for <see cref="UserRoleType.Admin"/>.</summary>
        public static string Admin() => UserRoleType.Admin.ToString();

        /// <summary>Policy role string for <see cref="UserRoleType.DepartmentAdmin"/>.</summary>
        public static string DepartmentAdmin() => UserRoleType.DepartmentAdmin.ToString();

        /// <summary>Policy role string for <see cref="UserRoleType.RegularEmployee"/>.</summary>
        public static string RegularEmployee() => UserRoleType.RegularEmployee.ToString();

        /// <summary>Policy role string for <see cref="UserRoleType.Operator"/>.</summary>
        public static string Operator() => UserRoleType.Operator.ToString();

        /// <summary>Policy role string for <see cref="UserRoleType.RequesterManager"/>.</summary>
        public static string RequesterManager() => UserRoleType.RequesterManager.ToString();

        /// <summary>Policy role string for <see cref="UserRoleType.HRAdmin"/>.</summary>
        public static string HRAdmin() => UserRoleType.HRAdmin.ToString();

        /// <summary>Policy role string for <see cref="UserRoleType.IAAdmin"/>.</summary>
        public static string IAAdmin() => UserRoleType.IAAdmin.ToString();

        /// <summary>Policy role string for <see cref="UserRoleType.FiscalAdmin"/>.</summary>
        public static string FiscalAdmin() => UserRoleType.FiscalAdmin.ToString();

        /// <summary>Policy role string for <see cref="UserRoleType.CommCenter"/>.</summary>
        public static string CommCenter() => UserRoleType.CommCenter.ToString();

        /// <summary>Policy role string for suspended employees.</summary>
        public static string IsSuspended() => "isSuspended";
    }
}
