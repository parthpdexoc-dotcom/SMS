using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing requester configuration operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RequesterConfigurationController : ControllerBase
    {
        private readonly IRequesterConfigurationService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterConfigurationController"/> class.
        /// </summary>
        /// <param name="service">The requester configuration service.</param>
        public RequesterConfigurationController(IRequesterConfigurationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all requester configurations.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of requester configurations.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequesterConfigurationDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequesterConfigurationDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a requester configuration by ID.
        /// </summary>
        /// <param name="id">The requester configuration ID.</param>
        /// <returns>The requester configuration if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RequesterConfigurationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequesterConfigurationDto>> GetByIdAsync(long id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"RequesterConfiguration with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new requester configuration.
        /// </summary>
        /// <param name="dto">The requester configuration data to create.</param>
        /// <returns>The created requester configuration.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RequesterConfigurationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RequesterConfigurationDto>> UpsertAsync([FromBody] RequesterConfigurationDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing requester configuration.
        /// </summary>
        /// <param name="id">The requester configuration ID.</param>
        /// <param name="dto">The requester configuration data to update.</param>
        /// <returns>The updated requester configuration.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RequesterConfigurationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequesterConfigurationDto>> UpdateAsync(long id, [FromBody] RequesterConfigurationDto dto)
        {
            if (id != dto.ReqConID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"RequesterConfiguration with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a requester configuration by ID.
        /// </summary>
        /// <param name="id">The requester configuration ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            var deleted = await _service.DeleteAsync<long>(id);
            if (!deleted)
            {
                return NotFound($"RequesterConfiguration with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

