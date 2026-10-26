using SMS.Generic.Enums;

namespace SMS.Generic.Services.Interfaces;

public interface IPostWorkflowAuditService
{
    Task LogSuccessAsync(
        PostPositionStatus actionType,
        PostWorkflowAuditContext? context = null,
        CancellationToken cancellationToken = default);

    Task LogFailureAsync(
        PostPositionStatus actionType,
        Exception? exception = null,
        PostWorkflowAuditContext? context = null,
        CancellationToken cancellationToken = default);
}
