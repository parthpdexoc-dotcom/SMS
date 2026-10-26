using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Post-specific repository implementation.
    /// </summary>
    public class PostPositionRepository : GenericRepository<PostPosition, SMSDbContext>, IPostPositionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostPositionRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="mapper">The automapper instance.</param>
        private readonly ILogger<PostPositionRepository> _logger;
        private readonly SMSDbContext _context;
        private readonly IMapper _mapper;

        public PostPositionRepository(
            SMSDbContext context,
            ILogger<PostPositionRepository> logger,
            IMapper mapper) : base(context, logger)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<List<PostPosition>> GetAssignedWithDateOfWorkAsync(
            Guid? employeeId,
            bool restrictToEmployee,
            Guid? requestDepartmentId,
            IReadOnlyCollection<PostPositionStatus> statuses,
            Expression<Func<PostPosition, object>>[]? includes)
        {
            if (statuses == null || statuses.Count == 0)
                return new List<PostPosition>();

            var hasDepartmentFilter = requestDepartmentId.HasValue && requestDepartmentId.Value != Guid.Empty;
            var filterByEmployee = restrictToEmployee && employeeId.HasValue && employeeId.Value != Guid.Empty;

            var rows = await GetAllAsync(
                filter: x =>
                    (!filterByEmployee || x.EmployeeID == employeeId!.Value) &&
                    (!hasDepartmentFilter || (x.Post != null && x.Post.Request != null && x.Post.Request.DepartmentID == requestDepartmentId)) &&
                    statuses.Contains(x.PostPositionStatus) &&
                    x.DateOfWork.HasValue,
                includes,
                pageParam: null).ConfigureAwait(false);

            return rows.ToList();
        }

        public async Task<List<PostPosition>> GetPostPositionByEmployeeAndDateRangeAsync(Guid employeeId,DateOnly startDate,DateOnly endDate,Guid? departmentId = null,bool excludeCancelledOnly = true,bool enableExemptRequestsJobsFromHoursRules = false)
        {
            try
            {
                var query = _context.PostPosition
            .Where(pp => pp.EmployeeID == employeeId &&
                         !pp.IsPlaceholder &&
                         pp.DateOfWork >= startDate &&
                         pp.DateOfWork <= endDate);

                query = excludeCancelledOnly
                    ? query.Where(pp => pp.PostPositionStatus != PostPositionStatus.Cancelled && pp.PostPositionStatus != PostPositionStatus.Declined && pp.PostPositionStatus != PostPositionStatus.Transferred)
                    : query.Where(pp =>
                        pp.PostPositionStatus == PostPositionStatus.Assigned ||
                        pp.PostPositionStatus == PostPositionStatus.Closed ||
                        pp.PostPositionStatus == PostPositionStatus.Unclosed ||
                        pp.PostPositionStatus == PostPositionStatus.CancellationPending ||
                        pp.PostPositionStatus == PostPositionStatus.SplitDecisionPending ||
                        pp.PostPositionStatus == PostPositionStatus.ClosedAndSplit)
                    .Include(pp => pp.ClosedPostPositions.Where(cpp => cpp.Status != ClosedPostPositionStatus.Unclosed).OrderByDescending(x => x.CreatedDate).Take(1));

                query = enableExemptRequestsJobsFromHoursRules
                    ? query.Where(pp => pp.Post == null || !pp.Post.IsExemptHours)
                    : query;

                if (departmentId.HasValue && departmentId.Value != Guid.Empty)
                {
                    query = query
                        .Where(pp => pp.Post != null && pp.Post.Request != null && pp.Post.Request.DepartmentID == departmentId.Value);
                }

                return await query.ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex) {
                _logger?.LogError(ex, "Failed to get post positions for employee {EmployeeId} between {StartDate} and {EndDate}", employeeId, startDate, endDate);
                return new List<PostPosition>();
            }
        }

        public async Task<int> GetPostsTakenLast24HoursAsync(Guid employeeId, DateTime dateTime)
        {
            var since = dateTime.AddHours(-24);
            var sinceDate = DateOnly.FromDateTime(since);
            var sinceTime = TimeOnly.FromDateTime(since);

            return await _context.PostPosition
                .Where(p =>
                    p.EmployeeID == employeeId &&
                    (
                        p.DateOfWork > sinceDate ||
                        (p.DateOfWork == sinceDate && p.StartTime >= sinceTime)
                    ))
                .CountAsync();
        }

        public async Task<PostPosition?> AssignTransferredPostAsync(
            Guid targetEmployeeId,
            Guid postId,
            DateOnly dateOfWork,
            TimeOnly postStartTime,
            TimeOnly postEndTime,
            int postLength,
            string? callSign,
            Guid? createdUserId,
            string? createdBy,
            bool enableInvoiceAndPayroll = false)
        {
            var nowUtc = DateTime.UtcNow;
            var isExemptPosition = await _context.Set<Post>()
                .Where(p => p.PostID == postId && p.IsExemptHours && p.Request != null && p.Request.DepartmentID != null)
                .Select(p => _context.Set<DepartmentConfiguration>().Any(c =>
                    c.DepartmentID == p.Request!.DepartmentID &&
                    c.EnableExemptRequestsJobsFromHoursRules == true))
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            var newPostPosition = new PostPosition
            {
                PostID = postId,
                EmployeeID = targetEmployeeId,
                DateOfWork = dateOfWork,
                StartTime = postStartTime,
                EndTime = postEndTime,
                LengthOfWork = postLength,
                CallSign = callSign,
                EnableInvoiceAndPayroll = enableInvoiceAndPayroll,
                IsExemptPosition = isExemptPosition,
                PostPositionStatus = PostPositionStatus.Assigned,
                CreatedDate = nowUtc,
                EditedDate = nowUtc,
                CreatedUserID = createdUserId,
                EditedUserID = createdUserId,
                CreatedBy = createdBy
            };

            _context.Set<PostPosition>().Add(newPostPosition);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return newPostPosition;
        }

        public async Task<bool> TransferSourceAsync(Guid sourcePostPositionId, Guid? editedUserId, string? editedBy)
        {
            var updated = await UpdateAsync(
                pp => pp.PostPositionID == sourcePostPositionId,
                pp =>
                {
                    pp.PostPositionStatus = PostPositionStatus.Transferred;
                    pp.EditedDate = DateTime.UtcNow;
                    pp.EditedUserID = editedUserId;
                    pp.EditedBy = editedBy;
                }).ConfigureAwait(false);

            return updated > 0;
        }

        public async Task<bool> SetClosedAndSplitStatusAsync(Guid sourcePostPositionId, Guid? editedUserId, string? editedBy)
        {
            var updated = await UpdateAsync(
                pp => pp.PostPositionID == sourcePostPositionId,
                pp =>
                {
                    pp.PostPositionStatus = PostPositionStatus.ClosedAndSplit;
                    pp.IsSplit = true;
                    pp.EditedDate = DateTime.UtcNow;
                    pp.EditedUserID = editedUserId;
                    pp.EditedBy = editedBy;
                }).ConfigureAwait(false);

            return updated > 0;
        }

        /// <summary>
        /// Updates post position status directly in the database, bypassing the EF change tracker.
        /// Use when another scoped DbContext may have already changed the row (e.g. cancel flow:
        /// CheckAvailablePost sets Cancelled in a separate scope while this context still tracks
        /// CancellationPending). Tracked <see cref="UpdateAsync"/> would see no change and write 0 rows.
        /// </summary>
        public Task<int> UpdatePostPositionStatusDirectAsync(Guid postPositionId, PostPositionStatus status)
        {
            return _context.Set<PostPosition>()
                .Where(x => x.PostPositionID == postPositionId)
                .ExecuteUpdateAsync(st => st
                    .SetProperty(x => x.PostPositionStatus, status)
                    .SetProperty(x => x.EditedDate, DateTime.UtcNow));
        }

        /// <inheritdoc />
        public async Task<int> CancelPostPositionsByIdsAsync(
            IReadOnlyCollection<Guid> postPositionIds,
            CancellationToken cancellationToken = default)
        {
            if (postPositionIds == null || postPositionIds.Count == 0)
                return 0;

            var ids = postPositionIds.Where(id => id != Guid.Empty).Distinct().ToList();
            if (ids.Count == 0)
                return 0;

            return await _context.PostPosition
                .Where(pp => ids.Contains(pp.PostPositionID))
                .ExecuteUpdateAsync(
                    st => st
                        .SetProperty(x => x.PostPositionStatus, PostPositionStatus.Cancelled)
                        .SetProperty(x => x.ReasonForCancel, (string?)null)
                        .SetProperty(x => x.EditedDate, DateTime.UtcNow),
                    cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<PostPosition>> GetOfficerSchedulePostPositionsAsync(
            Guid departmentId,
            DateOnly minDateOfWork,
            DateOnly maxDateOfWork,
            CancellationToken cancellationToken = default)
        {
            return await _context.PostPosition
                .AsNoTracking()
                .Where(pp =>
                    (pp.PostPositionStatus == PostPositionStatus.Assigned
                     || pp.PostPositionStatus == PostPositionStatus.CancellationPending)
                    && !pp.IsPlaceholder
                    && pp.EmployeeID.HasValue
                    && pp.EmployeeID != Guid.Empty
                    && pp.DateOfWork >= minDateOfWork
                    && pp.DateOfWork <= maxDateOfWork
                    && pp.Post != null
                    && pp.Post.Request != null
                    && pp.Post.Request.DepartmentID == departmentId)
                .Include(pp => pp.Employee!)
                    .ThenInclude(e => e.EmployeeType)
                .Include(pp => pp.Employee!)
                    .ThenInclude(e => e.Status)
                .Include(pp => pp.Post)
                    .ThenInclude(p => p.Request!)
                    .ThenInclude(r => r.RequestSchedules)
                .Include(pp => pp.Post)
                    .ThenInclude(p => p.PayCode)
                .Include(pp => pp.Post)
                    .ThenInclude(p => p.EmployeeType)
                .Include(pp => pp.PostCallSign)
                .AsSplitQuery()
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<List<PostPosition>> GetAssignedRealEmployeesByRequestAndDateAsync(
            Guid requestId,
            DateOnly dateOfWork,
            CancellationToken cancellationToken = default)
        {
            return await _context.PostPosition
                .AsNoTracking()
                .Where(pp =>
                    pp.Post != null &&
                    pp.Post.RequestID == requestId &&
                    pp.DateOfWork == dateOfWork &&
                    !pp.IsPlaceholder &&
                    pp.EmployeeID.HasValue &&
                    pp.EmployeeID != Guid.Empty &&
                    (pp.PostPositionStatus == PostPositionStatus.Assigned ||
                     pp.PostPositionStatus == PostPositionStatus.CancellationPending))
                .Include(pp => pp.Employee)
                .Include(pp => pp.Post)
                .Include(pp => pp.PostCallSign)
                .OrderBy(pp => pp.StartTime)
                .ThenBy(pp => pp.Employee!.LastName)
                .ThenBy(pp => pp.Employee!.FirstName)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<OneTimeDateChangeReviewDataDto> GetOneTimeDateChangeReviewDataAsync(
            Guid requestId,
            DateOnly oldDate,
            DateOnly newDate,
            Guid? departmentId = null,
            CancellationToken cancellationToken = default)
        {
            var assignedOnOldDate = (await GetAssignedRealEmployeesByRequestAndDateAsync(requestId, oldDate, cancellationToken)
                .ConfigureAwait(false))
                .Select(pp => new OneTimeDateChangeAssignedRowDto
                {
                    PostPositionID = pp.PostPositionID,
                    EmployeeID = pp.EmployeeID!.Value,
                    EmployeeFirstName = pp.Employee!.FirstName,
                    EmployeeLastName = pp.Employee!.LastName,
                    PostID = pp.PostID,
                    PostTitle = pp.Post!.PostTitle,
                    DateOfWork = pp.DateOfWork!.Value,
                    StartTime = pp.StartTime ?? pp.Post.PostStartTime,
                    EndTime = pp.EndTime ?? pp.Post.PostEndTime,
                    Duration = pp.LengthOfWork > 0 ? pp.LengthOfWork : pp.Post.PostLength
                })
                .ToList();

            if (assignedOnOldDate.Count == 0)
                return new OneTimeDateChangeReviewDataDto();

            var employeeIds = assignedOnOldDate.Select(row => row.EmployeeID).Distinct().ToList();

            var employeeSlotsOnNewDate = await _context.PostPosition
                .AsNoTracking()
                .Where(pp =>
                    pp.EmployeeID.HasValue &&
                    employeeIds.Contains(pp.EmployeeID.Value) &&
                    !pp.IsPlaceholder &&
                    pp.DateOfWork == newDate &&
                    pp.PostPositionStatus != PostPositionStatus.Cancelled &&
                    pp.PostPositionStatus != PostPositionStatus.Declined &&
                    pp.PostPositionStatus != PostPositionStatus.Transferred &&
                    (!departmentId.HasValue || departmentId.Value == Guid.Empty ||
                     (pp.Post != null && pp.Post.Request != null && pp.Post.Request.DepartmentID == departmentId.Value)))
                .Select(pp => new OneTimeDateChangeConflictSlotRowDto
                {
                    EmployeeID = pp.EmployeeID!.Value,
                    PostID = pp.PostID,
                    DateOfWork = pp.DateOfWork!.Value,
                    StartTime = pp.StartTime ?? pp.Post!.PostStartTime,
                    EndTime = pp.EndTime ?? pp.Post!.PostEndTime,
                    LengthOfWork = pp.LengthOfWork > 0 ? pp.LengthOfWork : pp.Post!.PostLength
                })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new OneTimeDateChangeReviewDataDto
            {
                AssignedOnOldDate = assignedOnOldDate,
                EmployeeSlotsOnNewDate = employeeSlotsOnNewDate
            };
        }

        /// <inheritdoc />
        public async Task<(List<PostPosition> Rows, int Total)> GetAssignedRecipientsPagedAsync(
            Expression<Func<PostPosition, bool>> filter,
            PageParam pageParam)
        {
            // Employees: one row per recipient email (off-duty callback preferred over primary,
            // employee id when both are missing). Placeholders: one row per post position.
            var filtered = _context.PostPosition
                .AsNoTracking()
                .Where(filter);

            var total = await filtered
                .Select(pp => pp.IsPlaceholder
                    ? "placeholder:" + pp.PostPositionID.ToString()
                    : ((pp.Employee!.OffDutyCallBackEmail ?? pp.Employee!.PrimaryEmail) ?? pp.EmployeeID.ToString()))
                .Distinct()
                .CountAsync()
                .ConfigureAwait(false);

            // Scalar aggregate projection translates to GROUP BY ... MIN in SQL
            // (grouping by whole entity + navigation ordering is not translatable).
            var pageIds = await filtered
                .GroupBy(pp => pp.IsPlaceholder
                    ? "placeholder:" + pp.PostPositionID.ToString()
                    : ((pp.Employee!.OffDutyCallBackEmail ?? pp.Employee!.PrimaryEmail) ?? pp.EmployeeID.ToString()))
                .Select(g => new
                {
                    PostPositionId = g.Min(pp => pp.PostPositionID),
                    LastName = g.Min(pp => pp.IsPlaceholder ? pp.Placeholder!.LastName : pp.Employee!.LastName),
                    FirstName = g.Min(pp => pp.IsPlaceholder ? pp.Placeholder!.FirstName : pp.Employee!.FirstName),
                })
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Skip(pageParam.Offset!.Value)
                .Take(pageParam.Limit!.Value)
                .Select(x => x.PostPositionId)
                .ToListAsync()
                .ConfigureAwait(false);

            if (pageIds.Count == 0)
                return (new List<PostPosition>(), total);

            var rows = await _context.PostPosition
                .AsNoTracking()
                .AsSplitQuery()
                .Where(pp => pageIds.Contains(pp.PostPositionID))
                .Include(pp => pp.Employee)
                .Include(pp => pp.Placeholder)
                .ToListAsync()
                .ConfigureAwait(false);

            var orderById = pageIds.Select((id, index) => (id, index)).ToDictionary(x => x.id, x => x.index);
            return (rows.OrderBy(pp => orderById[pp.PostPositionID]).ToList(), total);
        }

        /// <inheritdoc />
        public async Task<List<DateOnly>> GetDistinctDatesOfWorkByPostIdAsync(
            Guid postId,
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            CancellationToken cancellationToken = default)
        {
            return await _context.PostPosition
                .AsNoTracking()
                .Where(x =>
                    x.PostID == postId &&
                    x.DateOfWork.HasValue &&
                    (!startDate.HasValue || x.DateOfWork.Value >= startDate.Value) &&
                    (!endDate.HasValue || x.DateOfWork.Value <= endDate.Value) &&
                    (x.PostPositionStatus == PostPositionStatus.Assigned ||
                     x.PostPositionStatus == PostPositionStatus.Closed ||
                     x.PostPositionStatus == PostPositionStatus.Cancelled ||
                     x.PostPositionStatus == PostPositionStatus.CancellationPending ||
                     x.PostPositionStatus == PostPositionStatus.Unclosed ||
                     x.PostPositionStatus == PostPositionStatus.ClosedAndSplit))
                .Select(x => x.DateOfWork!.Value)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<PagedList<PostPositionInRequestListDto>> GetByPostFilterAsync(
            Guid postId,
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            PageParam? pagination = null,
            CancellationToken cancellationToken = default)
        {
            var pageParam = pagination ?? new PageParam();

            IQueryable<PostPosition> query = _context.PostPosition
                .AsNoTracking()
                .Where(x =>
                    x.PostID == postId &&
                    x.DateOfWork.HasValue &&
                    (!startDate.HasValue || x.DateOfWork!.Value >= startDate.Value) &&
                    (!endDate.HasValue || x.DateOfWork!.Value <= endDate.Value) &&
                    (x.PostPositionStatus == PostPositionStatus.Assigned ||
                     x.PostPositionStatus == PostPositionStatus.Closed ||
                     x.PostPositionStatus == PostPositionStatus.Cancelled ||
                     x.PostPositionStatus == PostPositionStatus.CancellationPending ||
                     x.PostPositionStatus == PostPositionStatus.Unclosed ||
                     x.PostPositionStatus == PostPositionStatus.ClosedAndSplit))
                .OrderBy(x => x.DateOfWork)
                .ThenBy(x => x.CallSign)
                .ThenBy(x => x.Employee != null ? x.Employee.LastName : (x.Placeholder != null ? x.Placeholder.LastName : null));

            var totalRecords = await query.LongCountAsync(cancellationToken).ConfigureAwait(false);

            if (pagination != null)
            {
                query = query
                    .Skip(pageParam.Offset ?? 0)
                    .Take(pageParam.Limit ?? PageParam.MinLimit);
            }

            query = query
                .Include(x => x.Employee!)
                    .ThenInclude(e => e.EmployeeType)
                .Include(x => x.Placeholder)
                .Include(x => x.PostCallSign)
                .AsSplitQuery();

            var rows = await query.ToListAsync(cancellationToken).ConfigureAwait(false);

            var data = rows.Select(pp => new PostPositionInRequestListDto
            {
                PostPositionId = pp.PostPositionID,
                PostCallSignId = pp.PostCallSignID,
                CallSign = pp.CallSign ?? pp.PostCallSign?.CallSign,
                EmployeeName = pp.Employee != null
                    ? pp.Employee.LastName + ", " + pp.Employee.FirstName
                    : (pp.Placeholder != null
                        ? pp.Placeholder.LastName + ", " + pp.Placeholder.FirstName
                        : null),
                DateOfWork = pp.DateOfWork,
                EmployeeType = pp.Employee?.EmployeeType?.EmployeeTypeShort,
                BadgeNumber = pp.Employee != null
                    ? pp.Employee.BadgeNumber
                    : pp.Placeholder?.EmployeeNumber,
                Phone = pp.Employee != null
                    ? pp.Employee.PrimaryPhone
                    : pp.Placeholder?.ContactNumber,
                Status = pp.PostPositionStatus,
                IsPlaceholder = pp.IsPlaceholder,
                PlaceholderId = pp.PlaceholderID,
                IsUniquePerPosition = pp.Placeholder != null && pp.Placeholder.IsUniquePerPosition
            }).ToList();

            return new PagedList<PostPositionInRequestListDto>(data, pageParam, totalRecords);
        }

        /// <inheritdoc />
        public async Task<List<PostPositionResponseDto>> GetPostPositionsWithEmployeeColumnsAsync(
            Guid postId,
            DateOnly? dateOfWork,
            IReadOnlyCollection<DateOnly>? datesOfWork,
            IReadOnlyCollection<RequestStatus>? requestStatuses,
            bool includeCompleted,
            PageParam? pagination = null,
            CancellationToken cancellationToken = default)
        {
            var dates = datesOfWork is { Count: > 0 } ? datesOfWork.ToList() : null;

            IQueryable<PostPosition> source = _context.PostPosition
                .AsNoTracking()
                .Where(x =>
                    x.PostID == postId &&
                    (
                        dateOfWork.HasValue
                            ? x.DateOfWork == dateOfWork.Value
                            : dates == null || (x.DateOfWork.HasValue && dates.Contains(x.DateOfWork.Value))
                    ) &&
                    (x.PostPositionStatus == PostPositionStatus.Assigned ||
                     x.PostPositionStatus == PostPositionStatus.Closed ||
                     x.PostPositionStatus == PostPositionStatus.Cancelled ||
                     x.PostPositionStatus == PostPositionStatus.CancellationPending ||
                     x.PostPositionStatus == PostPositionStatus.Unclosed ||
                     x.PostPositionStatus == PostPositionStatus.ClosedAndSplit) &&
                    x.DateOfWork != null);

            // Same rule as the previous in-memory filter: status of the schedule that covers
            // DateOfWork and has the latest StartDate. No covering schedule yields Pending (0).
            var withStatus = source.Select(pp => new
            {
                Position = pp,
                Status = pp.Post.Request.RequestSchedules
                    .Where(rs =>
                        (rs.StartDate == null || pp.DateOfWork >= rs.StartDate) &&
                        (rs.EndDate == null || pp.DateOfWork <= rs.EndDate))
                    .OrderByDescending(rs => rs.StartDate)
                    .Select(rs => rs.RequestStatus)
                    .FirstOrDefault()
            });

            if (requestStatuses is { Count: > 0 })
            {
                var statuses = requestStatuses.ToList();
                withStatus = withStatus.Where(x =>
                    statuses.Contains(x.Status) &&
                    (includeCompleted || x.Status != RequestStatus.Completed));
            }

            var query = withStatus.Select(x => new PostPositionEmployeeColumnDto
            {
                PostPositionId = x.Position.PostPositionID,
                PostId = x.Position.PostID,
                IsPlaceholder = x.Position.IsPlaceholder,
                PlaceholderId = x.Position.PlaceholderID,
                EnableInvoiceAndPayroll = x.Position.EnableInvoiceAndPayroll,
                DateOfWork = x.Position.DateOfWork,
                PostStartTime = x.Position.StartTime,
                CallSign = x.Position.CallSign ?? (x.Position.PostCallSign != null ? x.Position.PostCallSign.CallSign : null),
                PostCallSignId = x.Position.PostCallSignID,
                PostPositionStatus = x.Position.PostPositionStatus,
                HasEmployee = x.Position.Employee != null,
                FirstName = x.Position.Employee != null ? x.Position.Employee.FirstName : (x.Position.Placeholder != null ? x.Position.Placeholder.FirstName : null),
                LastName = x.Position.Employee != null ? x.Position.Employee.LastName : (x.Position.Placeholder != null ? x.Position.Placeholder.LastName : null),
                BadgeNumber = x.Position.Employee != null ? x.Position.Employee.BadgeNumber : (x.Position.Placeholder != null ? x.Position.Placeholder.EmployeeNumber : null),
                PrimaryPhone = x.Position.Employee != null ? x.Position.Employee.PrimaryPhone : (x.Position.Placeholder != null ? x.Position.Placeholder.ContactNumber : null),
                EmployeeTypeName = x.Position.Employee != null && x.Position.Employee.EmployeeType != null
                    ? x.Position.Employee.EmployeeType.EmployeeTypeShort
                    : null,
                IsUniquePerPosition = x.Position.Placeholder != null && x.Position.Placeholder.IsUniquePerPosition
            });

            if (pagination != null)
            {
                query = query
                    .Skip(pagination.Offset ?? 0)
                    .Take(pagination.Limit ?? PageParam.MinLimit);
            }

            var rows = await query.ToListAsync(cancellationToken).ConfigureAwait(false);

            return rows
                .Select(row => new PostPositionResponseDto
                {
                    PostPositionID = row.PostPositionId,
                    PostID = row.PostId,
                    IsPlaceholder = row.IsPlaceholder,
                    PlaceholderID = row.PlaceholderId,
                    EnableInvoiceAndPayroll = row.EnableInvoiceAndPayroll,
                    DateOfWork = row.DateOfWork,
                    PostStartTime = row.PostStartTime,
                    CallSign = row.CallSign,
                    PostCallSignID = row.PostCallSignId,
                    PostPositionStatus = row.PostPositionStatus,
                    EmployeeName = row.HasEmployee
                        ? $"{row.LastName}, {row.FirstName}".Trim()
                        : PlaceholderDto.FormatDisplayName(row.LastName, row.FirstName),
                    FirstName = row.FirstName,
                    LastName = row.LastName,
                    BadgeNumber = row.BadgeNumber,
                    PrimaryPhone = row.PrimaryPhone,
                    EmployeeTypeName = row.EmployeeTypeName,
                    IsUniquePerPosition = row.IsUniquePerPosition
                })
                .ToList();
        }

        public async Task<bool> SwapCallSignAsync(Guid postPositionId1, Guid postPositionId2)
        {
            var postPositions = (await GetAllAsync(
                    filter: x => x.PostPositionID == postPositionId1 || x.PostPositionID == postPositionId2,
                    includes: null,
                    pageParam: null).ConfigureAwait(false)).ToList();

            if (postPositions.Count != 2)
                return false;

            var position1 = postPositions.First(x => x.PostPositionID == postPositionId1);
            var position2 = postPositions.First(x => x.PostPositionID == postPositionId2);
            var position1CallSign = (position1.PostCallSignID, position1.CallSign);
            var position2CallSign = (position2.PostCallSignID, position2.CallSign);

            position1.PostCallSignID = position2CallSign.PostCallSignID;
            position1.CallSign = position2CallSign.CallSign;
            position2.PostCallSignID = position1CallSign.PostCallSignID;
            position2.CallSign = position1CallSign.CallSign;

            await _context.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        /// <inheritdoc />
        public Task<PostPosition?> GetByIdForFlowAuditAsync(
            Guid postPositionId,
            CancellationToken cancellationToken = default) =>
            _context.PostPosition
                .IgnoreQueryFilters()
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.Post!)
                .Include(x => x.Employee!)
                .FirstOrDefaultAsync(x => x.PostPositionID == postPositionId, cancellationToken);

        /// <inheritdoc />
        public async Task<List<EmployeePostListingItemDto>> GetTodaysJobsAsync(
            Guid employeeId,
            DateOnly dateOfWork,
            Guid? departmentId,
            CancellationToken cancellationToken = default)
        {
            var hasDepartmentFilter = departmentId.HasValue && departmentId.Value != Guid.Empty;
            var filterDepartmentId = departmentId.GetValueOrDefault();

            var query = _context.PostPosition.AsNoTracking()
                .Include(pp => pp.Post)
                    .ThenInclude(p => p.Request)
                .Include(pp => pp.Employee)
                .Where(pp => !pp.IsPlaceholder
                      && pp.EmployeeID == employeeId
                      && pp.DateOfWork == dateOfWork
                      && (pp.PostPositionStatus == PostPositionStatus.Assigned
                          || pp.PostPositionStatus == PostPositionStatus.SplitDecisionPending
                          || pp.PostPositionStatus == PostPositionStatus.Unclosed
                          || pp.PostPositionStatus == PostPositionStatus.CancellationPending));

            if (hasDepartmentFilter)
            {
                query = query.Where(pp =>
                    pp.Post != null
                    && pp.Post.Request != null
                    && pp.Post.Request.DepartmentID == filterDepartmentId);
            }

            var positions = await query
                .OrderBy(pp => pp.StartTime ?? (pp.Post != null ? (TimeOnly?)pp.Post.PostStartTime : null))
                .ThenBy(pp => pp.EndTime ?? (pp.Post != null ? (TimeOnly?)pp.Post.PostEndTime : null))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return _mapper.Map<List<EmployeePostListingItemDto>>(positions);
        }
    }
}

