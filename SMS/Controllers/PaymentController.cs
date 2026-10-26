using System.Linq.Expressions;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Common;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(
                null,
                new Expression<Func<Payment, object>>[] { p => p.PaymentTypeMaster! },
                pagination);
            return Ok(items);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Payment with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Gets payment balances and invoices that received allocations from this payment (paid or partially paid).
        /// </summary>
        [HttpGet("{paymentId:guid}/detail")]
        [ProducesResponseType(typeof(PaymentInvoicesResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentInvoicesResponseDto>> GetDetailAsync(
            [FromRoute] Guid paymentId,
            [FromQuery] PageParam? pagination = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _service.GetPaymentDetailAsync(paymentId, pagination, cancellationToken).ConfigureAwait(false);
            if (result == null)
                return NotFound($"Payment with ID {paymentId} not found.");

            return Ok(result);
        }

        /// <summary>
        /// Gets payment balances and unpaid requester invoices for allocating this payment.
        /// </summary>
        [HttpGet("{paymentId:guid}/unpaid-invoices")]
        [ProducesResponseType(typeof(PaymentInvoicesResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentInvoicesResponseDto>> GetUnpaidInvoicesAsync(
            [FromRoute] Guid paymentId,
            [FromQuery] Guid requesterId,
            [FromQuery] PageParam? pagination = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _service.GetPaymentUnpaidInvoicesAsync(paymentId, requesterId, pagination, cancellationToken).ConfigureAwait(false);
                if (result == null)
                    return NotFound($"Payment with ID {paymentId} not found.");

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Allocates an existing payment to one or more invoices in list order.
        /// </summary>
        [HttpPost("allocate")]
        [ProducesResponseType(typeof(PaymentAllocateResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaymentAllocateResponseDto>> AllocateAsync(
            [FromBody] PaymentAllocateDto dto,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                return Ok(await _service.AllocatePaymentAsync(dto, cancellationToken).ConfigureAwait(false));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PaymentDto>> UpsertAsync([FromBody] PaymentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdItem = await _service.UpsertAsync(dto);
                return createdItem;
            }
            catch (WebApiException ex) when (ex.HttpStatusCode == HttpStatusCode.Conflict)
            {
                return Conflict(ex.ErrorMessage);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PaymentDto>> UpdateAsync(Guid id, [FromBody] PaymentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.PaymentID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"Payment with ID {id} not found.");
            }

            try
            {
                var updatedItem = await _service.UpsertAsync(dto);
                return Ok(updatedItem);
            }
            catch (WebApiException ex) when (ex.HttpStatusCode == HttpStatusCode.Conflict)
            {
                return Conflict(ex.ErrorMessage);
            }
        }

        /// <summary>
        /// Gets payment balances and active allocations for the review / readjust screen.
        /// </summary>
        [HttpGet("{paymentId:guid}/review-allocations")]
        [ProducesResponseType(typeof(PaymentReviewAllocationsResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentReviewAllocationsResponseDto>> GetReviewAllocationsAsync(
            [FromRoute] Guid paymentId,
            [FromQuery] PageParam? pagination = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _service.GetPaymentReviewAllocationsAsync(paymentId, pagination, cancellationToken).ConfigureAwait(false);
            if (result == null)
                return NotFound($"Payment with ID {paymentId} not found.");

            return Ok(result);
        }

        /// <summary>
        /// Updates allocation amounts, recalculates affected invoices, then updates the payment amount.
        /// </summary>
        [HttpPost("adjust-allocations")]
        [ProducesResponseType(typeof(PaymentAdjustAllocationsResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PaymentAdjustAllocationsResultDto>> AdjustAllocationsAsync([FromBody] PaymentAdjustAllocationsDto dto,CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.AdjustPaymentAmountAgainstAllocationsAsync(dto, cancellationToken).ConfigureAwait(false);
                return Ok(result);
            }
            catch (WebApiException ex) when (ex.HttpStatusCode == HttpStatusCode.Conflict)
            {
                return Conflict(ex.ErrorMessage);
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
        /// Voids a payment and all active allocations on linked invoices, then recalculates each affected invoice.
        /// </summary>
        [HttpPost("{paymentId:guid}/void")]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaymentDto>> VoidAsync(
            [FromRoute] Guid paymentId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(await _service.VoidPaymentAsync(paymentId, cancellationToken).ConfigureAwait(false));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Requester payment records for a date range with per-payment balances, allocation status, and overall totals.
        /// </summary>
        [HttpGet("requester-payment-summary")]
        [ProducesResponseType(typeof(RequesterPaymentSummaryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RequesterPaymentSummaryResponseDto>> GetRequesterPaymentSummaryAsync(
            [FromQuery] Guid requesterId,
            [FromQuery] DateOnly? startDate = null,
            [FromQuery] DateOnly? endDate = null,
            [FromQuery] PageParam? pagination = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _service.GetRequesterPaymentHistoryByRequesterIdAndDateRange(requesterId, startDate, endDate, pagination, cancellationToken).ConfigureAwait(false);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Requester payments with escrow balance available to refund (server-filtered; only unallocated balance &gt; 0).
        /// </summary>
        [HttpGet("requester-refundable-payments")]
        [ProducesResponseType(typeof(RequesterPaymentSummaryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RequesterPaymentSummaryResponseDto>> GetRequesterRefundablePaymentsAsync(
            [FromQuery] Guid requesterId,
            [FromQuery] PageParam? pagination = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _service.GetRequesterRefundablePaymentsAsync(requesterId, pagination, cancellationToken).ConfigureAwait(false);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Soft-deletes a payment: blocks when active allocations exist, posts ledger reversal, requires reason.
        /// </summary>
        [HttpPost("{paymentId:guid}/delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] Guid paymentId,
            [FromBody] PaymentDeleteRequestDto request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var deleted = await _service.DeleteAsync(paymentId, request?.Reason ?? string.Empty, cancellationToken).ConfigureAwait(false);
                return deleted ? NoContent() : NotFound($"Payment with ID {paymentId} not found.");
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
