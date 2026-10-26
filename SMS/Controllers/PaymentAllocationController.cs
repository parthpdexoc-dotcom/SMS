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
    /// Controller for managing payment allocation operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentAllocationController : ControllerBase
    {
        private readonly IPaymentAllocationService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentAllocationController"/> class.
        /// </summary>
        /// <param name="service">The payment allocation service.</param>
        public PaymentAllocationController(IPaymentAllocationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all payment allocations.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of payment allocations.</returns>
        [HttpGet]
        [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
        [ProducesResponseType(typeof(IEnumerable<PaymentAllocationDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PaymentAllocationDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a payment allocation by ID.
        /// </summary>
        /// <param name="id">The payment allocation ID.</param>
        /// <returns>The payment allocation if found; otherwise, NotFound.</returns>
        [HttpGet("{id:guid}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
        [ProducesResponseType(typeof(PaymentAllocationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentAllocationDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"PaymentAllocation with ID {id} not found.");
            }

            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a payment allocation.
        /// </summary>
        /// <param name="dto">The payment allocation data.</param>
        /// <returns>The created or updated payment allocation.</returns>
        [HttpPost]
        [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
        [ProducesResponseType(typeof(PaymentAllocationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaymentAllocationDto>> UpsertAsync([FromBody] PaymentAllocationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.UpsertAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a payment allocation by ID.
        /// </summary>
        /// <param name="id">The payment allocation ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"PaymentAllocation with ID {id} not found.");
            }

            return NoContent();
        }

        /// <summary>
        /// Voids an active payment allocation and returns updated payment balances.
        /// </summary>
        [HttpPost("{id:guid}/void")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
        [ProducesResponseType(typeof(PaymentAllocateResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaymentAllocateResponseDto>> VoidAsync(
            Guid id,
            [FromBody] PaymentAllocationVoidRequestDto? request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(await _service.VoidPaymentAllocationAsync(id, request?.Reason ?? string.Empty, cancellationToken).ConfigureAwait(false));
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
        /// Updates active allocation amounts and recalculates affected invoices.
        /// </summary>
        [HttpPost("update-amount")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
        [ProducesResponseType(typeof(PaymentAllocationUpdateResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaymentAllocationUpdateResultDto>> UpdateAmountAsync(
            [FromBody] PaymentUpdateAllocationAmountDto dto,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                return Ok(await _service.UpdatePaymentAllocationAmountAsync(dto.PaymentId, dto.Allocations, cancellationToken: cancellationToken).ConfigureAwait(false));
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
        /// Applies credit allocations across payments for an invoice refund before unclose.
        /// </summary>
        [HttpPost("invoice-refund")]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [ProducesResponseType(typeof(InvoicePaymentSummaryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<InvoicePaymentSummaryResponseDto>> ApplyInvoiceRefundAsync(
            [FromBody] InvoiceRefundAllocationsDto dto,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                return Ok(await _service.ApplyInvoiceRefundAllocationsAsync(dto, cancellationToken).ConfigureAwait(false));
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
