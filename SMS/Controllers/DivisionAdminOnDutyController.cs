using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing division admin on duty operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class DivisionAdminOnDutyController : ControllerBase
    {
        private readonly IDivisionAdminOnDutyService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionAdminOnDutyController"/> class.
        /// </summary>
        /// <param name="service">The division admin on duty service.</param>
        public DivisionAdminOnDutyController(IDivisionAdminOnDutyService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all division admin on duty records.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of division admin on duty records.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DivisionAdminOnDutyDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DivisionAdminOnDutyDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a division admin on duty record by ID.
        /// </summary>
        /// <param name="id">The division admin on duty ID.</param>
        /// <returns>The division admin on duty record if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DivisionAdminOnDutyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DivisionAdminOnDutyDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"DivisionAdminOnDuty with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new division admin on duty record.
        /// </summary>
        /// <param name="dto">The division admin on duty data to create.</param>
        /// <returns>The created division admin on duty record.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(DivisionAdminOnDutyDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DivisionAdminOnDutyDto>> UpsertAsync([FromBody] DivisionAdminOnDutyDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing division admin on duty record.
        /// </summary>
        /// <param name="id">The division admin on duty ID.</param>
        /// <param name="dto">The division admin on duty data to update.</param>
        /// <returns>The updated division admin on duty record.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(DivisionAdminOnDutyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DivisionAdminOnDutyDto>> UpdateAsync(int id, [FromBody] DivisionAdminOnDutyDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.DivisionAdminOnDutyID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"DivisionAdminOnDuty with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a division admin on duty record by ID.
        /// </summary>
        /// <param name="id">The division admin on duty ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"DivisionAdminOnDuty with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

