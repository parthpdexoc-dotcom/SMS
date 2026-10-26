using SMS.Generic.Common.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Interface for request-specific repository operations, extending IGenericRepository.
    /// </summary>
    public interface IRequestRepository : IGenericRepository<Request>
    {
        Task<Request> GetRequestByPostId(Guid postId, Guid departmentId);
        Task<Request?> GetRequestForCloneAsync(Guid requestId, Guid departmentId);
        Task<IEnumerable<Request>> SearchRequests(DateOnly startDate, DateOnly endDate, Guid departmentId,ReleaseAs release, DateTime currentDateTime);

        /// <summary>
        /// Distinct request id/title for dashboard filter dropdown: released schedules overlapping
        /// <paramref name="startDate"/>–<paramref name="endDate"/> in the given department.
        /// </summary>
        Task<IReadOnlyList<RequestFilterOptionDto>> GetRequestTitlesByDateRange(DateOnly startDate,DateOnly endDate,Guid departmentId);

        /// <summary>
        /// Distinct requester id/title for dashboard filter dropdown: requesters with released schedules
        /// overlapping <paramref name="startDate"/>–<paramref name="endDate"/> in the given department.
        /// </summary>
        Task<IReadOnlyList<RequestFilterOptionDto>> GetRequestersByDateRange(DateOnly startDate, DateOnly endDate, Guid departmentId);

        Task<ReleaseAs> GetLatestReleaseAsForRequestAsync(Guid requestId);
        /// <summary>
        /// Read-only: returns tenure-based requests whose department-local ProcessDate + ProcessTime
        /// matches the current department-local minute for <paramref name="asOfUtc"/>.
        /// Does not update any data.
        /// </summary>
        Task<RequestScheduleProcessDueRequestDto> GetTenureBasedRequestsDueForProcessingAsync(DateTime asOfUtc);
        /// <summary>
        /// Returns the next due preferred-release request (by department-local ProcessDate + ProcessTime),
        /// and reserves its matching schedule by setting ProcessingStatus = InProgress.
        /// </summary>
        Task<RequestScheduleProcessDueRequestDto> GetPreferredRequestsDueForProcessingAsync(DateTime asOfUtc);

        /// <summary>
        /// Gets the next project number for the given fiscal year (max + 1, or 1 if no records exist for that year).
        /// </summary>
        /// <param name="fiscalYear">The fiscal year.</param>
        /// <returns>The next project number to use as string (1-based, resets each year).</returns>
        Task<string> GetNextProjectNumberForYearAsync(int fiscalYear);


        /// <summary>
        /// Gets requests from Request table by fiscal year, project number, and date range.
        /// </summary>
        /// <param name="fiscalYear">The fiscal year.</param>
        /// <param name="projectNumber">The project number (e.g. 00001).</param>
        /// <param name="startDate">Filter start date.</param>
        /// <param name="endDate">Filter end date.</param>
        /// <returns>Matching requests.</returns>
        Task<Request?> GetDataForInvoice(int fiscalYear, string projectNumber, DateOnly startDate, DateOnly endDate, Guid departmentId);

        /// <summary>
        /// Gets request with requester, all posts, and postpositions filtered by date for roster information.
        /// </summary>
        /// <param name="requestId">The request ID.</param>
        /// <param name="dateOfWork">The date of work to filter postpositions.</param>
        /// <returns>The request entity with all related data loaded.</returns>
        Task<Request?> GetRequestWithPostsAndPositionsForRosterAsync(Guid requestId, DateOnly dateOfWork);

        Task UpdateTenureRequestStatusByRequestId(Guid requestId, ProcessingStatus status, CancellationToken ct);

        /// <summary>
        /// Read-only: loads request, posts, and department for tenure waiting-list processing without mutating schedules.
        /// </summary>
        Task<RequestScheduleProcessDueRequestDto?> GetTenureWaitingListProcessingContextReadOnlyAsync(
            Guid requestId,
            Guid departmentId,
            CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<Request> Items, long TotalRecords, int TotalPostCount)> GetRequestsByRequesterAsync(
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
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets requests for a requester filtered to active jobs/shifts (from 30 minutes before start until 30 minutes after end),
        /// including all posts, post positions, and assigned employees.
        /// </summary>
        Task<IReadOnlyList<Request>> GetRequestsWithLocationsByRequesterAsync(
            Guid requesterId,
            Guid departmentId,
            DateTime shiftWindowStart,
            DateTime shiftWindowEnd,
            IReadOnlyCollection<Guid>? requesterManagerIds = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets latest GPS location readings for the specified post positions or employees.
        /// </summary>
        Task<IReadOnlyList<EmployeePostLocation>> GetLatestLocationsForPositionsAsync(
            IEnumerable<Guid> postPositionIds,
            IEnumerable<Guid> employeeIds,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aggregated requester request summaries (id, name, count, project numbers) for the department,
        /// with the same manager / IsRequestShared visibility rules as <see cref="GetRequestsByRequesterAsync"/>.
        /// </summary>
        Task<IReadOnlyList<RequesterRequestSummaryDto>> GetRequesterRequestSummariesAsync(
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
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Paged requester summaries (rollup only or with request rows when <paramref name="searchText"/> is set).
        /// </summary>
        Task<PagedList<RequesterRequestSummaryDto>> GetRequesterRequestSummariesPagedAsync(
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
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Dedicated query for requester summaries whose jobs/shifts are active in the specified shift window.
        /// </summary>
        Task<IReadOnlyList<RequesterRequestSummaryDto>> GetRequesterRequestSummariesByShiftWindowAsync(
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
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Paged requester summaries for the shift-window query.
        /// </summary>
        Task<PagedList<RequesterRequestSummaryDto>> GetRequesterRequestSummariesByShiftWindowPagedAsync(
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
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Distinct requester IDs for requests matching <paramref name="filter"/> (no includes).
        /// </summary>
        Task<List<Guid>> GetDistinctRequesterIdsAsync(
            System.Linq.Expressions.Expression<Func<Request, bool>> filter,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Paged requesters that have at least one request matching <paramref name="filter"/> (subquery; no ID list materialization).
        /// </summary>
        Task<(long Total, IReadOnlyList<(Guid RequesterId, string RequesterName)> Page)> GetPagedRequestersForWorkspaceListAsync(
            RequesterWorkspaceListQuery query,
            PageParam pageParam,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Requests for workspace list: split query, no post rows; post counts loaded in a second query.
        /// </summary>
        Task<(IReadOnlyList<Request> Items, IReadOnlyDictionary<Guid, int> PostCountsByRequestId)> GetRequestsForWorkspaceListAsync(
            RequesterWorkspaceListQuery query,
            IReadOnlyCollection<Guid> requesterIds,
            CancellationToken cancellationToken = default);
    }
}

