using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Enums;


namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Shared identity fields for requester-level request summaries.
    /// </summary>
    public class RequesterSummaryBaseDto
    {
        public Guid RequesterId { get; set; }
        public string RequesterName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Requester summary with a flat list of request rows for list/grid APIs (not full RequesterDto / RequestDto).
    /// </summary>
    public class RequesterWithRequestsListDto : RequesterSummaryBaseDto
    {
        public int RequestsCount { get; set; }

        public bool HasUnpaidInvoice { get; set; }

        public List<RequestInRequesterListDto> Requests { get; set; } = new();
    }

    /// <summary>
    /// Lightweight requester summary: matching request count and project numbers only.
    /// </summary>
    public class RequesterRequestSummaryDto : RequesterSummaryBaseDto
    {
        public RequesterRequestSummaryDto() { }

        public RequesterRequestSummaryDto(
            Guid requesterId,
            string? requesterName,
            int requestCount,
            IEnumerable<string?>? projectNumbers)
        {
            RequesterId = requesterId;
            RequesterName = requesterName ?? string.Empty;
            RequestCount = requestCount;
            ProjectNumbers = (projectNumbers ?? Enumerable.Empty<string?>())
                .Where(pn => !string.IsNullOrWhiteSpace(pn))
                .Select(pn => pn!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(pn => pn, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public int RequestCount { get; set; }

        public List<string> ProjectNumbers { get; set; } = new();

        /// <summary>
        /// Matching requests and schedules. Set only when a requester or request search is applied.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<RequestInRequesterListDto>? Requests { get; set; }
    }

    /// <summary>
    /// One request row under a requester for list/grid display.
    /// </summary>
    public class RequestInRequesterListDto
    {
        public Guid RequestId { get; set; }
        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public int? FiscalYear { get; set; }

        public string? Name { get; set; }

        public string? ProjectNumber { get; set; }

        public RequestTypeDto? RequestType { get; set; }

        public string ReleaseStatus { get; set; } = string.Empty;

        /// <summary>
        /// Selected schedule window for this request based on release-status rules.
        /// </summary>
        public List<RequestScheduleInRequesterListDto> RequestSchedules { get; set; } = new();

        public int PostCount { get; set; }

        /// <summary>
        /// Respective jobs (posts) and assigned employees' locations.
        /// Populated for location-tracking queries.
        /// </summary>
        public List<PostWithEmployeeLocationDto>? Posts { get; set; }
    }

    /// <summary>
    /// Paged requests for a single requester, including unpaid-invoice flag and aggregate counts.
    /// </summary>
    public class RequesterRequestsResponseDto : PagedList<RequestInRequesterListDto>
    {
        public RequesterRequestsResponseDto() { }

        public RequesterRequestsResponseDto(PageParam pageParam) : base(pageParam) { }

        public RequesterRequestsResponseDto(
            IEnumerable<RequestInRequesterListDto> data,
            PageParam pageParam,
            long totalRecords,
            bool hasUnpaidInvoice,
            int totalRequestCount,
            int totalPostCount)
            : base(data, pageParam, totalRecords)
        {
            HasUnpaidInvoice = hasUnpaidInvoice;
            TotalRequestCount = totalRequestCount;
            TotalPostCount = totalPostCount;
        }

        public bool HasUnpaidInvoice { get; set; }
        public int TotalRequestCount { get; set; }
        public int TotalPostCount { get; set; }
    }

    /// <summary>
    /// One request-schedule window row for list APIs.
    /// </summary>
    public class RequestScheduleInRequesterListDto
    {
        public Guid RequestScheduleID { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string ReleaseStatus { get; set; } = string.Empty;

        public string ReleaseAs { get; set; } = string.Empty;
    }

    /// <summary>
    /// Shared date / type / status filters for requester-scoped request queries.
    /// </summary>
    public class RequesterRequestQueryFilterDto
    {
        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public List<RequestTypes>? RequestTypes { get; set; }

        public List<RequestStatus>? RequestStatuses { get; set; }

        /// <summary>
        /// When set, only requests with a schedule of one of these release types are included.
        /// The matching schedule rows are limited to these release types.
        /// </summary>
        public List<ReleaseAs>? ReleaseAs { get; set; }
    }

    /// <summary>
    /// Filter payload for requests under a single requester.
    /// </summary>
    public class RequesterRequestsFilterDto : RequesterRequestQueryFilterDto
    {
        [Required]
        public Guid RequesterId { get; set; }

        public string? Search { get; set; }
        public PageParam? PageParam { get; set; }

        /// <summary>
        /// When true, <see cref="RequesterRequestQueryFilterDto.StartDate"/> and
        /// <see cref="RequesterRequestQueryFilterDto.EndDate"/> overlap schedule dates,
        /// and only those schedule rows are returned.
        /// When false, dates stay on the request start/end.
        /// </summary>
        public bool ApplyDateOnSchedules { get; set; }

        /// <summary>
        /// When true, limits requests and jobs to active shifts (from 30 minutes before job start time until 30 minutes after job end time).
        /// Defaults to false for standard queries.
        /// </summary>
        public bool IsShiftStartingWithin30Minutes { get; set; } = false;
    }

    /// <summary>
    /// Filter payload for requester request summaries across requesters.
    /// </summary>
    public class RequesterRequestSummaryFilterDto : RequesterRequestQueryFilterDto
    {
        public string? RequesterName { get; set; }

        /// <summary>
        /// When true, <see cref="RequesterRequestQueryFilterDto.StartDate"/> and
        /// <see cref="RequesterRequestQueryFilterDto.EndDate"/> overlap schedule dates.
        /// When false, dates stay on the request start/end.
        /// </summary>
        public bool ApplyDateOnSchedules { get; set; }

        /// <summary>
        /// Partial match on requester name or request title.
        /// When set, each summary also includes the matching requests.
        /// </summary>
        public string? SearchText { get; set; }

        /// <summary>
        /// When true, returns only requesters with active shifts (from 30 minutes before job start time
        /// until 30 minutes after job end time). Defaults to false.
        /// </summary>
        public bool IsShiftStartingWithin30Minutes { get; set; } = false;
    }

    /// <summary>
    /// Filter payload for posts under a single request.
    /// </summary>
    public class RequestPostsFilterDto
    {
        public Guid RequestId { get; set; }
        public string? Search { get; set; }
        public PageParam? PageParam { get; set; }
    }

    /// <summary>
    /// One post row under a request for list/grid display.
    /// </summary>
    public class PostInRequestListDto
    {
        public Guid PostId { get; set; }
        public string PostTitle { get; set; } = string.Empty;
        public TimeOnly PostStartTime { get; set; }
        public TimeOnly PostEndTime { get; set; }
        public string PostLocation { get; set; } = string.Empty;
        public int TotalPositions { get; set; }

        public bool IsSupervisorPost { get; set; }

        public Guid? PostParentId { get; set; }

        /// <summary>
        /// Hierarchical display level (e.g. 1, 1-1, 1-2).
        /// </summary>
        public string? Level { get; set; }

        public bool? DefineCallSign { get; set; }
    }

    /// <summary>
    /// Response for filtered posts under a request.
    /// </summary>
    public class RequestPostsResponseDto
    {
        public RequestPostsResponseDto() { }

        public RequestPostsResponseDto(IEnumerable<PostInRequestListDto> posts)
        {
            var list = posts?.ToList() ?? new List<PostInRequestListDto>();
            Posts = list;
            TotalPostCount = list.Count;
            TotalPositionCount = list.Sum(x => x.TotalPositions);
        }

        public int TotalPostCount { get; set; }
        public int TotalPositionCount { get; set; }
        public IReadOnlyList<PostInRequestListDto> Posts { get; set; } = Array.Empty<PostInRequestListDto>();
    }

    /// <summary>
    /// Filter payload for post positions under a single post.
    /// </summary>
    public class RequestPostPositionsFilterDto
    {
        [Required]
        public Guid PostId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        /// <summary>
        /// Optional subset of dates within <see cref="StartDate"/>–<see cref="EndDate"/>.
        /// When provided, only post positions on these dates (that fall inside the range) are returned.
        /// </summary>
        public List<DateOnly>? Dates { get; set; }
        public List<RequestStatus>? RequestStatuses { get; set; }
        public bool IncludeCompleted { get; set; }
        public PageParam? PageParam { get; set; }
    }

    /// <summary>
    /// One post-position row under a post for list/grid display.
    /// </summary>
    public class PostPositionInRequestListDto
    {
        public Guid PostPositionId { get; set; }
        public Guid? PostCallSignId { get; set; }
        public string? CallSign { get; set; }
        public string? EmployeeName { get; set; }
        public DateOnly? DateOfWork { get; set; }
        public string? EmployeeType { get; set; }
        public string? BadgeNumber { get; set; }
        public string? Phone { get; set; }
        public PostPositionStatus Status { get; set; }
        public bool IsPlaceholder { get; set; }
        public Guid? PlaceholderId { get; set; }
        public bool IsUniquePerPosition { get; set; }

        /// <summary>
        /// Keeps rows whose <see cref="DateOfWork"/> is in <paramref name="dates"/> and inside the optional range.
        /// When <paramref name="dates"/> is null/empty, returns the original list.
        /// </summary>
        public static List<PostPositionInRequestListDto> FilterByDates(
            IReadOnlyList<PostPositionInRequestListDto> postPositions,
            IReadOnlyCollection<DateOnly>? dates,
            DateOnly? startDate = null,
            DateOnly? endDate = null)
        {
            if (postPositions == null || postPositions.Count == 0)
                return new List<PostPositionInRequestListDto>();

            if (dates == null || dates.Count == 0)
                return postPositions.ToList();

            var allowed = dates
                .Where(d =>
                    (!startDate.HasValue || d >= startDate.Value) &&
                    (!endDate.HasValue || d <= endDate.Value))
                .ToHashSet();

            return postPositions
                .Where(p => p.DateOfWork.HasValue && allowed.Contains(p.DateOfWork.Value))
                .ToList();
        }

        /// <summary>
        /// Distinct work dates from the given post-position rows, ordered ascending.
        /// </summary>
        public static List<DateOnly> ExtractDates(IEnumerable<PostPositionInRequestListDto>? postPositions)
            => (postPositions ?? Enumerable.Empty<PostPositionInRequestListDto>())
                .Select(p => p.DateOfWork)
                .ExtractWorkDates();
    }

    /// <summary>
    /// Paged post positions under a post, with distinct work dates from the full (unpaged) result set.
    /// </summary>
    public class RequestPostPositionsResponseDto : PagedList<PostPositionInRequestListDto>
    {
        public RequestPostPositionsResponseDto() { }

        public RequestPostPositionsResponseDto(PageParam pageParam) : base(pageParam) { }

        public RequestPostPositionsResponseDto(
            IEnumerable<PostPositionInRequestListDto> data,
            PageParam pageParam,
            long totalRecords,
            IReadOnlyList<DateOnly>? dates = null)
            : base(data, pageParam, totalRecords)
        {
            Dates = dates ?? PostPositionInRequestListDto.ExtractDates(data);
        }

        public IReadOnlyList<DateOnly> Dates { get; set; } = Array.Empty<DateOnly>();
    }

    /// <summary>
    /// Paged post positions for a post, with distinct work dates from the full (unpaged, unselected-date) result set.
    /// Dates are produced with the same <see cref="PostPositionExtension.ExtractWorkDates"/> used by <see cref="PostPositionInRequestListDto.ExtractDates"/>.
    /// </summary>
    public class PostPositionsByPostResponseDto : PagedList<PostPositionResponseDto>
    {
        public PostPositionsByPostResponseDto() { }

        public PostPositionsByPostResponseDto(PageParam pageParam) : base(pageParam) { }

        public PostPositionsByPostResponseDto(
            IEnumerable<PostPositionResponseDto> data,
            PageParam pageParam,
            long totalRecords,
            IReadOnlyList<DateOnly>? dates = null)
            : base(data, pageParam, totalRecords)
        {
            Dates = dates ?? data.Select(p => p.DateOfWork).ExtractWorkDates();
        }

        public IReadOnlyList<DateOnly> Dates { get; set; } = Array.Empty<DateOnly>();
    }

    /// <summary>
    /// Body payload for requester-with-requests filtering.
    /// </summary>
    public class RequesterWithRequestsFilterDto : RequesterRequestQueryFilterDto
    {
        public PageParam? PageParam { get; set; }
        public string? RequesterName { get; set; }
        public bool? IncludeCompleted { get; set; }
    }

    /// <summary>
    /// Unified filter for daily workspace vs manager/audit requester list (single endpoint).
    /// When <see cref="IsDailyWorkspace"/> is true, send <see cref="PageParam"/> (page, pageSize, optional sortBy/sortDirection)
    /// for server-side paging; the response includes <c>totalRecords</c> and one page of summary rows in <c>data</c>.
    /// </summary>
    public class RequesterWorkspaceListFilterDto : RequesterRequestQueryFilterDto
    {
        public bool IsDailyWorkspace { get; set; }

        public string? RequesterName { get; set; }

        /// <summary>
        /// Paging and sort (defaults: page 1, pageSize 25, requesterName asc). Required for daily workspace infinite scroll.
        /// </summary>
        public PageParam? PageParam { get; set; }

        public bool ApplyDateOnSchedules { get; set; }

        public string? SearchText { get; set; }

        public bool IsShiftStartingWithin30Minutes { get; set; }

        public bool? IncludeCompleted { get; set; }
    }

    /// <summary>
    /// Unified requester row for workspace list APIs.
    /// </summary>
    public class RequesterWorkspaceListItemDto : RequesterSummaryBaseDto
    {
        public int RequestCount { get; set; }

        public List<string> ProjectNumbers { get; set; } = new();

        public bool HasUnpaidInvoice { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<RequesterWorkspaceRequestItemDto>? Requests { get; set; }
    }

    /// <summary>
    /// Request row under a requester for unified workspace list APIs.
    /// </summary>
    public class RequesterWorkspaceRequestItemDto
    {
        public Guid RequestId { get; set; }

        public Guid? RequesterId { get; set; }

        public string? Title { get; set; }

        public string? Name { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public int? FiscalYear { get; set; }

        public string? ProjectNumber { get; set; }

        public int PostCount { get; set; }

        public string ReleaseStatus { get; set; } = string.Empty;

        public RequestTypeDto? RequestType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public RequesterWorkspaceRequestScheduleDto? RequestSchedule { get; set; }

        public List<RequesterWorkspaceRequestScheduleDto> RequestSchedules { get; set; } = new();
    }

    /// <summary>
    /// Schedule window for unified workspace request rows.
    /// </summary>
    public class RequesterWorkspaceRequestScheduleDto
    {
        public Guid RequestScheduleID { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public RequestStatus RequestStatus { get; set; }

        public ReleaseAs ReleaseAs { get; set; }

        public string ReleaseStatus { get; set; } = string.Empty;
    }
}
