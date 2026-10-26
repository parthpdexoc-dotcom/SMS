using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Filters;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing requester operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RequesterController : ControllerBase
    {
        private readonly IRequesterService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterController"/> class.
        /// </summary>
        /// <param name="service">The requester service.</param>
        public RequesterController(IRequesterService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all requester.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of requester.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequesterDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequesterDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.IAAuditViewOnly))]
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<RequesterDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<RequesterDto>>> SearchAsync(
            [FromQuery] string? searchTerm,
            [FromQuery] bool? isActive,
            [FromQuery] PageParam? pageParam = null)
        {
            pageParam ??= new PageParam();

            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
            {
                pageParam.SortBy = nameof(RequesterDto.Name);
            }

            var result = await _service.SearchAsync(
                searchTerm,
                pageParam,
                isActive.HasValue ? x => x.Status == isActive.Value : null
            );

            return Ok(result);
        }
        /// <summary>Gets all requesters with details. Requires RequesterManagerOnly OR IAAuditViewOnly policy.</summary>
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.IAAuditViewOnly))]
        [HttpGet("getallrequesterwithdetails")]
        public async Task<IActionResult> GetAllRequesterWithDetails([FromQuery] PageParam pageParam)
        {
            pageParam ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
            {
                pageParam.SortBy = nameof(RequesterDto.Name);
            }

            var result = await _service.GetAllRequesterWithDetailsAsync(pageParam);
            return Ok(result);
        }

        /// <summary>
        /// Gets lightweight requester options for dropdowns (requesterId + name only).
        /// </summary>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpPost("requesteroptions")]
        [ProducesResponseType(typeof(IReadOnlyList<SelectOptionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<SelectOptionDto>>> GetRequesterOptionsAsync([FromBody] SelectOptionsRequestDto? body)
        {
            var request = SelectOptionsRequestDto.OrDefault(body);
            return Ok(await _service.GetOptionsAsync(
                r => r.Name!,
                x => x.Status == true,
                request.SearchTerm));
        }

        /// <summary>
        /// Gets a requester by ID.
        /// </summary>
        /// <param name="id">The requester ID (RequesterID as Guid).</param>
        /// <returns>The requester if found; otherwise, NotFound.</returns>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RequesterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequesterDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Requester with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new requester.
        /// </summary>
        /// <param name="dto">The requester data to create.</param>
        /// <returns>The created requester.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpPost]
        [ProducesResponseType(typeof(RequesterDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RequesterDto>> UpsertAsync([FromBody] RequesterDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing requester.
        /// </summary>
        /// <param name="id">The requester ID (RequesterID as Guid).</param>
        /// <param name="dto">The requester data to update.</param>
        /// <returns>The updated requester.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RequesterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequesterDto>> UpdateAsync(Guid id, [FromBody] RequesterDto dto)
        {

            if (dto.RequesterID.HasValue && id != dto.RequesterID.Value)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"Requester with ID {id} not found.");
            }

            // Ensure the DTO has the ID from the URL
            dto.RequesterID = id;

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a requester by ID.
        /// </summary>
        /// <param name="id">The requester ID (RequesterID as Guid).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Requester with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

