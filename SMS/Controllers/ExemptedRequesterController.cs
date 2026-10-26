using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing exempted requester operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class ExemptedRequesterController : ControllerBase
    {
        private readonly IExemptedRequesterService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExemptedRequesterController"/> class.
        /// </summary>
        /// <param name="service">The exempted requester service.</param>
        public ExemptedRequesterController(IExemptedRequesterService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all exempted requester records.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of exempted requester records.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ExemptedRequesterDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ExemptedRequesterDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an exempted requester record by ID.
        /// </summary>
        /// <param name="id">The exempted requester ID.</param>
        /// <returns>The exempted requester record if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ExemptedRequesterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExemptedRequesterDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"ExemptedRequester with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new exempted requester record.
        /// </summary>
        /// <param name="dto">The exempted requester data to create.</param>
        /// <returns>The created exempted requester record.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ExemptedRequesterDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ExemptedRequesterDto>> UpsertAsync([FromBody] ExemptedRequesterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing exempted requester record.
        /// </summary>
        /// <param name="id">The exempted requester ID.</param>
        /// <param name="dto">The exempted requester data to update.</param>
        /// <returns>The updated exempted requester record.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ExemptedRequesterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExemptedRequesterDto>> UpdateAsync(int id, [FromBody] ExemptedRequesterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.ExemptedRequestersID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"ExemptedRequester with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an exempted requester record by ID.
        /// </summary>
        /// <param name="id">The exempted requester ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"ExemptedRequester with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

