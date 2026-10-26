namespace SMS.Generic.Dtos.Dtos.Audit;

/// <summary>Single property change for JSON serialization into audit <c>ChangedFields</c> columns.</summary>
public sealed class AuditFieldChange
{
    public string Property { get; set; } = string.Empty;
    public object? Old { get; set; }
    public object? New { get; set; }
}
