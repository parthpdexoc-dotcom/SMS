using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Filters;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;
using SMS.Generic.Services.Extensions;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing equipment type operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EquipmentTypeController : ControllerBase
    {
        private readonly IEquipmentTypeService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentTypeController"/> class.
        /// </summary>
        /// <param name="service">The equipment type service.</param>
        public EquipmentTypeController(IEquipmentTypeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all equipment types (paged).
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <param name="isActive">When true, only active; when false, only inactive; when omitted, no status filter.</param>
        /// <returns>Paged equipment types.</returns>
        [HttpGet]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [ProducesResponseType(typeof(PagedList<EquipmentTypeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<EquipmentTypeDto>>> GetAllAsync(
        [FromQuery] PageParam? pagination = null,
        [FromQuery] bool? isActive = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(EquipmentTypeDto.EquipementTypeShort);
            }

            var result = await _service.GetAllPagedAsyncWithIsActive(
                isActive,
                pagination,
                x => x.EquipementTypeStatus == true,
                x => x.EquipementTypeStatus != true);
            return Ok(result);
        }

        [HttpGet("search")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [ProducesResponseType(typeof(PagedList<EquipmentTypeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<EquipmentTypeDto>>> SearchAsync(
            [FromQuery] string? searchTerm,
            [FromQuery] bool? isActive,
            [FromQuery] PageParam? pageParam = null)
        {
            pageParam ??= new PageParam();

            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
            {
                pageParam.SortBy = nameof(EquipmentTypeDto.EquipementTypeShort);
            }

            var result = await _service.SearchAsync(
                searchTerm,
                pageParam,
                isActive.HasValue ? x => x.EquipementTypeStatus == isActive.Value : null
            );

            return Ok(result);
        }
        [HttpGet("GetAllActiveequipment")]
        [AuthorizeAny(nameof(AuthorizationPolicyType.DepartmentAdminOnly), nameof(AuthorizationPolicyType.OperatorOnly))]
        [ProducesResponseType(typeof(IEnumerable<EquipmentTypeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EquipmentTypeDto>>> GetAllActiveequipmentAsync(
        [FromQuery] PageParam? pagination = null,
        [FromQuery] Guid? selectedId = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(EquipmentTypeDto.EquipementTypeShort);
            }
            // When isActive is not provided, default to true (active only). When false, get all.
            var items = await _service.GetAllAsync(
                x => x.EquipementTypeStatus == true || (selectedId.HasValue && x.EquipementTypeID == selectedId.Value),
                null,
                pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an equipment type by ID.
        /// </summary>
        /// <param name="id">The equipment type ID (EquipementTypeID as Guid).</param>
        /// <returns>The equipment type if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [ProducesResponseType(typeof(EquipmentTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EquipmentTypeDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Equipment type with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new equipment type.
        /// </summary>
        /// <param name="dto">The equipment type data to create.</param>
        /// <returns>The created equipment type.</returns>
        [HttpPost]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [ProducesResponseType(typeof(EquipmentTypeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EquipmentTypeDto>> UpsertAsync([FromBody] EquipmentTypeDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing equipment type.
        /// </summary>
        /// <param name="id">The equipment type ID (EquipementTypeID as Guid).</param>
        /// <param name="dto">The equipment type data to update.</param>
        /// <returns>The updated equipment type.</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [ProducesResponseType(typeof(EquipmentTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EquipmentTypeDto>> UpdateAsync(Guid id, [FromBody] EquipmentTypeDto dto)
        {

            if (dto.EquipementTypeID.HasValue && id != dto.EquipementTypeID.Value)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"Equipment type with ID {id} not found.");
            }

            // Ensure the DTO has the ID from the URL
            dto.EquipementTypeID = id;

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an equipment type by ID.
        /// </summary>
        /// <param name="id">The equipment type ID (EquipementTypeID as Guid).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Equipment type with ID {id} not found.");
            }
            return NoContent();
        }
    }
}


