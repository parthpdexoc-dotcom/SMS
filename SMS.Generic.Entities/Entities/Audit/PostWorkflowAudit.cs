using System.ComponentModel.DataAnnotations;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities.Audit;

/// <summary>
/// Operational audit row for post workflow attempts (success/failure), independent of field-level entity diffs.
/// </summary>
public sealed class PostWorkflowAudit
{
    public long Id { get; set; }

    public PostPositionStatus ActionType { get; set; }

    public PostWorkflowAttemptStatus Status { get; set; }

    public Guid? PostId { get; set; }

    public Guid? PostPositionId { get; set; }

    public Guid? RequestId { get; set; }

    public Guid? EmployeeId { get; set; }

    public Guid? DepartmentID { get; set; }

    [MaxLength(200)]
    public string? PostTitle { get; set; }

    [MaxLength(200)]
    public string? RequestTitle { get; set; }

    [MaxLength(200)]
    public string? EmployeeName { get; set; }

    public DateTime RecordedDate { get; set; }

    [MaxLength(512)]
    public string? FailureReason { get; set; }

    [MaxLength(128)]
    public string? CorrelationId { get; set; }
}
