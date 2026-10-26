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
    /// Controller for managing request equipment operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RequestEquipmentController : ControllerBase
    {
        private readonly IRequestEquipmentService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestEquipmentController"/> class.
        /// </summary>
        /// <param name="service">The request equipment service.</param>
        public RequestEquipmentController(IRequestEquipmentService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all request equipment records.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of request equipment records.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequestEquipmentDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]

        public async Task<ActionResult<IEnumerable<RequestEquipmentDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }
        /// <summary>Gets request equipment for a request, including resolved equipment type display names.</summary>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="pagination">The pagination.</param>
        [HttpGet("by-request/{requestId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<RequestEquipmentWithNameDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RegularEmployeeOnly), nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        public async Task<ActionResult<IEnumerable<RequestEquipmentWithNameDto>>> GetRequestEquipmentByRequestIdAsync(Guid requestId, [FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetByRequestIdWithNamesAsync(requestId, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a request equipment record by ID.
        /// </summary>
        /// <param name="id">The request equipment ID.</param>
        /// <returns>The request equipment record if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RequestEquipmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]

        public async Task<ActionResult<RequestEquipmentDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"RequestEquipment with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a request equipment record.
        /// </summary>
        /// <param name="dto">The request equipment data to create or update.</param>
        /// <returns>The created or updated request equipment record.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RequestEquipmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]

        public async Task<ActionResult<RequestEquipmentDto>> UpsertAsync([FromBody] RequestEquipmentDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing request equipment record.
        /// </summary>
        /// <param name="id">The request equipment ID.</param>
        /// <param name="dto">The request equipment data to update.</param>
        /// <returns>The updated request equipment record.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RequestEquipmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]

        public async Task<ActionResult<RequestEquipmentDto>> UpdateAsync(Guid id, [FromBody] RequestEquipmentDto dto)
        {
            if (dto.RequestEquipmentID != id)
            {
                return BadRequest("ID mismatch between route and body.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a request equipment record by ID.
        /// </summary>
        /// <param name="id">The request equipment ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]

        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound($"RequestEquipment with ID {id} not found.");
            }
            return Ok(new { message = "Record deleted successfully." });
        }
    }
}
