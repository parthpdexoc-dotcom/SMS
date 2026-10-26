using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing block post day operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    [ApiExplorerSettings(IgnoreApi = true)] 

    public class BlockPostDayController : ControllerBase
    {
        private readonly IBlockPostDayService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockPostDayController"/> class.
        /// </summary>
        /// <param name="service">The block post day service.</param>
        public BlockPostDayController(IBlockPostDayService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all block post days.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of block post days.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BlockPostDayDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BlockPostDayDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a block post day by ID.
        /// </summary>
        /// <param name="id">The block post day ID.</param>
        /// <returns>The block post day if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BlockPostDayDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BlockPostDayDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"BlockPostDay with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new block post day.
        /// </summary>
        /// <param name="dto">The block post day data to create.</param>
        /// <returns>The created block post day.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(BlockPostDayDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BlockPostDayDto>> UpsertAsync([FromBody] BlockPostDayDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing block post day.
        /// </summary>
        /// <param name="id">The block post day ID.</param>
        /// <param name="dto">The block post day data to update.</param>
        /// <returns>The updated block post day.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BlockPostDayDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BlockPostDayDto>> UpdateAsync(int id, [FromBody] BlockPostDayDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.BlockPostID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"BlockPostDay with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a block post day by ID.
        /// </summary>
        /// <param name="id">The block post day ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"BlockPostDay with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

