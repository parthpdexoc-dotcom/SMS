namespace SMS.Generic.Dtos.Dtos;

/// <summary>Audit Log grid row + modal payload for Job Position Flow (frontend contract).</summary>
public sealed class JobPositionFlowStoryDto
{
    /// <summary>Unique row id (post position story).</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Post position Guid (also used for detail API).</summary>
    public Guid PostPositionId { get; set; }

    public string JobTitle { get; set; } = string.Empty;

    /// <summary>YYYY-MM-DD</summary>
    public string WorkDate { get; set; } = string.Empty;

    /// <summary>HH:mm</summary>
    public string StartTime { get; set; } = string.Empty;

    /// <summary>HH:mm</summary>
    public string EndTime { get; set; } = string.Empty;

    /// <summary>e.g. Assigned, Closed, Cancelled</summary>
    public string CurrentStatus { get; set; } = string.Empty;

    /// <summary>Current employee display name, or empty / "—" when waiting.</summary>
    public string EmployeeName { get; set; } = string.Empty;

    /// <summary>Lifecycle steps, oldest → newest.</summary>
    public IReadOnlyList<JobPositionFlowEventDto> Events { get; set; } = Array.Empty<JobPositionFlowEventDto>();
}

/// <summary>One timeline step for Job Position Flow.</summary>
public sealed class JobPositionFlowEventDto
{
    public long Id { get; set; }

    /// <summary>
    /// created | assigned_by_admin | assigned_by_other | assigned_by_system | self_taken | transferred |
    /// unassigned_by_admin | cancellation_pending | cancelled | cancellation_revoked | closed | split | unclosed
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>ISO-8601 UTC timestamp.</summary>
    public string OccurredAt { get; set; } = string.Empty;

    /// <summary>Actor display name (maps from PerformedByName).</summary>
    public string PerformedBy { get; set; } = string.Empty;

    /// <summary>admin | employee | system</summary>
    public string Role { get; set; } = "system";

    public string? EmployeeName { get; set; }

    public string Summary { get; set; } = string.Empty;

    public IReadOnlyList<JobPositionFlowDetailItemDto>? Details { get; set; }
}

public sealed class JobPositionFlowDetailItemDto
{
    public string Label { get; set; } = string.Empty;

    /// <summary>Simple string value. Null/omitted when <see cref="Items"/> is used (e.g. Vehicle).</summary>
    public string? Value { get; set; }

    /// <summary>
    /// Nested label/value rows, e.g. Vehicle → type / number / mileage.
    /// </summary>
    public IReadOnlyList<JobPositionFlowDetailItemDto>? Items { get; set; }
}
