using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing state operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class StateController : ControllerBase
    {
        private readonly IStateService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateController"/> class.
        /// </summary>
        /// <param name="service">The state service.</param>
        public StateController(IStateService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all states.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of states.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StateDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<StateDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a state by ID.
        /// </summary>
        /// <param name="id">The state ID.</param>
        /// <returns>The state if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StateDto>> GetByIdAsync(long id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"State with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new state.
        /// </summary>
        /// <param name="dto">The state data to create.</param>
        /// <returns>The created state.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(StateDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StateDto>> UpsertAsync([FromBody] StateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing state.
        /// </summary>
        /// <param name="id">The state ID.</param>
        /// <param name="dto">The state data to update.</param>
        /// <returns>The updated state.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(StateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StateDto>> UpdateAsync(long id, [FromBody] StateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.StateID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"State with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a state by ID.
        /// </summary>
        /// <param name="id">The state ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            var deleted = await _service.DeleteAsync<long>(id);
            if (!deleted)
            {
                return NotFound($"State with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

