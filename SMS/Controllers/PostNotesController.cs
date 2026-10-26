using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing post notes operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PostNotesController : ControllerBase
    {
        private readonly IPostNoteService _postNoteService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostNotesController"/> class.
        /// </summary>
        /// <param name="postNoteService">The post note service.</param>
        public PostNotesController(IPostNoteService postNoteService)
        {
            _postNoteService = postNoteService;
        }

        /// <summary>
        /// Gets all post notes.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of post notes.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PostNotesDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PostNotesDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _postNoteService.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets all notes for a specific post ID ordered by created date in ascending order.
        /// </summary>
        /// <param name="id">The post ID.</param>
        /// <returns>A collection of post notes ordered by created date.</returns>
        [HttpGet("{id}/notes")]
        [ProducesResponseType(typeof(IEnumerable<PostNotesDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PostNotesDto>>> GetAllNotesByPostIdAsync(Guid id)
        {
            var items = await _postNoteService.GetAllAsync(x => x.PostID == id, null, null);
            var sortedItems = items.OrderBy(x => x.CreatedDate).ToList();
            return Ok(sortedItems);
        }

        /// <summary>
        /// Gets the latest note for a specific post ID where IsLatest is true.
        /// </summary>
        /// <param name="id">The post ID.</param>
        /// <returns>The latest post note if found; otherwise, NotFound.</returns>
        [HttpGet("{id}/latest-notes")]
        [ProducesResponseType(typeof(PostNotesDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostNotesDto>> GetLatestNoteByPostIdAsync(Guid id)
        {
            var item = await _postNoteService.GetLatestNoteByPostIdAsync(id);
            if (item == null)
            {
                return NotFound($"Latest note for post with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Gets a post note by ID.
        /// </summary>
        /// <param name="id">The post note ID.</param>
        /// <returns>The post note if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PostNotesDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostNotesDto>> GetByIdAsync(Guid id)
        {
            var item = await _postNoteService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Post note with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new post note.
        /// </summary>
        /// <param name="dto">The post note data to create.</param>
        /// <returns>The created post note.</returns>
        [HttpPost]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [ProducesResponseType(typeof(PostNotesDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PostNotesDto>> UpsertAsync([FromBody] PostNotesDto dto)
        {
            var createdItem = await _postNoteService.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Deletes a post note by ID.
        /// </summary>
        /// <param name="id">The post note ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _postNoteService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound($"Post note with ID {id} not found.");
            }
            return Ok(new { message = "Record deleted successfully." });
        }
    }
}
