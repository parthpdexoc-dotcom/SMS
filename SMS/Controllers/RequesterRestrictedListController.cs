using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing requester restricted list operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
    public class RequesterRestrictedListController : ControllerBase
    {
        private readonly IRequesterRestrictedListService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterRestrictedListController"/> class.
        /// </summary>
        /// <param name="service">The requester restricted list service.</param>
        public RequesterRestrictedListController(IRequesterRestrictedListService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all requester restricted list records with requester and employee names.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <param name="requesterId">When set, only rows for this requester are returned.</param>
        /// <returns>Paged requester restricted list records with names.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<RequesterRestrictedListResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<RequesterRestrictedListResponseDto>>> GetAllAsync(
            [FromQuery] PageParam? pagination = null,
            [FromQuery] Guid? requesterId = null)
        {
            var result = await _service.GetAllWithNamesPagedAsync(null, null, pagination, requesterId);
            return Ok(result);
        }

        /// <summary>
        /// Gets a requester restricted list record by ID with requester and employee names.
        /// </summary>
        /// <param name="id">The requester restricted list ID (RequesterRestrictID as Guid).</param>
        /// <returns>The requester restricted list record with names if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RequesterRestrictedListResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequesterRestrictedListResponseDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdWithNamesAsync(id);
            if (item == null)
            {
                return NotFound($"RequesterRestrictedList with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new requester restricted list record.
        /// </summary>
        /// <param name="dto">The requester restricted list data to create.</param>
        /// <returns>The created requester restricted list record.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RequesterRestrictedListDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RequesterRestrictedListDto>> UpsertAsync([FromBody] RequesterRestrictedListDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Gets employee options for the restrict form, excluding employees already restricted for the requester.
        /// </summary>
        [HttpPost("availableemployeeoptions")]
        [ProducesResponseType(typeof(IReadOnlyList<SelectOptionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<SelectOptionDto>>> GetAvailableEmployeeOptionsAsync(
            [FromBody] SelectOptionsRequestDto? body)
        {
            var request = SelectOptionsRequestDto.OrDefault(body);
            return Ok(await _service.GetAvailableEmployeeOptionsAsync(request.SearchTerm, request.RequesterId));
        }

    }
}

