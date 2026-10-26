using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing requester contacts operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class RequesterContactsController : ControllerBase
    {
        private readonly IRequesterContactsService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterContactsController"/> class.
        /// </summary>
        /// <param name="service">The requester contacts service.</param>
        public RequesterContactsController(IRequesterContactsService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all requester contacts.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of requester contacts.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequesterContactsDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequesterContactsDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a requester contact by ID.
        /// </summary>
        /// <param name="id">The requester contact ID.</param>
        /// <returns>The requester contact if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RequesterContactsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequesterContactsDto>> GetByIdAsync(long id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"RequesterContacts with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new requester contact.
        /// </summary>
        /// <param name="dto">The requester contact data to create.</param>
        /// <returns>The created requester contact.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RequesterContactsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RequesterContactsDto>> UpsertAsync([FromBody] RequesterContactsDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing requester contact.
        /// </summary>
        /// <param name="id">The requester contact ID.</param>
        /// <param name="dto">The requester contact data to update.</param>
        /// <returns>The updated requester contact.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RequesterContactsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequesterContactsDto>> UpdateAsync(long id, [FromBody] RequesterContactsDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.PkContactID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"RequesterContacts with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a requester contact by ID.
        /// </summary>
        /// <param name="id">The requester contact ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            var deleted = await _service.DeleteAsync<long>(id);
            if (!deleted)
            {
                return NotFound($"RequesterContacts with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

