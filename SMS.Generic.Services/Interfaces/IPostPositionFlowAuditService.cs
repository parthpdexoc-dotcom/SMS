using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;

namespace SMS.Generic.Services.Interfaces;

/// <summary>Context for writing a Job Position Flow audit step.</summary>
public sealed class PostPositionFlowAuditContext
{
    public Guid PostPositionId { get; init; }
    public Guid? RelatedPostPositionId { get; init; }
    public Guid? PostId { get; init; }
    public Guid? RequestId { get; init; }
    public Guid? EmployeeId { get; init; }
    public string? EmployeeName { get; init; }
    public Guid? RelatedEmployeeId { get; init; }
    public string? RelatedEmployeeName { get; init; }
    public Guid? PerformedByEmployeeId { get; init; }
    public string? PerformedByName { get; init; }
    public string? Description { get; init; }
    /// <summary>Flat or nested detail rows (Vehicle/Equipment use nested <c>items</c>).</summary>
    public IReadOnlyList<JobPositionFlowDetailItemDto>? Details { get; init; }
    public string? PostTitle { get; init; }

    public static PostPositionFlowAuditContext ForPosition(
        Guid postPositionId,
        Guid? postId = null,
        Guid? employeeId = null,
        Guid? relatedPostPositionId = null) =>
        new()
        {
            PostPositionId = postPositionId,
            PostId = postId,
            EmployeeId = employeeId,
            RelatedPostPositionId = relatedPostPositionId
        };
}

public interface IPostPositionFlowAuditService
{
    Task LogAsync(
        PostPositionFlowEventType eventType,
        PostPositionFlowAuditContext context,
        CancellationToken cancellationToken = default);

    /// <summary>Logs close/split flow on the source position and split-pending on the child when applicable.</summary>
    Task LogCloseFlowAsync(
        SplitPostPositionRequestDto request,
        PostPositionDto closedPosition,
        PostPositionDto closeResult,
        PostPositionStatus status,
        bool createSplitPostPosition,
        CancellationToken cancellationToken = default);

    Task LogSplitDeclinedFlowAsync(
        PostPositionDto declined,
        Guid parentId,
        CancellationToken cancellationToken = default);

    Task LogSplitAcceptedFlowAsync(
        PostPositionDto accepted,
        CancellationToken cancellationToken = default);

    Task LogStatusUpdateFlowAsync(
        PostPositionDto existingBeforeUpdate,
        PostPositionStatus newStatus,
        CancellationToken cancellationToken = default);

    Task LogCancellationFlowAsync(
        Guid postPositionId,
        Guid? postId,
        Guid? employeeId,
        bool isApproved,
        string? reason,
        CancellationToken cancellationToken = default);

    Task LogCancellationRevokedFlowAsync(
        Guid postPositionId,
        Guid? postId,
        Guid? employeeId,
        CancellationToken cancellationToken = default);
}

public interface IPostPositionFlowAuditReadService
{
    /// <summary>
    /// Paged Audit Log grid. Events within each story are oldest → newest.
    /// Stories ordered by latest event descending.
    /// </summary>
    Task<PagedList<JobPositionFlowStoryDto>> GetPagedAsync(
        PageParam? pageParam,
        string? search = null,
        CancellationToken cancellationToken = default);

    /// <summary>One position story by <paramref name="postPositionId"/>. Events oldest → newest.</summary>
    Task<JobPositionFlowStoryDto> GetByPostPositionIdAsync(
        Guid postPositionId,
        CancellationToken cancellationToken = default);
}
