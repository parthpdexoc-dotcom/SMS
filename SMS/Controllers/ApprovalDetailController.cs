using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    /// Controller for managing approval detail operations (ApprovalDetails table).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ApprovalDetailController : ControllerBase
    {
        private readonly IApprovalDetailService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApprovalDetailController"/> class.
        /// </summary>
        public ApprovalDetailController(IApprovalDetailService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all approval detail records.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ApprovalDetailDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ApprovalDetailDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets the approval request detail for the approval page using requestId, routingGroupId, and approvalType.
        /// </summary>
        [HttpGet("approval-request-detail/{requestId:guid}/{routingGroupId:guid}/{approvalType:int}")]
        [ProducesResponseType(typeof(ApprovalRequestDetailResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApprovalRequestDetailResponseDto>> GetApprovalRequestDetailAsync(
            Guid requestId,
            Guid routingGroupId,
            int approvalType,
            CancellationToken cancellationToken = default)
        {
            var result = await _service.GetApprovalRequestDetailAsync(requestId, routingGroupId, (ApprovalType)approvalType, cancellationToken);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Gets an approval detail record by ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApprovalDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApprovalDetailDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound($"ApprovalDetail with ID {id} not found.");

            return Ok(item);
        }

        /// <summary>
        /// Creates or updates an approval detail.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApprovalDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApprovalDetailDto>> UpsertAsync([FromBody] ApprovalDetailDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpsertAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Deletes an approval detail record by ID.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound($"ApprovalDetail with ID {id} not found.");

            return NoContent();
        }
    }
}
