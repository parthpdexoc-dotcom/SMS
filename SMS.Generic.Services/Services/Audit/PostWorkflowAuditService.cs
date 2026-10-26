using SMS.Data;
using SMS.Generic.Common.Audit;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services.Audit;

public sealed class PostWorkflowAuditService : IPostWorkflowAuditService
{
    private readonly AuditDbContext _auditDbContext;
    private readonly IPostRepository _postRepository;
    private readonly IRequestRepository _requestRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPostPositionRepository _postPositionRepository;
    private readonly IAuditUserContext _auditUserContext;

    public PostWorkflowAuditService(
        AuditDbContext auditDbContext,
        IPostRepository postRepository,
        IRequestRepository requestRepository,
        IEmployeeRepository employeeRepository,
        IPostPositionRepository postPositionRepository,
        IAuditUserContext auditUserContext)
    {
        _auditDbContext = auditDbContext;
        _postRepository = postRepository;
        _requestRepository = requestRepository;
        _employeeRepository = employeeRepository;
        _postPositionRepository = postPositionRepository;
        _auditUserContext = auditUserContext;
    }

    public Task LogSuccessAsync(
        PostPositionStatus actionType,
        PostWorkflowAuditContext? context = null,
        CancellationToken cancellationToken = default) =>
        LogAsync(
            actionType: actionType,
            status: PostWorkflowAttemptStatus.Success,
            context: context,
            failureReason: null,
            cancellationToken: cancellationToken);

    public Task LogFailureAsync(
        PostPositionStatus actionType,
        Exception? exception = null,
        PostWorkflowAuditContext? context = null,
        CancellationToken cancellationToken = default) =>
        LogAsync(
            actionType: actionType,
            status: PostWorkflowAttemptStatus.Failed,
            context: context,
            failureReason: context?.FailureReason,
            cancellationToken: cancellationToken);

    private async Task LogAsync(
        PostPositionStatus actionType,
        PostWorkflowAttemptStatus status,
        PostWorkflowAuditContext? context = null,
        string? failureReason = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Guid? employeeId = null;
            if (context?.PostPositionId is { } ppid && ppid != Guid.Empty)
            {
                var pp = await _postPositionRepository
                    .GetByIdAsync(ppid, nameof(PostPosition.PostPositionID))
                    .ConfigureAwait(false);
                if (pp is { EmployeeID: var eid } && eid != Guid.Empty)
                    employeeId = eid;
            }

            if (employeeId is null &&
                context?.EmployeeId is { } contextEmployeeId &&
                contextEmployeeId != Guid.Empty)
            {
                employeeId = contextEmployeeId;
            }

            var (postTitle, requestTitle, employeeName) = await ResolveRelatedDisplayValuesAsync(
                context?.PostId,
                context?.RequestId,
                employeeId,
                cancellationToken).ConfigureAwait(false);

            var row = new PostWorkflowAudit
            {
                ActionType = actionType,
                Status = status,
                PostId = context?.PostId,
                PostPositionId = context?.PostPositionId,
                RequestId = context?.RequestId,
                EmployeeId = employeeId,
                PostTitle = postTitle,
                RequestTitle = requestTitle,
                EmployeeName = employeeName,
                RecordedDate = DateTime.UtcNow,
                FailureReason = NormalizeFailureReason(failureReason),
                CorrelationId = _auditUserContext.CorrelationId,
                DepartmentID = _auditUserContext.DepartmentID
            };

            _auditDbContext.PostWorkflowAudits.Add(row);
            await _auditDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            // Intentionally swallow audit failures to avoid breaking the business flow.
        }
    }

    private static string? NormalizeFailureReason(string? failureReason)
    {
        if (string.IsNullOrWhiteSpace(failureReason))
            return null;

        var trimmed = failureReason.Trim();
        return trimmed.Length <= 512 ? trimmed : trimmed[..512];
    }

    private async Task<(string? PostTitle, string? RequestTitle, string? EmployeeName)> ResolveRelatedDisplayValuesAsync(
        Guid? postId,
        Guid? requestId,
        Guid? employeeId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string? postTitle = null;
        string? requestTitle = null;
        string? employeeName = null;

        if (postId.HasValue)
        {
            var post = await _postRepository
                .GetByIdAsync(postId.Value, nameof(Post.PostID))
                .ConfigureAwait(false);
            postTitle = post?.PostTitle;
            requestId ??= post?.RequestID;
        }

        if (requestId.HasValue)
        {
            var request = await _requestRepository
                .GetByIdAsync(requestId.Value, nameof(Request.RequestID))
                .ConfigureAwait(false);
            requestTitle = request?.Title;
        }

        if (employeeId.HasValue)
        {
            var employee = await _employeeRepository
                .GetByIdAsync(employeeId.Value, nameof(Employee.EmployeeID))
                .ConfigureAwait(false);
            if (employee is not null)
                employeeName = $"{employee.FirstName} {employee.LastName}".Trim();
        }

        return (postTitle, requestTitle, employeeName);
    }
}
