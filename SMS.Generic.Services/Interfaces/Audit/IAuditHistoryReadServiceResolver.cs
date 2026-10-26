using System.Linq.Expressions;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Enums;

namespace SMS.Generic.Services.Interfaces.Audit;

/// <summary>
/// Routes <see cref="AuditHistoryTable"/> to the correct typed audit read service for paging and search.
/// Implementations resolve only the read service for the requested table (lazy per call within the current scope).
/// </summary>
public interface IAuditHistoryReadServiceResolver
{
    Task<PagedList<AuditBase>> GetPagedAsync(
        AuditHistoryTable table,
        string? entityId,
        Guid departmentId,
        Guid employeeId,
        PageParam pageParam,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken);

    Task<PagedList<AuditBase>> SearchInTableAsync(
        AuditHistoryTable table,
        Expression<Func<AuditBase, bool>> searchFilter,
        PageParam pageParam,
        CancellationToken cancellationToken);
}
