using Newtonsoft.Json;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos;

/// <summary>Read model for <c>PostWorkflow_Audit</c> rows.</summary>
public sealed class PostWorkflowAuditDto
{
    public long Id { get; set; }

    public PostPositionStatus ActionType { get; set; }

    public PostWorkflowAttemptStatus Status { get; set; }

    public Guid? PostId { get; set; }

    public Guid? PostPositionId { get; set; }

    public Guid? RequestId { get; set; }

    public Guid? EmployeeId { get; set; }

    [JsonIgnore]
    public Guid? DepartmentID { get; set; }

    public string? PostTitle { get; set; }

    public string? RequestTitle { get; set; }

    public string? EmployeeName { get; set; }

    public DateTime RecordedDate { get; set; }

    public string? FailureReason { get; set; }

    public string? CorrelationId { get; set; }
}
