using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing employee contacts operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class EmployeeContactsController : ControllerBase
    {
        private readonly IEmployeeContactsService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeContactsController"/> class.
        /// </summary>
        /// <param name="service">The employee contacts service.</param>
        public EmployeeContactsController(IEmployeeContactsService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all employee contacts.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of employee contacts.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmployeeContactsDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeContactsDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an employee contact by ID.
        /// </summary>
        /// <param name="id">The employee contact ID.</param>
        /// <returns>The employee contact if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EmployeeContactsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeContactsDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"EmployeeContacts with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new employee contact.
        /// </summary>
        /// <param name="dto">The employee contact data to create.</param>
        /// <returns>The created employee contact.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeContactsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeeContactsDto>> UpsertAsync([FromBody] EmployeeContactsDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing employee contact.
        /// </summary>
        /// <param name="id">The employee contact ID.</param>
        /// <param name="dto">The employee contact data to update.</param>
        /// <returns>The updated employee contact.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EmployeeContactsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeContactsDto>> UpdateAsync(int id, [FromBody] EmployeeContactsDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.ContactID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"EmployeeContacts with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an employee contact by ID.
        /// </summary>
        /// <param name="id">The employee contact ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"EmployeeContacts with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

