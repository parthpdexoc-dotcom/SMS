using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing response operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
    public class ResponseController : ControllerBase
    {
        private readonly IResponseService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseController"/> class.
        /// </summary>
        /// <param name="service">The response service.</param>
        public ResponseController(IResponseService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all responses.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of responses.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ResponseDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a response by ID.
        /// </summary>
        /// <param name="id">The response ID (ResponseId as Guid).</param>
        /// <returns>The response if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Response with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a response.
        /// </summary>
        /// <param name="dto">The response data to create or update.</param>
        /// <returns>The created or updated response.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseDto>> UpsertAsync([FromBody] ResponseDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Deletes a response by ID.
        /// </summary>
        /// <param name="id">The response ID (ResponseId as Guid).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Response with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
