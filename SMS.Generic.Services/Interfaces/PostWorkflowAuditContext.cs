using SMS.Generic.Dtos.Dtos;

namespace SMS.Generic.Services.Interfaces;

public sealed class PostWorkflowAuditContext
{
    public Guid? PostId { get; init; }
    public Guid? PostPositionId { get; init; }
    public Guid? RequestId { get; init; }
    public Guid? EmployeeId { get; init; }
    public string? FailureReason { get; init; }

    public static PostWorkflowAuditContext ForPostPosition(Guid postPositionId, string? failureReason = null) =>
        new()
        {
            PostPositionId = postPositionId,
            FailureReason = failureReason
        };

    public static PostWorkflowAuditContext FromResult(
        PostPositionDto? result,
        Guid? postId = null,
        string? failureReason = null) =>
        new()
        {
            PostId = postId ?? result?.PostID,
            PostPositionId = result?.PostPositionID,
            FailureReason = failureReason
        };

    public static PostWorkflowAuditContext FromRequest(
        ClosedPostPositionDto request,
        PostPositionDto? result = null,
        string? failureReason = null) =>
        new()
        {
            PostId = result?.PostID ?? request.PostID,
            PostPositionId = result?.PostPositionID ?? request.PostPositionID,
            FailureReason = failureReason
        };
}
