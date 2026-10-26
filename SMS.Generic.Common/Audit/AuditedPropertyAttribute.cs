namespace SMS.Generic.Common.Audit;

/// <summary>
/// Marks a property to include in audit <c>ChangedFields</c> when the entity uses <see cref="AuditedEntityAttribute.OnlyAttributedProperties"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class AuditedPropertyAttribute : Attribute
{
    public AuditedPropertyAttribute(string displayName) => DisplayName = displayName;

    /// <summary>Label stored in audit JSON (e.g. <c>Secondary Phone</c>).</summary>
    public string DisplayName { get; }

    /// <summary>Related entity type to resolve a friendly value instead of storing the raw FK.</summary>
    public Type? LookupEntityType { get; init; }

    /// <summary>Name of the property on <see cref="LookupEntityType"/> to show.</summary>
    public string? LookupDisplayPropertyName { get; init; }

    /// <summary>
    /// When true, the property is treated as a collection of skill ids; audit stores comma-separated <c>Skill.SkillShort</c> values (one DB query).
    /// </summary>
    public bool GetSkillNamesByIds { get; init; }

    /// <summary>
    /// When true, the property is treated as a collection of employee type ids;
    /// audit stores comma-separated titles (<c>EmployeeTypeShort</c>, falling back to <c>EmployeeTypeLong</c>).
    /// </summary>
    public bool GetEmployeeTypeTitlesByIds { get; init; }

    /// <summary>
    /// When true, the property is treated as a collection of employee ids;
    /// audit stores comma-separated display names (<c>FirstName LastName</c>, falling back to <c>EmployeeNumber</c> then id).
    /// </summary>
    public bool GetEmployeeNamesByIds { get; init; }
}
