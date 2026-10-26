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
    /// Controller for managing department questionnaire operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentQuestionnaireController : ControllerBase
    {
        private readonly IDepartmentQuestionnaireService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentQuestionnaireController"/> class.
        /// </summary>
        /// <param name="service">The department questionnaire service.</param>
        public DepartmentQuestionnaireController(IDepartmentQuestionnaireService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets questionnaires that are not assigned to any department (for SuperUser), paged.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>Paged questionnaires not in DepartmentQuestionnaire.</returns>
        [HttpGet("questionnaires-by-admin")]
        [ProducesResponseType(typeof(PagedList<QuestionnaireDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<ActionResult<PagedList<QuestionnaireDto>>> GetQuestionnairesByAdminAsync([FromQuery] PageParam? pagination = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(QuestionnaireDto.Title);
            }

            var result = await _service.GetQuestionnairesByAdminAsync(pagination);
            return Ok(result);
        }

        [HttpGet("questionnaires-by-admin/search")]
        [ProducesResponseType(typeof(PagedList<QuestionnaireDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<PagedList<QuestionnaireDto>>> SearchQuestionnairesByAdminAsync([FromQuery] string? searchTerm, [FromQuery] PageParam? pagination = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
                pagination.SortBy = nameof(QuestionnaireDto.Title);

            var result = await _service.SearchQuestionnairesByAdminAsync(searchTerm, pagination);
            return Ok(result);
        }

        /// <summary>
        /// Gets questionnaires assigned to the current user's department (for Employee).
        /// Department ID is taken from the token.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of questionnaires assigned to the department from the token.</returns>
        [HttpGet("questionnaires-by-department")]
        [ProducesResponseType(typeof(IEnumerable<DepartmentQuestionnaireResponseDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly))]


        public async Task<ActionResult<IEnumerable<DepartmentQuestionnaireResponseDto>>> GetQuestionnairesByDepartmentAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetQuestionnairesByDepartmentAsync(pagination);
            return Ok(items);
        }

        [HttpGet("questionnaires-by-department/search")]
        [ProducesResponseType(typeof(PagedList<DepartmentQuestionnaireResponseDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<PagedList<DepartmentQuestionnaireResponseDto>>> SearchQuestionnairesByDepartmentAsync([FromQuery] string? searchTerm, [FromQuery] PageParam? pagination = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
                pagination.SortBy = nameof(DepartmentQuestionnaireResponseDto.Title);

            var items = await _service.SearchQuestionnairesByDepartmentAsync(searchTerm, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets all department questionnaires.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of department questionnaires.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DepartmentQuestionnaireDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<ActionResult<IEnumerable<DepartmentQuestionnaireDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a department questionnaire by ID.
        /// </summary>
        /// <param name="id">The department questionnaire ID (DepartmentQuestionnaireId as Guid).</param>
        /// <returns>The department questionnaire if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DepartmentQuestionnaireDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<ActionResult<DepartmentQuestionnaireDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Department questionnaire with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a department questionnaire.
        /// </summary>
        /// <param name="dto">The department questionnaire data to create or update.</param>
        /// <returns>The created or updated department questionnaire.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(DepartmentQuestionnaireDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<ActionResult<DepartmentQuestionnaireDto>> UpsertAsync([FromBody] DepartmentQuestionnaireDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Deletes a department questionnaire by ID.
        /// </summary>
        /// <param name="id">The department questionnaire ID (DepartmentQuestionnaireId as Guid).</param>
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
                return NotFound($"Department questionnaire with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
