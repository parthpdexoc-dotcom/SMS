using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing requester config operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
    public class RequesterConfigController : ControllerBase
    {
        private readonly IRequesterConfigService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterConfigController"/> class.
        /// </summary>
        /// <param name="service">The requester config service.</param>
        public RequesterConfigController(IRequesterConfigService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all requester configs.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of requester configs.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequesterConfigDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequesterConfigDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a requester config by ID.
        /// </summary>
        /// <param name="id">The requester config ID (RequesterConfigID as Guid).</param>
        /// <returns>The requester config if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RequesterConfigDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequesterConfigDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"RequesterConfig with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new requester config.
        /// </summary>
        /// <param name="dto">The requester config data to create.</param>
        /// <returns>The created requester config.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RequesterConfigDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RequesterConfigDto>> UpsertAsync([FromBody] RequesterConfigDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Deletes a requester config by ID.
        /// </summary>
        /// <param name="id">The requester config ID (RequesterConfigID as Guid).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"RequesterConfig with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
