using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Common.Implementations.Responses;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using static SMS.Generic.Dtos.Dtos.PostDto;
using static SMS.Generic.Dtos.Dtos.RosterDto;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for request service operations.
    /// </summary>
    public interface IRequestService : IGenericService<RequestDto, Request>
    {
        Task<SuccessResult<PostPositionDto>> AssignPostToMe(Guid postId, DateOnly dateOfWork, Guid? employeeId = null, bool confirmConflict = false, bool isBulkAssign = false, Guid? postPositionId = null, bool isPlaceHolder = false, PlaceholderDto? placeholder = null, bool enableInvoiceAndPayroll = false, decimal? waitingPostPositionPayRate = null);

        /// <summary>
        /// Available jobs for the JWT employee, or for <paramref name="employeeId"/> when a privileged
        /// caller (EmployeesViewManage roles) requests another employee in the same department.
        /// </summary>
        Task<PagedList<AvailablePostDto>> GetAllAvailablePostRequest(
            SearchModel searchRequest,
            PageParam? pagination = null,
            bool isAvailablePost = false,
            Guid? employeeId = null);

        /// <summary>
        /// Distinct request id/title options for the employee dashboard Request Title filter,
        /// limited to the same date window as <see cref="GetAllAvailablePostRequest"/>.
        /// </summary>
        Task<IReadOnlyList<RequestFilterOptionDto>> GetRequestTitlesByDateRangeAsync(DateOnly startDate, int numberOfDays);

        /// <summary>
        /// Distinct requester id/title options for the employee dashboard Requester filter,
        /// limited to the same date window as <see cref="GetAllAvailablePostRequest"/>.
        /// </summary>
        Task<IReadOnlyList<RequestFilterOptionDto>> GetRequestersByDateRangeAsync(DateOnly startDate, int numberOfDays);

        /// <summary>
        /// Same date-window requester logic as <see cref="GetRequestersByDateRangeAsync"/>,
        /// returned via generic options as <see cref="SelectOptionDto"/>.
        /// </summary>
        Task<IEnumerable<SelectOptionDto>> GetDashboardRequesterOptionsAsync(
            DateOnly startDate,
            int numberOfDays,
            string? searchTerm = null);

        /// <summary>
        /// Returns available posts for the current department week (start day from DepartmentPolicy.OnDutyStartDayOfWeek),
        /// in the same DTO shape as <see cref="GetAllAvailablePostRequest"/>.
        /// </summary>
        Task<PagedList<AvailablePostDto>> GetWeeklyAvailablePostRequestAsync(ReleaseAs release, PageParam? pagination = null);

        Task<ReleaseAs> GetLatestReleaseAsForRequestAsync(Guid requestId);
        Task<PagedList<RequesterWithRequestsListDto>> GetRequestersWithRequestsAsync(
            PageParam pageParam,
            string? requesterName = null,
            IReadOnlyCollection<RequestStatus>? requestStatuses = null,
            IReadOnlyCollection<RequestTypes>? requestTypes = null,
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            bool? includeCompleted = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Unified requester workspace list (daily workspace vs manager/audit) via <see cref="RequesterWorkspaceListFilterDto.IsDailyWorkspace"/>.
        /// </summary>
        Task<PagedList<RequesterWorkspaceListItemDto>> GetRequesterWorkspaceListAsync(
            RequesterWorkspaceListFilterDto filter,
            CancellationToken cancellationToken = default);

        Task<PagedList<RequestInRequesterListDto>> GetRequestsByRequesterFilterAsync(
            RequesterRequestsFilterDto filter,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets requests for a requester along with respective jobs and assigned employees' latest latitude/longitude,
        /// resolving current department date, active schedules, and jobs/shifts starting within 30 minutes before or after current department time.
        /// </summary>
        Task<PagedList<RequestInRequesterListDto>> GetRequestsWithLocationsByRequesterIdAsync(
            Guid requesterId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets requests for a requester along with respective jobs and assigned employees' latest latitude/longitude,
        /// filtering for jobs/shifts starting within 30 minutes before or after current department time.
        /// </summary>
        Task<PagedList<RequestInRequesterListDto>> GetRequestsWithLocationsByRequesterFilterAsync(
            RequesterRequestsFilterDto filter,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Builds requester request summaries (name, request count, project numbers) for the given visibility scope.
        /// </summary>
        Task<IReadOnlyList<RequesterRequestSummaryDto>> GetRequesterRequestSummariesAsync(
            RequesterRequestSummaryFilterDto filter,
            IReadOnlyCollection<Guid>? requesterManagerIds = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Dedicated method for requester summaries whose job/shift starts within the specified shift window.
        /// </summary>
        Task<IReadOnlyList<RequesterRequestSummaryDto>> GetRequesterRequestSummariesByShiftWindowAsync(
            RequesterRequestSummaryFilterDto filter,
            DateTime shiftWindowStart,
            DateTime shiftWindowEnd,
            IReadOnlyCollection<Guid>? requesterManagerIds = null,
            CancellationToken cancellationToken = default);

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
        /// Bulk assigns post positions to multiple employees. Same postId and date for all employees, but different employee IDs.
        /// </summary>
        /// <param name="postId">The post ID to assign.</param>
        /// <param name="dateOfWork">The date of work.</param>
        /// <param name="employeeIds">List of employee IDs to assign.</param>
        /// <returns>List of messages - success or error messages for each employee.</returns>
        Task<SuccessResult<List<BulkAssignPostPositionItemDto>>> BulkUpsertPostPositionsAsync(BulkAssignPostPositionRequestDto bulkAssignPostPositionRequestDto);
        Task<RequestDto> CloneRequestAsync(Guid requestId);

        /// <summary>
        /// Gets available dates for post positions by post ID.
        /// </summary>
        /// <param name="postId">The post ID.</param>
        /// <returns>List of available dates.</returns>
        Task<List<DateOnly>> GetPostPostitionsDatesByPostId(Guid postId);

        /// <summary>
        /// Gets all calculated roster work dates for a request from its schedules and recurrence
        /// (not filtered by open assignment slots).
        /// </summary>
        /// <param name="requestId">The request ID.</param>
        /// <returns>Distinct ascending list of work dates; empty when the request is not found or has no dates.</returns>
        Task<List<DateOnly>> GetRosterDatesAsync(Guid requestId);
        Task<AvailablePostResultDto> CheckAvailablePost(Guid postId, DateOnly dateOfWork, Guid? employeeId = null, bool isBulkAssign = false, Guid? postPositionId = null,bool isPlaceHolder=false,string placeHolder =null);

        /// <summary>
        /// Job-safe wrapper around <see cref="CheckAvailablePost"/> that sets minimal HttpContext claims
        /// (DepartmentID, EmployeeID, TimeZoneId, DepartmentPolicyConfiguration) for token-dependent logic.
        /// </summary>
        Task<AvailablePostResultDto> CheckAvailablePostForJobAsync(
            Guid postId,
            DateOnly dateOfWork,
            Guid employeeId,
            Guid departmentId,
            string timeZoneId,
            DepartmentPolicyDto? departmentPolicy,
            bool isBulkAssign = false);

        /// <summary>
        /// Sends Vehicle Approval emails to the given approval groups. Uses VehicleApprovalEmail template.
        /// For each routing group: fetches group email and title, builds link, replaces [FullName], [ApprovalLink], [RequestName].
        /// </summary>
        /// <param name="request">RequestId and RoutingGroupId (ApproverID) to send to.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SendApprovalEmailsAsync(SendApprovalEmailsRequestDto request, CancellationToken cancellationToken = default);
        Task SendAssignPostEmailAsync(
            Guid employeeId,
            string? email,
            string postTitle,
            DateOnly? dateOfWork,
            TimeOnly? startTime,
            int? postLength = null,
            string? postLocation = null,
            string? assignedEmployeeName = null,
            string? assignedEmployeeType = null,
            string? assignedEmployeeRank = null,
            string? pushNotificationTitle = null,
            string? pushNotificationBody = null,
            Guid? departmentId = null,
            CancellationToken cancellationToken = default);

        Task SendAssignedPostNotificationAsync(
            Guid postId,
            DateOnly dateOfWork,
            Guid employeeId,
            Guid? departmentId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets request data for invoice by project number and date range. Parses project number (e.g. 202600001) into fiscal year and number,
        /// finds matching requests, and returns SuccessResult with InvoiceDto when found and details exist; otherwise Status 404 and Message set.
        /// </summary>
        /// <param name="projectNumber">Combined project number (year + number, e.g. 202600001).</param>
        /// <param name="startDate">Start date filter.</param>
        /// <param name="endDate">End date filter.</param>
        /// <returns>SuccessResult with InvoiceDto; Status 404 and Message when no request or no invoice details.</returns>
        Task<SuccessResult<InvoiceDto>> GetRequestDataForInvoiceAsync(string projectNumber, DateOnly startDate, DateOnly endDate, int requestCount = 1);
        Task<RosterResponseDto> GetRosterInformationAsync(Guid postId, DateOnly dateOfWork);
        Task<RosterResponseDto> GetRosterInformationByRequestAsync(Guid posrequestIDtId, DateOnly dateOfWork);

        Task<List<EmployeeDto>> GetAssignableEmployeesForRequestAsync(Guid requestId, string? searchTerm = null);

        /// <summary>
        /// View Details: Available Job / waiting list (<paramref name="postId"/>, <paramref name="requestId"/>, <paramref name="isSpecialEvent"/>)
        /// or Scheduled / Ready-to-close (<paramref name="postPositionId"/>). Same branching as the legacy multi-call flows.
        /// </summary>
        Task<ViewDetailsDto> GetViewDetailsAsync(
            Guid? postPositionId,
            Guid? postId,
            Guid? requestId,
            bool? isSpecialEvent,
            DateOnly? dateOfWork = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets interested employees for a tenure post and work date (waiting list rows for that post/date).
        /// </summary>
        Task<List<TenureInterestedEmployeeRowDto>> GetTenureExpressInterestDashboardAsync(
            Guid postId,
            DateOnly dateOfWork,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// UI-triggered: runs the same tenure waiting validation/copy steps as the scheduled job for the waiting row identified by <paramref name="waitingPostPositionId"/>.
        /// Does not modify <see cref="RequestSchedule"/> or bulk-process all waiters (cron job unchanged).
        /// </summary>
        Task<RequestScheduleProcessingResultDto> ProcessTenureWaitingListForWaitingPostManualAsync(
            Guid waitingPostPositionId,
            CancellationToken cancellationToken = default);

        Task<PagedList<OpenJobsDashboardDto>> GetOpenJobsDashboardAsync(DateOnly startDate, DateOnly endDate, PageParam? pagination = null, string? searchTerm = null, string? statusFilter = null, CancellationToken cancellationToken = default);
        Task<PagedList<TenureJobsDashboardDto>> GetTenureJobsDashboardAsync(DateOnly startDate, DateOnly endDate, PageParam? pagination = null, CancellationToken cancellationToken = default);
        Task<SuccessResult<List<InvoiceDto>>> GetInvoicePreviewDataListAsync(IReadOnlyCollection<string> projectNumbers, DateOnly startDate, DateOnly endDate);
        Task<ExportFileDto> ExportRosterExcelAsync(Guid requestID, DateOnly dateOfWork);
        Task<ExportFileDto> ExportRosterExcelByPostAsync(Guid postId, DateOnly dateOfWork);
    }
}

