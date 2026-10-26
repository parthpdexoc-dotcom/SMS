using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;
using SMS.Generic.Services.Extensions;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing department fee operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    public class DepartmentFeeController : ControllerBase
    {
        private readonly IDepartmentFeeService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentFeeController"/> class.
        /// </summary>
        /// <param name="service">The department fee service.</param>
        public DepartmentFeeController(IDepartmentFeeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all department fees.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <param name="isActive">When true, only active; when false, only inactive; when omitted, no status filter.</param>
        /// <returns>Paged department fees.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<DepartmentFeeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<DepartmentFeeDto>>> GetAllAsync(
            [FromQuery] PageParam? pagination = null,
            [FromQuery] bool? isActive = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(DepartmentFeeDto.FeeShort);
            }

            var result = await _service.GetAllPagedAsyncWithIsActive(
                isActive,
                pagination,
                x => x.IsActive,
                x => !x.IsActive);
            return Ok(result);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<DepartmentFeeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<DepartmentFeeDto>>> SearchAsync([FromQuery] string? searchTerm, [FromQuery] PageParam? pageParam = null)
        {
            pageParam ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
                pageParam.SortBy = nameof(DepartmentFeeDto.FeeShort);

            var result = await _service.SearchAsync(searchTerm, pageParam);
            return Ok(result);
        }

        /// <summary>
        /// Returns active department fees for the logged-in department, enriched with selection state.
        /// Used on both Create Request (omit requestId) and Edit Request (pass requestId).
        /// Create mode: isSelected = IsDefault, isExisting = false.
        /// Edit mode: isSelected = saved OR default, isExisting = true when fee is in RequestDepartmentFee.
        /// UI grid: show where isSelected = true. Dropdown: show where isSelected = false.
        /// </summary>
        [HttpGet("request-selection")]
        [ProducesResponseType(typeof(IReadOnlyList<ApplicableDepartmentFeeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ApplicableDepartmentFeeDto>>> GetForRequestSelectionAsync(
            [FromQuery] Guid? requestId = null)
        {
            var result = await _service.GetFeesForRequestSelectionAsync(requestId);
            return Ok(result);
        }

        /// <summary>
        /// Gets a department fee by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the department fee.</param>
        /// <returns>The department fee if found; otherwise, returns NotFound.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(DepartmentFeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DepartmentFeeDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"DepartmentFee with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new department fee or updates an existing one.
        /// </summary>
        /// <param name="dto">The department fee DTO to create or update.</param>
        /// <returns>The created or updated department fee.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(DepartmentFeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DepartmentFeeDto>> UpsertAsync([FromBody] DepartmentFeeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.UpsertAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a department fee by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the department fee to delete.</param>
        /// <returns>NoContent if successful; otherwise, returns NotFound.</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"DepartmentFee with ID {id} not found.");
            }

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
