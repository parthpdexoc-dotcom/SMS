namespace SMS.Generic.Common.Audit;

/// <summary>
/// Marks an entity type for change auditing. Only types registered in the audit entity map are processed.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AuditedEntityAttribute : Attribute
{
    /// <summary>
    /// Optional name of a scalar property to use as the human-readable display value (e.g. nameof(Requester.Name)).
    /// </summary>
    public string? DisplayPropertyName { get; init; }

    /// <summary>
    /// When true, only properties marked with <see cref="AuditedPropertyAttribute"/> are written to audit JSON.
    /// </summary>
    public bool OnlyAttributedProperties { get; init; }
}
