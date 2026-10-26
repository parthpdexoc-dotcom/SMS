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
    /// Controller for managing request approval routing operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RequestApprovalRoutingController : ControllerBase
    {
        private readonly IRequestApprovalRoutingService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestApprovalRoutingController"/> class.
        /// </summary>
        /// <param name="service">The request approval routing service.</param>
        public RequestApprovalRoutingController(IRequestApprovalRoutingService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all request approval routing records.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of request approval routing records.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequestApprovalRoutingDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<IEnumerable<RequestApprovalRoutingDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets all request approval routing records by request ID with routing group information.
        /// </summary>
        /// <param name="requestId">The request ID to filter by.</param>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of request approval routing records with routing group names.</returns>
        [HttpGet("request/{requestId}")]
        [ProducesResponseType(typeof(IEnumerable<RequestApprovalRoutingResponseDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.OperatorOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly),nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        public async Task<ActionResult<IEnumerable<RequestApprovalRoutingResponseDto>>> GetAllRequestRoutingGroupByRequestIdAsync(Guid requestId, [FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllRequestRoutingGroupByRequestIdAsync(requestId, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a request approval routing record by ID.
        /// </summary>
        /// <param name="id">The request approval route ID.</param>
        /// <returns>The request approval routing record if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RequestApprovalRoutingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<RequestApprovalRoutingDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Request approval routing with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a single request approval routing record.
        /// </summary>
        /// <param name="dto">The request approval routing data to create or update.</param>
        /// <returns>The created or updated request approval routing record.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RequestApprovalRoutingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<RequestApprovalRoutingDto>> UpsertAsync([FromBody] RequestApprovalRoutingDto dto)
        {
            var result = await _service.UpsertAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Creates or updates multiple request approval routing records with rank-based sequencing.
        /// </summary>
        /// <param name="dtos">The collection of request approval routing data to create or update.</param>
        /// <returns>The created or updated request approval routing records.</returns>
        [HttpPost("bulk")]
        [ProducesResponseType(typeof(IEnumerable<RequestApprovalRoutingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<IEnumerable<RequestApprovalRoutingDto>>> BulkUpsertAsync(
            [FromBody] IEnumerable<RequestApprovalRoutingDto> dtos,
            [FromHeader] bool isSuperUser = false)
        {
            var results = await _service.BulkUpsertAsync(dtos, isSuperUser);
            return Ok(results);
        }

        /// <summary>
        /// Deletes a request approval routing record by ID.
        /// </summary>
        /// <param name="id">The request approval route ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Request approval routing with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

