using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing post type operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PostTypeController : ControllerBase
    {
        private readonly IPostTypeService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostTypeController"/> class.
        /// </summary>
        /// <param name="service">The post type service.</param>
        public PostTypeController(IPostTypeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all post types.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of post types.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PostTypeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PostTypeDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a post type by ID.
        /// </summary>
        /// <param name="id">The post type ID.</param>
        /// <returns>The post type if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PostTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostTypeDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"PostType with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new post type.
        /// </summary>
        /// <param name="dto">The post type data to create.</param>
        /// <returns>The created post type.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PostTypeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PostTypeDto>> UpsertAsync([FromBody] PostTypeDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing post type.
        /// </summary>
        /// <param name="id">The post type ID.</param>
        /// <param name="dto">The post type data to update.</param>
        /// <returns>The updated post type.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PostTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostTypeDto>> UpdateAsync(int id, [FromBody] PostTypeDto dto)
        {

            if (id != dto.PostTypeID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"PostType with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a post type by ID.
        /// </summary>
        /// <param name="id">The post type ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"PostType with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

