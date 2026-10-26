using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing close request details operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class CloseRequestDetailsController : ControllerBase
    {
        private readonly ICloseRequestDetailsService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseRequestDetailsController"/> class.
        /// </summary>
        /// <param name="service">The close request details service.</param>
        public CloseRequestDetailsController(ICloseRequestDetailsService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all close request details.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of close request details.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CloseRequestDetailsDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CloseRequestDetailsDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a close request detail by ID.
        /// </summary>
        /// <param name="id">The close request detail ID.</param>
        /// <returns>The close request detail if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CloseRequestDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CloseRequestDetailsDto>> GetByIdAsync(long id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"CloseRequestDetails with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new close request detail.
        /// </summary>
        /// <param name="dto">The close request detail data to create.</param>
        /// <returns>The created close request detail.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CloseRequestDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CloseRequestDetailsDto>> UpsertAsync([FromBody] CloseRequestDetailsDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing close request detail.
        /// </summary>
        /// <param name="id">The close request detail ID.</param>
        /// <param name="dto">The close request detail data to update.</param>
        /// <returns>The updated close request detail.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CloseRequestDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CloseRequestDetailsDto>> UpdateAsync(long id, [FromBody] CloseRequestDetailsDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.CloseRequestDetailID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"CloseRequestDetails with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a close request detail by ID.
        /// </summary>
        /// <param name="id">The close request detail ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            var deleted = await _service.DeleteAsync<long>(id);
            if (!deleted)
            {
                return NotFound($"CloseRequestDetails with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

