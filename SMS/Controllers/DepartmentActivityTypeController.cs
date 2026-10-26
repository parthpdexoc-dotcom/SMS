using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Extensions;
using SMS.Filters;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing department activity type operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentActivityTypeController : ControllerBase
    {
        private readonly IDepartmentActivityTypeService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentActivityTypeController"/> class.
        /// </summary>
        /// <param name="service">The department activity type service.</param>
        public DepartmentActivityTypeController(IDepartmentActivityTypeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all department activity types.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <param name="isActive">When true, only active; when false, only inactive; when omitted, no status filter.</param>
        /// <returns>Paged department activity types.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<DepartmentActivityTypeDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<PagedList<DepartmentActivityTypeDto>>> GetAllAsync(
            [FromQuery] PageParam? pagination = null,
            [FromQuery] bool? isActive = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(DepartmentActivityTypeDto.ActivityTypeShort);
            }

            var result = await _service.GetAllPagedAsyncWithIsActive(
                isActive,
                pagination,
                x => x.ActivityTypeStatus == true,
                x => x.ActivityTypeStatus != true);
            return Ok(result);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<DepartmentActivityTypeDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<PagedList<DepartmentActivityTypeDto>>> SearchAsync(
            [FromQuery] string? searchTerm,
            [FromQuery] bool? isActive,
            [FromQuery] PageParam? pageParam = null)
        {
            pageParam ??= new PageParam();

            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
            {
                pageParam.SortBy = nameof(DepartmentActivityTypeDto.ActivityTypeShort);
            }

            var result = await _service.SearchAsync(
                searchTerm,
                pageParam,
                isActive.HasValue ? x => x.ActivityTypeStatus == isActive.Value : null
            );

            return Ok(result);
        }

        /// <summary>
        /// Gets lightweight department activity type options for dropdowns.
        /// </summary>
        [HttpPost("departmentactivitytypeoptions")]
        [ProducesResponseType(typeof(IReadOnlyList<SelectOptionDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<IReadOnlyList<SelectOptionDto>>> GetDepartmentActivityTypeOptionsAsync([FromBody] SelectOptionsRequestDto? body)
        {
            var request = SelectOptionsRequestDto.OrDefault(body);
            var selectedId = request.SelectedId;
            return Ok(await _service.GetOptionsAsync(
                x => x.ActivityTypeShort!,
                x => x.ActivityTypeStatus == true || (selectedId.HasValue && x.DepartmentActivityTypeId == selectedId.Value),
                request.SearchTerm));
        }

        /// <summary>
        /// Gets all active department activity types.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of active department activity types.</returns>
        [HttpGet("GetAllActiveDepartmentActivityType")]
        [ProducesResponseType(typeof(IEnumerable<DepartmentActivityTypeDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<ActionResult<IEnumerable<DepartmentActivityTypeDto>>> GetAllActiveDepartmentActivityTypeAsync(
            [FromQuery] PageParam? pagination = null,
            [FromQuery] Guid? selectedId = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(DepartmentActivityTypeDto.ActivityTypeShort);
            }

            var items = await _service.GetAllAsync(
                x => x.ActivityTypeStatus == true || (selectedId.HasValue && x.DepartmentActivityTypeId == selectedId.Value),
                null,
                pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a department activity type by ID.
        /// </summary>
        /// <param name="id">The department activity type ID.</param>
        /// <returns>The department activity type if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DepartmentActivityTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<DepartmentActivityTypeDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"DepartmentActivityType with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new department activity type.
        /// </summary>
        /// <param name="departmentActivityTypeDto">The department activity type data to create.</param>
        /// <returns>The created department activity type.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(DepartmentActivityTypeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<DepartmentActivityTypeDto>> UpsertAsync([FromBody] DepartmentActivityTypeDto departmentActivityTypeDto)
        {
            try
            {
                return await _service.UpsertAsync(departmentActivityTypeDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing department activity type.
        /// </summary>
        /// <param name="id">The department activity type ID.</param>
        /// <param name="departmentActivityTypeDto">The department activity type data to update.</param>
        /// <returns>The updated department activity type.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(DepartmentActivityTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<ActionResult<DepartmentActivityTypeDto>> UpdateAsync(Guid id, [FromBody] DepartmentActivityTypeDto departmentActivityTypeDto)
        {

            if (id != departmentActivityTypeDto.DepartmentActivityTypeId)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"DepartmentActivityType with ID {id} not found.");
            }

            try
            {
                var updatedItem = await _service.UpsertAsync(departmentActivityTypeDto);
                return Ok(updatedItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a department activity type by ID.
        /// </summary>
        /// <param name="id">The department activity type ID.</param>
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
                return NotFound($"DepartmentActivityType with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
