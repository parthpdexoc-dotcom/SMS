using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing email access code operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class EmailAccessCodeController : ControllerBase
    {
        private readonly IEmailAccessCodeService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAccessCodeController"/> class.
        /// </summary>
        /// <param name="service">The email access code service.</param>
        public EmailAccessCodeController(IEmailAccessCodeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all email access codes.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of email access codes.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmailAccessCodeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmailAccessCodeDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an email access code by ID.
        /// </summary>
        /// <param name="id">The email access code ID.</param>
        /// <returns>The email access code if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EmailAccessCodeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmailAccessCodeDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"EmailAccessCode with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new email access code.
        /// </summary>
        /// <param name="dto">The email access code data to create.</param>
        /// <returns>The created email access code.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EmailAccessCodeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmailAccessCodeDto>> UpsertAsync([FromBody] EmailAccessCodeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing email access code.
        /// </summary>
        /// <param name="id">The email access code ID.</param>
        /// <param name="dto">The email access code data to update.</param>
        /// <returns>The updated email access code.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EmailAccessCodeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmailAccessCodeDto>> UpdateAsync(int id, [FromBody] EmailAccessCodeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.SL)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"EmailAccessCode with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an email access code by ID.
        /// </summary>
        /// <param name="id">The email access code ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"EmailAccessCode with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

