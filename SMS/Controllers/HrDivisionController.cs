using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing HR division operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class HrDivisionController : ControllerBase
    {
        private readonly IHrDivisionService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="HrDivisionController"/> class.
        /// </summary>
        /// <param name="service">The HR division service.</param>
        public HrDivisionController(IHrDivisionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all HR divisions.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of HR divisions.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<HrDivisionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<HrDivisionDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an HR division by ID.
        /// </summary>
        /// <param name="id">The HR division ID.</param>
        /// <returns>The HR division if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(HrDivisionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HrDivisionDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"HrDivision with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new HR division.
        /// </summary>
        /// <param name="dto">The HR division data to create.</param>
        /// <returns>The created HR division.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(HrDivisionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<HrDivisionDto>> UpsertAsync([FromBody] HrDivisionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing HR division.
        /// </summary>
        /// <param name="id">The HR division ID.</param>
        /// <param name="dto">The HR division data to update.</param>
        /// <returns>The updated HR division.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(HrDivisionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HrDivisionDto>> UpdateAsync(int id, [FromBody] HrDivisionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.HrDivisionID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"HrDivision with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an HR division by ID.
        /// </summary>
        /// <param name="id">The HR division ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"HrDivision with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

