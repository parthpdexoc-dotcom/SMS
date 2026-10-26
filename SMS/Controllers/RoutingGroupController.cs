using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;
using SMS.Generic.Services.Extensions;
using SMS.Filters;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing requester operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RoutingGroupController : ControllerBase
    {
        private readonly IRoutingGroupService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingGroupController"/> class.
        /// </summary>
        /// <param name="service">The requester service.</param>
        public RoutingGroupController(IRoutingGroupService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all routing groups.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <param name="isActive">When true, only active; when false, only inactive; when omitted, no status filter.</param>
        /// <returns>Paged routing groups.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<RoutingGroupDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.OperatorOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<ActionResult<PagedList<RoutingGroupDto>>> GetAllAsync(
            [FromQuery] PageParam? pagination = null,
            [FromQuery] bool? isActive = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(RoutingGroupDto.ApproverShort);
            }

            var result = await _service.GetAllPagedAsyncWithIsActive(
                isActive,
                pagination,
                x => x.ApproverStatus == true,
                x => x.ApproverStatus != true);
            return Ok(result);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<RoutingGroupDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.OperatorOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<PagedList<RoutingGroupDto>>> SearchAsync(
            [FromQuery] string? searchTerm,
            [FromQuery] bool? isActive,
            [FromQuery] PageParam? pageParam = null)
        {
            pageParam ??= new PageParam();

            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
            {
                pageParam.SortBy = nameof(RoutingGroupDto.ApproverShort);
            }

            var result = await _service.SearchAsync(
                searchTerm,
                pageParam,
                isActive.HasValue ? x => x.ApproverStatus == isActive.Value : null
            );

            return Ok(result);
        }

        /// <summary>
        /// Gets routing groups by department ID and active status (status = 1).
        /// </summary>
        /// <param name="departmentId">The department ID to filter by.</param>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of routing groups filtered by department ID and active status.</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<RoutingGroupDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<IEnumerable<RoutingGroupDto>>> GetByDepartmentAndStatusAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetByDepartmentAndStatusAsync( pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets all routing groups with employee first names mapped from EmployeeIDs (from employee service).
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>Paged routing group details including EmployeeFirstNames.</returns>
        [HttpGet("getallroutinggroupdetails")]
        [ProducesResponseType(typeof(PagedList<RoutingGroupDetailsDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<PagedList<RoutingGroupDetailsDto>>> GetAllRoutingGroupDetailsAsync([FromQuery] PageParam? pagination = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(RoutingGroupDto.ApproverShort);
            }

            var items = await _service.GetAllRoutingGroupDetailsAsync(pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a requester by ID.
        /// </summary>
        /// <param name="id">The requester ID (RequesterID as Guid).</param>
        /// <returns>The requester if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RoutingGroupDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<RoutingGroupDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Requester with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new requester.
        /// </summary>
        /// <param name="dto">The requester data to create.</param>
        /// <returns>The created requester.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RoutingGroupDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<RoutingGroupDto>> UpsertAsync([FromBody] RoutingGroupDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }


        /// <summary>
        /// Deletes a requester by ID.
        /// </summary>
        /// <param name="id">The requester ID (RequesterID as Guid).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Requester with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

