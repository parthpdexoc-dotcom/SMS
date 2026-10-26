using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing employee position operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class EmployeePositionController : ControllerBase
    {
        private readonly IEmployeePositionService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeePositionController"/> class.
        /// </summary>
        /// <param name="service">The employee position service.</param>
        public EmployeePositionController(IEmployeePositionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all employee positions.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of employee positions.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmployeePositionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeePositionDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an employee position by ID.
        /// </summary>
        /// <param name="id">The employee position ID.</param>
        /// <returns>The employee position if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EmployeePositionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeePositionDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"EmployeePosition with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new employee position.
        /// </summary>
        /// <param name="dto">The employee position data to create.</param>
        /// <returns>The created employee position.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EmployeePositionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeePositionDto>> UpsertAsync([FromBody] EmployeePositionDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing employee position.
        /// </summary>
        /// <param name="id">The employee position ID.</param>
        /// <param name="dto">The employee position data to update.</param>
        /// <returns>The updated employee position.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EmployeePositionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeePositionDto>> UpdateAsync(int id, [FromBody] EmployeePositionDto dto)
        {

            if (id != dto.EmployeePositionID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"EmployeePosition with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an employee position by ID.
        /// </summary>
        /// <param name="id">The employee position ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"EmployeePosition with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

