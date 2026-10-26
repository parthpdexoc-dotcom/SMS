using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing employee admin operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]
    public class EmployeeAdminController : ControllerBase
    {
        private readonly IEmployeeAdminService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeAdminController"/> class.
        /// </summary>
        /// <param name="service">The employee admin service.</param>
        public EmployeeAdminController(IEmployeeAdminService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all employee admins.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of employee admins.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmployeeAdminDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeAdminDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an employee admin by ID.
        /// </summary>
        /// <param name="id">The employee admin ID (EmployeeAdminID as Guid).</param>
        /// <returns>The employee admin if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EmployeeAdminDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeAdminDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"EmployeeAdmin with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new employee admin.
        /// </summary>
        /// <param name="dto">The employee admin data to create.</param>
        /// <returns>The created employee admin.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeAdminDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeeAdminDto>> UpsertAsync([FromBody] EmployeeAdminDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing employee admin.
        /// </summary>
        /// <param name="id">The employee admin ID (EmployeeAdminID as Guid).</param>
        /// <param name="dto">The employee admin data to update.</param>
        /// <returns>The updated employee admin.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EmployeeAdminDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeAdminDto>> UpdateAsync(Guid id, [FromBody] EmployeeAdminDto dto)
        {
            if (id != dto.EmployeeAdminID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"EmployeeAdmin with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an employee admin by ID.
        /// </summary>
        /// <param name="id">The employee admin ID (EmployeeAdminID as Guid).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"EmployeeAdmin with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
