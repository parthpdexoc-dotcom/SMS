using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing RequesterAuthorized operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
    public class RequesterAuthorizedController : ControllerBase
    {
        private readonly IRequesterAuthorizedService _service;

        public RequesterAuthorizedController(IRequesterAuthorizedService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedList<RequesterAuthorizedDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<RequesterAuthorizedDto>>> GetAllAsync(
        [FromQuery] PageParam? pagination = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] Guid? requesterId = null)
        {
            pagination ??= new PageParam();

            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(RequesterAuthorizedDto.FirstName);
            }

            var result = await _service.GetAllPagedAsyncWithFilters(isActive, requesterId, pagination);
            return Ok(result);
        }

        [HttpGet("GetAllActiveRequesterAuthorized")]
        [ProducesResponseType(typeof(IEnumerable<RequesterAuthorizedDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequesterAuthorizedDto>>> GetAllActiveRequesterAuthorizedAsync(
            [FromQuery] Guid? requesterId = null,
            [FromQuery] PageParam? pagination = null,
            [FromQuery] List<Guid>? selectedIds = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(RequesterAuthorizedDto.FirstName);
            }

            var selectedRequesterAuthorizedIds = (selectedIds ?? new List<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            var items = await _service.GetAllAsync(
                x => (!requesterId.HasValue || requesterId.Value == Guid.Empty || x.RequesterID == requesterId.Value) &&
                     (x.Status == true || selectedRequesterAuthorizedIds.Contains(x.RequesterAuthorizedID)),
                null,
                pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets lightweight requester authorized options for dropdowns (name label, searchable).
        /// </summary>
        [HttpPost("requesterauthorizedoptions")]
        [ProducesResponseType(typeof(IReadOnlyList<SelectOptionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<SelectOptionDto>>> GetRequesterAuthorizedOptionsAsync([FromBody] SelectOptionsRequestDto? body)
        {
            var request = SelectOptionsRequestDto.OrDefault(body);
            return Ok(await _service.GetOptionsAsync(request.SearchTerm, request.RequesterId, request.SelectedId));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(RequesterAuthorizedDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequesterAuthorizedDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound($"RequesterAuthorized with ID {id} not found.");

            return Ok(item);
        }

        [HttpPost]
        [ProducesResponseType(typeof(RequesterAuthorizedDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RequesterAuthorizedDto>> UpsertAsync([FromBody] RequesterAuthorizedDto dto)
        {
            var result = await _service.UpsertAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound($"RequesterAuthorized with ID {id} not found.");

            return NoContent();
        }
    }
}


