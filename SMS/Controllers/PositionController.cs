using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing position operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class PositionController : ControllerBase
    {
        private readonly IPositionService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionController"/> class.
        /// </summary>
        /// <param name="service">The position service.</param>
        public PositionController(IPositionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all positions.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of positions.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PositionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PositionDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a position by ID.
        /// </summary>
        /// <param name="id">The position ID.</param>
        /// <returns>The position if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PositionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PositionDto>> GetByIdAsync(long id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Position with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new position.
        /// </summary>
        /// <param name="dto">The position data to create.</param>
        /// <returns>The created position.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PositionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PositionDto>> UpsertAsync([FromBody] PositionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing position.
        /// </summary>
        /// <param name="id">The position ID.</param>
        /// <param name="dto">The position data to update.</param>
        /// <returns>The updated position.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PositionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PositionDto>> UpdateAsync(long id, [FromBody] PositionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.PositionID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"Position with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a position by ID.
        /// </summary>
        /// <param name="id">The position ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            var deleted = await _service.DeleteAsync<long>(id);
            if (!deleted)
            {
                return NotFound($"Position with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

