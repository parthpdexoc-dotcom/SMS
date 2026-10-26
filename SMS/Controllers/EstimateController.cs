using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing estimate operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class EstimateController : ControllerBase
    {
        private readonly IEstimateService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EstimateController"/> class.
        /// </summary>
        /// <param name="service">The estimate service.</param>
        public EstimateController(IEstimateService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all estimates.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of estimates.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EstimateDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EstimateDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an estimate by ID.
        /// </summary>
        /// <param name="id">The estimate ID.</param>
        /// <returns>The estimate if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EstimateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EstimateDto>> GetByIdAsync(long id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Estimate with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new estimate.
        /// </summary>
        /// <param name="dto">The estimate data to create.</param>
        /// <returns>The created estimate.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EstimateDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EstimateDto>> UpsertAsync([FromBody] EstimateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing estimate.
        /// </summary>
        /// <param name="id">The estimate ID.</param>
        /// <param name="dto">The estimate data to update.</param>
        /// <returns>The updated estimate.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EstimateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EstimateDto>> UpdateAsync(long id, [FromBody] EstimateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.EstimateID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"Estimate with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an estimate by ID.
        /// </summary>
        /// <param name="id">The estimate ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            var deleted = await _service.DeleteAsync<long>(id);
            if (!deleted)
            {
                return NotFound($"Estimate with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

