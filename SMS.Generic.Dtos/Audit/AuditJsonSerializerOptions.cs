using System.Text.Json;

namespace SMS.Generic.Dtos.Audit;

/// <summary>
/// Shared JSON options for audit <c>ChangedFields</c> serialization (read and write paths).
/// </summary>
public static class AuditJsonSerializerOptions
{
    public static JsonSerializerOptions ChangedFields { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };
}
