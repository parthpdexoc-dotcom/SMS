using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Filters;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing post equipment operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PostEquipmentController : ControllerBase
    {
        private readonly IPostEquipmentService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostEquipmentController"/> class.
        /// </summary>
        /// <param name="service">The post equipment service.</param>
        public PostEquipmentController(IPostEquipmentService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all post equipment records.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of post equipment records.</returns>
        [HttpGet]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [ProducesResponseType(typeof(IEnumerable<PostEquipmentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PostEquipmentDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }
        /// <summary>Gets post equipment for a post, including resolved equipment type display names.</summary>
        /// <param name="postId">The post identifier.</param>
        /// <param name="pagination">The pagination.</param>
        [HttpGet("by-post/{postId:guid}")]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RegularEmployeeOnly), nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [ProducesResponseType(typeof(IEnumerable<PostEquipmentWithNameDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PostEquipmentWithNameDto>>> GetPostEquipmentByPostIdAsync(Guid postId, [FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetByPostIdWithNamesAsync(postId, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a post equipment record by ID.
        /// </summary>
        /// <param name="id">The post equipment ID.</param>
        /// <returns>The post equipment record if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [ProducesResponseType(typeof(PostEquipmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostEquipmentDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"PostEquipment with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a post equipment record.
        /// </summary>
        /// <param name="dto">The post equipment data to create or update.</param>
        /// <returns>The created or updated post equipment record.</returns>
        [HttpPost]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [ProducesResponseType(typeof(PostEquipmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PostEquipmentDto>> UpsertAsync([FromBody] PostEquipmentDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing post equipment record.
        /// </summary>
        /// <param name="id">The post equipment ID.</param>
        /// <param name="dto">The post equipment data to update.</param>
        /// <returns>The updated post equipment record.</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [ProducesResponseType(typeof(PostEquipmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostEquipmentDto>> UpdateAsync(Guid id, [FromBody] PostEquipmentDto dto)
        {
            if (dto.PostEquipmentID != id)
            {
                return BadRequest("ID mismatch between route and body.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a post equipment record by ID.
        /// </summary>
        /// <param name="id">The post equipment ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound($"PostEquipment with ID {id} not found.");
            }
            return Ok(new { message = "Record deleted successfully." });
        }
    }
}
