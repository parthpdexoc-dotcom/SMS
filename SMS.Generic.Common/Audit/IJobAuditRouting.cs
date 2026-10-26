namespace SMS.Generic.Common.Audit;

/// <summary>
/// Per-request (scoped) switches so scheduled jobs can steer interceptor output to job-specific audit tables
/// instead of the default entity audit table. Add flags here as more jobs need the same pattern.
/// </summary>
public interface IJobAuditRouting
{
    /// <summary>
    /// When true, <c>Employee</c> changes captured on <c>SaveChanges</c> are written to <c>EmployeeJob_Audit</c>
    /// (<c>EmployeeJobRunAudit</c>) instead of <c>Employee_Audit</c> (<c>EmployeeAudit</c>).
    /// </summary>
    bool RouteEmployeeAuditsToJobAuditTable { get; set; }
}
