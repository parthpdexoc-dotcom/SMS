using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing request activity type operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class RequestActivityTypeController : ControllerBase
    {
        private readonly IRequestActivityTypeService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestActivityTypeController"/> class.
        /// </summary>
        /// <param name="service">The request activity type service.</param>
        public RequestActivityTypeController(IRequestActivityTypeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all request activity types.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of request activity types.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequestActivityTypeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequestActivityTypeDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a request activity type by ID.
        /// </summary>
        /// <param name="id">The request activity type ID.</param>
        /// <returns>The request activity type if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RequestActivityTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequestActivityTypeDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"RequestActivityType with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new request activity type.
        /// </summary>
        /// <param name="dto">The request activity type data to create.</param>
        /// <returns>The created request activity type.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RequestActivityTypeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RequestActivityTypeDto>> UpsertAsync([FromBody] RequestActivityTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing request activity type.
        /// </summary>
        /// <param name="id">The request activity type ID.</param>
        /// <param name="dto">The request activity type data to update.</param>
        /// <returns>The updated request activity type.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RequestActivityTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequestActivityTypeDto>> UpdateAsync(int id, [FromBody] RequestActivityTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.PkRequestActivityTypeID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"RequestActivityType with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a request activity type by ID.
        /// </summary>
        /// <param name="id">The request activity type ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"RequestActivityType with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

