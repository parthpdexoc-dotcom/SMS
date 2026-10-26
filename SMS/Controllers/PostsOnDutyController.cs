using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing posts on duty operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PostsOnDutyController : ControllerBase
    {
        private readonly IPostsOnDutyService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostsOnDutyController"/> class.
        /// </summary>
        /// <param name="service">The posts on duty service.</param>
        public PostsOnDutyController(IPostsOnDutyService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all posts on duty.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of posts on duty.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PostsOnDutyDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PostsOnDutyDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a posts on duty by ID.
        /// </summary>
        /// <param name="id">The posts on duty ID.</param>
        /// <returns>The posts on duty if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PostsOnDutyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostsOnDutyDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"PostsOnDuty with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new posts on duty.
        /// </summary>
        /// <param name="dto">The posts on duty data to create.</param>
        /// <returns>The created posts on duty.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PostsOnDutyDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PostsOnDutyDto>> UpsertAsync([FromBody] PostsOnDutyDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing posts on duty.
        /// </summary>
        /// <param name="id">The posts on duty ID.</param>
        /// <param name="dto">The posts on duty data to update.</param>
        /// <returns>The updated posts on duty.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PostsOnDutyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostsOnDutyDto>> UpdateAsync(int id, [FromBody] PostsOnDutyDto dto)
        {

            if (id != dto.PostsOnDutyID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"PostsOnDuty with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a posts on duty by ID.
        /// </summary>
        /// <param name="id">The posts on duty ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"PostsOnDuty with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

