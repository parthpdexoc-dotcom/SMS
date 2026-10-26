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
    public class RequesterLedgerController : ControllerBase
    {
        private readonly IRequesterLedgerService _service;

        public RequesterLedgerController(IRequesterLedgerService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequesterLedgerDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequesterLedgerDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(RequesterLedgerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequesterLedgerDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound($"Requester ledger entry with ID {id} not found.");

            return Ok(item);
        }

        /// <summary>
        /// Bank-statement style ledger for a requester.
        /// </summary>
        [HttpGet("by-requester/{requesterId:guid}")]
        [ProducesResponseType(typeof(RequesterLedgerStatementDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<RequesterLedgerStatementDto>> GetRequesterLedgerStatementAsync(
            [FromRoute] Guid requesterId,
            [FromQuery] DateTime? startDateTime = null,
            [FromQuery] DateTime? endDateTime = null,
            [FromQuery] PageParam? pagination = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _service
                .GetRequesterLedgerStatementAsync(requesterId, startDateTime, endDateTime, pagination, cancellationToken)
                .ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// Running balance for a requester from the latest ledger row.
        /// </summary>
        [HttpGet("by-requester/{requesterId:guid}/balance")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        public async Task<ActionResult<decimal>> GetRequesterRunningBalanceAsync(
            [FromRoute] Guid requesterId,
            CancellationToken cancellationToken = default)
        {
            var balance = await _service
                .GetRequesterCurrentBalanceAsync(requesterId, cancellationToken)
                .ConfigureAwait(false);

            return Ok(balance);
        }
    }
}
