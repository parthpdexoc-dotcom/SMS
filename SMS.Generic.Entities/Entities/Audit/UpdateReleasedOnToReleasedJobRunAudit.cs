using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities.Audit;

/// <summary>Audit trail rows for scheduled ReleasedOn-to-Released update job runs (<c>UpdateReleasedOnToReleased_Audit</c>).</summary>
public sealed class UpdateReleasedOnToReleasedJobRunAudit : AuditBase
{
    public UpdateReleasedOnToReleasedJobRunAudit()
    {
    }

    public UpdateReleasedOnToReleasedJobRunAudit(
        string entityId,
        string displayName,
        string changedFields,
        AuditAction action,
        DateTime changedOn,
        string userId,
        Guid? departmentId,
        string? correlationId,
        Guid? employeeId)
    {
        EntityId = entityId;
        DisplayName = displayName;
        ChangedFields = changedFields;
        Action = action;
        ChangedOn = changedOn;
        UserId = userId;
        DepartmentID = departmentId;
        CorrelationId = correlationId;
        EmployeeId = employeeId;
    }
}
