using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing prerequisite type operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class PrerequisiteTypeController : ControllerBase
    {
        private readonly IPrerequisiteTypeService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrerequisiteTypeController"/> class.
        /// </summary>
        /// <param name="service">The prerequisite type service.</param>
        public PrerequisiteTypeController(IPrerequisiteTypeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all prerequisite types.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of prerequisite types.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PrerequisiteTypeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PrerequisiteTypeDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a prerequisite type by ID.
        /// </summary>
        /// <param name="id">The prerequisite type ID.</param>
        /// <returns>The prerequisite type if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PrerequisiteTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PrerequisiteTypeDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"PrerequisiteType with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new prerequisite type.
        /// </summary>
        /// <param name="dto">The prerequisite type data to create.</param>
        /// <returns>The created prerequisite type.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PrerequisiteTypeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PrerequisiteTypeDto>> UpsertAsync([FromBody] PrerequisiteTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing prerequisite type.
        /// </summary>
        /// <param name="id">The prerequisite type ID.</param>
        /// <param name="dto">The prerequisite type data to update.</param>
        /// <returns>The updated prerequisite type.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PrerequisiteTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PrerequisiteTypeDto>> UpdateAsync(int id, [FromBody] PrerequisiteTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.PkPrerequisiteTypeID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"PrerequisiteType with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a prerequisite type by ID.
        /// </summary>
        /// <param name="id">The prerequisite type ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"PrerequisiteType with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

