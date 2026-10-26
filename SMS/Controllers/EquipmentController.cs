using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Extensions;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing equipment operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentController"/> class.
        /// </summary>
        /// <param name="service">The equipment service.</param>
        public EquipmentController(IEquipmentService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all equipment.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <param name="isActive">When true, only active; when false, only inactive; when omitted, no status filter.</param>
        /// <returns>Paged equipment.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<EquipmentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<EquipmentDto>>> GetAllAsync(
            [FromQuery] PageParam? pagination = null,
            [FromQuery] bool? isActive = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(EquipmentDto.EquipementShort);
            }

            var result = await _service.GetAllPagedAsyncWithIsActive(
                isActive,
                pagination,
                x => x.EquipementStatus == true,
                x => x.EquipementStatus != true);
            return Ok(result);
        }

        /// <summary>
        /// Gets an equipment by ID.
        /// </summary>
        /// <param name="id">The equipment ID (EquipementID as Guid).</param>
        /// <returns>The equipment if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EquipmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EquipmentDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Equipment with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new equipment.
        /// </summary>
        /// <param name="dto">The equipment data to create.</param>
        /// <returns>The created equipment.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EquipmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EquipmentDto>> UpsertAsync([FromBody] EquipmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing equipment.
        /// </summary>
        /// <param name="id">The equipment ID (EquipementID as Guid).</param>
        /// <param name="dto">The equipment data to update.</param>
        /// <returns>The updated equipment.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EquipmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EquipmentDto>> UpdateAsync(Guid id, [FromBody] EquipmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dto.EquipementID.HasValue && id != dto.EquipementID.Value)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"Equipment with ID {id} not found.");
            }

            // Ensure the DTO has the ID from the URL
            dto.EquipementID = id;

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an equipment by ID.
        /// </summary>
        /// <param name="id">The equipment ID (EquipementID as Guid).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Equipment with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

