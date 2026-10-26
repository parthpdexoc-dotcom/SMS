namespace SMS.Generic.Common.Audit;

/// <summary>
/// Provides ambient user and correlation identifiers for audit entries (typically from the current HTTP request).
/// </summary>
public interface IAuditUserContext
{
    /// <summary>Logical user id for the change (e.g. employee id).</summary>
    string? UserId { get; }

    /// <summary>Department from JWT when present (e.g. employee or admin scoped to a department).</summary>
    Guid? DepartmentID { get; }

    /// <summary>Optional correlation id for tracing (e.g. request / Activity id).</summary>
    string? CorrelationId { get; }
}
