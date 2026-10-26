using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;
using SMS.Filters;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing PostQuestionnaire operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RequesterQuestionnaireController : ControllerBase
    {
        private readonly IRequesterQuestionnaireService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterQuestionnaireController"/> class.
        /// </summary>
        /// <param name="service">The PostQuestionnaire service.</param>
        public RequesterQuestionnaireController(IRequesterQuestionnaireService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all PostQuestionnaires.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of PostQuestionnaires.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequesterQuestionnaireDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]

        public async Task<ActionResult<IEnumerable<RequesterQuestionnaireDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a PostQuestionnaire by ID.
        /// </summary>
        /// <param name="id">The PostQuestionnaire ID (PostQuestionnaireId as Guid).</param>
        /// <returns>The PostQuestionnaire if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RequesterQuestionnaireDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]

        public async Task<ActionResult<RequesterQuestionnaireDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"PostQuestionnaire with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Gets all RequesterQuestionnaires for a specific Requester.
        /// </summary>
        /// <param name="requesterId">The Requester ID.</param>
        /// <returns>A collection of RequesterQuestionnaires for the given requester.</returns>
        [HttpGet("by-requester/{requesterId}")]
        [ProducesResponseType(typeof(IEnumerable<RequesterQuestionnaireDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly),nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        public async Task<ActionResult<IEnumerable<RequesterQuestionnaireDto>>> GetByRequesterIdAsync(Guid requesterId)
        {
            var items = await _service.GetAllAsync(x => x.RequesterId == requesterId, null, null);
            return Ok(items);
        }

        /// <summary>
        /// Creates or updates a PostQuestionnaire.
        /// </summary>
        /// <param name="dto">The PostQuestionnaire data to create or update.</param>
        /// <returns>The created or updated PostQuestionnaire.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RequesterQuestionnaireDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]

        public async Task<ActionResult<RequesterQuestionnaireDto>> UpsertAsync([FromBody] RequesterQuestionnaireDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Deletes a PostQuestionnaire by ID.
        /// </summary>
        /// <param name="id">The PostQuestionnaire ID (PostQuestionnaireId as Guid).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]

        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"PostQuestionnaire with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
