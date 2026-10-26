using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Common.Implementations.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for post service operations.
    /// </summary>
    public interface IPostPositionService : IGenericService<PostPositionDto, PostPosition>
    {
        /// <summary>
        /// Gets post positions for the currently logged-in employee.
        /// Supports scheduled (Applied), ongoing (Applied + time passed), and closed views.
        /// </summary>
        /// <param name="status">The post position status to filter by.</param>
        /// <param name="isOngoing">
        /// When true and status is Applied, returns only ongoing posts based on date/time.
        /// When false, returns scheduled posts for Applied or closed posts for Closed.
        /// </param>
        /// <param name="pagination">Optional pagination parameters.</param>
        /// <returns>A paginated list of post positions for the logged-in employee.</returns>
        Task<PagedList<PostTypeResponseDto>> GetForLoggedInEmployeeAsync(
            PostPositionStatus status,
            bool isOngoing = false,
            [FromQuery] PageParam? pagination = null);

        /// <summary>
        /// Gets post positions for an employee filtered by one or more statuses.
        /// By default, uses the currently logged-in employee; when <paramref name="employeeId"/> is supplied,
        /// returns results for that employee instead (authorization must be enforced by the caller).
        /// Uses the same logic as GetForLoggedInEmployeeAsync (Applied/Transferred ongoing vs scheduled when isOngoing is used).
        /// </summary>
        /// <param name="statuses">The post position statuses to filter by (e.g. Applied, Closed).</param>
        /// <param name="isOngoing">When true and statuses include Applied, returns only ongoing Applied/Transferred posts; when false, scheduled only. Other statuses are included as-is.</param>
        /// <param name="pagination">Optional pagination parameters.</param>
        /// <param name="employeeId">
        /// Optional employee ID override. When null, the employee ID is taken from the authenticated user token.
        /// </param>
        /// <param name="useLoggedInEmployeeWhenEmployeeIdMissing">
        /// When true and <paramref name="employeeId"/> is null/empty, uses employee ID from token.
        /// When false and <paramref name="employeeId"/> is null/empty, no employee filter is applied.
        /// </param>
        /// <returns>A paginated list of post positions for the effective employee.</returns>
        Task<PagedList<PostTypeResponseDto>> GetForLoggedInEmployeeByStatusesAsync(
            IReadOnlyList<PostPositionStatus> statuses,
            bool isOngoing = false,
            PageParam? pagination = null,
            Guid? employeeId = null,
            bool useLoggedInEmployeeWhenEmployeeIdMissing = true,
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            string? searchTerm = null,
            bool isPlaceholderOnly = false,
            SearchModel? searchModel = null,
            Guid? requesterId = null);

        /// <summary>
        /// Gets ongoing assigned posts with optional filters.
        /// By default this API returns ongoing posts across all employees (department-scoped by token),
        /// and can be narrowed by employee, date range, search term, and requester.
        /// When <paramref name="isPlaceholderOnly"/> is true, returns only placeholder post positions.
        /// </summary>
        Task<PagedList<PostTypeResponseDto>> GetOngoingPostPositionsAsync(
            PageParam? pagination = null,
            Guid? employeeId = null,
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            string? searchTerm = null,
            bool useLoggedInEmployeeWhenEmployeeIdMissing = false,
            bool isPlaceholderOnly = false,
            Guid? requesterId = null);

        Task<int> UpdateStatusByPostIdAsync(Guid postId, PostPositionStatus status);

        /// <summary>
        /// Cancels a post position (sets status to Cancelled), then sends a cancellation email to the current employee's primary email using the CancelledEmail template with [CancellationDate] and [CancellationTime] replaced from the post position.
        /// </summary>
        /// <param name="postPositionId">The post position ID to cancel.</param>
        /// <param name="reasonForCancel">Optional cancellation reason to store on the post position.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The number of rows updated (1 when successful).</returns>
        Task<int> CancelPostPositionAsync(Guid postPositionId, string? reasonForCancel = null, bool isApproveCancellation = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Revokes a pending cancellation: restores the post position from <see cref="PostPositionStatus.CancellationPending"/>
        /// to <see cref="PostPositionStatus.Assigned"/> and clears the cancellation reason.
        /// </summary>
        /// <param name="postPositionId">The post position ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The number of rows updated (1 when successful).</returns>
        Task<int> RevokeCancellationAsync(Guid postPositionId, CancellationToken cancellationToken = default);

        /// <summary>
        /// For a department, sets <see cref="PostPositionStatus.CancellationPending"/> back to
        /// <see cref="PostPositionStatus.Assigned"/> when the scheduled shift start has passed (department-local time).
        /// Status only; does not clear <c>ReasonForCancel</c>.
        /// </summary>
        Task<IReadOnlyList<Guid>> RevertCancellationPendingToAssignedAsync(Guid departmentId, DateTime departmentNow, CancellationToken cancellationToken = default);

        /// <summary>
        /// Post positions for a department whose scheduled start time has passed (same date/time rule as
        /// <see cref="RevertCancellationPendingToAssignedAsync"/>).
        /// When <paramref name="onlyUnfinalizedAllocations"/> is true, rows with
        /// <see cref="PostPositionDto.IsAllocationFinalized"/> already set are excluded.
        /// </summary>
        Task<IReadOnlyList<PostPositionDto>> GetPostPositionsPastScheduledStartAsync(
            Guid departmentId,
            DateTime departmentNow,
            PostPositionStatus? status = null,
            bool excludePlaceholders = false,
            bool requireEmployee = false,
            bool requireRequestId = false,
            IEnumerable<PostPositionStatus>? statuses = null,
            bool onlyUnfinalizedAllocations = false,
            CancellationToken cancellationToken = default);

        Task<int> UpdateCallSignAsync(Guid postPositionId, string fieldName, string? fieldValue);

        Task SwapCallSignAsync(Guid postPositionId1, Guid postPositionId2);

        /// <summary>
        /// Gets dashboard counts for the logged-in employee including available posts, scheduled jobs, and closed jobs.
        /// </summary>
        /// <param name="searchModel">Search model for available posts.</param>
        /// <param name="pagination">Pagination parameters.</param>
        /// <returns>Dashboard counts for the logged-in employee.</returns>
        Task<EmployeeDashboardCountsDto> GetEmployeeDashboardCountsAsync(SearchModel searchModel, PageParam? pagination = null);
        Task<PostPositionsByPostResponseDto> GetPostPositionsWithEmployeeDetailsAsync(
            Guid postId,
            PageParam? pagination = null,
            IReadOnlyList<DateOnly>? dates = null,
            IReadOnlyCollection<RequestStatus>? requestStatuses = null,
            bool includeCompleted = false,
            bool isViewHistory = true);

        /// <summary>
        /// Loads post positions for a post with employee details.
        /// Same status and request-schedule filtering used by <see cref="GetCollectDatesForPostAsync"/>.
        /// </summary>
        Task<List<PostPositionResponseDto>> GetPostPositionsWithEmployeeAsync(
            Guid postId,
            DateOnly? dateOfWork,
            PageParam? pagination = null,
            IReadOnlyCollection<DateOnly>? datesOfWork = null,
            IReadOnlyCollection<RequestStatus>? requestStatuses = null,
            bool includeCompleted = false);

        /// <summary>
        /// Gets distinct <see cref="PostPosition.DateOfWork"/> values for a post in ascending order.
        /// </summary>
        Task<List<DateOnly>> GetCollectDatesForPostAsync(
            Guid postId,
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            IReadOnlyCollection<RequestStatus>? requestStatuses = null,
            bool includeCompleted = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets post positions for a post with optional date and status filters.
        /// </summary>
        Task<PagedList<PostPositionInRequestListDto>> GetPostPositionsByPostFilterAsync(
            RequestPostPositionsFilterDto filter,
            bool isViewHistory = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Assigned post positions (Applied/Cancelled/Closed) and active waiting-list rows for a post.
        /// When <paramref name="startDate"/> is provided, filters to an inclusive range starting at startDate.
        /// If numberOfDays is omitted, defaults to 15 days. If startDate is omitted, returns all dates.
        /// </summary>
        Task<(List<PostPositionResponseDto> PostPositions, List<WaitingPostPositionResponseDto> WaitingPostPositions)>
            GetAssignmentsAndWaitingList(Guid postId, DateOnly? startDate = null, int? numberOfDays = null);

        /// <summary>
        /// Gets a single post position with Post details by post position ID.
        /// </summary>
        /// <param name="postPositionId">The post position ID to fetch (required).</param>
        /// <returns>The post position with full post details, or null if not found.</returns>
        Task<PostTypeResponseDto?> GetPostPositionWithPostDetailsByIdAsync(Guid postPositionId);

        /// <summary>
        /// Declines a split post position, records history, and creates a new available post position.
        /// </summary>
        /// <param name="postPositionId">The declined post position ID.</param>
        /// <returns>The newly created post position.</returns>
        Task<PostPositionDto> DeclineSplitAsync(Guid postPositionId);

        /// <summary>
        /// Checks if an employee has any existing post positions on a specific date.
        /// Used to prevent date conflicts when assigning employees to posts.
        /// </summary>
        /// <param name="employeeId">The employee ID to check.</param>
        /// <param name="dateOfWork">The date to check for conflicts.</param>
        /// <returns>True if employee has a non-cancelled post position on the date; otherwise, false.</returns>
        Task<bool> HasEmployeePostPositionOnDateAsync(Guid employeeId, DateOnly dateOfWork);
        //Task<bool> HasEmployeePostPositionOnDateAsync(Guid employeeId, DateOnly dateOfWork);
        Task<PostPositionDto> AssignToPostMeAsync(Guid postId, Guid employeeId, DateOnly dateOfWork, bool isPlaceHolder, PlaceholderDto? placeholder = null, Guid? sourcePostPositionId = null, bool enableInvoiceAndPayroll = false, decimal? waitingPostPositionPayRate = null);

        /// <summary>
        /// Finds the latest cancelled post position for the same <paramref name="postId"/> and
        /// <paramref name="dateOfWork"/> so bulk assign can inherit <see cref="PostPositionDto.CallSign"/>.
        /// Prefers the most recently updated row that has a call sign.
        /// </summary>
        Task<Guid?> CallSignBulkAssignSourcePostPositionIdAsync(Guid postId, DateOnly dateOfWork);
        Task<List<PostPositionDto>> GetPostPositionByEmployeeAndDateRangeAsync(Guid employeeId,DateOnly startDate,DateOnly endDate,Guid? departmentId = null,bool excludeCancelledOnly = true,bool enableExemptRequestsJobsFromHoursRules = false);
        Task<EmployeePolicyUsageDto> GetEmployeeStatistics(DepartmentPolicyDto departmentPolicy, Guid? employeeId = null, DateOnly? date = null, DateTime? dateTime = null);
        Task<List<PostPositionDto>> GetEmployeesPostPositionsByEmployeeIdAndDateRangeAsync(List<Guid> employeeIds, DateOnly rangeStart, DateOnly rangeEnd, Guid? departmentId = null);

        /// <summary>
        /// Gets weekly dashboard posts for the logged-in employee based on department week configuration.
        /// </summary>
        /// <param name="isOngoing">
        /// When true: returns only ongoing Applied/Transferred posts.
        /// When false: returns only scheduled Applied/Transferred posts.
        /// </param>
        Task<PagedList<PostTypeResponseDto>> GetWeeklyDashboardPostsAsync(bool isOngoing, PageParam? pagination = null);

        /// <summary>
        /// Gets posts that were cancelled in the last 72 hours for the logged-in employee.
        /// </summary>
        Task<PagedList<PostTypeResponseDto>> GetRecentCancelledPostsAsync(PageParam? pagination = null);

        /// <summary>
        /// Returns close assignment post positions.
        /// Fixed statuses: Closed, ClosedAndSplit, Unclosed.
        /// By default does not force logged-in employee filtering when employeeId is omitted.
        /// </summary>
        Task<PagedList<PostTypeResponseDto>> GetCloseAssignmentsForLoggedInEmployeeAsync(
            string? searchTerm = null,
            PageParam? pagination = null,
            Guid? employeeId = null,
            DateOnly? startDate = null,
            DateOnly? endDate = null);

        /// <summary>
        /// Job helper: copies tenure waiting rows into PostPosition as Applied.
        /// Does NOT delete tenure rows (testing phase).
        /// Inserts only (does not upsert).
        /// </summary>
        Task<int> CopyTenureWaitingToPostPositionsAsync(
            IReadOnlyList<WaitingPostPositionDto> tenureRows,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates transfer source post position against requested post/date and target employee.
        /// Throws <see cref="WebApiException"/> when invalid.
        /// </summary>
        Task<PostPositionDto> ValidateTransferSourceAsync(Guid sourcePostPositionId, Guid requestedPostId, DateOnly requestedDateOfWork, Guid targetEmployeeId);
        Task<Guid?> GetHighestRankEmployeeIdAsync(IEnumerable<PostPositionDto> positions);

        /// <summary>
        /// Loads post positions for a post and optional work date with employee and employee type (all statuses).
        /// </summary>
        Task<List<PostPosition>> GetPostPositionsWithEmployeeAllStatusesAsync(
            Guid postId,
            DateOnly? dateOfWork,
            PageParam? pagination);

        /// <summary>
        /// Sends assignment email for a post position using the post position ID.
        /// Retrieves post position details and calls the modified SendAssignPostEmailAsync method.
        /// </summary>
        /// <param name="postPositionId">The post position ID to send email for.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendAssignPostEmailByPostPositionIdAsync(Guid postPositionId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Officers in a single paged list with schedule status relative to department-local now:
        /// ended in the last 30 minutes, currently working, or starting within the next 8 hours.
        /// </summary>
        Task<PagedList<OfficerScheduledAssignmentDto>> GetOfficerScheduleOverviewAsync(
            string? searchTerm,
            PageParam? pagination = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Real-employee assigned / cancellation-pending post positions for a one-time request on a specific work date.
        /// </summary>
        Task<List<PostPosition>> GetAssignedRealEmployeesByRequestAndDateAsync(
            Guid requestId,
            DateOnly dateOfWork,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lightweight projections for one-time request date-change assignment review and conflict checks.
        /// </summary>
        Task<OneTimeDateChangeReviewDataDto> GetOneTimeDateChangeReviewDataAsync(
            Guid requestId,
            DateOnly oldDate,
            DateOnly newDate,
            Guid? departmentId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Assigned or cancellation-pending post positions with an employee, for all posts under a request.
        /// </summary>
        Task<List<PostPositionDto>> GetAssignedPostPositionsForRequestAsync(Guid requestId);

        /// <summary>
        /// Updates future assignment pay rate and premium flag from each post's current values.
        /// </summary>
        Task UpdateFuturePostPositionPayRatesForRequestAsync(Guid requestId);

        /// <summary>
        /// Sets <see cref="PostPositionDto.IsExemptPosition"/> on future assigned rows for a post when <see cref="PostDto.IsExemptHours"/> changes.
        /// </summary>
        Task UpdateFuturePostPositionExemptFlagForPostAsync(Guid postId, bool isExemptHours);

        /// <summary>
        /// Gets today's jobs (mixed scheduled and ready to close) for an employee.
        /// When <paramref name="pagination"/> is null, every matching job is returned.
        /// </summary>
        /// <param name="pagination">Optional paging. Uses the same offset/limit and page/pageSize rules as <see cref="SMS.Generic.Common.Implementations.Paginations.PageParam"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paged today's jobs for the authenticated employee. <c>totalRecords</c> is the full match count.</returns>
        Task<PagedList<EmployeePostListingItemDto>> GetTodaysJobsAsync(
            PageParam? pagination = null,
            CancellationToken cancellationToken = default);
    }
}

