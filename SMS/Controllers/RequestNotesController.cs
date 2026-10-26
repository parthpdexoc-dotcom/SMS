using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;
using SMS.Filters;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing request notes operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RequestNotesController : ControllerBase
    {
        private readonly IRequestNoteService _requestNoteService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestNotesController"/> class.
        /// </summary>
        /// <param name="requestNoteService">The request note service.</param>
        public RequestNotesController(IRequestNoteService requestNoteService)
        {
            _requestNoteService = requestNoteService;
        }

        /// <summary>
        /// Gets all request notes.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of request notes.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequestNotesDto>), StatusCodes.Status200OK)]

        public async Task<ActionResult<IEnumerable<RequestNotesDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _requestNoteService.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets all notes for a specific request ID ordered by created date in ascending order.
        /// </summary>
        /// <param name="id">The request ID.</param>
        /// <returns>A collection of request notes ordered by created date.</returns>
        [HttpGet("{id}/notes")]
        [ProducesResponseType(typeof(IEnumerable<RequestNotesDto>), StatusCodes.Status200OK)]

        public async Task<ActionResult<IEnumerable<RequestNotesDto>>> GetAllNotesByRequestIdAsync(Guid id)
        {
            var items = await _requestNoteService.GetAllAsync(x => x.RequestId == id, null, null);
            var sortedItems = items.OrderBy(x => x.CreatedDate).ToList();
            return Ok(sortedItems);
        }

        /// <summary>
        /// Gets the latest note for a specific request ID where IsLatest is true.
        /// </summary>
        /// <param name="id">The request ID.</param>
        /// <returns>The latest request note if found; otherwise, NotFound.</returns>
        [HttpGet("{id}/latest-notes")]
        [ProducesResponseType(typeof(RequestNotesDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<RequestNotesDto>> GetLatestNoteByRequestIdAsync(Guid id)
        {
            var item = await _requestNoteService.GetLatestNoteByRequestIdAsync(id);
            if (item == null)
            {
                return NotFound($"Latest note for request with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Gets a request note by ID.
        /// </summary>
        /// <param name="id">The request note ID.</param>
        /// <returns>The request note if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RequestNotesDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequestNotesDto>> GetByIdAsync(Guid id)
        {
            var item = await _requestNoteService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Request note with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new request note.
        /// </summary>
        /// <param name="dto">The request note data to create.</param>
        /// <returns>The created request note.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RequestNotesDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]

        public async Task<ActionResult<RequestNotesDto>> UpsertAsync([FromBody] RequestNotesDto dto)
        {
            var createdItem = await _requestNoteService.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Deletes a request note by ID.
        /// </summary>
        /// <param name="id">The request note ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]

        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _requestNoteService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound($"Request note with ID {id} not found.");
            }
            return Ok(new { message = "Record deleted successfully." });
        }
    }
}
