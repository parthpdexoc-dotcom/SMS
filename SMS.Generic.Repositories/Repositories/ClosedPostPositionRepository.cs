using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository for ClosedPostPosition entity operations.
    /// </summary>
    public class ClosedPostPositionRepository : GenericRepository<ClosedPostPosition, SMSDbContext>, IClosedPostPositionRepository
    {
        private readonly SMSDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClosedPostPositionRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public ClosedPostPositionRepository(
            SMSDbContext context,
            ILogger<ClosedPostPositionRepository> logger)
            : base(context, logger)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyDictionary<Guid, ClosedPostPositionAuditListDto>> GetAuditEnrichmentByIdsAsync(
            IReadOnlyCollection<Guid> closedPostPositionIds,
            CancellationToken cancellationToken = default)
        {
            if (closedPostPositionIds.Count == 0)
                return new Dictionary<Guid, ClosedPostPositionAuditListDto>();

            var raw = await (
                from c in _context.ClosedPostPosition.IgnoreQueryFilters().AsNoTracking()
                where closedPostPositionIds.Contains(c.ClosedPostPositionID)
                join pp in _context.PostPosition.IgnoreQueryFilters().AsNoTracking() on c.PostPositionID equals pp.PostPositionID into ppGroup
                from pp in ppGroup.DefaultIfEmpty()
                join e in _context.Employee.IgnoreQueryFilters().AsNoTracking() on pp.EmployeeID equals e.EmployeeID into empGroup
                from e in empGroup.DefaultIfEmpty()
                join p in _context.Post.IgnoreQueryFilters().AsNoTracking() on c.PostID equals p.PostID into postGroup
                from p in postGroup.DefaultIfEmpty()
                select new
                {
                    c.ClosedPostPositionID,
                    EmployeeLastName = e != null ? e.LastName : null,
                    EmployeeFirstName = e != null ? e.FirstName : null,
                    c.WorkStartDate,
                    DateOfWork = pp != null ? pp.DateOfWork : null,
                    c.WorkStartTime,
                    c.WorkEndTime,
                    c.PostStartTime,
                    c.PostEndTime,
                    c.HoursWorked,
                    PostPositionLengthOfWork = pp != null ? pp.LengthOfWork : 0,
                    PostLength = p != null ? p.PostLength : 0,
                    JobTitle = p != null ? p.PostTitle : null,
                    c.Status
                })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return raw.ToDictionary(
                x => x.ClosedPostPositionID,
                x =>
                {
                    var hoursRequired = x.PostPositionLengthOfWork > 0 ? x.PostPositionLengthOfWork : x.PostLength;
                    return new ClosedPostPositionAuditListDto
                    {
                        ClosedPostPositionId = x.ClosedPostPositionID,
                        EmployeeName = PersonNameFormatHelper.FormatLastNameCommaFirstName(
                            x.EmployeeLastName,
                            x.EmployeeFirstName),
                        Date = x.WorkStartDate ?? x.DateOfWork,
                        StartTime = x.WorkStartTime ?? x.PostStartTime,
                        EndTime = x.WorkEndTime ?? x.PostEndTime,
                        HoursWorked = DateHelper.ToDisplayHoursAndMinutesFromMinutes(x.HoursWorked),
                        HoursRequired = DateHelper.ToDisplayHoursAndMinutesFromMinutes(hoursRequired),
                        JobTitle = x.JobTitle,
                        Type = (x.Status ?? ClosedPostPositionStatus.Closed).ToString()
                    };
                });
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<ClosedPostPosition>> GetLatestPerPostPositionForEmployeeAsync(Guid employeeId)
        {
            if (employeeId == Guid.Empty)
                throw new ArgumentException("Employee id cannot be empty.", nameof(employeeId));

            var raw = await _context.ClosedPostPosition
                .AsNoTracking()
                .AsSplitQuery()
                .Where(x => x.PostPosition.EmployeeID == employeeId && !x.PostPosition.IsPlaceholder && x.PostPositionID != Guid.Empty)
                .Include(x => x.PostPosition)
                .Include(x => x.Post)
                .ToListAsync()
                .ConfigureAwait(false);

            return raw
                .Where(x => x.PostPositionID != Guid.Empty)
                .GroupBy(x => x.PostPositionID)
                .Select(g => g
                    .OrderByDescending(x => x.IsEdited)
                    .ThenByDescending(x => x.WorkStartDate ?? DateOnly.MinValue)
                    .ThenByDescending(x => x.CreatedDate ?? DateTime.MinValue)
                    .First())
                .OrderByDescending(x => x.RequestedStartDate ?? DateOnly.MinValue)
                .ThenByDescending(x => x.CreatedDate ?? DateTime.MinValue)
                .ThenBy(x => x.PostPositionID)
                .ToList();
        }

        /// <inheritdoc />
        public async Task<(IReadOnlyList<ClosedPostPosition> Items, int TotalCount)> GetCloseAssignmentsPageAsync(
            IReadOnlyList<ClosedPostPositionStatus> statusFilter,
            string? searchTerm,
            bool filterByEmployee,
            Guid? employeeId,
            bool filterByDepartment,
            Guid? departmentId,
            DateOnly? startDate,
            DateOnly? endDate,
            PageParam pageParam,
            bool isPlaceholderOnly = false,
            Guid? requesterId = null)
        {
            if (statusFilter == null || statusFilter.Count == 0)
                throw new ArgumentException("At least one status is required.", nameof(statusFilter));

            IQueryable<ClosedPostPosition> q = _context.ClosedPostPosition
                .AsNoTracking()
                .Where(x => x.Status.HasValue && statusFilter.Contains(x.Status.Value))
                .Where(x => x.PostPositionID != Guid.Empty)
                .Where(x => x.PostPosition != null && (isPlaceholderOnly ? x.PostPosition.IsPlaceholder : !x.PostPosition.IsPlaceholder));

            if (filterByEmployee)
            {
                if (!employeeId.HasValue || employeeId.Value == Guid.Empty)
                    throw new ArgumentException("Employee id is required when filtering by employee.", nameof(employeeId));
                var emp = employeeId.Value;
                q = q.Where(x => x.PostPosition!.EmployeeID == emp);
            }

            if (filterByDepartment)
            {
                if (!departmentId.HasValue || departmentId.Value == Guid.Empty)
                    throw new ArgumentException("Department id is required when filtering by department.", nameof(departmentId));
                var dept = departmentId.Value;
                q = q.Where(x =>
                    (x.Post != null && x.Post.Request != null && x.Post.Request.DepartmentID == dept) ||
                    (x.PostPosition != null && x.PostPosition.Post != null && x.PostPosition.Post.Request != null
                        && x.PostPosition.Post.Request.DepartmentID == dept));
            }

            if (startDate.HasValue)
                q = q.Where(x => x.PostPosition!.DateOfWork != null && x.PostPosition.DateOfWork >= startDate.Value);

            if (endDate.HasValue)
                q = q.Where(x => x.PostPosition!.DateOfWork != null && x.PostPosition.DateOfWork <= endDate.Value);

            if (requesterId.HasValue && requesterId.Value != Guid.Empty)
            {
                var req = requesterId.Value;
                q = q.Where(x =>
                    (x.Post != null && x.Post.Request != null && x.Post.Request.RequesterID == req)
                    || (x.PostPosition != null && x.PostPosition.Post != null && x.PostPosition.Post.Request != null
                        && x.PostPosition.Post.Request.RequesterID == req));
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var pattern = $"%{searchTerm.Trim()}%";
                q = q.Where(x =>
                    // Job title (post)
                    (x.Post != null && x.Post.PostTitle != null && EF.Functions.Like(x.Post.PostTitle, pattern))
                    || (x.PostPosition != null && x.PostPosition.Post != null && x.PostPosition.Post.PostTitle != null
                        && EF.Functions.Like(x.PostPosition.Post.PostTitle, pattern))
                    // Requester name (job level)
                    || (x.Post != null && x.Post.Request != null && x.Post.Request.Requester != null
                        && x.Post.Request.Requester.Name != null
                        && EF.Functions.Like(x.Post.Request.Requester.Name, pattern))
                    || (x.PostPosition != null && x.PostPosition.Post != null && x.PostPosition.Post.Request != null
                        && x.PostPosition.Post.Request.Requester != null
                        && x.PostPosition.Post.Request.Requester.Name != null
                        && EF.Functions.Like(x.PostPosition.Post.Request.Requester.Name, pattern))
                    // Employee name
                    || (x.PostPosition != null && x.PostPosition.Employee != null
                        && ((x.PostPosition.Employee.FirstName != null
                                && EF.Functions.Like(x.PostPosition.Employee.FirstName, pattern))
                            || (x.PostPosition.Employee.LastName != null
                                && EF.Functions.Like(x.PostPosition.Employee.LastName, pattern)))));
            }

            // One row per PostPosition: keep the same "latest" row as GetLatestPerPostPositionForEmployeeAsync
            // (history edits collapse to a single list entry).
            var candidates = await q
                .Select(x => new
                {
                    x.PostPositionID,
                    x.ClosedPostPositionID,
                    x.IsEdited,
                    x.WorkStartDate,
                    x.CreatedDate,
                    DateOfWork = x.PostPosition!.DateOfWork
                })
                .ToListAsync()
                .ConfigureAwait(false);

            var latestPerPosition = candidates
                .GroupBy(x => x.PostPositionID)
                .Select(g => g
                    .OrderByDescending(x => x.IsEdited)
                    .ThenByDescending(x => x.WorkStartDate ?? DateOnly.MinValue)
                    .ThenByDescending(x => x.CreatedDate ?? DateTime.MinValue)
                    .First())
                .OrderBy(x => x.DateOfWork)
                .ThenBy(x => x.ClosedPostPositionID)
                .ToList();

            var total = latestPerPosition.Count;

            var skip = pageParam.Offset ?? 0;
            var take = pageParam.Limit ?? PageParam.MinLimit;
            if (take <= 0)
                take = PageParam.MinLimit;

            var pageIds = latestPerPosition
                .Skip(skip)
                .Take(take)
                .Select(x => x.ClosedPostPositionID)
                .ToList();

            if (pageIds.Count == 0)
                return (Array.Empty<ClosedPostPosition>(), total);

            var byId = await _context.ClosedPostPosition
                .AsNoTracking()
                .AsSplitQuery()
                .Where(x => pageIds.Contains(x.ClosedPostPositionID))
                .Include(x => x.PostPosition!)
                    .ThenInclude(pp => pp.Post)
                    .ThenInclude(p => p!.Request!)
                    .ThenInclude(r => r.Requester)
                .Include(x => x.PostPosition!)
                    .ThenInclude(pp => pp.Employee)
                .Include(x => x.PostPosition!)
                    .ThenInclude(pp => pp.Placeholder)
                .Include(x => x.Post!)
                    .ThenInclude(p => p.Request!)
                    .ThenInclude(r => r.Requester)
                .ToListAsync()
                .ConfigureAwait(false);

            var order = pageIds
                .Select((id, index) => (id, index))
                .ToDictionary(t => t.id, t => t.index);

            var items = byId
                .OrderBy(x => order[x.ClosedPostPositionID])
                .ToList();

            return (items, total);
        }
    }
}
