namespace SMS.Generic.Enums
{
    /// <summary>
    /// Authorization policies used for role-based access control.
    /// Use these policy names with [Authorize(Policy = nameof(AuthorizationPolicyType.X))] or .ToString().
    /// </summary>
    public enum AuthorizationPolicyType
    {
        /// <summary>Only SuperUser (e.g. AdminType, Department CRUD, assign IA Admin).</summary>
        SuperUserOnly = 0,

        /// <summary>Full portal: DepartmentAdmin + SuperUser. Includes department settings/config.</summary>
        DepartmentAdminOnly = 1,

        /// <summary>Portal operations: Operator and above. Full control of Employees, Requesters, Requests, Invoices. No Department Settings.</summary>
        OperatorOnly = 2,

        /// <summary>Requester-scoped: Requester Manager and above. Posts, Post Assignments, Rosters, Managed Lists (designated requesters).</summary>
        RequesterManagerOnly = 3,

        /// <summary>View and manage employees (and related lookup data). HRAdmin, Operator, DepartmentAdmin, SuperUser.</summary>
        EmployeesViewManage = 4,

        /// <summary>View-only audit: IAAdmin, SuperUser. Employee/requestor/invoice history for internal auditing.</summary>
        IAAuditViewOnly = 5,

        /// <summary>Fiscal admin access. Rights TBD with stakeholder.</summary>
        FiscalAdminOnly = 6,

        /// <summary>Any authenticated user (regular employee default and all admin types).</summary>
        Authenticated = 7,

        /// <summary>Legacy: same as DepartmentAdminOnly.</summary>
        AdminOnly = 8,

        /// <summary>Legacy: any authenticated user (same as Authenticated).</summary>
        RegularEmployeeOnly = 9,

        /// <summary>Read-only access for department divisions (and similar lookup): DepartmentAdmin, HRAdmin, SuperUser. Does not include Operator.</summary>
        DepartmentAdminOrHRAdmin = 10,

        /// <summary>Watch commander report access: CommCenter and DepartmentAdmin only.</summary>
        CommCenterOrDepartmentAdmin = 11

    }
}
