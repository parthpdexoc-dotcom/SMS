using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;
using System.Linq.Expressions;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing department operations.
    /// Only accessible to super users (AdminType with IsSuperUser = true).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentController"/> class.
        /// </summary>
        /// <param name="service">The department service.</param>
        public DepartmentController(IDepartmentService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all departments (paged).
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>Paged departments; each page is ordered by DepartmentCode then DepartmentName (case-insensitive).</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.SuperUserOnly))]
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<DepartmentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<DepartmentDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(DepartmentDto.DepartmentCode);
            }

            var result = await _service.GetAllPagedAsync(null, pagination);

            var ordered = result.Data
                .OrderBy(d => d.DepartmentCode ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(d => d.DepartmentName ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToList();

            return Ok(new PagedList<DepartmentDto>(ordered, pagination, result.TotalRecords));
        }

        [Authorize(Policy = nameof(AuthorizationPolicyType.SuperUserOnly))]
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<DepartmentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<DepartmentDto>>> SearchAsync([FromQuery] string? searchTerm, [FromQuery] bool? isActive = true, [FromQuery] PageParam? pageParam = null)
        {
            pageParam ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
                pageParam.SortBy = nameof(DepartmentDto.DepartmentCode);

            var result = await _service.SearchAsync(searchTerm, pageParam);
            return Ok(result);
        }

        /// <summary>
        /// Gets a department by ID.
        /// </summary>
        /// <param name="id">The department ID.</param>
        /// <returns>The department if found; otherwise, NotFound.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DepartmentDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Department with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Gets the location (latitude, longitude, and address details) for the authenticated user's department to showcase on Master Maps.
        /// Resolves DepartmentID automatically from the JWT token.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The department location details.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpGet("location")]
        [ProducesResponseType(typeof(DepartmentLocationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DepartmentLocationDto>> GetLocationAsync(CancellationToken cancellationToken = default)
        {
            var location = await _service.GetDepartmentLocationAsync(cancellationToken);
            if (location == null)
            {
                return NotFound("Department location not found.");
            }
            return Ok(location);
        }

        /// <summary>
        /// Creates a new department.
        /// </summary>
        /// <param name="dto">The department data to create.</param>
        /// <returns>The created department.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [HttpPost]
        [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DepartmentDto>> UpsertAsync([FromBody] DepartmentDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing department.
        /// </summary>
        /// <param name="id">The department ID.</param>
        /// <param name="dto">The department data to update.</param>
        /// <returns>The updated department.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DepartmentDto>> UpdateAsync(Guid id, [FromBody] DepartmentDto dto)
        {

            if (id != dto.DepartmentID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a department by ID.
        /// </summary>
        /// <param name="id">The department ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.SuperUserOnly))]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteDepartmentAsync(id);
            if (!deleted)
            {
                return NotFound($"Department with ID {id} not found.");
            }
            return Ok(new { message = "Record deleted successfully." });
        }
    }
}


