using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities.Audit;

/// <summary>
/// Base type for audit rows. Each concrete audit type maps to its own table (TPH not used).
/// </summary>
public abstract class AuditBase
{
    public long Id { get; set; }

    [MaxLength(128)]
    public string EntityId { get; set; } = string.Empty;

    [MaxLength(512)]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>JSON payload of field-level changes.</summary>
    public string ChangedFields { get; set; } = "[]";

    public AuditAction Action { get; set; }

    public DateTime ChangedOn { get; set; }

    [MaxLength(128)]
    public string UserId { get; set; } = string.Empty;

    /// <summary>Department from the caller context (JWT) when the change was recorded; null for jobs or missing claim.</summary>
    [JsonIgnore]
    public Guid? DepartmentID { get; set; }

    [MaxLength(128)]
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Worker from the related <see cref="SMS.Generic.Entities.Entities.PostPosition"/> when the audited entity links to one; set at save time from the main database (not from the JWT).
    /// </summary>
    public Guid? EmployeeId { get; set; }
}
