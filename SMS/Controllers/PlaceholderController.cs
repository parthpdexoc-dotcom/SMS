using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Common.Implementations.Responses;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for placeholder profile operations (virtual assignment names).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
    public class PlaceholderController : ControllerBase
    {
        private readonly IPlaceholderService _service;

        public PlaceholderController(IPlaceholderService service)
        {
            _service = service;
        }

        /// <param name="status">
        /// When set, only that status (Visible, Hidden, or Archived); when omitted, Visible and Hidden.
        /// </param>
        /// <param name="pagination">Optional paging/sorting parameters.</param>
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<PlaceholderDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<PlaceholderDto>>> GetAllAsync(
            [FromQuery] PlaceholderStatus? status = null,
            [FromQuery] PageParam? pagination = null)
        {
            var result = await _service.GetAllAsync(status, pagination).ConfigureAwait(false);
            return Ok(result);
        }

        /// <param name="status">
        /// When set, search that status only; when omitted, search Visible and Hidden.
        /// </param>
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<PlaceholderDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<PlaceholderDto>>> SearchAsync(
            [FromQuery] string? searchTerm,
            [FromQuery] PlaceholderStatus? status = null)
        {
            var items = await _service.SearchAsync(searchTerm, status).ConfigureAwait(false);
            return Ok(new PagedList<PlaceholderDto>(items, new PageParam(), items.Count));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(PlaceholderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PlaceholderDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id).ConfigureAwait(false);
            if (item == null)
                return NotFound($"Placeholder with ID {id} not found.");
            return Ok(item);
        }

        [HttpPost]
        [ProducesResponseType(typeof(PlaceholderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PlaceholderDto>> UpsertAsync([FromBody] PlaceholderDto dto)
        {
            var result = await _service.UpsertAsync(dto).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost("bulk-status")]
        [ProducesResponseType(typeof(SuccessResult<List<BulkPlaceholderStatusItemDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<SuccessResult<List<BulkPlaceholderStatusItemDto>>>> BulkUpdateStatusAsync([FromBody] BulkPlaceholderStatusRequestDto request)
        {
            return Ok(await _service.BulkUpdateStatusAsync(request.Placeholders).ConfigureAwait(false));
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id).ConfigureAwait(false);
            if (!deleted)
                return NotFound($"Placeholder with ID {id} not found.");
            return NoContent();
        }
    }
}
