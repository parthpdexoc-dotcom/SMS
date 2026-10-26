using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Api;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Common.Implementations.Responses;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing tenure waiting position operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class WaitingPostPositionController : BaseController
    {
        private readonly IWaitingPostPositionService _service;
        private readonly ILogger<WaitingPostPositionController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitingPostPositionController"/> class.
        /// </summary>
        /// <param name="service">The tenure waiting position service.</param>
        /// <param name="logger">The logger instance.</param>
        public WaitingPostPositionController(
            IWaitingPostPositionService service,
            ILogger<WaitingPostPositionController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Gets all tenure waiting positions.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of tenure waiting positions.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WaitingPostPositionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<WaitingPostPositionDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a tenure waiting position by ID.
        /// </summary>
        /// <param name="id">The tenure waiting position ID.</param>
        /// <returns>The tenure waiting position if found; otherwise, NotFound.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WaitingPostPositionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WaitingPostPositionDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"TenureWaitingPosition with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a tenure waiting position.
        /// </summary>
        /// <param name="dto">The tenure waiting position data.</param>
        /// <returns>The created or updated tenure waiting position.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpPost]
        [ProducesResponseType(typeof(WaitingPostPositionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<WaitingPostPositionDto>> UpsertAsync([FromBody] WaitingPostPositionDto dto)
        {
            var result = await _service.UpsertAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Gets tenure waiting positions for the currently logged-in employee only,
        /// filtered by one or more <see cref="ReleaseAs"/> values in the body (same pattern as
        /// <c>PostPosition/my-post-positions/by-statuses</c>). Body example: <c>[5]</c> or <c>[1, 5]</c>.
        /// </summary>
        /// <param name="releaseAs">Body: non-empty list of release-as values to include.</param>
        /// <param name="isViewWLHistory">Query: when true, includes all waiting-list rows; when false, rows from one week ago (department time) onward.</param>
        /// <param name="pagination">Query: optional pagination.</param>
        /// <returns>A paged list of tenure waiting positions for the logged-in employee.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpPost("my-tenure-waiting-positions")]
        [ProducesResponseType(typeof(PagedList<EmployeePostListingItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedList<EmployeePostListingItemDto>>> GetForLoggedInEmployeeAsync(
            [FromBody] List<ReleaseAs>? releaseAs,
            [FromQuery] bool isViewWLHistory = false,
            [FromQuery] PageParam? pagination = null)
        {
            try
            {
                if (releaseAs == null || releaseAs.Count == 0)
                    return BadRequest("At least one releaseAs value is required.");

                var result = await _service.GetForLoggedInEmployeeAsync(releaseAs, isViewWLHistory, pagination);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        /// <summary>
        /// Assigns a tenure/waiting interest for the logged-in employee, or for <paramref name="employeeId"/>
        /// when a privileged caller acts on behalf of another same-department employee.
        /// </summary>
        /// <param name="postId">The post ID.</param>
        /// <param name="dateOfWork">The date of work.</param>
        /// <param name="employeeId">Optional employee override for privileged callers.</param>
        /// <returns>Success result with status and message.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpGet("assign-tenure-position-to-me")]
        [ProducesResponseType(typeof(SuccessResult<WaitingPostPositionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> AssignTenurePositionToMe(
            Guid postId,
            DateOnly dateOfWork,
            Guid? employeeId = null)
        {
            try
            {
                var result = await _service.AssignWaitingPostPositiontoMeAsync(postId, dateOfWork, employeeId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Records a special-event waiting post position for a request and work date (no concrete post yet).
        /// Employee id defaults to the authenticated user when omitted.
        /// </summary>
        /// <param name="requestId">The request ID (must be a special-event request).</param>
        /// <param name="dateOfWork">The date of work.</param>
        /// <param name="employeeId">Optional employee id; when null, the token employee id is used.</param>
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpPost("special-event")]
        [ProducesResponseType(typeof(SuccessResult<WaitingPostPositionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> ApplySpecialEventAsync(
            [FromQuery] Guid requestId,
            [FromQuery] DateOnly dateOfWork,
            [FromQuery] Guid? employeeId = null)
        {
            try
            {
                var result = await _service.ApplySpecialEventAsync(requestId, dateOfWork, employeeId);
                return this.GetStatusMessageOnly(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Updates the waiting post position status for the currently logged-in employee.
        /// </summary>
        /// <param name="waitingPostPositionId">The waiting post position ID.</param>
        /// <param name="status">The target status.</param>
        /// <returns>Update result payload.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpPut("{waitingPostPositionId:guid}/status/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateWaitingPostPositionStatusAsync(Guid waitingPostPositionId, PostPositionStatus status)
        {
            try
            {
                var updatedCount = await _service.UpdateStatusByIdAsync(waitingPostPositionId, status);

                return Ok(new
                {
                    updated = updatedCount,
                    status = status.ToString(),
                    waitingPostPositionId
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    error = "Unauthorized to update waiting post position status",
                    message = ex.Message,
                    waitingPostPositionId,
                    requestedStatus = status.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update waiting post position status for {WaitingPostPositionId} to {Status}", waitingPostPositionId, status);
                return BadRequest(new
                {
                    error = "Failed to update waiting post position status",
                    message = ex.Message,
                    waitingPostPositionId,
                    requestedStatus = status.ToString()
                });
            }
        }

        /// <summary>
        /// Gets active waiting-list employees for a post and date of work.
        /// </summary>
        /// <param name="postId">The post ID.</param>
        /// <param name="dateOfWork">The date of work.</param>
        /// <returns>A list of waiting-list rows with employee display fields.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpGet("waiting-list-employees")]
        [ProducesResponseType(typeof(List<WaitingPostPositionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<WaitingPostPositionResponseDto>>> GetWaitingListEmployeesAsync(
            [FromQuery] Guid postId,
            [FromQuery] DateOnly dateOfWork)
        {
            if (postId == Guid.Empty)
            {
                return BadRequest("postId is required.");
            }

            var result = await _service.GetWaitingListEmployee(postId, dateOfWork);
            return Ok(result);
        }

        /// <summary>
        /// Gets employee IDs who applied to a special-event request for a specific date.
        /// </summary>
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpGet("special-event-employee-ids")]
        [ProducesResponseType(typeof(List<WaitingPostPositionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<WaitingPostPositionResponseDto>>> GetSpecialEventEmployeeIdsAsync(
            [FromQuery] Guid requestId,
            [FromQuery] DateOnly dateOfWork)
        {
            if (requestId == Guid.Empty)
                return BadRequest("requestId is required.");

            var result = await _service.GetSpecialEventEmployeeIdsAsync(requestId, dateOfWork);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a tenure waiting position by ID.
        /// </summary>
        /// <param name="id">The tenure waiting position ID.</param>
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
                return NotFound($"TenureWaitingPosition with ID {id} not found.");
            }
            return Ok(new { message = "Record deleted successfully." });
        }
    }
}

