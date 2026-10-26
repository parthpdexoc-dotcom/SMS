using AutoMapper;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Common.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;
using System.Globalization;
using System.Linq.Expressions;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Request-specific repository implementation.
    /// </summary>
    public class RequestRepository : GenericRepository<Request, SMSDbContext>, IRequestRepository
    {
        private const int TenureReclaimLeaseMinutes = 1;

        private readonly SMSDbContext _context;
        private readonly ILogger<RequestRepository> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public RequestRepository(
            SMSDbContext context,
            ILogger<RequestRepository> logger,
            IMapper mapper) : base(context, logger)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Request>> SearchRequests(DateOnly startDate, DateOnly endDate, Guid departmentId, ReleaseAs release, DateTime currentDateTime)
        {
            try
            {
                // Slot logic only considers DateOfWork matching calendar days in the search window; the API also
                // applies a -24h window when deriving the first eligible date, which can sit one calendar day before
                // this repository's startDate. Keep positions in [startDate-1, endDate] plus undated rows.
                var positionRangeStart = startDate.AddDays(-1);
                var positionRangeEnd = endDate.AddDays(1);

                var requests = await _context.Requests
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Include(r => r.Posts)
                        .ThenInclude(p => p.PayCode)
                    .Include(r => r.Posts)
                        .ThenInclude(p => p.PostPositions.Where(pp =>
                            pp.DateOfWork == null ||
                            (pp.DateOfWork >= positionRangeStart && pp.DateOfWork <= positionRangeEnd)))
                            .ThenInclude(pp => pp.PostCallSign)
                    .Include(r => r.Posts)
                        .ThenInclude(p => p.WaitingPostPosition.Where(wp => wp.DateOfWork ==null ||(wp.DateOfWork>= positionRangeStart && wp.DateOfWork <= positionRangeEnd)))
                    .Include(r => r.Posts)
                        .ThenInclude(p => p.BlockDates.Where(bd =>
                                bd.StartDateTime.Date <= positionRangeEnd.ToDateTime(TimeOnly.MaxValue).Date &&
                                bd.EndDateTime.Date >= positionRangeStart.ToDateTime(TimeOnly.MinValue).Date))
                    .Include(r => r.Posts)
                        .ThenInclude(p => p.EmployeeType)
                    .Include(r => r.RequestType)
                    .Include(r => r.RequestSchedules)
                    .Include(r => r.Requester)
                        .ThenInclude(req => req.RequesterManagers)
                            .ThenInclude(rm => rm.RequesterEmployees)
                    .SelectMany(
                        r => r.RequestSchedules
                            .Where(s => s.EndDate.HasValue),
                        (r, s) => new { Request = r, Schedule = s })
                    .Where(x =>
                        x.Request.DepartmentID == departmentId &&
                        x.Schedule.StartDate.HasValue &&
                        x.Schedule.EndDate.HasValue &&
                        x.Schedule.StartDate.Value <= endDate &&
                        x.Schedule.EndDate.Value >= startDate &&
                        x.Schedule.RequestStatus == RequestStatus.ReleaseNow &&
                        x.Schedule.ReleaseAs == release)
                    .Select(x => x.Request)
                    .Distinct()
                    .ToListAsync()
                    .ConfigureAwait(false);

                return requests
                    .Select(r =>
                    {
                        var validSchedules = r.RequestSchedules
                            .Where(s =>
                                s.StartDate.HasValue &&
                                s.EndDate.HasValue &&
                                s.StartDate.Value <= endDate &&
                                s.EndDate.Value >= startDate &&
                                s.RequestStatus == RequestStatus.ReleaseNow &&
                                s.ReleaseAs == release &&
                                (s.ReleaseAs != ReleaseAs.TenureBased || IsWithinReleaseWindow(s, currentDateTime)))
                            .ToList();

                        r.RequestSchedules = validSchedules;
                        return r;
                    })
                    .Where(r => r.RequestSchedules.Any()) // keep only valid ones
                    .ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search requests for DepartmentId {DepartmentId} from {StartDate} to {EndDate}", departmentId, startDate, endDate);
                return new List<Request>();
            }
        }

        private static bool IsWithinReleaseWindow(RequestSchedule s, DateTime now)
        {
            // Backward compatibility (same as your earlier logic)
            if (!s.ScheduleDate.HasValue ||
                !s.ScheduledTime.HasValue ||
                !s.ReleaseHours.HasValue)
                return true;

            var start = s.ScheduleDate.Value.ToDateTime(s.ScheduledTime.Value);
            var end = start.AddHours(s.ReleaseHours.Value);

            return now >= start && now <= end;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<RequestFilterOptionDto>> GetRequestTitlesByDateRange(DateOnly startDate,DateOnly endDate,Guid departmentId)
        {
            return await _context.Requests
                .AsNoTracking()
                .AsSplitQuery()
                .Where(r => r.DepartmentID == departmentId && r.Posts.Any())
                .Where(r => r.RequestSchedules.Any(s =>
                    s.EndDate.HasValue &&
                    s.StartDate.HasValue &&
                    s.StartDate.Value <= endDate &&
                    s.EndDate.Value >= startDate &&
                    s.RequestStatus == RequestStatus.ReleaseNow))
                .Select(r => new RequestFilterOptionDto
                {
                    Id = r.RequestID,
                    Title = r.Title ?? string.Empty
                })
                .OrderBy(x => x.Title)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<RequestFilterOptionDto>> GetRequestersByDateRange(
            DateOnly startDate,
            DateOnly endDate,
            Guid departmentId)
        {
            return await _context.Requests
                .AsNoTracking()
                .Where(r => r.DepartmentID == departmentId && r.Posts.Any())
                .Where(r => r.RequestSchedules.Any(s =>
                    s.EndDate.HasValue &&
                    s.StartDate.HasValue &&
                    s.StartDate.Value <= endDate &&
                    s.EndDate.Value >= startDate &&
                    s.RequestStatus == RequestStatus.ReleaseNow))
                .GroupBy(r => new
                {
                    r.RequesterID,
                    Title = r.Requester != null ? r.Requester.Name ?? string.Empty : string.Empty
                })
                .Select(g => new RequestFilterOptionDto
                {
                    Id = g.Key.RequesterID,
                    Title = g.Key.Title
                })
                .OrderBy(x => x.Title)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<ReleaseAs> GetLatestReleaseAsForRequestAsync(Guid requestId)
        {
            var releaseAs = await _context.RequestSchedule
                .AsNoTracking()
                .Where(s => s.RequestId == requestId)
                .OrderByDescending(s => s.EditedDate ?? s.CreatedDate)
                .ThenByDescending(s => s.EndDate)
                .Select(s => s.ReleaseAs)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            return (ReleaseAs)releaseAs;
        }
        public async Task<Request> GetRequestByPostId(Guid postId, Guid departmentId)
        {
            try
            {
                return await _context.Requests
                    .AsSplitQuery()
                    .Include(r => r.Posts)
                    .ThenInclude(p => p.PostPositions)
                .Include(r => r.Posts)
                    .ThenInclude(p => p.WaitingPostPosition)
                .Include(r => r.Posts)
                    .ThenInclude(p => p.BlockDates)
                    .Include(p => p.Posts)
                    .ThenInclude(p => p.EmployeeType)
                .Include(r => r.RequestType)
                .Include(r => r.RequestSchedules)
                .Include(r => r.Requester)
                    .ThenInclude(req => req.RequesterManagers)
                        .ThenInclude(rm => rm.RequesterEmployees)
                .Where(r => r.DepartmentID == departmentId && r.Posts.Any(p => p.PostID == postId))
                .FirstAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get request by PostId {PostId} for DepartmentId {DepartmentId}", postId, departmentId);
                return null;
            }
        }

        public async Task<Request?> GetRequestForCloneAsync(Guid requestId, Guid departmentId)
        {
            try
            {
                return await _context.Requests
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Include(r => r.Posts)
                        .ThenInclude(p => p.BlockDates)
                    .Include(r => r.Posts)
                        .ThenInclude(p => p.EmployeeType)
                    .Include(r => r.RequestType)
                    .Include(r => r.Requester)
                        .ThenInclude(req => req.RequesterManagers)
                            .ThenInclude(rm => rm.RequesterEmployees)
                    .FirstOrDefaultAsync(r => r.RequestID == requestId && r.DepartmentID == departmentId)
                    .ConfigureAwait(false);
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<RequestScheduleProcessDueRequestDto?> GetTenureBasedRequestsDueForProcessingAsync(DateTime asOfUtc)
        {
            return await GetDueRequestsForProcessingAsync(asOfUtc, ReleaseAs.TenureBased, retryIncomplete: true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<RequestScheduleProcessDueRequestDto?> GetPreferredRequestsDueForProcessingAsync(DateTime asOfUtc)
        {
            return await GetDueRequestsForProcessingAsync(asOfUtc, ReleaseAs.Preferred).ConfigureAwait(false);
        }

        private async Task<RequestScheduleProcessDueRequestDto?> GetDueRequestsForProcessingAsync(
            DateTime currentDepartmentTime,
            ReleaseAs releaseAs,
            bool retryIncomplete = false)
        {
            var reclaimBefore = DateTime.UtcNow.AddMinutes(-TenureReclaimLeaseMinutes);

            _logger.LogInformation("Checking due request schedules for processing. {ReleaseAs} {CheckedTime:O}",releaseAs, currentDepartmentTime);
            // Keep tracking enabled because picked schedules are reserved immediately.
            // Tenure reruns Pending, InProgress, and Failed. Complete and Cancelled stay finished.
            // Preferred stays Pending-only.
            var candidates = await _context.Requests
                .AsSplitQuery()
                .Include(r => r.Department)
                .Include(r => r.RequestType)
                .Include(r => r.Posts)
                .SelectMany(
                    r => r.RequestSchedules
                        .Where(s =>
                            s.ReleaseAs == releaseAs &&
                            s.ProcessDate.HasValue &&
                            s.ProcessTime.HasValue &&
                            (
                                (s.ProcessingStatus == ProcessingStatus.Pending && s.ProcessedAt == null)
                                || (retryIncomplete
                                    && s.ProcessingStatus == ProcessingStatus.InProgress
                                    && (s.EditedDate == null || s.EditedDate <= reclaimBefore))
                                || (retryIncomplete
                                    && s.ProcessingStatus == ProcessingStatus.Failed
                                    && (s.ProcessedAt == null || s.ProcessedAt <= reclaimBefore))
                            ))
                        .OrderBy(s => s.ProcessDate)
                        .ThenBy(s => s.ProcessTime)
                        .Take(1),
                    (r, s) => new { Request = r, Schedule = s })
                .Where(x =>
                    x.Request.DepartmentID != null &&
                    x.Request.RequestType != null)
                .OrderBy(x => x.Schedule.ProcessDate)
                .ThenBy(x => x.Schedule.ProcessTime)
                .ToListAsync()
                .ConfigureAwait(false);

            if (candidates.Count == 0)
            {
                _logger.LogInformation("No request schedules due for processing. {ReleaseAs} {UtcChecked:O}", releaseAs, currentDepartmentTime);
                return null;
            }

            foreach (var candidate in candidates)
            {
                var requestTitle = candidate.Request.Title ?? string.Empty;
                var scheduledLocal = candidate.Schedule.ProcessDate!.Value.ToDateTime(candidate.Schedule.ProcessTime!.Value);

                if (currentDepartmentTime < scheduledLocal)
                {
                    _logger.LogDebug("Request not due yet. {RequestId} {RequestTitle} {TimeZoneId} {CurrentLocalTime:O} {ScheduledProcessDateTime:O}", candidate.Request.RequestID, requestTitle, candidate.Request.Department.TimeZoneId, currentDepartmentTime, scheduledLocal);
                    continue;
                }

                _logger.LogInformation("Request schedule matched. Starting processing. {RequestId} {RequestTitle} {TimeZoneId} {CurrentLocalTime:O} {ScheduledProcessDateTime:O} {ReleaseAs}",
                    candidate.Request.RequestID,
                    requestTitle,
                    candidate.Request.Department.TimeZoneId,
                    currentDepartmentTime,
                    scheduledLocal,
                    releaseAs);

                candidate.Schedule.ProcessingStatus = ProcessingStatus.InProgress;
                candidate.Schedule.EditedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync().ConfigureAwait(false);

                return new RequestScheduleProcessDueRequestDto
                {
                    RequestID = candidate.Request.RequestID,
                    RequestTitle = candidate.Request.Title ?? string.Empty,
                    DepartmentID = candidate.Request.DepartmentID!.Value,
                    TimeZoneId = candidate.Request.Department?.TimeZoneId,
                    ProcessLocalDateTime = scheduledLocal,
                    EvaluatedAtUtc = DateTime.UtcNow,
                    PostIDs = (candidate.Request.Posts ?? new List<Post>())
                        .Select(p => p.PostID)
                        .ToList()
                };
            }
            _logger.LogInformation("No request schedules due for processing. {ReleaseAs} {UtcChecked:O}", releaseAs, currentDepartmentTime);
            return null;
        }

        /// <inheritdoc />
        public async Task<RequestScheduleProcessDueRequestDto?> GetTenureWaitingListProcessingContextReadOnlyAsync(
            Guid requestId,
            Guid departmentId,
            CancellationToken cancellationToken = default)
        {
            var request = await _context.Requests
                    .AsSplitQuery()
                .Include(r => r.Department)
                .Include(r => r.Posts)
                .Include(r => r.RequestSchedules)
                .FirstOrDefaultAsync(
                    r => r.RequestID == requestId && r.DepartmentID == departmentId,
                    cancellationToken)
                .ConfigureAwait(false);

            if (request?.DepartmentID == null)
                return null;

            var tenureSchedule = (request.RequestSchedules ?? new List<RequestSchedule>())
                .Where(s =>
                    s.ReleaseAs == ReleaseAs.TenureBased &&
                    s.ProcessDate.HasValue &&
                    s.ProcessTime.HasValue)
                .OrderBy(s => s.ProcessDate)
                .ThenBy(s => s.ProcessTime)
                .FirstOrDefault();

            var processLocal = tenureSchedule != null
                ? tenureSchedule.ProcessDate!.Value.ToDateTime(tenureSchedule.ProcessTime!.Value)
                : DateTime.UtcNow;

            return new RequestScheduleProcessDueRequestDto
            {
                RequestID = request.RequestID,
                RequestTitle = request.Title ?? string.Empty,
                DepartmentID = request.DepartmentID.Value,
                TimeZoneId = request.Department?.TimeZoneId ?? "UTC",
                ProcessLocalDateTime = processLocal,
                EvaluatedAtUtc = DateTime.UtcNow,
                PostIDs = (request.Posts ?? new List<Post>())
                    .Select(p => p.PostID)
                    .ToList()
            };
        }

        public async Task UpdateTenureRequestStatusByRequestId(Guid requestId, ProcessingStatus status, CancellationToken ct)
        {
            var schedule = await _context.RequestSchedule
                .Where(s =>
                    s.RequestId == requestId &&
                    s.ProcessingStatus == ProcessingStatus.InProgress)
                .OrderByDescending(s => s.ProcessDate)
                .FirstOrDefaultAsync(ct);

            if (schedule == null)
                return;

            schedule.ProcessingStatus = status;
            schedule.ProcessedAt = DateTime.UtcNow;
            schedule.EditedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
        }
        /// <inheritdoc />
        public async Task<string> GetNextProjectNumberForYearAsync(int fiscalYear)
        {
            var projectNumbers = await _context.Requests
                .Where(r => r.FiscalYear == fiscalYear && r.ProjectNumber != null && r.ProjectNumber != "")
                .Select(r => r.ProjectNumber)
                .ToListAsync()
                .ConfigureAwait(false);

            var maxNumber = 0;
            foreach (var pn in projectNumbers)
            {
                if (int.TryParse(pn, out var num) && num > maxNumber)
                    maxNumber = num;
            }

            // Format as 5-digit zero-padded (00001, 00002, ...); resets each year
            return (maxNumber + 1).ToString("D5");
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Guid>> GetRequesterIdsByFiscalYearProjectNumberAndDateRangeAsync(int fiscalYear, string projectNumber, DateOnly startDate, DateOnly endDate)
        {
            return await _context.Requests
                .Where(r => r.FiscalYear == fiscalYear
                    && r.ProjectNumber == projectNumber
                    && r.StartDate <= endDate
                    && r.EndDate >= startDate)
                .Select(r => r.RequesterID)
                .Distinct()
                .ToListAsync();
        }

        public async Task<Request?> GetDataForInvoice(int fiscalYear, string projectNumber, DateOnly startDate, DateOnly endDate, Guid departmentId)
        {
            return await _context.Requests
                    .AsSplitQuery()
                .Include(r => r.Requester)
                .Include(r => r.Posts)
                    .ThenInclude(p => p.DepartmentActivityType)
                .Include(r => r.Posts)
                    .ThenInclude(p => p.PostPositions.Where(pp =>
                        pp.DateOfWork >= startDate &&
                        pp.DateOfWork <= endDate &&
                        (!pp.IsPlaceholder || pp.EnableInvoiceAndPayroll) &&
                        (pp.PostPositionStatus == PostPositionStatus.Closed ||
                         pp.PostPositionStatus == PostPositionStatus.ClosedAndSplit ||
                         pp.PostPositionStatus == PostPositionStatus.Unclosed ||
                         pp.PostPositionStatus == PostPositionStatus.Assigned)))
                    .ThenInclude(pp => pp.Placeholder)
                .FirstOrDefaultAsync(r =>
                    r.FiscalYear == fiscalYear &&
                    r.ProjectNumber == projectNumber &&
                    r.DepartmentID == departmentId)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Request?> GetRequestWithPostsAndPositionsForRosterAsync(Guid requestId, DateOnly dateOfWork)
        {
            try
            {
                return await _context.Requests
                    .AsSplitQuery()
                    .Include(r => r.Requester)
                    .Include(r => r.Posts)
                        .ThenInclude(p => p.PayCode)
                    // Employee path
                    .Include(r => r.Posts)
                        .ThenInclude(p => p.PostPositions.Where(pp =>
                            pp.DateOfWork == dateOfWork &&
                            (
                                pp.PostPositionStatus == PostPositionStatus.Assigned ||
                                pp.PostPositionStatus == PostPositionStatus.CancellationPending ||
                                pp.PostPositionStatus == PostPositionStatus.Closed ||
                                pp.PostPositionStatus == PostPositionStatus.SplitDecisionPending ||
                                pp.PostPositionStatus == PostPositionStatus.Unclosed
                            )))
                                .ThenInclude(pp => pp.Employee)
                                    .ThenInclude(e => e.EmployeeType)
                    .Include(r => r.Posts)
                        .ThenInclude(p => p.PostPositions.Where(pp =>
                            pp.DateOfWork == dateOfWork &&
                            (
                                pp.PostPositionStatus == PostPositionStatus.Assigned ||
                                pp.PostPositionStatus == PostPositionStatus.CancellationPending ||
                                pp.PostPositionStatus == PostPositionStatus.Closed ||
                                pp.PostPositionStatus == PostPositionStatus.SplitDecisionPending ||
                                pp.PostPositionStatus == PostPositionStatus.Unclosed
                            )))
                                .ThenInclude(pp => pp.PostCallSign)

                    // Placeholder path
                    .Include(r => r.Posts)
                        .ThenInclude(p => p.PostPositions.Where(pp =>
                            pp.DateOfWork == dateOfWork &&
                            (
                                pp.PostPositionStatus == PostPositionStatus.Assigned ||
                                pp.PostPositionStatus == PostPositionStatus.CancellationPending ||
                                pp.PostPositionStatus == PostPositionStatus.Closed ||
                                pp.PostPositionStatus == PostPositionStatus.SplitDecisionPending ||
                                pp.PostPositionStatus == PostPositionStatus.Unclosed
                            )))
                                .ThenInclude(pp => pp.Placeholder)

                    .FirstOrDefaultAsync(r => r.RequestID == requestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get request {RequestId} with posts and positions for roster on {DateOfWork}", requestId, dateOfWork);
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<(IReadOnlyList<Request> Items, long TotalRecords, int TotalPostCount)> GetRequestsByRequesterAsync(
            Guid requesterId,
            Guid departmentId,
            DateOnly? startDate,
            DateOnly? endDate,
            IReadOnlyCollection<RequestTypes>? requestTypes,
            IReadOnlyCollection<RequestStatus>? requestStatuses,
            IReadOnlyCollection<ReleaseAs>? releaseAs = null,
            IReadOnlyCollection<Guid>? requesterManagerIds = null,
            string? search = null,
            PageParam? pageParam = null,
            bool applyDateOnSchedules = false,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Requests
                .AsNoTracking()
                .Where(r => r.RequesterID == requesterId && r.DepartmentID == departmentId);

            query = ApplyRequesterRequestFilters(
                query,
                startDate,
                endDate,
                requestTypes,
                requestStatuses,
                requesterManagerIds,
                releaseAs,
                applyDateOnSchedules);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(r =>
                    (r.Title != null && r.Title.Contains(term)) ||
                    (r.ProjectNumber != null && r.ProjectNumber.Contains(term)));
            }

            var totalRecords = await query.LongCountAsync(cancellationToken).ConfigureAwait(false);
            var totalPostCount = await query.SumAsync(r => r.Posts.Count, cancellationToken).ConfigureAwait(false);

            query = query
                .OrderBy(r => r.StartDate)
                .ThenBy(r => r.Title);

            pageParam ??= new PageParam();
            query = query
                .Skip(pageParam.Offset ?? 0)
                .Take(pageParam.Limit ?? PageParam.MinLimit);

            var items = await IncludeMatchingSchedules(query, startDate, endDate, releaseAs, applyDateOnSchedules, requestStatuses)
                .Include(r => r.RequestType)
                .Include(r => r.Posts)
                .AsSplitQuery()
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return (items, totalRecords, totalPostCount);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<Request>> GetRequestsWithLocationsByRequesterAsync(
            Guid requesterId,
            Guid departmentId,
            DateTime shiftWindowStart,
            DateTime shiftWindowEnd,
            IReadOnlyCollection<Guid>? requesterManagerIds = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Requests
                .AsNoTracking()
                .Where(r => r.RequesterID == requesterId && r.DepartmentID == departmentId);

            if (requesterManagerIds is { Count: > 0 })
            {
                query = query.Where(r =>
                    r.Requester != null &&
                    r.Requester.RequesterManagers.Any(rm => requesterManagerIds.Contains(rm.RequesterManagerID)) &&
                    (
                        !r.RequesterManagerID.HasValue ||
                        requesterManagerIds.Contains(r.RequesterManagerID.Value) ||
                        (r.RequesterManager != null && r.RequesterManager.IsRequestShared)
                    ));
            }

            query = query.Where(r => r.RequestSchedules.Any(rs =>
                rs.RequestStatus == RequestStatus.Pending ||
                rs.RequestStatus == RequestStatus.ReleaseNow ||
                rs.RequestStatus == RequestStatus.ReleasedOn));

            var startDt = shiftWindowStart;
            var endDt = shiftWindowEnd;
            bool isSameDate = startDt.Date == endDt.Date;

            if (isSameDate)
            {
                var targetDate = DateOnly.FromDateTime(startDt);
                var prevDate = targetDate.AddDays(-1);
                var minTime = TimeOnly.FromDateTime(startDt);
                var maxTime = TimeOnly.FromDateTime(endDt);

                query = query.Where(r => r.Posts.Any(p =>
                    p.PostPositions.Any(pp =>
                        pp.EmployeeID.HasValue &&
                        pp.EmployeeID.Value != Guid.Empty &&
                        pp.PostPositionStatus != PostPositionStatus.Cancelled &&
                        pp.PostPositionStatus != PostPositionStatus.Closed &&
                        pp.PostPositionStatus != PostPositionStatus.ClosedAndSplit &&
                        pp.PostPositionStatus != PostPositionStatus.Declined &&
                        (
                            (pp.DateOfWork == targetDate &&
                             (pp.StartTime ?? p.PostStartTime) <= maxTime &&
                             ((pp.EndTime ?? p.PostEndTime) >= minTime || (pp.EndTime ?? p.PostEndTime) <= (pp.StartTime ?? p.PostStartTime))) ||
                            (pp.DateOfWork == prevDate &&
                             (pp.EndTime ?? p.PostEndTime) <= (pp.StartTime ?? p.PostStartTime) &&
                             (pp.EndTime ?? p.PostEndTime) >= minTime)
                        ))
                ));
            }
            else
            {
                var startDatePart = DateOnly.FromDateTime(startDt);
                var prevDatePart = startDatePart.AddDays(-1);
                var minTime = TimeOnly.FromDateTime(startDt);
                var endDatePart = DateOnly.FromDateTime(endDt);
                var maxTime = TimeOnly.FromDateTime(endDt);

                query = query.Where(r => r.Posts.Any(p =>
                    p.PostPositions.Any(pp =>
                        pp.EmployeeID.HasValue &&
                        pp.EmployeeID.Value != Guid.Empty &&
                        pp.PostPositionStatus != PostPositionStatus.Cancelled &&
                        pp.PostPositionStatus != PostPositionStatus.Closed &&
                        pp.PostPositionStatus != PostPositionStatus.ClosedAndSplit &&
                        pp.PostPositionStatus != PostPositionStatus.Declined &&
                        (
                            (pp.DateOfWork == startDatePart &&
                             ((pp.EndTime ?? p.PostEndTime) >= minTime || (pp.EndTime ?? p.PostEndTime) <= (pp.StartTime ?? p.PostStartTime))) ||
                            (pp.DateOfWork == endDatePart && (pp.StartTime ?? p.PostStartTime) <= maxTime) ||
                            (pp.DateOfWork == prevDatePart &&
                             (pp.EndTime ?? p.PostEndTime) <= (pp.StartTime ?? p.PostStartTime) &&
                             (pp.EndTime ?? p.PostEndTime) >= minTime)
                        ))
                ));
            }

            var windowStartDate = DateOnly.FromDateTime(shiftWindowStart).AddDays(-1);
            var windowEndDate = DateOnly.FromDateTime(shiftWindowEnd);

            return await query
                .OrderBy(r => r.StartDate)
                .ThenBy(r => r.Title)
                .Include(r => r.RequestSchedules.Where(rs =>
                    (!rs.StartDate.HasValue || rs.StartDate <= windowEndDate) &&
                    (!rs.EndDate.HasValue || rs.EndDate >= windowStartDate)))
                .Include(r => r.RequestType)
                .Include(r => r.Posts)
                    .ThenInclude(p => p.PostPositions)
                        .ThenInclude(pp => pp.Employee)
                            .ThenInclude(e => e!.EmployeeType)
                .AsSplitQuery()
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<EmployeePostLocation>> GetLatestLocationsForPositionsAsync(
            IEnumerable<Guid> postPositionIds,
            IEnumerable<Guid> employeeIds,
            CancellationToken cancellationToken = default)
        {
            var posIds = postPositionIds.Distinct().ToList();
            var empIds = employeeIds.Distinct().ToList();

            if (posIds.Count == 0 && empIds.Count == 0)
                return Array.Empty<EmployeePostLocation>();

            return await _context.EmployeePostLocation
                .AsNoTracking()
                .Where(l => posIds.Contains(l.PostPositionID) || empIds.Contains(l.EmployeeID))
                .OrderByDescending(l => l.CapturedAt)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<RequesterRequestSummaryDto>> GetRequesterRequestSummariesAsync(
            Guid departmentId,
            string? requesterName,
            DateOnly? startDate,
            DateOnly? endDate,
            IReadOnlyCollection<RequestTypes>? requestTypes,
            IReadOnlyCollection<RequestStatus>? requestStatuses,
            IReadOnlyCollection<ReleaseAs>? releaseAs = null,
            IReadOnlyCollection<Guid>? requesterManagerIds = null,
            bool applyDateOnSchedules = false,
            string? searchText = null,
            CancellationToken cancellationToken = default)
        {
            var unpaged = new PageParam { Page = 1, PageSize = int.MaxValue / 4 };
            var paged = await GetRequesterRequestSummariesPagedAsync(
                departmentId,
                unpaged,
                requesterName,
                startDate,
                endDate,
                requestTypes,
                requestStatuses,
                releaseAs,
                requesterManagerIds,
                applyDateOnSchedules,
                searchText,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            return paged.Data ?? new List<RequesterRequestSummaryDto>();
        }

        /// <inheritdoc />
        public async Task<PagedList<RequesterRequestSummaryDto>> GetRequesterRequestSummariesPagedAsync(
            Guid departmentId,
            PageParam pageParam,
            string? requesterName,
            DateOnly? startDate,
            DateOnly? endDate,
            IReadOnlyCollection<RequestTypes>? requestTypes,
            IReadOnlyCollection<RequestStatus>? requestStatuses,
            IReadOnlyCollection<ReleaseAs>? releaseAs = null,
            IReadOnlyCollection<Guid>? requesterManagerIds = null,
            bool applyDateOnSchedules = false,
            string? searchText = null,
            DateTime? shiftWindowStart = null,
            DateTime? shiftWindowEnd = null,
            CancellationToken cancellationToken = default)
        {
            pageParam ??= new PageParam();
            var query = BuildRequesterSummaryRequestQuery(
                departmentId,
                requesterName,
                startDate,
                endDate,
                requestTypes,
                requestStatuses,
                requesterManagerIds,
                releaseAs,
                applyDateOnSchedules,
                searchText);

            if (shiftWindowStart.HasValue && shiftWindowEnd.HasValue)
                query = ApplyShiftWindowFilter(query, shiftWindowStart.Value, shiftWindowEnd.Value);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                return await GetRequesterSummariesWithRequestsPagedAsync(
                    query,
                    pageParam,
                    startDate,
                    endDate,
                    releaseAs,
                    applyDateOnSchedules,
                    requestStatuses,
                    cancellationToken).ConfigureAwait(false);
            }

            return await GetRequesterSummaryRollupsPagedAsync(query, pageParam, cancellationToken).ConfigureAwait(false);
        }

        public async Task<List<Guid>> GetDistinctRequesterIdsAsync(
            Expression<Func<Request, bool>> filter,
            CancellationToken cancellationToken = default)
        {
            return await _context.Requests
                .AsNoTracking()
                .Where(filter)
                .Select(r => r.RequesterID)
                .Distinct()
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<(long Total, IReadOnlyList<(Guid RequesterId, string RequesterName)> Page)> GetPagedRequestersForWorkspaceListAsync(
            RequesterWorkspaceListQuery query,
            PageParam pageParam,
            CancellationToken cancellationToken = default)
        {
            pageParam ??= new PageParam();
            var distinctRequesters = FilterWorkspaceRequests(query)
                .Select(r => new { r.RequesterID, Name = r.Requester!.Name ?? string.Empty })
                .Distinct();

            var total = await distinctRequesters.LongCountAsync(cancellationToken).ConfigureAwait(false);
            if (total == 0)
                return (0, Array.Empty<(Guid, string)>());

            var sortDesc = string.Equals(pageParam.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
            var ordered = sortDesc
                ? distinctRequesters.OrderByDescending(x => x.Name).ThenByDescending(x => x.RequesterID)
                : distinctRequesters.OrderBy(x => x.Name).ThenBy(x => x.RequesterID);

            var skip = pageParam.Offset ?? PageParam.MinOffset;
            var take = pageParam.Limit ?? PageParam.MinLimit;
            var page = await ordered
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return (total, page.Select(r => (r.RequesterID, r.Name)).ToList());
        }

        /// <inheritdoc />
        public async Task<(IReadOnlyList<Request> Items, IReadOnlyDictionary<Guid, int> PostCountsByRequestId)> GetRequestsForWorkspaceListAsync(
            RequesterWorkspaceListQuery query,
            IReadOnlyCollection<Guid> requesterIds,
            CancellationToken cancellationToken = default)
        {
            if (requesterIds.Count == 0)
                return (Array.Empty<Request>(), new Dictionary<Guid, int>());

            var idList = requesterIds as IList<Guid> ?? requesterIds.ToList();
            var baseQuery = FilterWorkspaceRequests(query).Where(r => idList.Contains(r.RequesterID));

            IQueryable<Request> withIncludes = baseQuery.Include(r => r.RequestType);
            if (query.RequestStatuses is { Length: > 0 })
            {
                var statuses = query.RequestStatuses;
                withIncludes = withIncludes.Include(r => r.RequestSchedules.Where(rs =>
                    statuses.Contains(rs.RequestStatus) &&
                    (query.IncludePast || rs.RequestStatus != RequestStatus.Completed)));
            }
            else
            {
                withIncludes = withIncludes.Include(r => r.RequestSchedules);
            }

            var items = await withIncludes
                .AsSplitQuery()
                .OrderBy(r => r.RequesterID)
                .ThenBy(r => r.StartDate)
                .ThenBy(r => r.Title)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            if (items.Count == 0)
                return (items, new Dictionary<Guid, int>());

            var requestIds = items.Select(r => r.RequestID).ToList();
            var postCounts = await _context.Post
                .AsNoTracking()
                .Where(p => p.RequestID.HasValue && requestIds.Contains(p.RequestID.Value))
                .GroupBy(p => p.RequestID!.Value)
                .Select(g => new { RequestId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.RequestId, x => x.Count, cancellationToken)
                .ConfigureAwait(false);

            return (items, postCounts);
        }

        private IQueryable<Request> FilterWorkspaceRequests(RequesterWorkspaceListQuery q)
        {
            var query = _context.Requests.AsNoTracking().Where(r => r.DepartmentID == q.DepartmentId);

            if (!string.IsNullOrWhiteSpace(q.RequesterName))
            {
                var term = q.RequesterName.Trim();
                query = query.Where(r => r.Requester != null && r.Requester.Name != null && r.Requester.Name.Contains(term));
            }

            if (q.RequestStatuses is { Length: > 0 })
            {
                var statuses = q.RequestStatuses;
                query = query.Where(r =>
                    r.RequestSchedules.Any(rs =>
                        statuses.Contains(rs.RequestStatus) &&
                        (q.IncludePast || rs.RequestStatus != RequestStatus.Completed)) ||
                    (!r.RequestSchedules.Any() && statuses.Contains(RequestStatus.Pending)));
            }

            if (q.RequestTypes is { Length: > 0 })
            {
                var types = q.RequestTypes;
                query = query.Where(r => r.RequestType != null && types.Contains(r.RequestType.Type));
            }

            if (q.StartDate.HasValue || q.EndDate.HasValue)
            {
                var start = q.StartDate;
                var end = q.EndDate;
                query = query.Where(r =>
                    ((!start.HasValue || r.EndDate >= start.Value) && (!end.HasValue || r.StartDate <= end.Value)) ||
                    _context.PostPosition.Any(pp =>
                        pp.DateOfWork != null &&
                        pp.Post != null &&
                        pp.Post.RequestID == r.RequestID &&
                        (!start.HasValue || pp.DateOfWork.Value >= start.Value) &&
                        (!end.HasValue || pp.DateOfWork.Value <= end.Value)));
            }

            return query;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<RequesterRequestSummaryDto>> GetRequesterRequestSummariesByShiftWindowAsync(
            Guid departmentId,
            DateTime shiftWindowStart,
            DateTime shiftWindowEnd,
            string? requesterName = null,
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            IReadOnlyCollection<RequestTypes>? requestTypes = null,
            IReadOnlyCollection<RequestStatus>? requestStatuses = null,
            IReadOnlyCollection<ReleaseAs>? releaseAs = null,
            IReadOnlyCollection<Guid>? requesterManagerIds = null,
            bool applyDateOnSchedules = false,
            string? searchText = null,
            CancellationToken cancellationToken = default)
        {
            var unpaged = new PageParam { Page = 1, PageSize = int.MaxValue / 4 };
            var paged = await GetRequesterRequestSummariesByShiftWindowPagedAsync(
                departmentId,
                shiftWindowStart,
                shiftWindowEnd,
                unpaged,
                requesterName,
                startDate,
                endDate,
                requestTypes,
                requestStatuses,
                releaseAs,
                requesterManagerIds,
                applyDateOnSchedules,
                searchText,
                cancellationToken).ConfigureAwait(false);
            return paged.Data ?? new List<RequesterRequestSummaryDto>();
        }

        /// <inheritdoc />
        public Task<PagedList<RequesterRequestSummaryDto>> GetRequesterRequestSummariesByShiftWindowPagedAsync(
            Guid departmentId,
            DateTime shiftWindowStart,
            DateTime shiftWindowEnd,
            PageParam pageParam,
            string? requesterName = null,
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            IReadOnlyCollection<RequestTypes>? requestTypes = null,
            IReadOnlyCollection<RequestStatus>? requestStatuses = null,
            IReadOnlyCollection<ReleaseAs>? releaseAs = null,
            IReadOnlyCollection<Guid>? requesterManagerIds = null,
            bool applyDateOnSchedules = false,
            string? searchText = null,
            CancellationToken cancellationToken = default)
        {
            return GetRequesterRequestSummariesPagedAsync(
                departmentId,
                pageParam,
                requesterName,
                startDate,
                endDate,
                requestTypes,
                requestStatuses,
                releaseAs,
                requesterManagerIds,
                applyDateOnSchedules,
                searchText,
                shiftWindowStart,
                shiftWindowEnd,
                cancellationToken);
        }

        private IQueryable<Request> BuildRequesterSummaryRequestQuery(
            Guid departmentId,
            string? requesterName,
            DateOnly? startDate,
            DateOnly? endDate,
            IReadOnlyCollection<RequestTypes>? requestTypes,
            IReadOnlyCollection<RequestStatus>? requestStatuses,
            IReadOnlyCollection<Guid>? requesterManagerIds,
            IReadOnlyCollection<ReleaseAs>? releaseAs,
            bool applyDateOnSchedules,
            string? searchText)
        {
            requesterName = string.IsNullOrWhiteSpace(requesterName) ? null : requesterName.Trim();

            var query = _context.Requests
                .AsNoTracking()
                .Where(r => r.DepartmentID == departmentId);

            if (requesterName != null)
            {
                query = query.Where(r =>
                    r.Requester != null &&
                    r.Requester.Name != null &&
                    r.Requester.Name.Contains(requesterName));
            }

            query = ApplyRequesterRequestFilters(
                query,
                startDate,
                endDate,
                requestTypes,
                requestStatuses,
                requesterManagerIds,
                releaseAs,
                applyDateOnSchedules);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var term = searchText.Trim();
                query = query.Where(r =>
                    (r.Requester != null && r.Requester.Name != null && r.Requester.Name.Contains(term)) ||
                    (r.Title != null && r.Title.Contains(term)));
            }

            return query;
        }

        private static IQueryable<Request> ApplyShiftWindowFilter(
            IQueryable<Request> query,
            DateTime shiftWindowStart,
            DateTime shiftWindowEnd)
        {
            var startDt = shiftWindowStart;
            var endDt = shiftWindowEnd;
            var isSameDate = startDt.Date == endDt.Date;

            if (isSameDate)
            {
                var targetDate = DateOnly.FromDateTime(startDt);
                var prevDate = targetDate.AddDays(-1);
                var minTime = TimeOnly.FromDateTime(startDt);
                var maxTime = TimeOnly.FromDateTime(endDt);

                return query.Where(r => r.Posts.Any(p =>
                    p.PostPositions.Any(pp =>
                        pp.EmployeeID.HasValue &&
                        pp.EmployeeID.Value != Guid.Empty &&
                        pp.PostPositionStatus != PostPositionStatus.Cancelled &&
                        pp.PostPositionStatus != PostPositionStatus.Closed &&
                        pp.PostPositionStatus != PostPositionStatus.ClosedAndSplit &&
                        pp.PostPositionStatus != PostPositionStatus.Declined &&
                        (
                            (pp.DateOfWork == targetDate &&
                             (pp.StartTime ?? p.PostStartTime) <= maxTime &&
                             ((pp.EndTime ?? p.PostEndTime) >= minTime || (pp.EndTime ?? p.PostEndTime) <= (pp.StartTime ?? p.PostStartTime))) ||
                            (pp.DateOfWork == prevDate &&
                             (pp.EndTime ?? p.PostEndTime) <= (pp.StartTime ?? p.PostStartTime) &&
                             (pp.EndTime ?? p.PostEndTime) >= minTime)
                        ))));
            }

            var startDatePart = DateOnly.FromDateTime(startDt);
            var prevDatePart = startDatePart.AddDays(-1);
            var minTimeSpan = TimeOnly.FromDateTime(startDt);
            var endDatePart = DateOnly.FromDateTime(endDt);
            var maxTimeSpan = TimeOnly.FromDateTime(endDt);

            return query.Where(r => r.Posts.Any(p =>
                p.PostPositions.Any(pp =>
                    pp.EmployeeID.HasValue &&
                    pp.EmployeeID.Value != Guid.Empty &&
                    pp.PostPositionStatus != PostPositionStatus.Cancelled &&
                    pp.PostPositionStatus != PostPositionStatus.Closed &&
                    pp.PostPositionStatus != PostPositionStatus.ClosedAndSplit &&
                    pp.PostPositionStatus != PostPositionStatus.Declined &&
                    (
                        (pp.DateOfWork == startDatePart &&
                         ((pp.EndTime ?? p.PostEndTime) >= minTimeSpan || (pp.EndTime ?? p.PostEndTime) <= (pp.StartTime ?? p.PostStartTime))) ||
                        (pp.DateOfWork == endDatePart && (pp.StartTime ?? p.PostStartTime) <= maxTimeSpan) ||
                        (pp.DateOfWork == prevDatePart &&
                         (pp.EndTime ?? p.PostEndTime) <= (pp.StartTime ?? p.PostStartTime) &&
                         (pp.EndTime ?? p.PostEndTime) >= minTimeSpan)
                    ))));
        }

        private async Task<PagedList<RequesterRequestSummaryDto>> GetRequesterSummaryRollupsPagedAsync(
            IQueryable<Request> query,
            PageParam pageParam,
            CancellationToken cancellationToken)
        {
            var grouped = query
                .GroupBy(r => new
                {
                    r.RequesterID,
                    RequesterName = r.Requester != null ? r.Requester.Name ?? string.Empty : string.Empty
                })
                .Select(g => new RequesterRequestSummaryDto
                {
                    RequesterId = g.Key.RequesterID,
                    RequesterName = g.Key.RequesterName,
                    RequestCount = g.Count(),
                    ProjectNumbers = g
                        .Where(r => r.ProjectNumber != null && r.ProjectNumber != "")
                        .Select(r => r.ProjectNumber!)
                        .Distinct()
                        .OrderBy(pn => pn)
                        .ToList()
                });

            var total = await grouped.LongCountAsync(cancellationToken).ConfigureAwait(false);
            var sortNameDesc = string.Equals(pageParam.SortDirection, "desc", StringComparison.OrdinalIgnoreCase) &&
                               (string.IsNullOrWhiteSpace(pageParam.SortBy) ||
                                string.Equals(pageParam.SortBy, "requesterName", StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(pageParam.SortBy, "Name", StringComparison.OrdinalIgnoreCase));
            var ordered = sortNameDesc
                ? grouped.OrderByDescending(x => x.RequesterName).ThenByDescending(x => x.RequesterId)
                : grouped.OrderBy(x => x.RequesterName).ThenBy(x => x.RequesterId);
            var skip = pageParam.Offset ?? PageParam.MinOffset;
            var take = pageParam.Limit ?? PageParam.MinLimit;
            var items = await ordered.Skip(skip).Take(take).ToListAsync(cancellationToken).ConfigureAwait(false);
            return new PagedList<RequesterRequestSummaryDto>(items, pageParam, total);
        }

        private async Task<PagedList<RequesterRequestSummaryDto>> GetRequesterSummariesWithRequestsPagedAsync(
            IQueryable<Request> query,
            PageParam pageParam,
            DateOnly? startDate,
            DateOnly? endDate,
            IReadOnlyCollection<ReleaseAs>? releaseAs,
            bool applyDateOnSchedules,
            IReadOnlyCollection<RequestStatus>? requestStatuses,
            CancellationToken cancellationToken)
        {
            var groupQuery = query.GroupBy(r => new
                {
                    r.RequesterID,
                    RequesterName = r.Requester != null ? r.Requester.Name ?? string.Empty : string.Empty
                })
                .Select(g => new
                {
                    g.Key.RequesterID,
                    g.Key.RequesterName,
                    RequestCount = g.Count(),
                    ProjectNumbers = g
                        .Where(r => r.ProjectNumber != null && r.ProjectNumber != "")
                        .Select(r => r.ProjectNumber!)
                        .Distinct()
                        .OrderBy(pn => pn)
                        .ToList()
                });

            var total = await groupQuery.LongCountAsync(cancellationToken).ConfigureAwait(false);
            var sortNameDesc = string.Equals(pageParam.SortDirection, "desc", StringComparison.OrdinalIgnoreCase) &&
                               (string.IsNullOrWhiteSpace(pageParam.SortBy) ||
                                string.Equals(pageParam.SortBy, "requesterName", StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(pageParam.SortBy, "Name", StringComparison.OrdinalIgnoreCase));
            var orderedGroups = sortNameDesc
                ? groupQuery.OrderByDescending(g => g.RequesterName).ThenByDescending(g => g.RequesterID)
                : groupQuery.OrderBy(g => g.RequesterName).ThenBy(g => g.RequesterID);
            var skip = pageParam.Offset ?? PageParam.MinOffset;
            var take = pageParam.Limit ?? PageParam.MinLimit;
            var pageGroups = await orderedGroups.Skip(skip).Take(take).ToListAsync(cancellationToken).ConfigureAwait(false);

            if (pageGroups.Count == 0)
                return new PagedList<RequesterRequestSummaryDto>(new List<RequesterRequestSummaryDto>(), pageParam, total);

            var requesterIds = pageGroups.Select(g => g.RequesterID).ToList();
            var requests = await IncludeMatchingSchedules(
                    query.Where(r => requesterIds.Contains(r.RequesterID)),
                    startDate,
                    endDate,
                    releaseAs,
                    applyDateOnSchedules,
                    requestStatuses)
                .Include(r => r.RequestType)
                .Include(r => r.Posts)
                .AsSplitQuery()
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var requestsByRequester = requests
                .GroupBy(r => r.RequesterID)
                .ToDictionary(g => g.Key, g => g.OrderBy(r => r.StartDate).ThenBy(r => r.Title).ToList());

            var summaries = new List<RequesterRequestSummaryDto>();
            foreach (var group in pageGroups)
            {
                var summary = new RequesterRequestSummaryDto
                {
                    RequesterId = group.RequesterID,
                    RequesterName = group.RequesterName,
                    RequestCount = group.RequestCount,
                    ProjectNumbers = group.ProjectNumbers
                };

                if (requestsByRequester.TryGetValue(group.RequesterID, out var rows))
                    summary.Requests = _mapper.Map<List<RequestInRequesterListDto>>(rows);

                summaries.Add(summary);
            }

            return new PagedList<RequesterRequestSummaryDto>(summaries, pageParam, total);
        }

        private static IQueryable<Request> IncludeMatchingSchedules(
            IQueryable<Request> query,
            DateOnly? startDate,
            DateOnly? endDate,
            IReadOnlyCollection<ReleaseAs>? releaseAs,
            bool applyDateOnSchedules,
            IReadOnlyCollection<RequestStatus>? requestStatuses = null)
        {
            var filterBySchedule = releaseAs is { Count: > 0 } ||
                                   (applyDateOnSchedules && (startDate.HasValue || endDate.HasValue));
            if (!filterBySchedule)
                return query.Include(r => r.RequestSchedules);

            var selectedReleaseAs = releaseAs is { Count: > 0 } ? releaseAs.ToArray() : Array.Empty<ReleaseAs>();
            var selectedStatuses = applyDateOnSchedules && requestStatuses is { Count: > 0 }
                ? requestStatuses.ToArray()
                : Array.Empty<RequestStatus>();
            var filterReleaseAs = selectedReleaseAs.Length > 0;
            var filterStatus = selectedStatuses.Length > 0;
            return query.Include(r => r.RequestSchedules.Where(rs =>
                (!filterStatus || selectedStatuses.Contains(rs.RequestStatus)) &&
                (!filterReleaseAs || selectedReleaseAs.Contains(rs.ReleaseAs)) &&
                (!startDate.HasValue || (rs.EndDate.HasValue && rs.EndDate >= startDate)) &&
                (!endDate.HasValue || (rs.StartDate.HasValue && rs.StartDate <= endDate))));
        }

        /// <summary>
        /// Shared manager / type / status / date filters used by requester request list and summary queries.
        /// When <paramref name="requesterManagerIds"/> is provided, only requests under requesters linked to those
        /// manager rows are included (own manager, no manager, or IsRequestShared).
        /// When <paramref name="applyDateOnSchedules"/> is true, the date range overlaps schedule dates.
        /// Otherwise dates stay on the request start and end unless <paramref name="releaseAs"/> is set.
        /// </summary>
        private static IQueryable<Request> ApplyRequesterRequestFilters(
            IQueryable<Request> query,
            DateOnly? startDate,
            DateOnly? endDate,
            IReadOnlyCollection<RequestTypes>? requestTypes,
            IReadOnlyCollection<RequestStatus>? requestStatuses,
            IReadOnlyCollection<Guid>? requesterManagerIds,
            IReadOnlyCollection<ReleaseAs>? releaseAs = null,
            bool applyDateOnSchedules = false)
        {
            if (requesterManagerIds is { Count: > 0 })
            {
                query = query.Where(r =>
                    r.Requester != null &&
                    r.Requester.RequesterManagers.Any(rm => requesterManagerIds.Contains(rm.RequesterManagerID)) &&
                    (
                        !r.RequesterManagerID.HasValue ||
                        requesterManagerIds.Contains(r.RequesterManagerID.Value) ||
                        (r.RequesterManager != null && r.RequesterManager.IsRequestShared)
                    ));
            }

            if (requestTypes is { Count: > 0 })
                query = query.Where(r => r.RequestType != null && requestTypes.Contains(r.RequestType.Type));

            var selectedStatuses = requestStatuses is { Count: > 0 } ? requestStatuses.ToArray() : null;
            var filterBySchedule = releaseAs is { Count: > 0 } ||
                                   (applyDateOnSchedules && (startDate.HasValue || endDate.HasValue));
            if (selectedStatuses != null && !filterBySchedule)
                query = query.Where(r => r.RequestSchedules.Any(rs => selectedStatuses.Contains(rs.RequestStatus)));

            if (filterBySchedule)
            {
                var selectedReleaseAs = releaseAs is { Count: > 0 } ? releaseAs.ToArray() : Array.Empty<ReleaseAs>();
                var statusValues = selectedStatuses ?? Array.Empty<RequestStatus>();
                var filterReleaseAs = selectedReleaseAs.Length > 0;
                var filterStatus = statusValues.Length > 0;
                query = query.Where(r => r.RequestSchedules.Any(rs =>
                    (!filterStatus || statusValues.Contains(rs.RequestStatus)) &&
                    (!filterReleaseAs || selectedReleaseAs.Contains(rs.ReleaseAs)) &&
                    (!startDate.HasValue || (rs.EndDate.HasValue && rs.EndDate >= startDate)) &&
                    (!endDate.HasValue || (rs.StartDate.HasValue && rs.StartDate <= endDate))));
            }
            else if (startDate.HasValue || endDate.HasValue)
            {
                query = query.Where(r =>
                    (!startDate.HasValue || r.EndDate >= startDate.Value) &&
                    (!endDate.HasValue || r.StartDate <= endDate.Value));
            }

            return query;
        }
    }
}

