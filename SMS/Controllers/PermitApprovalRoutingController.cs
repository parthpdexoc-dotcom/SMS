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
    /// Controller for managing permit approval routing operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeAny(nameof(AuthorizationPolicyType.DepartmentAdminOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
    public class PermitApprovalRoutingController : ControllerBase
    {
        private readonly IPermitApprovalRoutingService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PermitApprovalRoutingController"/> class.
        /// </summary>
        /// <param name="service">The permit approval routing service.</param>
        public PermitApprovalRoutingController(IPermitApprovalRoutingService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all permit approval routing records.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of permit approval routing records.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PermitApprovalRoutingDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PermitApprovalRoutingDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets all permit approval routing records by request ID with routing group information.
        /// </summary>
        /// <param name="requestId">The request ID to filter by.</param>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of permit approval routing records with routing group names.</returns>
        [HttpGet("request/{requestId}")]
        [ProducesResponseType(typeof(IEnumerable<PermitApprovalRoutingResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PermitApprovalRoutingResponseDto>>> GetAllByRequestIdAsync(Guid requestId, [FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllByRequestIdAsync(requestId, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a permit approval routing record by ID.
        /// </summary>
        /// <param name="id">The permit approval routing ID.</param>
        /// <returns>The permit approval routing record if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PermitApprovalRoutingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PermitApprovalRoutingDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Permit approval routing with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a single permit approval routing record.
        /// </summary>
        /// <param name="dto">The permit approval routing data to create or update.</param>
        /// <returns>The created or updated permit approval routing record.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PermitApprovalRoutingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PermitApprovalRoutingDto>> UpsertAsync([FromBody] PermitApprovalRoutingDto dto)
        {
            var result = await _service.UpsertAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Replaces all permit approval routing records for a request with the provided list.
        /// Send multiple records with the same RequestID, e.g. [{ requestID, routingGroupId }, { requestID, routingGroupId }, ...].
        /// Existing routings for that RequestID are removed and replaced by the list.
        /// </summary>
        /// <param name="dtos">Collection of { RequestID, RoutingGroupId } (all must have the same RequestID).</param>
        /// <returns>The inserted permit approval routing records.</returns>
        [HttpPost("bulk")]
        [ProducesResponseType(typeof(IEnumerable<PermitApprovalRoutingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<PermitApprovalRoutingDto>>> BulkUpsertByRequestIdAsync([FromBody] IEnumerable<PermitApprovalRoutingDto> dtos)
        {
            var results = await _service.BulkUpsertByRequestIdAsync(dtos);
            return Ok(results);
        }

        /// <summary>
        /// Deletes a permit approval routing record by ID.
        /// </summary>
        /// <param name="id">The permit approval routing ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Permit approval routing with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
