using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing RequestSchedule operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RequestScheduleController : ControllerBase
    {
        private readonly IRequestScheduleService _requestScheduleService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestScheduleController"/> class.
        /// </summary>
        /// <param name="requestScheduleService">The RequestSchedule service.</param>
        public RequestScheduleController(IRequestScheduleService requestScheduleService)
        {
            _requestScheduleService = requestScheduleService;
        }

        /// <summary>
        /// Gets all RequestSchedule records.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of RequestSchedule records.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequestScheduleDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequestScheduleDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _requestScheduleService.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets RequestSchedule records for a request. Optional <paramref name="startDate"/> and <paramref name="endDate"/>
        /// keep windows that overlap that range. Optional <paramref name="releaseAs"/> keeps the selected release types.
        /// Optional <paramref name="requestStatus"/> keeps the selected release statuses.
        /// Results are ordered by start date, oldest first.
        /// </summary>
        [HttpGet("by-request/{requestId}")]
        [ProducesResponseType(typeof(PagedList<RequestScheduleDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<RequestScheduleDto>>> GetAllByRequestIdAsync(
            Guid requestId,
            [FromQuery] DateOnly? startDate = null,
            [FromQuery] DateOnly? endDate = null,
            [FromQuery] List<ReleaseAs>? releaseAs = null,
            [FromQuery] List<RequestStatus>? requestStatus = null,
            [FromQuery] PageParam? pagination = null)
        {
            pagination ??= new PageParam();
            pagination.SortBy = nameof(RequestScheduleDto.StartDate);
            pagination.SortDirection = "asc";

            var items = await _requestScheduleService.GetByRequestIdAsync(
                requestId,
                startDate,
                endDate,
                releaseAs,
                requestStatus,
                pagination);

            return Ok(items);
        }

        /// <summary>
        /// Gets a RequestSchedule record by ID.
        /// </summary>
        /// <param name="id">The RequestSchedule ID.</param>
        /// <returns>The record if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RequestScheduleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequestScheduleDto>> GetByIdAsync(Guid id)
        {
            var item = await _requestScheduleService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"RequestSchedule with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a RequestSchedule record.
        /// </summary>
        /// <param name="dto">The RequestSchedule data.</param>
        /// <returns>The upserted RequestSchedule record.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RequestScheduleDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RequestScheduleDto>> UpsertAsync([FromBody] RequestScheduleDto dto)
        {
            var result = await _requestScheduleService.UpsertAsync(dto);
            return result;
        }

        /// <summary>
        /// Generates new schedules from request type + release before date.
        /// </summary>
        [HttpPost("generate")]
        [ProducesResponseType(typeof(RequestScheduleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RequestScheduleDto>> GenerateAsync([FromBody] GenerateRequestScheduleDto dto)
        {
            var result = await _requestScheduleService.GenerateAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a RequestSchedule record by ID.
        /// </summary>
        /// <param name="id">The RequestSchedule ID.</param>
        /// <returns>Ok if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _requestScheduleService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound($"RequestSchedule with ID {id} not found.");
            }
            return Ok(new { message = "Record deleted successfully." });
        }
    }
}

