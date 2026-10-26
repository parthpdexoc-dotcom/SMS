using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Filters;
using SMS.Generic.Common.Constants;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Helpers;
using SMS.Generic.Services.Services;
using System.Linq.Expressions;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing admin type operations. List/get: DepartmentAdmin+. Create/update/delete: SuperUser only (IA Admin assignable by SuperUser only).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AdminTypeController : ControllerBase
    {
        private readonly IAdminTypeService _service;
        private readonly ITokenParser _tokenParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminTypeController"/> class.
        /// </summary>
        /// <param name="service">The admin type service.</param>
        /// <param name="tokenParser">The token parser for checking user claims.</param>
        public AdminTypeController(IAdminTypeService service, ITokenParser tokenParser)
        {
            _service = service;
            _tokenParser = tokenParser;
        }

        /// <summary>
        /// Gets admin types (paged). Super user sees all; non-super user sees only types where IsSuperUserType is false (Super User, Customer Support, IA Admin hidden).
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <param name="isActive">When true, only active (<c>Status</c>); when false, only inactive; when omitted, no status filter.</param>
        /// <returns>Paged admin types.</returns>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.IAAuditViewOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<AdminTypeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<AdminTypeDto>>> GetAllAsync(
            [FromQuery] PageParam? pagination = null,
            [FromQuery] bool? isActive = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(AdminTypeDto.AdminTypeShort);
            }

            var isSuperUser = _tokenParser.GetClaimValue(ClaimNames.IsSuperUser) == "true";

            var result = await _service.GetAllPagedAsync(at =>
                (isSuperUser || !at.IsSuperUserType) &&
                (!isActive.HasValue || at.Status == isActive.Value),
                pagination);
            return Ok(result);
        }

        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.IAAuditViewOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<AdminTypeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<AdminTypeDto>>> SearchAsync([FromQuery] string? searchTerm, [FromQuery] bool? isActive = true, [FromQuery] PageParam? pageParam = null)
        {
            pageParam ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
                pageParam.SortBy = nameof(AdminTypeDto.AdminTypeShort);

            var isSuperUser = _tokenParser.GetClaimValue(ClaimNames.IsSuperUser) == "true";

            Expression<Func<AdminType, bool>> visibilityFilter = at => isSuperUser || !at.IsSuperUserType;
            if (isActive.HasValue)
            {
                var active = isActive.Value;
                visibilityFilter = at => (isSuperUser || !at.IsSuperUserType) && at.Status == active;
            }

            var result = await _service.SearchAsync(searchTerm, pageParam, visibilityFilter);
            return Ok(result);
        }

        /// <summary>
        /// Gets an admin type by ID.
        /// </summary>
        /// <param name="id">The admin type ID (AdminTypeID as Guid).</param>
        /// <returns>The admin type if found; otherwise, NotFound.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AdminTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdminTypeDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Admin type with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new admin type.
        /// </summary>
        /// <param name="dto">The admin type data to create.</param>
        /// <returns>The created admin type.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.SuperUserOnly))]
        [HttpPost]
        [ProducesResponseType(typeof(AdminTypeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AdminTypeDto>> UpsertAsync([FromBody] AdminTypeDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing admin type.
        /// </summary>
        /// <param name="id">The admin type ID (AdminTypeID as Guid).</param>
        /// <param name="dto">The admin type data to update.</param>
        /// <returns>The updated admin type.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.SuperUserOnly))]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AdminTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdminTypeDto>> UpdateAsync(Guid id, [FromBody] AdminTypeDto dto)
        {
            if (dto.AdminTypeID.HasValue && id != dto.AdminTypeID.Value)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"Admin type with ID {id} not found.");
            }

            // Ensure the DTO has the ID from the URL
            dto.AdminTypeID = id;

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an admin type by ID.
        /// </summary>
        /// <param name="id">The admin type ID (AdminTypeID as Guid).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.SuperUserOnly))]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Admin type with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

