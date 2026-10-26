using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Services.Interfaces;
using System.Linq.Expressions;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing RequesterManager operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RequesterManagerController : ControllerBase
    {
        private readonly IRequesterManagerService _service;

        public RequesterManagerController(IRequesterManagerService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequesterManagerDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequesterManagerDto>>> GetAllAsync(
            [FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        [HttpGet("getallRequestManagerwithdetails")]
        [ProducesResponseType(typeof(PagedList<RequestManagerResponceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<RequestManagerResponceDto>>> GetAllManagerWithDetails(
            [FromQuery] PageParam? pageParam = null,
            [FromQuery] Guid? requesterId = null)
        {
            var result = await _service.GetAllRequestManagerWithDetailsAsync(pageParam, requesterId);
            return Ok(result);
        }

        /// <summary>
        /// Gets lightweight requester manager options for dropdowns (employee name label, searchable).
        /// </summary>
        [HttpPost("requestermanageroptions")]
        [ProducesResponseType(typeof(IReadOnlyList<SelectOptionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<SelectOptionDto>>> GetRequesterManagerOptionsAsync([FromBody] SelectOptionsRequestDto? body)
        {
            var request = SelectOptionsRequestDto.OrDefault(body);
            return Ok(await _service.GetOptionsAsync(request.SearchTerm, request.RequesterId, request.SelectedId));
        }

        /// <summary>
        /// Gets employee options for the manager form, excluding employees already assigned for the requester.
        /// </summary>
        [HttpPost("availableemployeeoptions")]
        [ProducesResponseType(typeof(IReadOnlyList<SelectOptionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<SelectOptionDto>>> GetAvailableEmployeeOptionsAsync(
            [FromBody] SelectOptionsRequestDto? body)
        {
            var request = SelectOptionsRequestDto.OrDefault(body);
            return Ok(await _service.GetAvailableEmployeeOptionsAsync(request.SearchTerm, request.RequesterId));
        }

        /// <summary>
        /// Lookup a single RequesterManager by requester and employee (manager) IDs. 404 if no row exists.
        /// </summary>
        [HttpGet("by-requester-and-employee")]
        [ProducesResponseType(typeof(RequesterManagerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequesterManagerDto>> GetByRequesterAndEmployeeAsync(
            [FromQuery] Guid requesterId,
            [FromQuery] Guid employeeId)
        {
            var item = await _service.GetByRequesterIdAndEmployeeIdAsync(requesterId, employeeId);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(RequesterManagerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequesterManagerDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"RequesterManager with ID {id} not found.");
            }
            return Ok(item);
        }

        [HttpPost]
        [ProducesResponseType(typeof(RequesterManagerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        public async Task<ActionResult<RequesterManagerDto>> UpsertAsync([FromBody] RequesterManagerDto dto)
        {
            var result = await _service.UpsertAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]

        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound($"RequesterManager with ID {id} not found.");

            return NoContent();
        }
    }
}


