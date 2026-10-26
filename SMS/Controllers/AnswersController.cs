using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing answers operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
    public class AnswersController : ControllerBase
    {
        private readonly IAnswersService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnswersController"/> class.
        /// </summary>
        /// <param name="service">The answers service.</param>
        public AnswersController(IAnswersService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all answers.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of answers.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AnswersDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AnswersDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an answer by ID.
        /// </summary>
        /// <param name="id">The answer ID (AnswerId as Guid).</param>
        /// <returns>The answer if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AnswersDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AnswersDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Answer with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates an answer.
        /// </summary>
        /// <param name="dto">The answer data to create or update.</param>
        /// <returns>The created or updated answer.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(AnswersDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AnswersDto>> UpsertAsync([FromBody] AnswersDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Deletes an answer by ID.
        /// </summary>
        /// <param name="id">The answer ID (AnswerId as Guid).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Answer with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
