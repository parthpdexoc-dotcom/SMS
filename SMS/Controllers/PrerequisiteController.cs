using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing prerequisite operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class PrerequisiteController : ControllerBase
    {
        private readonly IPrerequisiteService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrerequisiteController"/> class.
        /// </summary>
        /// <param name="service">The prerequisite service.</param>
        public PrerequisiteController(IPrerequisiteService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all prerequisites.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of prerequisites.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PrerequisiteDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PrerequisiteDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a prerequisite by ID.
        /// </summary>
        /// <param name="id">The prerequisite ID.</param>
        /// <returns>The prerequisite if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PrerequisiteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PrerequisiteDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Prerequisite with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new prerequisite.
        /// </summary>
        /// <param name="dto">The prerequisite data to create.</param>
        /// <returns>The created prerequisite.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PrerequisiteDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PrerequisiteDto>> UpsertAsync([FromBody] PrerequisiteDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdItem = await _service.UpsertAsync(dto);
                return createdItem;
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing prerequisite.
        /// </summary>
        /// <param name="id">The prerequisite ID.</param>
        /// <param name="dto">The prerequisite data to update.</param>
        /// <returns>The updated prerequisite.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PrerequisiteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PrerequisiteDto>> UpdateAsync(int id, [FromBody] PrerequisiteDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.PkPrerequisiteID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"Prerequisite with ID {id} not found.");
            }

            try
            {
                var updatedItem = await _service.UpsertAsync(dto);
                return Ok(updatedItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a prerequisite by ID.
        /// </summary>
        /// <param name="id">The prerequisite ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"Prerequisite with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

