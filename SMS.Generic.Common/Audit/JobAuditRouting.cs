namespace SMS.Generic.Common.Audit;

/// <inheritdoc cref="IJobAuditRouting" />
public sealed class JobAuditRouting : IJobAuditRouting
{
    public bool RouteEmployeeAuditsToJobAuditTable { get; set; }
}
