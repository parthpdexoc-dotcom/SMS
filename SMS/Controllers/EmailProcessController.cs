using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing email process operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class EmailProcessController : ControllerBase
    {
        private readonly IEmailProcessService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailProcessController"/> class.
        /// </summary>
        /// <param name="service">The email process service.</param>
        public EmailProcessController(IEmailProcessService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all email processes.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of email processes.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmailProcessDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmailProcessDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an email process by ID.
        /// </summary>
        /// <param name="id">The email process ID.</param>
        /// <returns>The email process if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EmailProcessDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmailProcessDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"EmailProcess with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new email process.
        /// </summary>
        /// <param name="dto">The email process data to create.</param>
        /// <returns>The created email process.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EmailProcessDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmailProcessDto>> UpsertAsync([FromBody] EmailProcessDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing email process.
        /// </summary>
        /// <param name="id">The email process ID.</param>
        /// <param name="dto">The email process data to update.</param>
        /// <returns>The updated email process.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EmailProcessDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmailProcessDto>> UpdateAsync(int id, [FromBody] EmailProcessDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.EmailProcessID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"EmailProcess with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an email process by ID.
        /// </summary>
        /// <param name="id">The email process ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"EmailProcess with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

