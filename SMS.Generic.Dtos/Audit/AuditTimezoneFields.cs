using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Dtos.Audit;

/// <summary>
/// Display names and property names eligible for audit read-path timezone conversion.
/// Values mirror <see cref="SMS.Generic.Common.Audit.AuditedPropertyAttribute"/> on audited entities.
/// </summary>
public static class AuditTimezoneFields
{
    // Tier A — BaseEntity audit timestamps
    public const string CreatedDateTime = "Created date/time";
    public const string EditedDateTime = "Edited date/time";
    public const string DeletedAt = "Deleted at";

    // Tier B — visible business/processing datetimes in Audit Log UI
    public const string ProcessedAt = "Processed at";
    public const string PaymentDate = "Payment date";
    public const string AllocationDate = "Allocation date";
    public const string CloseDateTime = "Close date/time";

    /// <summary>
    /// <see cref="ClosedPostPosition.WorkStartDate"/> is DateOnly; equipment work start is DateTime — DateOnly values are excluded at parse time.
    /// </summary>
    public const string WorkStartDate = "Work start date";

    // Defensive C# property aliases (JSON may use property name in edge cases)
    public static readonly string CreatedDateProperty = nameof(BaseEntity.CreatedDate);
    public static readonly string EditedDateProperty = nameof(BaseEntity.EditedDate);
    public static readonly string DeletedAtProperty = nameof(BaseEntity.DeletedAt);
}
