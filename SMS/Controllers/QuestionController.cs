using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing question operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionController"/> class.
        /// </summary>
        /// <param name="service">The question service.</param>
        public QuestionController(IQuestionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all questions.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of questions.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<QuestionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a question by ID.
        /// </summary>
        /// <param name="id">The question ID (QuestionId as Guid).</param>
        /// <returns>The question if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(QuestionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QuestionDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Question with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a question.
        /// </summary>
        /// <param name="dto">The question data to create or update.</param>
        /// <returns>The created or updated question.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(QuestionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<QuestionDto>> UpsertAsync([FromBody] QuestionDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Deletes a question by ID.
        /// </summary>
        /// <param name="id">The question ID (QuestionId as Guid).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Question with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
