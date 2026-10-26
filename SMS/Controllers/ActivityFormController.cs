using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;
using SMS.Filters;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing activity form operations (questionnaire with questions and options).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityFormController : ControllerBase
    {
        private readonly IActivityFormService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityFormController"/> class.
        /// </summary>
        /// <param name="service">The activity form service.</param>
        public ActivityFormController(IActivityFormService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets the complete questionnaire form data including questions and options by questionnaire ID.
        /// </summary>
        /// <param name="id">The questionnaire ID (QuestionnaireId as Guid).</param>
        /// <returns>The questionnaire form data with questions and options if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(QuestionnaireFormDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RegularEmployeeOnly),nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<QuestionnaireFormDto>> GetQuestionnaireFormByIdAsync(Guid id)
        {
            var formData = await _service.GetQuestionnaireFormByIdAsync(id);
            if (formData == null)
            {
                return NotFound($"Questionnaire with ID {id} not found.");
            }
            return Ok(formData);
        }

        /// <summary>
        /// Inserts or updates a complete questionnaire form including questions and options.
        /// </summary>
        /// <param name="formDto">The questionnaire form data to insert or update.</param>
        /// <returns>The upserted questionnaire form data with questions and options.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(QuestionnaireFormDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<ActionResult<QuestionnaireFormDto>> UpsertQuestionnaireFormAsync([FromBody] QuestionnaireFormDto formDto)
        {
            if (formDto == null)
            {
                return BadRequest("Questionnaire form data cannot be null.");
            }

            var result = await _service.UpsertQuestionnaireFormAsync(formDto);
            return Ok(result);
        }

        /// <summary>
        /// Deletes an activity form (questionnaire) by id.
        /// </summary>
        /// <param name="id">The questionnaire id.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
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
