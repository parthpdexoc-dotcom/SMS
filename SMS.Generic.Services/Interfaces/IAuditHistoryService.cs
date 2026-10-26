using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Enums;

namespace SMS.Generic.Services.Interfaces;

/// <summary>Reads audit rows by table and optional entity.</summary>
public interface IAuditHistoryService
{
    /// <summary>
    /// Audit history (paged via <see cref="PageParam"/>), scoped to the current token department and optionally by entity id, date filters, and/or persisted <see cref="AuditBase.EmployeeId"/>.
    /// When <paramref name="changedOn"/> is set, it filters to that calendar day and overrides <paramref name="startDate"/> / <paramref name="endDate"/>.
    /// When <paramref name="employeeId"/> is <see cref="Guid.Empty"/>, rows are not restricted by employee.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled (e.g. HTTP client disconnected).</param>
    Task<PagedList<AuditBase>> GetAllAsync(
        AuditHistoryTable table,
        string? entityId,
        PageParam pageParam,
        Guid employeeId = default,
        DateTime? startDate = null,
        DateTime? endDate = null,
        DateTime? changedOn = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Paged ClosedPostPosition audit rows enriched with current job/employee data from the main database.
    /// </summary>
    Task<PagedList<ClosedPostPositionAuditListDto>> GetClosedPostPositionAuditListAsync(
        PageParam pageParam,
        Guid employeeId = default,
        DateTime? startDate = null,
        DateTime? endDate = null,
        DateTime? changedOn = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches audit records by EntityId, DisplayName, or UserId with paging.
    /// When <paramref name="employeeId"/> is set, restricts to rows whose persisted <see cref="AuditBase.EmployeeId"/> matches.
    /// When <paramref name="changedOn"/> is set, filters to that calendar day (overrides start/end if those are added later).
    /// </summary>
    /// <param name="table">Optional audit table to search within. If not specified, searches all tables.</param>
    /// <param name="searchTerm">Free-text search term.</param>
    /// <param name="pageParam">Optional paging/sorting parameters.</param>
    /// <param name="employeeId">Optional employee filter (matches <see cref="AuditBase.EmployeeId"/>).</param>
    /// <param name="changedOn">Optional calendar-day filter on <see cref="AuditBase.ChangedOn"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<PagedList<AuditBase>> SearchAsync(
        AuditHistoryTable? table = null,
        string? searchTerm = null,
        PageParam? pageParam = null,
        Guid? employeeId = null,
        DateTime? changedOn = null,
        CancellationToken cancellationToken = default);

    Task<PagedList<ClosedPostPositionAuditListDto>> SearchClosedPostPositionAuditListAsync(
        string? searchTerm,
        PageParam pageParam,
        Guid? employeeId = null,
        DateTime? changedOn = null,
        CancellationToken cancellationToken = default);
}
