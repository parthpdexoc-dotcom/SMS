using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing questionnaire operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    public class QuestionnaireController : ControllerBase
    {
        private readonly IQuestionnaireService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionnaireController"/> class.
        /// </summary>
        /// <param name="service">The questionnaire service.</param>
        public QuestionnaireController(IQuestionnaireService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all questionnaires.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of questionnaires.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<QuestionnaireDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<QuestionnaireDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a questionnaire by ID.
        /// </summary>
        /// <param name="id">The questionnaire ID (QuestionnaireId as Guid).</param>
        /// <returns>The questionnaire if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(QuestionnaireDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QuestionnaireDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Questionnaire with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a questionnaire.
        /// </summary>
        /// <param name="dto">The questionnaire data to create or update.</param>
        /// <returns>The created or updated questionnaire.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(QuestionnaireDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<QuestionnaireDto>> UpsertAsync([FromBody] QuestionnaireDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Deletes a questionnaire by ID.
        /// </summary>
        /// <param name="id">The questionnaire ID (QuestionnaireId as Guid).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Questionnaire with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
