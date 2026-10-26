using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Filters;
using SMS.Generic.Common;
using SMS.Generic.Common.Constants;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Dtos.Mapping;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing post operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PostPositionController : ControllerBase
    {
        private readonly IPostPositionService _service;
        private readonly ILogger<PostPositionController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostPositionController"/> class.
        /// </summary>
        /// <param name="service">The post service.</param>
        /// <param name="logger">The logger instance.</param>
        public PostPositionController(IPostPositionService service, ILogger<PostPositionController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Gets all postpositions.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of postpositions.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PostPositionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PostPositionDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<PostPositionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<PostPositionDto>>> SearchAsync([FromQuery] string? searchTerm, [FromQuery] PageParam? pageParam = null)
        {
            pageParam ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
                pageParam.SortBy = nameof(PostPositionDto.DateOfWork);

            var result = await _service.SearchAsync(searchTerm, pageParam);
            return Ok(result);
        }

        /// <summary>
        /// Officers for the comm center dashboard in one paged list:
        /// recently ended (last 30 minutes), currently working, and upcoming (next 8 hours).
        /// </summary>
        [HttpGet("officer-schedule-overview")]
        [ProducesResponseType(typeof(PagedList<OfficerScheduledAssignmentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<OfficerScheduledAssignmentDto>>> GetOfficerScheduleOverviewAsync(
            [FromQuery] string? searchTerm = null,
            [FromQuery] PageParam? pagination = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _service
                .GetOfficerScheduleOverviewAsync(searchTerm, pagination, cancellationToken)
                .ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Gets a postposition by ID.
        /// </summary>
        /// <param name="id">The postposition ID.</param>
        /// <returns>The post if found; otherwise, NotFound.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PostPositionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostPositionDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Post with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new post position.
        /// </summary>
        /// <param name="dto">The post position data to create.</param>
        /// <returns>The created post position.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpPost]
        [ProducesResponseType(typeof(PostPositionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PostPositionDto>> UpsertAsync([FromBody] PostPositionDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Deletes a postposition by ID.
        /// </summary>
        /// <param name="id">The postposition ID.</param>
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
                return NotFound($"PostPosition with ID {id} not found.");
            }
            return Ok(new { message = "Record deleted successfully." });
        }

        /// <summary>
        /// Gets a single post position with Post details by post position ID (PostTitle, PostLocation, RequestID, PayRate, PostLength, CallSign, etc.).
        /// </summary>
        /// <param name="postPositionId">The post position ID to fetch (required).</param>
        /// <returns>The post position with full post details, or 404 if not found.</returns>
        [HttpGet("with-post")]
        [ProducesResponseType(typeof(PostTypeResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostTypeResponseDto>> GetPostPositionWithPostDetailsByIdAsync(
            [FromQuery] Guid? postPositionId = null)
        {
            if (!postPositionId.HasValue || postPositionId.Value == Guid.Empty)
                return BadRequest("postPositionId is required.");
            var result = await _service.GetPostPositionWithPostDetailsByIdAsync(postPositionId.Value);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>Gets distinct post-position dates of work for a post in ascending order (earliest first).</summary>
        /// <param name="postId">The post identifier.</param>
        /// <param name="startDate">Optional inclusive start date filter on <see cref="PostPosition.DateOfWork"/>.</param>
        /// <param name="endDate">Optional inclusive end date filter on <see cref="PostPosition.DateOfWork"/>.</param>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpGet("by-post/{postId:guid}/dates")]
        [ProducesResponseType(typeof(List<DateOnly>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<DateOnly>>> GetCollectDatesForPostAsync(
            Guid postId,
            [FromQuery] DateOnly? startDate = null,
            [FromQuery] DateOnly? endDate = null,
            [FromQuery] List<RequestStatus>? requestStatuses = null,
            [FromQuery] bool includeCompleted = false,
            CancellationToken cancellationToken = default)
        {
            var dates = await _service.GetCollectDatesForPostAsync(
                postId,
                startDate,
                endDate,
                requestStatuses,
                includeCompleted,
                cancellationToken);
            return Ok(dates);
        }

        /// <summary>Gets post positions for a post with optional date and status filters.</summary>
        /// <param name="filter">Filter including postId and dates.</param>
        /// <param name="isViewHistory">When true (default), returns only Cancelled records. When false, excludes Cancelled records.</param>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpPost("by-post/filter")]
        [ProducesResponseType(typeof(RequestPostPositionsResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedList<PostPositionInRequestListDto>>> GetPostPositionsByPostFilterAsync(
            [FromBody] RequestPostPositionsFilterDto filter,
            [FromQuery] bool isViewHistory = true,
            CancellationToken cancellationToken = default)
        {
            if (filter.PostId == Guid.Empty)
                return BadRequest("PostId is required.");

            var result = await _service.GetPostPositionsByPostFilterAsync(filter, isViewHistory, cancellationToken);
            return Ok(result);
        }

        /// <summary>Gets post positions for a post, optionally filtered by specific dates of work.</summary>
        /// <param name="postId">The post identifier.</param>
        /// <param name="pagination">The pagination.</param>
        /// <param name="dates">Optional list of dates to filter by <see cref="PostPosition.DateOfWork"/>.</param>
        /// <param name="isViewHistory">When true (default), returns only Cancelled records. When false, excludes Cancelled records.</param>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpGet("by-post/{postId:guid}")]
        [ProducesResponseType(typeof(PostPositionsByPostResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<PostPositionsByPostResponseDto>> GetPostPositionByPostIdAsync(
            Guid postId,
            [FromQuery] PageParam? pagination = null,
            [FromQuery] List<DateOnly>? dates = null,
            [FromQuery] List<RequestStatus>? requestStatuses = null,
            [FromQuery] bool includeCompleted = false,
            [FromQuery] bool isViewHistory = true)
        {
            var result = await _service.GetPostPositionsWithEmployeeDetailsAsync(
                postId,
                pagination,
                dates,
                requestStatuses,
                includeCompleted,
                isViewHistory);
            return Ok(result);
        }

        /// <summary>
        /// Returns assigned employees (post positions with status Applied/Cancelled/Closed) and active waiting-list applicants.
        /// Optional filter: provide <paramref name="startDate"/> to filter by date window.
        /// If <paramref name="numberOfDays"/> is omitted, defaults to 15 days from startDate.
        /// If startDate is omitted, all dates are returned for the post.
        /// </summary>
        /// <param name="postId">The post ID.</param>
        /// <param name="startDate">Optional window start date.</param>
        /// <param name="numberOfDays">Optional window length in days.</param>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpGet("by-post/{postId:guid}/assignments-and-waiting-list")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<object>> GetAssignmentsAndWaitingListForDateAsync(
            Guid postId,
            [FromQuery] DateOnly? startDate,
            [FromQuery] int? numberOfDays)
        {
            var result = await _service.GetAssignmentsAndWaitingList(postId, startDate, numberOfDays);
            return Ok(new
            {
                postPositions = result.PostPositions,
                waitingPostPositions = result.WaitingPostPositions
            });
        }

        /// <summary>
        /// Returns post positions for an employee filtered by one or more statuses.
        /// By default, uses the logged-in employee; when <paramref name="employeeId"/> is supplied
        /// and the caller has appropriate admin roles, returns positions for that employee instead.
        /// Same logic as single-status: when statuses include Applied, isOngoing controls ongoing vs scheduled.
        /// </summary>
        /// <param name="statuses">Body: array of statuses (required), e.g. [1, 2].</param>
        /// <param name="isOngoing">Query: when true and statuses include Applied, returns only ongoing; when false, scheduled only.</param>
        /// <param name="pagination">Query: optional pagination.</param>
        /// <param name="employeeId">
        /// Query: optional employee ID override. When provided and the caller is authorized (DepartmentAdmin, HRAdmin, SuperUser),
        /// returns post positions for this employee instead of the authenticated employee.
        /// </param>
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpPost("my-post-positions/by-statuses")]
        [ProducesResponseType(typeof(PagedList<EmployeePostListingItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PagedList<EmployeePostListingItemDto>>> GetForLoggedInEmployeeByStatusesAsync(
            [FromBody] List<PostPositionStatus>? statuses,
            [FromQuery] bool isOngoing = false,
            [FromQuery] PageParam? pagination = null,
            [FromQuery] Guid? employeeId = null,
            [FromQuery] SearchModel? searchModel = null)
        {
            try
            {
                if (statuses == null || statuses.Count == 0)
                    return BadRequest("At least one status is required.");

                var result = await _service.GetForLoggedInEmployeeByStatusesAsync(statuses, isOngoing, pagination, employeeId, searchModel: searchModel);
                return Ok(result.ToEmployeePostListingPaged());
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        /// <summary>
        /// Returns ongoing assigned posts across all employees (department scoped), with optional filters.
        /// Supports date range filtering, employee filtering, search by title/duration, requester filtering, and placeholder-only mode.
        /// </summary>
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpGet("job-ready-to-close")]
        [ProducesResponseType(typeof(PagedList<EmployeePostListingItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PagedList<EmployeePostListingItemDto>>> GetOngoingPostPositionsAsync(
            [FromQuery] PageParam? pagination = null,
            [FromQuery] Guid? employeeId = null,
            [FromQuery] DateOnly? startDate = null,
            [FromQuery] DateOnly? endDate = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool isPlaceholder = false,
            [FromQuery] Guid? requesterId = null)
        {
            if (!(User.IsInRole(RoleTypeNames.DepartmentAdmin())
                || User.IsInRole(RoleTypeNames.SuperUser())
                || User.IsInRole(RoleTypeNames.Operator())
                || User.IsInRole(RoleTypeNames.HRAdmin())
                || User.IsInRole(RoleTypeNames.FiscalAdmin())
                || User.IsInRole(RoleTypeNames.RequesterManager())
                || User.IsInRole(RoleTypeNames.CommCenter())))
                return Forbid();

            var result = await _service.GetOngoingPostPositionsAsync(
                pagination: pagination,
                employeeId: employeeId,
                startDate: startDate,
                endDate: endDate,
                searchTerm: searchTerm,
                useLoggedInEmployeeWhenEmployeeIdMissing: false,
                isPlaceholderOnly: isPlaceholder,
                requesterId: requesterId);

            return Ok(result.ToEmployeePostListingPaged());
        }

        /// <summary>
        /// Returns close assignment post positions with optional search.
        /// Fixed statuses: Closed (2), ClosedAndSplit (4), Unclosed (11).
        /// By default filters for the logged-in employee; when <paramref name="employeeId"/> is supplied
        /// and the caller has appropriate admin roles, returns positions for that employee instead.
        /// </summary>
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpGet("my-post-positions/close-assignments")]
        [ProducesResponseType(typeof(PagedList<PostTypeResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PagedList<PostTypeResponseDto>>> GetCloseAssignmentsForLoggedInEmployeeAsync(
            [FromQuery] string? searchTerm = null,
            [FromQuery] PageParam? pagination = null,
            [FromQuery] Guid? employeeId = null,
            [FromQuery] DateOnly? startDate = null,
            [FromQuery] DateOnly? endDate = null)
        {
            try
            {
                var result = await _service.GetCloseAssignmentsForLoggedInEmployeeAsync(
                    searchTerm,
                    pagination,
                    employeeId,
                    startDate,
                    endDate);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        /// <summary>Updates the post status asynchronous.</summary>
        /// <param name="postPositionId">The post position identifier.</param>
        /// <param name="status">The status.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage),nameof(AuthorizationPolicyType.RequesterManagerOnly),nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpPut("{postPositionId:guid}/status/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePostStatusAsync(Guid postPositionId, PostPositionStatus status)
        {
            try
            {
                var updatedCount = await _service.UpdateStatusByPostIdAsync(postPositionId, status);

                return Ok(new
                {
                    updated = updatedCount,
                    status = status.ToString(),
                    postPositionId = postPositionId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update job status for PostPositionId {PostPositionId} to {Status}", postPositionId, status);
                return BadRequest(new
                {
                    error = "Failed to update job status",
                    message = ex.Message,
                    postPositionId = postPositionId,
                    requestedStatus = status.ToString()
                });
            }
        }

        /// <summary>
        /// Cancels a post position (sets status to Cancelled), then sends a cancellation email to the current employee's primary email using the CancelledEmail template with [CancellationDate] and [CancellationTime] placeholders.
        /// </summary>
        /// <param name="postPositionId">The post position ID to cancel.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>OK with updated count and postPositionId; BadRequest or NotFound on failure.</returns>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpPut("{postPositionId:guid}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelPostPositionAsync(
            Guid postPositionId,
            [FromQuery] bool isApproveCancellation = false,
            [FromBody] Dictionary<string, string?>? request = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                string? reasonForCancel = null;
                if (request != null)
                {
                    if (request.TryGetValue("cancellationReason", out var cancellationReason) &&
                        !string.IsNullOrWhiteSpace(cancellationReason))
                    {
                        reasonForCancel = cancellationReason.Trim();
                    }
                    else if (request.TryGetValue("reason", out var reason) &&
                             !string.IsNullOrWhiteSpace(reason))
                    {
                        reasonForCancel = reason.Trim();
                    }
                }

                var updatedCount = await _service.CancelPostPositionAsync(
                    postPositionId,
                    reasonForCancel,
                    isApproveCancellation,
                    cancellationToken);
                return Ok(new
                {
                    updated = updatedCount,
                    status = isApproveCancellation
                        ? PostPositionStatus.Cancelled.ToString()
                        : PostPositionStatus.CancellationPending.ToString(),
                    postPositionId = postPositionId
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    error = ex.Message,
                    postPositionId = postPositionId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel job position {PostPositionId}", postPositionId);
                return BadRequest(new
                {
                    error = "Failed to cancel job position",
                    message = ex.Message,
                    postPositionId = postPositionId
                });
            }
        }

        /// <summary>
        /// Revokes a pending cancellation: restores the post position from CancellationPending to Assigned
        /// and clears the stored cancellation reason. Only valid while status is CancellationPending.
        /// </summary>
        /// <param name="postPositionId">The post position ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpPut("{postPositionId:guid}/revoke-cancellation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RevokeCancellationAsync(
            Guid postPositionId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var updatedCount = await _service.RevokeCancellationAsync(postPositionId, cancellationToken);
                return Ok(new
                {
                    updated = updatedCount,
                    status = PostPositionStatus.Assigned.ToString(),
                    postPositionId
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    error = ex.Message,
                    postPositionId
                });
            }
        }

        /// <summary>
        /// Declines a split post position and creates a new available post position.
        /// </summary>
        /// <param name="postPositionId">The declined post position ID.</param>
        /// <returns>The newly created post position.</returns>
         [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpPost("{postPositionId:guid}/decline-split")]
        [ProducesResponseType(typeof(PostPositionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeclineSplitAsync(Guid postPositionId)
        {
            try
            {
                var result = await _service.DeclineSplitAsync(postPositionId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to decline split job position {PostPositionId}", postPositionId);
                return BadRequest(new
                {
                    error = "Failed to decline split job position",
                    message = ex.Message,
                    postPositionId
                });
            }
        }

        /// <summary>Gets the employee dashboard counts asynchronous.</summary>
        /// <param name="searchModel">The search model.</param>
        /// <param name="pagination">The pagination.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpGet("dashboard/counts")]
        [ProducesResponseType(typeof(EmployeeDashboardCountsDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<EmployeeDashboardCountsDto>> GetEmployeeDashboardCountsAsync([FromQuery] SearchModel searchModel, PageParam? pagination = null)
        {
            try
            {
                var counts = await _service.GetEmployeeDashboardCountsAsync(searchModel, pagination);
                return Ok(counts);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get employee dashboard counts");
                return BadRequest(new
                {
                    error = "Failed to get dashboard counts",
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Gets weekly dashboard posts for the logged-in employee based on department week configuration.
        /// Reuses the same ongoing/scheduled logic as "my-post-positions" APIs.
        /// </summary>
        /// <param name="isOngoing">
        /// When true: returns only ongoing Applied/Transferred posts for the week.
        /// When false: returns only scheduled Applied/Transferred posts for the week.
        /// </param>
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpGet("dashboard/weekly")]
        [ProducesResponseType(typeof(PagedList<EmployeePostListingItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<EmployeePostListingItemDto>>> GetWeeklyDashboardPostsAsync(
            [FromQuery] bool isOngoing = false,
            [FromQuery] PageParam? pagination = null)
        {
            var result = await _service.GetWeeklyDashboardPostsAsync(isOngoing, pagination);
            return Ok(result.ToEmployeePostListingPaged());
        }

        /// <summary>
        /// Gets posts that were cancelled in the last 72 hours for the logged-in employee.
        /// </summary>
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpGet("dashboard/cancelled-recent")]
        [ProducesResponseType(typeof(PagedList<EmployeePostListingItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<EmployeePostListingItemDto>>> GetRecentCancelledPostsAsync([FromQuery] PageParam? pagination = null)
        {
            var result = await _service.GetRecentCancelledPostsAsync(pagination);
            return Ok(result.ToEmployeePostListingPaged());
        }

        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpPut("swap-callsign")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SwapCallSign(
            [FromQuery] Guid postPositionId1,
            [FromQuery] Guid postPositionId2)
        {
            await _service.SwapCallSignAsync(postPositionId1, postPositionId2);
            return Ok(new { postPositionId1, postPositionId2 });
        }

        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpPut("{postPositionId:guid}/updatecallsign")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCallSign(Guid postPositionId, [FromQuery] string fieldName, [FromQuery] string? fieldValue)
        {
            try
            {
                var updatedCount = await _service.UpdateCallSignAsync(postPositionId, fieldName, fieldValue);
                return Ok(new
                {
                    updated = updatedCount,
                    postPositionId = postPositionId,
                    fieldName = fieldName,
                    fieldValue = fieldValue
                });
            }
            catch (WebApiException ex)
            {
                return StatusCode((int)ex.HttpStatusCode, new
                {
                    error = ex.ErrorCode.ToString(),
                    message = ex.ErrorMessage,
                    postPositionId = postPositionId,
                    fieldName = fieldName,
                    fieldValue = fieldValue
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update job position {PostPositionId} field {FieldName}", postPositionId, fieldName);
                return BadRequest(new
                {
                    error = "Failed to update job position",
                    message = ex.Message,
                    postPositionId = postPositionId,
                    fieldName = fieldName,
                    fieldValue = fieldValue
                });
            }
        }


        /// <summary>
        /// Sends assign-post notification email for a post position using post position ID.
        /// Uses current authenticated employee from token.
        /// </summary>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpGet("send-assign-post-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendAssignPostEmailAsync(
            [FromQuery] Guid postPositionId,
            CancellationToken cancellationToken = default)
        {
            if (postPositionId == Guid.Empty)
                return BadRequest("Post position ID is required.");

            await _service.SendAssignPostEmailByPostPositionIdAsync(postPositionId, cancellationToken);

            return Ok(new { message = "Assign post email sent successfully." });
        }

        /// <summary>
        /// Gets today's jobs (mixed scheduled and ready to close) for an employee.
        /// Omit <paramref name="pagination"/> to return every matching job.
        /// </summary>
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpPost("todays-job")]
        [ProducesResponseType(typeof(PagedList<EmployeePostListingItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<EmployeePostListingItemDto>>> GetTodaysJobsAsync(
            [FromQuery] PageParam? pagination = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _service.GetTodaysJobsAsync(pagination, cancellationToken);
            return Ok(result);
        }

    }
}