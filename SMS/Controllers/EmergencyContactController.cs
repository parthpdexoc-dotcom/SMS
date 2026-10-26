using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing emergency contact operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class EmergencyContactController : ControllerBase
    {
        private readonly IEmergencyContactService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmergencyContactController"/> class.
        /// </summary>
        /// <param name="service">The emergency contact service.</param>
        public EmergencyContactController(IEmergencyContactService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all emergency contacts.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of emergency contacts.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmergencyContactDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmergencyContactDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an emergency contact by ID.
        /// </summary>
        /// <param name="id">The emergency contact ID.</param>
        /// <returns>The emergency contact if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EmergencyContactDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmergencyContactDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"EmergencyContact with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new emergency contact.
        /// </summary>
        /// <param name="dto">The emergency contact data to create.</param>
        /// <returns>The created emergency contact.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EmergencyContactDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmergencyContactDto>> UpsertAsync([FromBody] EmergencyContactDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing emergency contact.
        /// </summary>
        /// <param name="id">The emergency contact ID.</param>
        /// <param name="dto">The emergency contact data to update.</param>
        /// <returns>The updated emergency contact.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EmergencyContactDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmergencyContactDto>> UpdateAsync(int id, [FromBody] EmergencyContactDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.EmergencyContactID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"EmergencyContact with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an emergency contact by ID.
        /// </summary>
        /// <param name="id">The emergency contact ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"EmergencyContact with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

