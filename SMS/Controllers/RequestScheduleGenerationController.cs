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
    public class RequestScheduleGenerationController : ControllerBase
    {
        private readonly IRequestScheduleGenerationService _service;

        public RequestScheduleGenerationController(IRequestScheduleGenerationService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequestScheduleGenerationDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequestScheduleGenerationDto>>> GetAllAsync(
            [FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        [HttpGet("by-request/{requestId}")]
        [ProducesResponseType(typeof(PagedList<RequestScheduleGenerationDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<RequestScheduleGenerationDto>>> GetByRequestIdAsync(
            Guid requestId,
            [FromQuery] PageParam? pagination = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(RequestScheduleGenerationDto.CreatedDate);
                pagination.SortDirection = "desc";
            }

            return Ok(await _service.GetByRequestIdAsync(requestId, pagination));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RequestScheduleGenerationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequestScheduleGenerationDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound($"RequestScheduleGeneration with ID {id} not found.");
            return Ok(item);
        }

        [HttpPost]
        [ProducesResponseType(typeof(RequestScheduleGenerationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RequestScheduleGenerationDto>> UpsertAsync(
            [FromBody] RequestScheduleGenerationDto dto)
        {
            return Ok(await _service.UpsertAsync(dto));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound($"RequestScheduleGeneration with ID {id} not found.");
            return NoContent();
        }
    }
}
