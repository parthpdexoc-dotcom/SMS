using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;
using SMS.Filters;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing department divisions operations.
    /// GET list/ById: DepartmentAdmin, HRAdmin, SuperUser. Create/Update/Delete: DepartmentAdminOnly (DepartmentAdmin, SuperUser).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentDivisionsController : ControllerBase
    {
        private readonly IDepartmentDivisionsService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentDivisionsController"/> class.
        /// </summary>
        /// <param name="service">The department divisions service.</param>
        public DepartmentDivisionsController(IDepartmentDivisionsService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all department divisions, optionally filtered by department.
        /// </summary>
        /// <param name="departmentId">When set, only divisions for this department are returned.</param>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <param name="isActive">When true, only active; when false, only inactive; when omitted, no status filter.</param>
        /// <returns>Paged department divisions.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<DepartmentDivisionsDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.OperatorOnly), nameof(AuthorizationPolicyType.DepartmentAdminOrHRAdmin), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        public async Task<ActionResult<PagedList<DepartmentDivisionsDto>>> GetAllAsync(
            [FromQuery] Guid? departmentId = null,
            [FromQuery] PageParam? pagination = null,
            [FromQuery] bool? isActive = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(DepartmentDivisionsDto.DivisionShort);
            }

            var result = await _service.GetAllPagedAsync(x =>
                (!departmentId.HasValue || x.DepartmentID == departmentId.Value) &&
                (!isActive.HasValue || x.DivisionActive == isActive.Value),
                pagination);
            return Ok(result);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<DepartmentDivisionsDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.OperatorOnly), nameof(AuthorizationPolicyType.DepartmentAdminOrHRAdmin), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        public async Task<ActionResult<PagedList<DepartmentDivisionsDto>>> SearchAsync(
            [FromQuery] string? searchTerm,
            [FromQuery] bool? isActive,
            [FromQuery] PageParam? pageParam = null)
        {
            pageParam ??= new PageParam();

            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
            {
                pageParam.SortBy = nameof(DepartmentDivisionsDto.DivisionShort);
            }

            var result = await _service.SearchAsync(
                searchTerm,
                pageParam,
                isActive.HasValue ? x => x.DivisionActive == isActive.Value : null
            );

            return Ok(result);
        }

        /// <summary>
        /// Gets all active department divisions, optionally filtered by department.
        /// Also includes any explicitly selected division IDs (even if inactive) for edit-screen dropdown binding.
        /// </summary>
        [HttpGet("GetAllActiveDepartmentDivision")]
        [ProducesResponseType(typeof(IEnumerable<DepartmentDivisionsDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.OperatorOnly), nameof(AuthorizationPolicyType.DepartmentAdminOrHRAdmin), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        public async Task<ActionResult<IEnumerable<DepartmentDivisionsDto>>> GetAllActiveDepartmentDivisionAsync(
            [FromQuery] Guid? departmentId = null,
            [FromQuery] Guid? selectedId = null)
        {
            var items = await _service.GetAllAsync(
                x => (!departmentId.HasValue || x.DepartmentID == departmentId.Value) &&
                     (x.DivisionActive == true || (selectedId.HasValue && x.DivisionID == selectedId.Value)),
                null,
                pageParam: null);

            return Ok(items.OrderBy(x => x.DivisionShort));
        }

        /// <summary>
        /// Gets a department division by ID.
        /// </summary>
        /// <param name="id">The division ID (DivisionID as Guid).</param>
        /// <returns>The department division if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DepartmentDivisionsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOrHRAdmin))]
        public async Task<ActionResult<DepartmentDivisionsDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Department division with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new department division.
        /// </summary>
        /// <param name="dto">The department division data to create.</param>
        /// <returns>The created department division.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(DepartmentDivisionsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<DepartmentDivisionsDto>> UpsertAsync([FromBody] DepartmentDivisionsDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing department division.
        /// </summary>
        /// <param name="id">The division ID (DivisionID as Guid).</param>
        /// <param name="dto">The department division data to update.</param>
        /// <returns>The updated department division.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(DepartmentDivisionsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<DepartmentDivisionsDto>> UpdateAsync(Guid id, [FromBody] DepartmentDivisionsDto dto)
        {

            if (dto.DivisionID.HasValue && id != dto.DivisionID.Value)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"Department division with ID {id} not found.");
            }

            // Ensure the DTO has the ID from the URL
            dto.DivisionID = id;

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a department division by ID.
        /// </summary>
        /// <param name="id">The division ID (DivisionID as Guid).</param>
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
                return NotFound($"Department division with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

