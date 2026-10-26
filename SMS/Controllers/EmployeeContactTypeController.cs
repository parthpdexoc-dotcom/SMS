using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing employee contact type operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class EmployeeContactTypeController : ControllerBase
    {
        private readonly IEmployeeContactTypeService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeContactTypeController"/> class.
        /// </summary>
        /// <param name="service">The employee contact type service.</param>
        public EmployeeContactTypeController(IEmployeeContactTypeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all employee contact types.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of employee contact types.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmployeeContactTypeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeContactTypeDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an employee contact type by ID.
        /// </summary>
        /// <param name="id">The employee contact type ID.</param>
        /// <returns>The employee contact type if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EmployeeContactTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeContactTypeDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"EmployeeContactType with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new employee contact type.
        /// </summary>
        /// <param name="dto">The employee contact type data to create.</param>
        /// <returns>The created employee contact type.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeContactTypeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeeContactTypeDto>> UpsertAsync([FromBody] EmployeeContactTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing employee contact type.
        /// </summary>
        /// <param name="id">The employee contact type ID.</param>
        /// <param name="dto">The employee contact type data to update.</param>
        /// <returns>The updated employee contact type.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EmployeeContactTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeContactTypeDto>> UpdateAsync(int id, [FromBody] EmployeeContactTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.PkEmployeeContactTypeID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"EmployeeContactType with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an employee contact type by ID.
        /// </summary>
        /// <param name="id">The employee contact type ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"EmployeeContactType with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

