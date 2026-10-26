using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    public class RefundPaymentAllocationController : ControllerBase
    {
        private readonly IRefundPaymentAllocationService _service;

        public RefundPaymentAllocationController(IRefundPaymentAllocationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Voids (discards) a single refund payment allocation with reason, freeing escrow on the linked payment.
        /// </summary>
        [HttpPost("{id:guid}/void")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VoidAsync(
            Guid id,
            [FromBody] PaymentAllocationVoidRequestDto? request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _service.VoidRefundPaymentAllocationAsync(id, request?.Reason ?? string.Empty, cancellationToken).ConfigureAwait(false);
                return NoContent();
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
