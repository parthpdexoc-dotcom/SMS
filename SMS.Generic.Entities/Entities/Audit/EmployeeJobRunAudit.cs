namespace SMS.Generic.Entities.Entities.Audit;

/// <summary>Audit trail rows for scheduled employee Excel import job runs (<c>EmployeeJob_Audit</c>).</summary>
public sealed class EmployeeJobRunAudit : AuditBase
{
    public EmployeeJobRunAudit()
    {
    }

    /// <summary>Copies interceptor-built <see cref="EmployeeAudit"/> payload for the job audit table.</summary>
    public EmployeeJobRunAudit(EmployeeAudit source)
    {
        ArgumentNullException.ThrowIfNull(source);
        EntityId = source.EntityId;
        DisplayName = source.DisplayName;
        ChangedFields = source.ChangedFields;
        Action = source.Action;
        ChangedOn = source.ChangedOn;
        UserId = source.UserId;
        DepartmentID = source.DepartmentID;
        CorrelationId = source.CorrelationId;
        EmployeeId = source.EmployeeId;
    }
}
