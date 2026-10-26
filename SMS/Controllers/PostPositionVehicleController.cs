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
    /// Controller for managing post position vehicle operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PostPositionVehicleController : ControllerBase
    {
        private readonly IPostPositionVehicleService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostPositionVehicleController"/> class.
        /// </summary>
        /// <param name="service">The post position vehicle service.</param>
        public PostPositionVehicleController(IPostPositionVehicleService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all post position vehicle records.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of post position vehicle records.</returns>
        [HttpGet]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [ProducesResponseType(typeof(IEnumerable<PostPositionVehicleDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PostPositionVehicleDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets post position vehicle records for a post position.
        /// </summary>
        /// <param name="postPositionId">The post position identifier.</param>
        /// <param name="pagination">The pagination parameters (optional).</param>
        [HttpGet("by-post-position/{postPositionId:guid}")]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RegularEmployeeOnly), nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [ProducesResponseType(typeof(IEnumerable<PostPositionVehicleDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PostPositionVehicleDto>>> GetByPostPositionIdAsync(Guid postPositionId, [FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(v => v.PostPositionID == postPositionId, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a post position vehicle record by ID.
        /// </summary>
        /// <param name="id">The post position vehicle ID.</param>
        /// <returns>The post position vehicle record if found; otherwise, NotFound.</returns>
        [HttpGet("{id:guid}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [ProducesResponseType(typeof(PostPositionVehicleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostPositionVehicleDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"PostPositionVehicle with ID {id} not found.");
            }

            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a post position vehicle record.
        /// </summary>
        /// <param name="dto">The post position vehicle data to create or update.</param>
        /// <returns>The created or updated post position vehicle record.</returns>
        [HttpPost]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [ProducesResponseType(typeof(PostPositionVehicleDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PostPositionVehicleDto>> UpsertAsync([FromBody] PostPositionVehicleDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing post position vehicle record.
        /// </summary>
        /// <param name="id">The post position vehicle ID.</param>
        /// <param name="dto">The post position vehicle data to update.</param>
        /// <returns>The updated post position vehicle record.</returns>
        [HttpPut("{id:guid}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [ProducesResponseType(typeof(PostPositionVehicleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostPositionVehicleDto>> UpdateAsync(Guid id, [FromBody] PostPositionVehicleDto dto)
        {
            if (dto.PostPositionVehicleID != id)
            {
                return BadRequest("ID mismatch between route and body.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a post position vehicle record by ID.
        /// </summary>
        /// <param name="id">The post position vehicle ID.</param>
        /// <returns>Ok if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound($"PostPositionVehicle with ID {id} not found.");
            }

            return Ok(new { message = "Record deleted successfully." });
        }
    }
}
