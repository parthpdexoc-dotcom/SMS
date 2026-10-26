using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;
using SMS.Generic.Services.Extensions;
using SMS.Filters;
using System.Linq.Expressions;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing skill operations.
    /// GET list/ById: DepartmentAdmin, HRAdmin, SuperUser. Create/Update/Delete: DepartmentAdminOnly (DepartmentAdmin, SuperUser).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SkillController : ControllerBase
    {
        private readonly ISkillService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillController"/> class.
        /// </summary>
        /// <param name="service">The skill service.</param>
        public SkillController(ISkillService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all skills.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <param name="isActive">When true, only active skills; when false, only inactive (non-active) skills; when omitted, no status filter.</param>
        /// <returns>Paged skills (<see cref="PagedList{SkillDto}"/>).</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<SkillDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOrHRAdmin))]
        public async Task<ActionResult<PagedList<SkillDto>>> GetAllAsync(
         [FromQuery] PageParam? pagination = null,
         [FromQuery] bool? isActive = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(SkillDto.SkillShort);
            }

            var result = await _service.GetAllPagedAsyncWithIsActive(
                isActive,
                pagination,
                x => x.SkillStatus == true,
                x => x.SkillStatus != true);
            return Ok(result);
        }

        /// <summary>
        /// Searches active skills by short/long name with paging.
        /// </summary>
        /// <param name="searchTerm">Free-text search term.</param>
        [HttpGet("search")]
        public async Task<ActionResult<PagedList<SkillDto>>> SearchAsync(
            [FromQuery] string? searchTerm,
            [FromQuery] bool? isActive,
            [FromQuery] PageParam? pageParam = null)
        {
            pageParam ??= new PageParam();

            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
            {
                pageParam.SortBy = nameof(SkillDto.SkillShort);
            }

            var result = await _service.SearchAsync(
                searchTerm,
                pageParam,
                isActive.HasValue ? x => x.SkillStatus == isActive.Value : null
            );

            return Ok(result);
        }

        /// <summary>
        /// Gets lightweight skill options for dropdowns (skillId + skillShort only).
        /// </summary>
        [HttpPost("skilloptions")]
        [ProducesResponseType(typeof(IReadOnlyList<SelectOptionDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOrHRAdmin), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        public async Task<ActionResult<IReadOnlyList<SelectOptionDto>>> GetSkillOptionsAsync([FromBody] SelectOptionsRequestDto? body)
        {
            var request = SelectOptionsRequestDto.OrDefault(body);
            return Ok(await _service.GetOptionsAsync(s => s.SkillShort, x => x.SkillStatus == true, request.SearchTerm));
        }

        [HttpGet("GetAllActiveskill")]
        [ProducesResponseType(typeof(IEnumerable<SkillDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOrHRAdmin), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        public async Task<ActionResult<IEnumerable<SkillDto>>> GetAllActiveskillAsync(
            [FromQuery] Guid? departmentId = null,
            [FromQuery] PageParam? pagination = null,
            [FromQuery] List<Guid>? selectedIds = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(SkillDto.SkillShort);
            }

            var selectedSkillIds = (selectedIds ?? new List<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            var items = await _service.GetAllAsync(
                x => (!departmentId.HasValue || x.DepartmentID == departmentId.Value) &&
                     (x.SkillStatus == true || selectedSkillIds.Contains(x.SkillID)),
                null,
                pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a skill by ID.
        /// </summary>
        /// <param name="id">The skill ID (SkillID as Guid).</param>
        /// <returns>The skill if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SkillDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOrHRAdmin))]
        public async Task<ActionResult<SkillDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Skill with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new skill.
        /// </summary>
        /// <param name="dto">The skill data to create.</param>
        /// <returns>The created skill.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(SkillDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<SkillDto>> UpsertAsync([FromBody] SkillDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing skill.
        /// </summary>
        /// <param name="id">The skill ID (SkillID as Guid).</param>
        /// <param name="dto">The skill data to update.</param>
        /// <returns>The updated skill.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SkillDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<SkillDto>> UpdateAsync(Guid id, [FromBody] SkillDto dto)
        {

            if (id != dto.SkillID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"Skill with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a skill by ID.
        /// </summary>
        /// <param name="id">The skill ID (SkillID as Guid).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Skill with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
