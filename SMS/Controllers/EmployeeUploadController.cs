using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing employee upload operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class EmployeeUploadController : ControllerBase
    {
        private readonly IEmployeeUploadService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeUploadController"/> class.
        /// </summary>
        /// <param name="service">The employee upload service.</param>
        public EmployeeUploadController(IEmployeeUploadService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all employee upload records.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of employee upload records.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmployeeUploadDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeUploadDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an employee upload record by ID.
        /// </summary>
        /// <param name="id">The employee upload ID.</param>
        /// <returns>The employee upload record if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EmployeeUploadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeUploadDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"EmployeeUpload with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new employee upload record.
        /// </summary>
        /// <param name="dto">The employee upload data to create.</param>
        /// <returns>The created employee upload record.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeUploadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeeUploadDto>> UpsertAsync([FromBody] EmployeeUploadDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing employee upload record.
        /// </summary>
        /// <param name="id">The employee upload ID.</param>
        /// <param name="dto">The employee upload data to update.</param>
        /// <returns>The updated employee upload record.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EmployeeUploadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeUploadDto>> UpdateAsync(int id, [FromBody] EmployeeUploadDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.UploadID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"EmployeeUpload with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an employee upload record by ID.
        /// </summary>
        /// <param name="id">The employee upload ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"EmployeeUpload with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

