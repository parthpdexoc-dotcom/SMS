using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing email communication operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class EmailCommunicationController : ControllerBase
    {
        private readonly IEmailCommunicationService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailCommunicationController"/> class.
        /// </summary>
        /// <param name="service">The email communication service.</param>
        public EmailCommunicationController(IEmailCommunicationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all email communications.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of email communications.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmailCommunicationDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmailCommunicationDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an email communication by ID.
        /// </summary>
        /// <param name="id">The email communication ID.</param>
        /// <returns>The email communication if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EmailCommunicationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmailCommunicationDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"EmailCommunication with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new email communication.
        /// </summary>
        /// <param name="dto">The email communication data to create.</param>
        /// <returns>The created email communication.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EmailCommunicationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmailCommunicationDto>> UpsertAsync([FromBody] EmailCommunicationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing email communication.
        /// </summary>
        /// <param name="id">The email communication ID.</param>
        /// <param name="dto">The email communication data to update.</param>
        /// <returns>The updated email communication.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EmailCommunicationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmailCommunicationDto>> UpdateAsync(int id, [FromBody] EmailCommunicationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.ID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"EmailCommunication with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an email communication by ID.
        /// </summary>
        /// <param name="id">The email communication ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"EmailCommunication with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

