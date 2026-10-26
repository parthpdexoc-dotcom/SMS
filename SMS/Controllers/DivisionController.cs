using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing division operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DivisionController : ControllerBase
    {
        private readonly IDivisionService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionController"/> class.
        /// </summary>
        /// <param name="service">The division service.</param>
        public DivisionController(IDivisionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all divisions.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of divisions.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DivisionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DivisionDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(DivisionDto.DivisionName);
            }

            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a division by ID.
        /// </summary>
        /// <param name="id">The division ID.</param>
        /// <returns>The division if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DivisionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DivisionDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Division with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new division.
        /// </summary>
        /// <param name="dto">The division data to create.</param>
        /// <returns>The created division.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(DivisionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DivisionDto>> UpsertAsync([FromBody] DivisionDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing division.
        /// </summary>
        /// <param name="id">The division ID.</param>
        /// <param name="dto">The division data to update.</param>
        /// <returns>The updated division.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(DivisionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DivisionDto>> UpdateAsync(int id, [FromBody] DivisionDto dto)
        {

            if (id != dto.DivisionID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"Division with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a division by ID.
        /// </summary>
        /// <param name="id">The division ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"Division with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

