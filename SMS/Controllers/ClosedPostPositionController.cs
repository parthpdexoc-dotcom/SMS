using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;
using SMS.Filters;
using System.Security;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing closed post position operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ClosedPostPositionController : ControllerBase
    {
        private readonly IClosedPostPositionService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClosedPostPositionController"/> class.
        /// </summary>
        /// <param name="service">The closed post position service.</param>
        public ClosedPostPositionController(IClosedPostPositionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all closed post position records.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of closed post position records.</returns>
        [HttpGet]

        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [ProducesResponseType(typeof(IEnumerable<ClosedPostPositionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClosedPostPositionDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets the latest closed post position row for each distinct post position assigned to the employee in the token, with pagination.
        /// By default uses the authenticated employee; when <paramref name="employeeId"/> is supplied and the caller is a department admin,
        /// returns positions for that employee instead (same pattern as <c>PostPosition/my-post-positions/by-statuses</c>).
        /// Ordering within a post position matches closed-details APIs (edited rows first, then by created date).
        /// </summary>
        /// <param name="pagination">Pagination (offset/limit or page/pageSize).</param>
        /// <param name="employeeId">Optional employee ID override for privileged callers.</param>
        /// <returns>A paged list including total distinct post positions for that employee.</returns>
        [HttpGet("latest-per-post-position")]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [ProducesResponseType(typeof(PagedList<ClosedPostPositionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PagedList<EmployeePostListingItemDto>>> GetLatestPerPostPositionPagedAsync(
            [FromQuery] PageParam? pagination = null,
            [FromQuery] Guid? employeeId = null)
        {
            try
            {
                var result = await _service.GetLatestPerPostPositionPagedAsync(pagination, employeeId);
                return Ok(result);
            }
            catch (SecurityException)
            {
                return Forbid();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        /// <summary>
        /// Returns close assignment rows from the closed post position table with optional search.
        /// Fixed <see cref="ClosedPostPositionStatus"/>: Closed (1) and Unclosed (2) on the closed post position row.
        /// By default does not filter to the logged-in employee when <paramref name="employeeId"/> is omitted;
        /// when it is set, restricts to that employee. Department scoping comes from the token.
        /// When <paramref name="isPlaceholder"/> is true, returns only placeholder post positions.
        /// When <paramref name="requesterId"/> is set, returns only rows for that job-level requester.
        /// </summary>
        [HttpGet("my-post-positions/close-assignments")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [ProducesResponseType(typeof(PagedList<CloseAssignmentClosedPostPositionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PagedList<CloseAssignmentClosedPostPositionResponseDto>>> GetCloseAssignmentsForLoggedInEmployeeAsync(
            [FromQuery] string? searchTerm = null,
            [FromQuery] PageParam? pagination = null,
            [FromQuery] Guid? employeeId = null,
            [FromQuery] DateOnly? startDate = null,
            [FromQuery] DateOnly? endDate = null,
            [FromQuery] bool isPlaceholder = false,
            [FromQuery] Guid? requesterId = null)
        {
            try
            {
                var result = await _service.GetCloseAssignmentsForLoggedInEmployeeAsync(
                    searchTerm,
                    pagination,
                    employeeId,
                    startDate,
                    endDate,
                    isPlaceholderOnly: isPlaceholder,
                    requesterId: requesterId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        /// <summary>
        /// Gets a closed post position record by ID.
        /// </summary>
        /// <param name="id">The closed post position ID (GUID).</param>
        /// <returns>The record if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [ProducesResponseType(typeof(ClosedPostPositionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClosedPostPositionDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Closed job position with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a closed post position record.
        /// </summary>
        /// <param name="dto">The closed post position data to create or update.</param>
        /// <returns>The created or updated record.</returns>
        [HttpPost]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [ProducesResponseType(typeof(ClosedPostPositionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ClosedPostPositionDto>> UpsertAsync([FromBody] ClosedPostPositionDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Deletes a closed post position record by ID.
        /// </summary>
        /// <param name="id">The closed post position ID (GUID).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Closed job position with ID {id} not found.");
            }
            return NoContent();
        }

    }
}
