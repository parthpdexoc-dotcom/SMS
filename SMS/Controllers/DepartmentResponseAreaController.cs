using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;
using SMS.Generic.Services.Extensions;
using SMS.Filters;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing response area operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentResponseAreaController : ControllerBase
    {
        private readonly IDepartmentResponseAreaService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentResponseAreaController"/> class.
        /// </summary>
        /// <param name="service">The response area service.</param>
        public DepartmentResponseAreaController(IDepartmentResponseAreaService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all response areas.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <param name="isActive">When true, only active; when false, only inactive; when omitted, no status filter.</param>
        /// <returns>Paged response areas.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<DepartmentResponseAreaDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<ActionResult<PagedList<DepartmentResponseAreaDto>>> GetAllAsync(
            [FromQuery] PageParam? pagination = null,
            [FromQuery] bool? isActive = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(DepartmentResponseAreaDto.DepartmentResponseAreaShort);
            }

            var result = await _service.GetAllPagedAsyncWithIsActive(
                isActive,
                pagination,
                x => x.DepartmentResponseAreaStatus == true,
                x => x.DepartmentResponseAreaStatus != true);
            return Ok(result);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<DepartmentResponseAreaDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<PagedList<DepartmentResponseAreaDto>>> SearchAsync(
            [FromQuery] string? searchTerm,
            [FromQuery] bool? isActive,
            [FromQuery] PageParam? pageParam = null)
        {
            pageParam ??= new PageParam();

            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
            {
                pageParam.SortBy = nameof(DepartmentResponseAreaDto.DepartmentResponseAreaShort);
            }

            var result = await _service.SearchAsync(
                searchTerm,
                pageParam,
                isActive.HasValue ? x => x.DepartmentResponseAreaStatus == isActive.Value : null
            );

            return Ok(result);
        }

        /// <summary>
        /// Gets lightweight department response area options for dropdowns.
        /// </summary>
        [HttpPost("departmentresponseareaoptions")]
        [ProducesResponseType(typeof(IReadOnlyList<SelectOptionDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<IReadOnlyList<SelectOptionDto>>> GetDepartmentResponseAreaOptionsAsync([FromBody] SelectOptionsRequestDto? body)
        {
            var request = SelectOptionsRequestDto.OrDefault(body);
            var selectedId = request.SelectedId;
            return Ok(await _service.GetOptionsAsync(
                x => x.DepartmentResponseAreaShort!,
                x => x.DepartmentResponseAreaStatus == true ||
                     (selectedId.HasValue && x.DepartmentResponseAreaID == selectedId.Value),
                request.SearchTerm));
        }

        /// <summary>
        /// Gets response areas by department ID from the token.
        /// </summary>
        /// <returns>A collection of response areas for the department from the token.</returns>
        [HttpGet("GetAllActiveDepartment")]
        [ProducesResponseType(typeof(IEnumerable<DepartmentResponseAreaDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<IEnumerable<DepartmentResponseAreaDto>>> GetByDepartmentIdAsync([FromQuery] Guid? selectedId = null)
        {
            var items = await _service.GetAllActiveDepartmentAsync(selectedId);
            return Ok(items);
        }

        /// <summary>
        /// Gets a response area by ID.
        /// </summary>
        /// <param name="id">The response area ID.</param>
        /// <returns>The response area if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DepartmentResponseAreaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<DepartmentResponseAreaDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"DepartmentResponseArea with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new response area.
        /// </summary>
        /// <param name="dto">The response area data to create.</param>
        /// <returns>The created response area.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(DepartmentResponseAreaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<DepartmentResponseAreaDto>> UpsertAsync([FromBody] DepartmentResponseAreaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }


        /// <summary>
        /// Deletes a response area by ID.
        /// </summary>
        /// <param name="id">The response area ID.</param>
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
                return NotFound($"DepartmentResponseArea with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

