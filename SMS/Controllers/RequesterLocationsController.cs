using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing requester locations operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class RequesterLocationsController : ControllerBase
    {
        private readonly IRequesterLocationsService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterLocationsController"/> class.
        /// </summary>
        /// <param name="service">The requester locations service.</param>
        public RequesterLocationsController(IRequesterLocationsService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all requester locations.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of requester locations.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequesterLocationsDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequesterLocationsDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a requester location by ID.
        /// </summary>
        /// <param name="id">The requester location ID.</param>
        /// <returns>The requester location if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RequesterLocationsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequesterLocationsDto>> GetByIdAsync(long id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"RequesterLocations with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new requester location.
        /// </summary>
        /// <param name="dto">The requester location data to create.</param>
        /// <returns>The created requester location.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RequesterLocationsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RequesterLocationsDto>> UpsertAsync([FromBody] RequesterLocationsDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing requester location.
        /// </summary>
        /// <param name="id">The requester location ID.</param>
        /// <param name="dto">The requester location data to update.</param>
        /// <returns>The updated requester location.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RequesterLocationsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequesterLocationsDto>> UpdateAsync(long id, [FromBody] RequesterLocationsDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.PkLocationID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"RequesterLocations with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a requester location by ID.
        /// </summary>
        /// <param name="id">The requester location ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            var deleted = await _service.DeleteAsync<long>(id);
            if (!deleted)
            {
                return NotFound($"RequesterLocations with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

