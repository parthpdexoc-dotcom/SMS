using System.Linq.Expressions;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Interfaces.Audit;

namespace SMS.Generic.Services.Services.Audit;

public sealed class AuditHistoryService : IAuditHistoryService
{
    private readonly IAuditHistoryReadServiceResolver _readResolver;
    private readonly ITokenParser _tokenParser;
    private readonly IClosedPostPositionRepository _closedPostPositionRepository;

    public AuditHistoryService(IAuditHistoryReadServiceResolver readResolver, ITokenParser tokenParser, IClosedPostPositionRepository closedPostPositionRepository)
    {
        _readResolver = readResolver ?? throw new ArgumentNullException(nameof(readResolver));
        _tokenParser = tokenParser ?? throw new ArgumentNullException(nameof(tokenParser));
        _closedPostPositionRepository = closedPostPositionRepository
            ?? throw new ArgumentNullException(nameof(closedPostPositionRepository));
    }

    /// <inheritdoc />
    public async Task<PagedList<AuditBase>> GetAllAsync(
        AuditHistoryTable table,
        string? entityId,
        PageParam pageParam,
        Guid employeeId = default,
        DateTime? startDate = null,
        DateTime? endDate = null,
        DateTime? changedOn = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(pageParam);
        var departmentId = table == AuditHistoryTable.EmailTemplate
            ? Guid.Empty
            : _tokenParser.GetDepartmentID().Value;
        var (effectiveStartDate, effectiveEndDate) = ResolveAuditDateRange(startDate, endDate, changedOn);

        var page = await _readResolver.GetPagedAsync(table, entityId, departmentId, employeeId, pageParam, effectiveStartDate, effectiveEndDate, cancellationToken).ConfigureAwait(false);

        ApplyDepartmentTimezone(page);
        return page;
    }

    /// <inheritdoc />
    public async Task<PagedList<ClosedPostPositionAuditListDto>> GetClosedPostPositionAuditListAsync(
        PageParam pageParam,
        Guid employeeId = default,
        DateTime? startDate = null,
        DateTime? endDate = null,
        DateTime? changedOn = null,
        CancellationToken cancellationToken = default)
    {
        var auditPage = await GetAllAsync(AuditHistoryTable.ClosedPostPosition,entityId: null,pageParam,employeeId,startDate,endDate,changedOn,cancellationToken).ConfigureAwait(false);

        return await BuildClosedPostPositionAuditListAsync(auditPage, pageParam, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>When <paramref name="changedOn"/> is set, filters to that day; otherwise uses <paramref name="startDate"/> / <paramref name="endDate"/>.</summary>
    internal static (DateTime? StartDate, DateTime? EndDate) ResolveAuditDateRange(DateTime? startDate,DateTime? endDate,DateTime? changedOn)
    {
        if (!changedOn.HasValue)
            return (startDate, endDate);

        var day = changedOn.Value.Date;
        return (day, day);
    }

    /// <inheritdoc />
    public async Task<PagedList<AuditBase>> SearchAsync(
        AuditHistoryTable? table = null,
        string? searchTerm = null,
        PageParam? pageParam = null,
        Guid? employeeId = null,
        DateTime? changedOn = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var searchTable = table ?? AuditHistoryTable.Requester;

        var effectivePageParam = pageParam ?? new PageParam();
        if (string.IsNullOrWhiteSpace(effectivePageParam.SortBy))
            effectivePageParam.SortBy = nameof(AuditBase.ChangedOn);
        if (string.IsNullOrWhiteSpace(effectivePageParam.SortDirection))
            effectivePageParam.SortDirection = "desc";

        var departmentId = _tokenParser.GetDepartmentID();
        var effectiveEmployeeId = employeeId.HasValue && employeeId.Value != Guid.Empty
            ? employeeId.Value
            : Guid.Empty;
        var (startDate, endDate) = ResolveAuditDateRange(startDate: null, endDate: null, changedOn);
        Expression<Func<AuditBase, bool>> searchFilter = searchTable == AuditHistoryTable.EmailTemplate
            ? BuildEmailTemplateSearchFilter(searchTerm, effectiveEmployeeId, startDate, endDate)
            : BuildSearchFilter(searchTerm, departmentId.Value, effectiveEmployeeId, startDate, endDate);

        var page = await _readResolver.SearchInTableAsync(searchTable, searchFilter, effectivePageParam, cancellationToken).ConfigureAwait(false);
        ApplyDepartmentTimezone(page);
        return page;
    }

    /// <inheritdoc />
    public async Task<PagedList<ClosedPostPositionAuditListDto>> SearchClosedPostPositionAuditListAsync(
        string? searchTerm,
        PageParam pageParam,
        Guid? employeeId = null,
        DateTime? changedOn = null,
        CancellationToken cancellationToken = default)
    {
        var page = await SearchAsync(AuditHistoryTable.ClosedPostPosition, searchTerm, pageParam, employeeId, changedOn, cancellationToken).ConfigureAwait(false);
        return await BuildClosedPostPositionAuditListAsync(page, pageParam, cancellationToken).ConfigureAwait(false);
    }

    private void ApplyDepartmentTimezone(PagedList<AuditBase> page) => AuditTimezoneConverter.Apply(page, _tokenParser.GetTimeZoneId());

    private static Expression<Func<AuditBase, bool>> BuildSearchFilter(
        string? searchTerm,
        Guid departmentId,
        Guid employeeId = default,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var hasEmployee = employeeId != Guid.Empty;
        var hasSearch = !string.IsNullOrWhiteSpace(searchTerm);
        var searchLower = hasSearch ? searchTerm!.Trim().ToLower() : string.Empty;
        var hasStartDate = startDate.HasValue;
        var hasEndDate = endDate.HasValue;
        var start = startDate?.Date;
        var end = endDate?.Date;

        return x =>
            x.DepartmentID == departmentId
            && (!hasEmployee || x.EmployeeId == employeeId)
            && (!hasStartDate || x.ChangedOn.Date >= start)
            && (!hasEndDate || x.ChangedOn.Date <= end)
            && (!hasSearch
                || (x.EntityId != null && x.EntityId.ToLower().Contains(searchLower))
                || (x.DisplayName != null && x.DisplayName.ToLower().Contains(searchLower))
                || (x.UserId != null && x.UserId.ToLower().Contains(searchLower)));
    }

    private static Expression<Func<AuditBase, bool>> BuildEmailTemplateSearchFilter(
        string? searchTerm,
        Guid employeeId,
        DateTime? startDate,
        DateTime? endDate)
    {
        var hasEmployee = employeeId != Guid.Empty;
        var hasSearch = !string.IsNullOrWhiteSpace(searchTerm);
        var searchLower = hasSearch ? searchTerm!.Trim().ToLower() : string.Empty;
        var hasStartDate = startDate.HasValue;
        var hasEndDate = endDate.HasValue;
        var start = startDate?.Date;
        var end = endDate?.Date;

        return x =>
            (!hasEmployee || x.EmployeeId == employeeId)
            && (!hasStartDate || x.ChangedOn.Date >= start)
            && (!hasEndDate || x.ChangedOn.Date <= end)
            && (!hasSearch
                || (x.EntityId != null && x.EntityId.ToLower().Contains(searchLower))
                || (x.DisplayName != null && x.DisplayName.ToLower().Contains(searchLower))
                || (x.UserId != null && x.UserId.ToLower().Contains(searchLower)));
    }

    private async Task<PagedList<ClosedPostPositionAuditListDto>> BuildClosedPostPositionAuditListAsync(PagedList<AuditBase> auditPage, PageParam pageParam, CancellationToken cancellationToken = default)
    {
        var closedPostIds = auditPage.Data
            .Select(a => Guid.TryParse(a.EntityId, out var id) ? id : Guid.Empty)
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        var byId = await _closedPostPositionRepository.GetAuditEnrichmentByIdsAsync(closedPostIds, cancellationToken).ConfigureAwait(false);

        var rows = auditPage.Data.Select(audit =>
        {
            Guid.TryParse(audit.EntityId, out var closedPostPositionId);
            byId.TryGetValue(closedPostPositionId, out var enriched);

            return new ClosedPostPositionAuditListDto
            {
                AuditId = audit.Id,
                ClosedPostPositionId = closedPostPositionId,
                EmployeeName = enriched?.EmployeeName,
                Date = enriched?.Date,
                StartTime = enriched?.StartTime,
                EndTime = enriched?.EndTime,
                HoursWorked = enriched?.HoursWorked ?? string.Empty,
                HoursRequired = enriched?.HoursRequired ?? string.Empty,
                JobTitle = enriched?.JobTitle,
                Type = enriched?.Type ?? nameof(ClosedPostPositionStatus.Closed),
                Action = audit.Action,
                ChangedOn = audit.ChangedOn,
                ChangedFields = audit.ChangedFields ?? "[]"
            };
        }).ToList();

        return new PagedList<ClosedPostPositionAuditListDto>(rows, pageParam, auditPage.TotalRecords);
    }
}
