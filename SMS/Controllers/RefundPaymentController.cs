using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    public class RefundPaymentController : ControllerBase
    {
        private readonly IRefundPaymentService _service;

        public RefundPaymentController(IRefundPaymentService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RefundPaymentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RefundPaymentDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(RefundPaymentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RefundPaymentDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound($"RefundPayment with ID {id} not found.");

            return Ok(item);
        }

        [HttpPost]
        [ProducesResponseType(typeof(RefundPaymentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RefundPaymentDto>> UpsertAsync([FromBody] RefundPaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(await _service.UpsertAsync(dto));
        }

        [HttpPost("allocate")]
        [ProducesResponseType(typeof(RefundPaymentAllocateResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RefundPaymentAllocateResponseDto>> AllocateAsync(
            [FromBody] RefundPaymentAllocateDto dto,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                return Ok(await _service.AllocateRefundPaymentAsync(dto, cancellationToken).ConfigureAwait(false));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Requester refund payment records for a date range with overall totals.
        /// </summary>
        [HttpGet("requester-refund-summary")]
        [ProducesResponseType(typeof(RequesterRefundPaymentSummaryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RequesterRefundPaymentSummaryResponseDto>> GetRequesterRefundSummaryAsync(
            [FromQuery] Guid requesterId,
            [FromQuery] DateOnly? startDate = null,
            [FromQuery] DateOnly? endDate = null,
            [FromQuery] PageParam? pagination = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _service
                    .GetRequesterRefundHistoryByRequesterIdAndDateRange(requesterId, startDate, endDate, pagination, cancellationToken)
                    .ConfigureAwait(false);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Refund payment header and allocations for the detail view.
        /// </summary>
        [HttpGet("{refundPaymentId:guid}/allocations")]
        [ProducesResponseType(typeof(RefundPaymentAllocationsResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RefundPaymentAllocationsResponseDto>> GetAllocationsAsync(
            [FromRoute] Guid refundPaymentId,
            [FromQuery] PageParam? pagination = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _service
                .GetRefundPaymentAllocationsAsync(refundPaymentId, pagination, cancellationToken)
                .ConfigureAwait(false);
            if (result == null)
                return NotFound($"RefundPayment with ID {refundPaymentId} not found.");

            return Ok(result);
        }

        /// <summary>
        /// Refund payment header and full allocation history for the detail view.
        /// </summary>
        [HttpGet("{refundPaymentId:guid}/allocation-history")]
        [ProducesResponseType(typeof(RefundPaymentAllocationHistoryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RefundPaymentAllocationHistoryResponseDto>> GetAllocationHistoryAsync(
            [FromRoute] Guid refundPaymentId,
            CancellationToken cancellationToken = default)
        {
            var result = await _service
                .GetRefundPaymentAllocationHistoryAsync(refundPaymentId, cancellationToken)
                .ConfigureAwait(false);
            if (result == null)
                return NotFound($"RefundPayment with ID {refundPaymentId} not found.");

            return Ok(result);
        }

        [HttpPost("{refundPaymentId:guid}/delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] Guid refundPaymentId,
            [FromBody] RefundPaymentDeleteRequestDto request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var deleted = await _service.DeleteAsync(refundPaymentId, request?.Reason ?? string.Empty, cancellationToken).ConfigureAwait(false);
                return deleted ? NoContent() : NotFound($"RefundPayment with ID {refundPaymentId} not found.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
