namespace SMS.Generic.Enums;

/// <summary>Lifecycle steps for the Job Position Flow audit timeline (SCRUM-953).</summary>
public enum PostPositionFlowEventType
{
    Created = 1,
    SelfTaken = 2,
    /// <summary>Assigned by a user who has an EmployeeAdmin / AdminType.</summary>
    AssignedByAdmin = 3,
    Transferred = 4,
    UnassignedByAdmin = 5,
    Closed = 6,
    ClosedAndSplit = 7,
    CancellationPending = 8,
    Cancelled = 9,
    SplitAccepted = 10,
    SplitDeclined = 11,
    Unclosed = 12,
    CancellationRevoked = 13,
    /// <summary>New split slot awaiting accept/decline (SplitDecisionPending).</summary>
    SplitPending = 14,
    /// <summary>Assigned by another employee (CreatedUserID != EmployeeID, no EmployeeAdmin).</summary>
    AssignedByOther = 15,
    /// <summary>Assigned by system (no CreatedUserID / performer).</summary>
    AssignedBySystem = 16
}
