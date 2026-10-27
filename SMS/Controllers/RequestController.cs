using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Common.Implementations.Responses;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Mapping;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;
using SMS.Filters;
using SMS.Generic.Api;
using SMS.Generic.Models.Constants;
using static SMS.Generic.Dtos.Dtos.PostDto;
using SMS.Generic.Dtos.Extensions;
using static SMS.Generic.Dtos.Dtos.RosterDto;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing request operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]    
    public class RequestController : BaseController
    {
        private readonly IRequestService _service;
        private readonly ILogger<RequestController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestController"/> class.
        /// </summary>
        /// <param name="service">The request service.</param>
        public RequestController(IRequestService service, ILogger<RequestController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Gets all requests.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of requests.</returns>
        [AuthorizeAny(
            nameof(AuthorizationPolicyType.RequesterManagerOnly),
            nameof(AuthorizationPolicyType.EmployeesViewManage),
            nameof(AuthorizationPolicyType.CommCenterOrDepartmentAdmin),
            nameof(AuthorizationPolicyType.IAAuditViewOnly),
            nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequestDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequestDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(RequestDto.StartDate);
            }

            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets requests for a specific requester (for parent request dropdown).
        /// </summary>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpGet("by-requester/{requesterId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<RequestDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequestDto>>> GetByRequesterIdAsync(
            Guid requesterId,
            [FromQuery] PageParam? pagination = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(RequestDto.StartDate);
            }

            var items = await _service.GetAllAsync(r => r.RequesterID == requesterId, null, pagination);
            return Ok(items);
        }


        /// <summary>
        /// Unified requester workspace list. Daily workspace (<c>isDailyWorkspace: true</c>) returns paged summary rows
        /// (<c>totalRecords</c> + one page in <c>data</c>); nested <c>requests</c> appear only when <c>searchText</c> is set.
        /// </summary>
        //[AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.IAAuditViewOnly))]
        //[HttpPost("requester-workspace-list")]
        //[ProducesResponseType(typeof(PagedList<RequesterWorkspaceListItemDto>), StatusCodes.Status200OK)]
        //public async Task<ActionResult<PagedList<RequesterWorkspaceListItemDto>>> GetRequesterWorkspaceListAsync(
        //    [FromBody] RequesterWorkspaceListFilterDto? filter,
        //    CancellationToken cancellationToken)
        //{
        //    filter ??= new RequesterWorkspaceListFilterDto();
        //    var pageParam = filter.PageParam ?? new PageParam();
        //    var traceId = HttpContext.TraceIdentifier;
        //    var sw = Stopwatch.StartNew();

        //    _logger.LogInformation(
        //        "RequesterWorkspaceList request TraceId={TraceId} IsDailyWorkspace={IsDailyWorkspace} Page={Page} PageSize={PageSize} " +
        //        "StartDate={StartDate} EndDate={EndDate} RequesterName={RequesterName} HasSearchText={HasSearchText} " +
        //        "ShiftWindow30Min={ShiftWindow30Min} IncludeCompleted={IncludeCompleted}",
        //        traceId,
        //        filter.IsDailyWorkspace,
        //        pageParam.Page,
        //        pageParam.PageSize,
        //        filter.StartDate,
        //        filter.EndDate,
        //        filter.RequesterName,
        //        !string.IsNullOrWhiteSpace(filter.SearchText),
        //        filter.IsShiftStartingWithin30Minutes,
        //        filter.IncludeCompleted);

        //    try
        //    {
        //        var result = await _service.GetRequesterWorkspaceListAsync(filter, cancellationToken);
        //        sw.Stop();

        //        var rowCount = result.Data?.Count ?? 0;
        //        var nestedRequestCount = result.Data?.Sum(r => r.Requests?.Count ?? 0) ?? 0;

        //        _logger.LogInformation(
        //            "RequesterWorkspaceList response TraceId={TraceId} ElapsedMs={ElapsedMs} TotalRecords={TotalRecords} " +
        //            "DataRows={DataRows} NestedRequests={NestedRequests}",
        //            traceId,
        //            sw.ElapsedMilliseconds,
        //            result.TotalRecords,
        //            rowCount,
        //            nestedRequestCount);

        //        if (sw.ElapsedMilliseconds >= 5000)
        //        {
        //            _logger.LogWarning(
        //                "RequesterWorkspaceList slow TraceId={TraceId} ElapsedMs={ElapsedMs} IsDailyWorkspace={IsDailyWorkspace} TotalRecords={TotalRecords}",
        //                traceId,
        //                sw.ElapsedMilliseconds,
        //                filter.IsDailyWorkspace,
        //                result.TotalRecords);
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        sw.Stop();
        //        _logger.LogError(
        //            ex,
        //            "RequesterWorkspaceList failed TraceId={TraceId} ElapsedMs={ElapsedMs} IsDailyWorkspace={IsDailyWorkspace}",
        //            traceId,
        //            sw.ElapsedMilliseconds,
        //            filter.IsDailyWorkspace);
        //        throw;
        //    }
        //}

        /// <summary>
        /// Gets requests for a requester with optional date, type, status, and release-type filters.
        /// Set <c>applyDateOnSchedules</c> to apply the date range to schedule dates and return only those schedule rows.
        /// </summary>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpPost("requester-requests")]
        [ProducesResponseType(typeof(RequesterRequestsResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedList<RequestInRequesterListDto>>> GetRequesterRequestsAsync(
            [FromBody] RequesterRequestsFilterDto filter,
            CancellationToken cancellationToken)
        {
            if (filter.RequesterId == Guid.Empty)
                return BadRequest("RequesterId is required.");

            var result = await _service.GetRequestsByRequesterFilterAsync(filter, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets requests for a requester along with respective jobs and assigned employees' latest latitude/longitude,
        /// resolving current department date, active schedules, and jobs/shifts starting within 30 minutes before or after current department time.
        /// </summary>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpPost("requester-requests-with-locations")]
        [ProducesResponseType(typeof(RequesterRequestsResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedList<RequestInRequesterListDto>>> GetRequesterRequestsWithLocationsAsync(
            [FromQuery] Guid requesterId,
            CancellationToken cancellationToken)
        {
            if (requesterId == Guid.Empty)
                return BadRequest("RequesterId is required.");

            var result = await _service.GetRequestsWithLocationsByRequesterIdAsync(requesterId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets request data for invoice by project number and date range. Project number format: year + number (e.g. 202600001).
        /// Returns InvoiceDto list with billing fields populated (BillingTo* from requester billing fields; BillingFrom* from department fields).
        /// Placeholder post positions are included only when <c>enableInvoiceAndPayroll</c> is true on the assignment.
        /// </summary>
        /// <param name="projectNumber">Combined project numbers (e.g. 202600001 = fiscal year 2026 + project number 00001).</param>
        /// <param name="startDate">Start date filter.</param>
        /// <param name="endDate">End date filter.</param>
        /// <returns>InvoiceDto list with request-related fields populated from matching requests.</returns>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpGet("invoices/request-data")]
        [ProducesResponseType(typeof(SuccessResult<List<InvoiceDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SuccessResult<List<InvoiceDto>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessResult<List<InvoiceDto>>>> GetRequestDataForInvoice(
            [FromQuery] List<string> projectNumber,
            [FromQuery] DateOnly startDate,
            [FromQuery] DateOnly endDate)
        {
            var result = await _service.GetInvoicePreviewDataListAsync(projectNumber, startDate, endDate);
            if (result.Status == (int)HttpStatusCode.NotFound)
                return NotFound(result);

            if (result.Status == (int)HttpStatusCode.BadRequest)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves all available requests based on search criteria.
        /// By default uses the logged-in employee; when <paramref name="employeeId"/> is supplied
        /// and the caller has EmployeesViewManage roles, returns available jobs for that employee
        /// (same department), for supervisor Employee Jobs views.
        /// </summary>
        /// <param name="searchModel">Search and pagination parameters.</param>
        /// <param name="pagination">Optional pagination.</param>
        /// <param name="employeeId">Optional employee override for privileged callers.</param>
        /// <returns>A collection of available requests.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpGet("available-post")]
        [ProducesResponseType(typeof(PagedList<EmployeePostListingItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PagedList<EmployeePostListingItemDto>>> GetAvailableRequests(
            [FromQuery] SearchModel searchModel,
            [FromQuery] PageParam? pagination = null,
            [FromQuery] Guid? employeeId = null)
        {
            if (searchModel is null)
                return BadRequest("Search model cannot be null.");

            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(RequestDto.FirstName);
            }

            try
            {
                var result = await _service.GetAllAvailablePostRequest(searchModel, pagination, true, employeeId);
                return Ok(result.ToEmployeePostListingPaged(searchModel.ReleaseAs));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Retrieves available posts for the current department week (start day from DepartmentPolicy.OnDutyStartDayOfWeek).
        /// Returns a paged list in the same style as weekly post-position APIs.
        /// </summary>
        /// <param name="release">Release type filter (default = Open).</param>
        /// <param name="pagination">Optional pagination parameters.</param>
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpGet("available-post-weekly")]
        [ProducesResponseType(typeof(PagedList<EmployeePostListingItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<EmployeePostListingItemDto>>> GetAvailableRequestsWeekly(
            [FromQuery] ReleaseAs release = ReleaseAs.Open,
            [FromQuery] PageParam? pagination = null)
        {
            var result = await _service.GetWeeklyAvailablePostRequestAsync(release, pagination);
            return Ok(result.ToEmployeePostListingPaged(release));
        }

        /// <summary>
        /// View Details for SMSPostInformationModal (all tabs).
        /// Available Job / waiting list: <c>postId</c>, <c>requestId</c>, <c>isSpecialEvent</c> (omit <c>postPositionId</c>); optional <c>dateOfWork</c> for <c>availablePositions</c>.
        /// Special event: <c>isSpecialEvent</c> true with <c>requestId</c> sets <c>availablePositions</c> and <c>specialEventJobDetailsMessage</c> (pooled posts on the request).
        /// Scheduled / Ready-to-close: <c>postPositionId</c> (non-empty); optional <c>dateOfWork</c> overrides work date for slot count when applicable.
        /// When a post is resolved, includes <c>latestPostNote</c> (same as <c>GET api/PostNotes/{postId}/latest-notes</c> when a latest note exists).
        /// </summary>
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpGet("view-details")]
        [ProducesResponseType(typeof(ViewDetailsDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ViewDetailsDto>> GetViewDetailsAsync(
            [FromQuery] Guid? postPositionId = null,
            [FromQuery] Guid? postId = null,
            [FromQuery] Guid? requestId = null,
            [FromQuery] bool? isSpecialEvent = null,
            [FromQuery] DateOnly? dateOfWork = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _service.GetViewDetailsAsync(postPositionId, postId, requestId, isSpecialEvent, dateOfWork, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Takes / assigns a job for the logged-in employee, or for <paramref name="employeeId"/> when a
        /// privileged caller (EmployeesViewManage roles) acts on behalf of another same-department employee.
        /// </summary>
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpGet("assign-post-to-me")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> AssignToMe(
            Guid postId,
            DateOnly dateOfWork,
            bool confirmConflict = false,
            Guid? postPositionId = null,
            Guid? employeeId = null)
        {
            try
            {
                var result = await _service.AssignPostToMe(postId, dateOfWork, employeeId, confirmConflict, false, postPositionId);
                return this.GetStatusMessageOnly(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Bulk assigns post positions to multiple employees or placeholders.
        /// For placeholders, send <c>placeholders</c> with one object per row (placeholderId and/or firstName+lastName, contactNumber, employeeNumber, enableInvoiceAndPayroll).
        /// </summary>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpPost("bulk-assign-post-positions")]
        [ProducesResponseType(typeof(SuccessResult<List<BulkAssignPostPositionItemDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<SuccessResult<List<BulkAssignPostPositionItemDto>>>> BulkAssignPostPositions([FromBody] BulkAssignPostPositionRequestDto bulkAssignPostPositionRequestDto)
        {
            if (bulkAssignPostPositionRequestDto.IsPlaceholder)
            {
                var hasPlaceholder = bulkAssignPostPositionRequestDto.Placeholders?.Any(x =>
                    (x.PlaceholderID.HasValue && x.PlaceholderID.Value != Guid.Empty)
                    || (!string.IsNullOrWhiteSpace(x.FirstName) && !string.IsNullOrWhiteSpace(x.LastName))) == true;
                if (!hasPlaceholder)
                    return BadRequest("At least one placeholder is required when isPlaceholder is true (use placeholders[] with placeholderId and/or firstName, lastName, and enableInvoiceAndPayroll per row).");
            }
            else
            {
                if (bulkAssignPostPositionRequestDto.EmployeeIDs == null || !bulkAssignPostPositionRequestDto.EmployeeIDs.Any())
                    return BadRequest("Employee IDs list cannot be empty.");
            }

            if (bulkAssignPostPositionRequestDto.IsTransfer)
            {
                if (!bulkAssignPostPositionRequestDto.SourcePostPositionId.HasValue ||
                    bulkAssignPostPositionRequestDto.SourcePostPositionId.Value == Guid.Empty)
                    return BadRequest(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.TRANSFER_SOURCE_JOB_POSITION_REQUIRED));

                if (bulkAssignPostPositionRequestDto.IsPlaceholder
                    ? bulkAssignPostPositionRequestDto.Placeholders?.Count != 1
                    : bulkAssignPostPositionRequestDto.EmployeeIDs.Count != 1)
                    return BadRequest(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.TRANSFER_REQUIRES_SINGLE_EMPLOYEE));
            }

            var results = await _service.BulkUpsertPostPositionsAsync(bulkAssignPostPositionRequestDto);
            return Ok(results);
        }
        /// <summary>
        /// Gets available dates for post positions by post ID.
        /// </summary>
        /// <param name="postId">The post ID.</param>
        /// <returns>List of available dates.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpGet("post/{postId}/positions-dates")]
        [ProducesResponseType(typeof(List<DateOnly>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<DateOnly>>> GetPostPostitionsDatesByPostId(Guid postId)
        {
            var dates = await _service.GetPostPostitionsDatesByPostId(postId);
            return Ok(dates);
        }

        /// <summary>
        /// Returns assignable employees for a request based on requester-manager employee-sharing rules.
        /// For manager-created requests: creator manager employees + employees under managers with IsEmployeeShared enabled.
        /// For admin-created requests: returns all employees.
        /// </summary>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpGet("{requestId:guid}/assignable-employees")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAssignableEmployeesForRequestAsync(
            Guid requestId,
            [FromQuery] string? searchTerm = null)
        {
            var employees = await _service.GetAssignableEmployeesForRequestAsync(requestId, searchTerm);
            return Ok(employees);
        }

        /// <summary>
        /// Distinct request id/title options for the employee dashboard Request Title filter.
        /// Limited to requests with released schedules overlapping the inclusive date window
        /// derived from <paramref name="startDate"/> and <paramref name="numberOfDays"/>
        /// (same window as <c>GET available-post</c>).
        /// </summary>
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpGet("dashboard-request-titles")]
        [ProducesResponseType(typeof(IEnumerable<RequestFilterOptionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequestFilterOptionDto>>> GetDashboardRequestTitlesAsync(
            [FromQuery] DateOnly startDate,
            [FromQuery] int numberOfDays)
        {
            var items = await _service.GetRequestTitlesByDateRangeAsync(startDate, numberOfDays);
            return Ok(items);
        }

        /// <summary>
        /// Distinct requester id/title options for the employee dashboard Requester filter.
        /// Limited to requests with released schedules overlapping the inclusive date window
        /// derived from <paramref name="startDate"/> and <paramref name="numberOfDays"/>
        /// (same window as <c>GET available-post</c>).
        /// </summary>
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpGet("dashboard-requesters")]
        [ProducesResponseType(typeof(IEnumerable<RequestFilterOptionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequestFilterOptionDto>>> GetDashboardRequestersAsync(
            [FromQuery] DateOnly startDate,
            [FromQuery] int numberOfDays)
        {
            var items = await _service.GetRequestersByDateRangeAsync(startDate, numberOfDays);
            return Ok(items);
        }

        /// <summary>
        /// Same date-window requester logic as <c>dashboard-requesters</c>, returned as <see cref="SelectOptionDto"/>
        /// via generic <c>GetOptionsAsync</c> (<c>id</c>/<c>label</c>). Supports <see cref="SelectOptionsRequestDto.SearchTerm"/>.
        /// </summary>
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpPost("dashboard-requesteroptions")]
        [ProducesResponseType(typeof(IReadOnlyList<SelectOptionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<SelectOptionDto>>> GetDashboardRequesterOptionsAsync(
            [FromQuery] DateOnly startDate,
            [FromQuery] int numberOfDays,
            [FromBody] SelectOptionsRequestDto? body)
        {
            var request = SelectOptionsRequestDto.OrDefault(body);
            var items = await _service.GetDashboardRequesterOptionsAsync(startDate, numberOfDays, request.SearchTerm);
            return Ok(items);
        }

        /// <summary>
        /// Gets a request by ID.
        /// </summary>
        /// <param name="id">The request ID.</param>
        /// <returns>The request if found; otherwise, NotFound.</returns>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly),nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RequestDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequestDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Request with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new request.
        /// </summary>
        /// <param name="dto">The request data to create.</param>
        /// <returns>The created request.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpPost]
        [ProducesResponseType(typeof(RequestDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RequestOneTimeDateChangeConflictDto), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<RequestDto>> UpsertAsync([FromBody] RequestDto dto)
        {
            try
            {
                var createdItem = await _service.UpsertAsync(dto);
                return createdItem;
            }
            catch (WebApiException ex) when (ex.HttpStatusCode == HttpStatusCode.Conflict && ex.ResponseData != null)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.ResponseData);
            }
        }

        /// <summary>
        /// Clones an existing request by request ID.
        /// </summary>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpPost("clone-request")]
        [ProducesResponseType(typeof(RequestDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequestDto>> CloneRequestAsync([FromQuery] Guid requestId)
        {
            var cloned = await _service.CloneRequestAsync(requestId);
            return Ok(cloned);
        }

        /// <summary>
        /// Sends Vehicle Approval emails to the given approval groups. Uses VehicleApprovalEmail template with [FullName], [ApprovalLink], [RequestName].
        /// To = approval group email (from RoutingGroup), [ApprovalLink] = SystemUrl, [RequestName] = request's FirstName + LastName.
        /// </summary>
        /// <param name="request">RequestId and RoutingGroupId (ApproverID) to send to.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpPost("send-vehicle-approval-emails")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SendVehicleApprovalEmailsAsync([FromBody] SendApprovalEmailsRequestDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                return BadRequest("Request body is required.");
            if (request.RoutingGroupId == Guid.Empty)
                return BadRequest("RoutingGroupId is required.");
            try
            {
                await _service.SendApprovalEmailsAsync(request, cancellationToken);
                return NoContent();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a request by ID.
        /// </summary>
        /// <param name="id">The request ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound($"Request with ID {id} not found.");
            }
            return NoContent();
        }
        [HttpGet("roster-information")]
        public async Task<ActionResult<RosterResponseDto>> GetRosterInformation([FromQuery] Guid postId, [FromQuery] DateOnly dateOfWork)
        {
            var result = await _service.GetRosterInformationAsync(postId, dateOfWork);

            if (result == null || result.Tables.Count == 0)
                return NotFound("No roster information found.");

            return Ok(result);
        }

        [HttpGet("roster-information-by-request")]
        public async Task<ActionResult<List<RosterInformationDto>>> GetRosterInformationByRequest([FromQuery] Guid requestID, [FromQuery] DateOnly dateOfWork)
        {
            var result = await _service.GetRosterInformationByRequestAsync(requestID, dateOfWork);

            if (result == null || result.Tables.Count == 0)
                return NotFound("No roster information found.");

            return Ok(result);
        }

        /// <summary>
        /// Gets all calculated roster work dates for a request (schedules + recurrence).
        /// Unlike <c>post/{postId}/positions-dates</c>, this is request-scoped and is not filtered
        /// by open assignment slots.
        /// </summary>
        /// <param name="requestID">The request ID.</param>
        /// <returns>Distinct ascending list of work dates (<see cref="DateOnly"/>).</returns>
        [HttpGet("roster-dates")]
        [ProducesResponseType(typeof(List<DateOnly>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<DateOnly>>> GetRosterDates([FromQuery] Guid requestID)
        {
            if (requestID == Guid.Empty)
                return BadRequest("requestID is required.");

            var dates = await _service.GetRosterDatesAsync(requestID);
            return Ok(dates);
        }

        /// <summary>
        /// Open jobs dashboard: released Open-release requests, one row per post/work date with slot counts
        /// (open, filled, pending cancellation).
        /// Date range is inclusive from <paramref name="startDate"/> to <paramref name="endDate"/>.
        /// When there are no rows, <see cref="PagedList{T}.Error"/> is populated (same pattern as available-post).
        /// Pagination (optional): <c>page</c> and <c>pageSize</c> together, or <c>offset</c> and <c>limit</c>
        /// (see <see cref="PageParam"/>). Example:
        /// <c>GET .../open-jobs-dashboard/2026-04-01/2026-04-07?page=1&amp;pageSize=20&amp;searchTerm=nurse&amp;statusFilter=open</c>.
        /// Optional <paramref name="statusFilter"/>: comma-separated <c>open</c>, <c>filled</c>, and/or <c>cancels</c>
        /// — keeps a row if any selected status has a count &gt; 0.
        /// </summary>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpGet("open-jobs-dashboard/{startDate}/{endDate}")]
        [ProducesResponseType(typeof(PagedList<OpenJobsDashboardDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<OpenJobsDashboardDto>>> GetOpenJobsDashboardAsync(
            [FromRoute] DateOnly startDate,
            [FromRoute] DateOnly endDate,
            [FromQuery] PageParam? pagination = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? statusFilter = null,
            CancellationToken cancellationToken = default)
        {
            pagination ??= new PageParam();
            var rows = await _service.GetOpenJobsDashboardAsync(startDate, endDate, pagination, searchTerm, statusFilter, cancellationToken);
            return Ok(rows);
        }

        /// <summary>
        /// Tenure jobs dashboard grouped by request and date of work for grid display.
        /// <paramref name="DateOfWork"/> is generated dynamically from request schedules and recurrence
        /// within the inclusive range <paramref name="startDate"/> to <paramref name="endDate"/>.
        /// </summary>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpGet("tenure-jobs-dashboard/{startDate}/{endDate}")]
        [ProducesResponseType(typeof(PagedList<TenureJobsDashboardDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedList<TenureJobsDashboardDto>>> GetTenureJobsDashboardAsync(
            [FromRoute] DateOnly startDate,
            [FromRoute] DateOnly endDate,
            [FromQuery] PageParam? pagination = null,
            CancellationToken cancellationToken = default)
        {
            pagination ??= new PageParam();
            var rows = await _service.GetTenureJobsDashboardAsync(startDate, endDate, pagination, cancellationToken);
            return Ok(rows);
        }

        /// <summary>
        /// Assigns the tenure waiting list row identified by <see cref="ProcessTenureWaitingListForWaitingPostRequestDto.WaitingPostPositionId"/>:
        /// same steps as the scheduled job (policy, tenure stats for ordering, <see cref="IRequestService.CheckAvailablePostForJobAsync"/>, copy to post positions, audits, reorder).
        /// Request id is resolved from the waiting row's post; route does not include request id.
        /// </summary>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpPost("process-tenure-waiting-list-for-waiting-post")]
        [ProducesResponseType(typeof(RequestScheduleProcessingResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ProcessTenureWaitingListForWaitingPostManualAsync(
            [FromBody] Guid waitingPostPositionId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _service
                    .ProcessTenureWaitingListForWaitingPostManualAsync(waitingPostPositionId, cancellationToken)
                    .ConfigureAwait(false);
                return Ok(result);
            }
            catch (WebApiException ex)
            {
                return StatusCode((int)ex.HttpStatusCode, new { error = ex.ErrorCode, message = ex.ErrorMessage });
            }
        }

        /// <summary>
        /// Tenure express-interest list by post id and date of work.
        /// Returns only interested employees on the waiting list for that post on the given date.
        /// </summary>
        /// <example>GET .../interested-list/{postId}/2026-04-30</example>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpGet("interested-list/{postId:guid}/{dateOfWork}")]
        [ProducesResponseType(typeof(List<TenureInterestedEmployeeRowDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTenureExpressInterestDashboardAsync(
            Guid postId,
            DateOnly dateOfWork,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var dto = await _service.GetTenureExpressInterestDashboardAsync(postId, dateOfWork, cancellationToken);
                return Ok(dto);
            }
            catch (WebApiException ex)
            {
                return StatusCode((int)ex.HttpStatusCode, new { error = ex.ErrorCode, message = ex.ErrorMessage });
            }
        }
        [HttpGet("export")]
        public async Task<IActionResult> ExportRoster([FromQuery] Guid? requestId, [FromQuery] Guid? postId, [FromQuery] DateOnly dateOfWork)
        {
            if (!requestId.HasValue && !postId.HasValue)
                return BadRequest("Either requestId or postId is required.");

            var result = requestId.HasValue
                ? await _service.ExportRosterExcelAsync(requestId.Value, dateOfWork)
                : await _service.ExportRosterExcelByPostAsync(postId!.Value, dateOfWork);
            return File(result.FileBytes,ExcelConstants.ExcelContentType,result.FileName);
        }
    }
}

