using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing HR status operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class HRStatusController : ControllerBase
    {
        private readonly IHRStatusService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="HRStatusController"/> class.
        /// </summary>
        /// <param name="service">The HR status service.</param>
        public HRStatusController(IHRStatusService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all HR statuses.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of HR statuses.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<HRStatusDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<HRStatusDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an HR status by ID.
        /// </summary>
        /// <param name="id">The HR status ID.</param>
        /// <returns>The HR status if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(HRStatusDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HRStatusDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"HRStatus with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new HR status.
        /// </summary>
        /// <param name="dto">The HR status data to create.</param>
        /// <returns>The created HR status.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(HRStatusDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<HRStatusDto>> UpsertAsync([FromBody] HRStatusDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing HR status.
        /// </summary>
        /// <param name="id">The HR status ID.</param>
        /// <param name="dto">The HR status data to update.</param>
        /// <returns>The updated HR status.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(HRStatusDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HRStatusDto>> UpdateAsync(int id, [FromBody] HRStatusDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.HRStatusID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"HRStatus with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an HR status by ID.
        /// </summary>
        /// <param name="id">The HR status ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"HRStatus with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

