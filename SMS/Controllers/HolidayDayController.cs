using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;
using SMS.Generic.Services.Extensions;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing holiday day operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    public class HolidayDayController : ControllerBase
    {
        private readonly IHolidayDayService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="HolidayDayController"/> class.
        /// </summary>
        /// <param name="service">The holiday day service.</param>
        public HolidayDayController(IHolidayDayService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all holiday days.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <param name="isActive">When true, only active; when false, only inactive; when omitted, no status filter.</param>
        /// <returns>Paged holiday days.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<HolidayDayDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<HolidayDayDto>>> GetAllAsync(
            [FromQuery] PageParam? pagination = null,
            [FromQuery] bool? isActive = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(HolidayDayDto.Date);
            }

            var result = await _service.GetAllPagedAsyncWithIsActive(
                isActive,
                pagination,
                x => x.IsActive == true,
                x => x.IsActive != true);
            return Ok(result);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<HolidayDayDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<HolidayDayDto>>> SearchAsync(
            [FromQuery] string? searchTerm,
            [FromQuery] bool? isActive,
            [FromQuery] PageParam? pageParam = null)
        {
            pageParam ??= new PageParam();

            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
            {
                pageParam.SortBy = nameof(HolidayDayDto.Date);
            }

            var result = await _service.SearchAsync(
                searchTerm,
                pageParam,
                isActive.HasValue ? x => x.IsActive == isActive.Value : null
            );

            return Ok(result);
        }

        /// <summary>
        /// Gets a holiday day by ID.
        /// </summary>
        /// <param name="id">The holiday day ID (HolidayDayID as Guid).</param>
        /// <returns>The holiday day if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(HolidayDayDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HolidayDayDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Holiday day with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a holiday day.
        /// </summary>
        /// <param name="dto">The holiday day data to create or update.</param>
        /// <returns>The created or updated holiday day.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(HolidayDayDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<HolidayDayDto>> UpsertAsync([FromBody] HolidayDayDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Deletes a holiday day by ID.
        /// </summary>
        /// <param name="id">The holiday day ID (HolidayDayID as Guid).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Holiday day with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
