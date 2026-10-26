using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing HR employee type operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class HrEmployeeTypeController : ControllerBase
    {
        private readonly IHrEmployeeTypeService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="HrEmployeeTypeController"/> class.
        /// </summary>
        /// <param name="service">The HR employee type service.</param>
        public HrEmployeeTypeController(IHrEmployeeTypeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all HR employee types.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of HR employee types.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<HrEmployeeTypeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<HrEmployeeTypeDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an HR employee type by ID.
        /// </summary>
        /// <param name="id">The HR employee type ID.</param>
        /// <returns>The HR employee type if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(HrEmployeeTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HrEmployeeTypeDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"HrEmployeeType with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new HR employee type.
        /// </summary>
        /// <param name="dto">The HR employee type data to create.</param>
        /// <returns>The created HR employee type.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(HrEmployeeTypeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<HrEmployeeTypeDto>> UpsertAsync([FromBody] HrEmployeeTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing HR employee type.
        /// </summary>
        /// <param name="id">The HR employee type ID.</param>
        /// <param name="dto">The HR employee type data to update.</param>
        /// <returns>The updated HR employee type.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(HrEmployeeTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HrEmployeeTypeDto>> UpdateAsync(int id, [FromBody] HrEmployeeTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.HrEmpTypeID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"HrEmployeeType with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an HR employee type by ID.
        /// </summary>
        /// <param name="id">The HR employee type ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"HrEmployeeType with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

