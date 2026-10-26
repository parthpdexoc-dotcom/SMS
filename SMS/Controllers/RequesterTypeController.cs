using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing requester type operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class RequesterTypeController : ControllerBase
    {
        private readonly IRequesterTypeService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterTypeController"/> class.
        /// </summary>
        /// <param name="service">The requester type service.</param>
        public RequesterTypeController(IRequesterTypeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all requester types.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of requester types.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequesterTypeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequesterTypeDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a requester type by ID.
        /// </summary>
        /// <param name="id">The requester type ID.</param>
        /// <returns>The requester type if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RequesterTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequesterTypeDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"RequesterType with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new requester type.
        /// </summary>
        /// <param name="dto">The requester type data to create.</param>
        /// <returns>The created requester type.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RequesterTypeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RequesterTypeDto>> UpsertAsync([FromBody] RequesterTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing requester type.
        /// </summary>
        /// <param name="id">The requester type ID.</param>
        /// <param name="dto">The requester type data to update.</param>
        /// <returns>The updated requester type.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RequesterTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequesterTypeDto>> UpdateAsync(int id, [FromBody] RequesterTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.PkRequesterTypeID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"RequesterType with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a requester type by ID.
        /// </summary>
        /// <param name="id">The requester type ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"RequesterType with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

