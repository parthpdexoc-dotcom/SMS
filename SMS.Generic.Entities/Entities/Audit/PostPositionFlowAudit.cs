using System.ComponentModel.DataAnnotations;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities.Audit;

/// <summary>
/// Operational lifecycle row for Job Position Flow (Created → assign/transfer/close/split/cancel).
/// Independent of field-level <see cref="PostPositionAudit"/> diffs and <see cref="PostWorkflowAudit"/> attempt logs.
/// Work date/time/current status come from joining <c>PostPosition</c> on read — not stored here.
/// </summary>
public sealed class PostPositionFlowAudit
{
    public long Id { get; set; }

    public PostPositionFlowEventType EventType { get; set; }

    public Guid PostPositionId { get; set; }

    public Guid? RelatedPostPositionId { get; set; }

    public Guid? PostId { get; set; }

    public Guid? RequestId { get; set; }

    public Guid? DepartmentID { get; set; }

    /// <summary>Employee on the position at this step.</summary>
    public Guid? EmployeeId { get; set; }

    [MaxLength(200)]
    public string? EmployeeName { get; set; }

    public Guid? PerformedByEmployeeId { get; set; }

    [MaxLength(200)]
    public string? PerformedByName { get; set; }

    [MaxLength(512)]
    public string Description { get; set; } = string.Empty;

    /// <summary>Extra key/values (hours, reason, from/to, request title, …).</summary>
    public string? DetailsJson { get; set; }

    [MaxLength(200)]
    public string? PostTitle { get; set; }

    public DateTime RecordedDate { get; set; }
}
