using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing Option operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    public class OptionController : ControllerBase
    {
        private readonly IOptionService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionController"/> class.
        /// </summary>
        /// <param name="service">The Option service.</param>
        public OptionController(IOptionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all Options.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of Options.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OptionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OptionDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an Option by ID.
        /// </summary>
        /// <param name="id">The Option ID (OptionId as Guid).</param>
        /// <returns>The Option if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OptionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OptionDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Option with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates an Option.
        /// </summary>
        /// <param name="dto">The Option data to create or update.</param>
        /// <returns>The created or updated Option.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(OptionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OptionDto>> UpsertAsync([FromBody] OptionDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Deletes an Option by ID.
        /// </summary>
        /// <param name="id">The Option ID (OptionId as Guid).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Option with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
