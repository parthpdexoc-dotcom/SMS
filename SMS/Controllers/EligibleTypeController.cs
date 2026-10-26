using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing eligible type operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class EligibleTypeController : ControllerBase
    {
        private readonly IEligibleTypeService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EligibleTypeController"/> class.
        /// </summary>
        /// <param name="service">The eligible type service.</param>
        public EligibleTypeController(IEligibleTypeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all eligible types.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of eligible types.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EligibleTypeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EligibleTypeDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(EligibleTypeDto.EligibleTitle);
            }

            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an eligible type by ID.
        /// </summary>
        /// <param name="id">The eligible type ID.</param>
        /// <returns>The eligible type if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EligibleTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EligibleTypeDto>> GetByIdAsync(long id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"EligibleType with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new eligible type.
        /// </summary>
        /// <param name="dto">The eligible type data to create.</param>
        /// <returns>The created eligible type.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EligibleTypeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EligibleTypeDto>> UpsertAsync([FromBody] EligibleTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing eligible type.
        /// </summary>
        /// <param name="id">The eligible type ID.</param>
        /// <param name="dto">The eligible type data to update.</param>
        /// <returns>The updated eligible type.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EligibleTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EligibleTypeDto>> UpdateAsync(long id, [FromBody] EligibleTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.EligibleCID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"EligibleType with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an eligible type by ID.
        /// </summary>
        /// <param name="id">The eligible type ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            var deleted = await _service.DeleteAsync<long>(id);
            if (!deleted)
            {
                return NotFound($"EligibleType with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

