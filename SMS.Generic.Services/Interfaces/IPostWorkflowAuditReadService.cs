using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;

namespace SMS.Generic.Services.Interfaces;

/// <summary>Read-only queries for post workflow operational audit (<c>PostWorkflow_Audit</c>).</summary>
public interface IPostWorkflowAuditReadService
{
    Task<PagedList<PostWorkflowAuditDto>> GetPagedByEmployeeIdAsync(
        PageParam? pageParam,
        PostWorkflowAttemptStatus? status = null,
        Guid? employeeId = null,
        CancellationToken cancellationToken = default);
}
