using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing email template operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.SuperUserOnly))]
    public class EmailTemplateController : ControllerBase
    {
        private readonly IEmailTemplateService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailTemplateController"/> class.
        /// </summary>
        /// <param name="service">The email template service.</param>
        public EmailTemplateController(IEmailTemplateService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all email templates (paged).
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>Paged email templates.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<EmailTemplateDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<EmailTemplateDto>>> GetAllAsync(
            [FromQuery] PageParam? pagination = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(EmailTemplateDto.Title);
            }

            var result = await _service.GetAllPagedAsync(null, pagination);
            return Ok(result);
        }

        /// <summary>
        /// Gets an email template by ID.
        /// </summary>
        /// <param name="id">The email template ID (EmailTemplateID as Guid).</param>
        /// <returns>The email template if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EmailTemplateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmailTemplateDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Email template with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new email template.
        /// </summary>
        /// <param name="dto">The email template data to create.</param>
        /// <returns>The created email template.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EmailTemplateDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmailTemplateDto>> UpsertAsync([FromBody] EmailTemplateDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing email template.
        /// </summary>
        /// <param name="id">The email template ID (EmailTemplateID as Guid).</param>
        /// <param name="dto">The email template data to update.</param>
        /// <returns>The updated email template.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EmailTemplateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmailTemplateDto>> UpdateAsync(Guid id, [FromBody] EmailTemplateDto dto)
        {
            if (id != dto.EmailTemplateID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"Email template with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an email template by ID.
        /// </summary>
        /// <param name="id">The email template ID (EmailTemplateID as Guid).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Email template with ID {id} not found.");
            }
            return NoContent();
        }

        /// <summary>
        /// Searches email templates by title, subject, or content with paging.
        /// </summary>
        /// <param name="searchTerm">Free-text search term.</param>
        /// <param name="pageParam">Optional paging/sorting parameters.</param>
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<EmailTemplateDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<EmailTemplateDto>>> SearchAsync([FromQuery] string? searchTerm, [FromQuery] PageParam? pageParam = null)
        {
            pageParam ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
            {
                pageParam.SortBy = nameof(EmailTemplateDto.Title);
            }

            var result = await _service.SearchAsync(searchTerm, pageParam);
            return Ok(result);
        }

        /// <summary>
        /// Sends email(s) using a template identified by EmailType. Template placeholders like [FullName], [DepartmentName] are replaced with values from the request.
        /// </summary>
        /// <param name="request">The send mail request: EmailType, Email (recipient), optional FromEmail, and optional TemplateParameters (e.g. { "FullName": "John", "DepartmentName": "IT" }).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>204 No Content on success.</returns>
        [HttpPost("send")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SendMailAsync([FromBody] SendMailRequestDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                return BadRequest("Request body is required.");
            if (request.Emails == null || !request.Emails.Any())
                return BadRequest("Recipient email is required.");

            await _service.SendMailAsync(request, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Gets supported template keywords for the provided email type.
        /// </summary>
        /// <param name="emailType">Email type to fetch keywords for.</param>
        /// <returns>Distinct keyword list mapped to the given email type.</returns>
        [HttpGet("keywords")]
        [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
        public ActionResult<IReadOnlyList<string>> GetKeywordsByEmailType([FromQuery] EmailType emailType)
        {
            var result = _service.GetKeywordsByEmailType(emailType);
            return Ok(result);
        }

        /// <summary>
        /// Gets all email types that have keyword mapping metadata (same source as template keywords), with value and display label.
        /// </summary>
        /// <returns>Email type options in enum value order.</returns>
        [HttpGet("email-types")]
        [ProducesResponseType(typeof(IReadOnlyList<EmailTypeOptionDto>), StatusCodes.Status200OK)]
        public ActionResult<IReadOnlyList<EmailTypeOptionDto>> GetEmailTypes()
        {
            var result = _service.GetEmailTypes();
            return Ok(result);
        }
    }
}
