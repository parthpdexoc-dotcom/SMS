using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;
using SMS.Generic.Services.Extensions;
using SMS.Filters;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing business type operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessTypeController : ControllerBase
    {
        private readonly IBusinessTypeService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessTypeController"/> class.
        /// </summary>
        /// <param name="service">The business type service.</param>
        public BusinessTypeController(IBusinessTypeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all business types.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <param name="isActive">When true, only active; when false, only inactive; when omitted, no status filter.</param>
        /// <returns>Paged business types.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<BusinessTypeDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<ActionResult<PagedList<BusinessTypeDto>>> GetAllAsync(
        [FromQuery] PageParam? pagination = null,
        [FromQuery] bool? isActive = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(BusinessTypeDto.BusinessTypeShort);
            }

            var result = await _service.GetAllPagedAsyncWithIsActive(
                isActive,
                pagination,
                x => x.BusinessTypeStatus == true,
                x => x.BusinessTypeStatus != true);
            return Ok(result);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<BusinessTypeDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<PagedList<BusinessTypeDto>>> SearchAsync(
            [FromQuery] string? searchTerm,
            [FromQuery] bool? isActive,
            [FromQuery] PageParam? pageParam = null)
        {
            pageParam ??= new PageParam();

            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
            {
                pageParam.SortBy = nameof(BusinessTypeDto.BusinessTypeShort);
            }

            var result = await _service.SearchAsync(
                searchTerm,
                pageParam,
                isActive.HasValue ? x => x.BusinessTypeStatus == isActive.Value : null
            );

            return Ok(result);
        }

        /// <summary>
        /// Gets lightweight business type options for dropdowns (businessTypeId + businessTypeShort only).
        /// </summary>
        [HttpPost("businesstypeoptions")]
        [ProducesResponseType(typeof(IReadOnlyList<SelectOptionDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<IReadOnlyList<SelectOptionDto>>> GetBusinessTypeOptionsAsync([FromBody] SelectOptionsRequestDto? body)
        {
            var request = SelectOptionsRequestDto.OrDefault(body);
            return Ok(await _service.GetOptionsAsync(
                bt => bt.BusinessTypeShort!,
                x => x.BusinessTypeStatus == true,
                request.SearchTerm));
        }

        [HttpGet("getAllActiveBusinessType")]
        [ProducesResponseType(typeof(IEnumerable<BusinessTypeDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<ActionResult<IEnumerable<BusinessTypeDto>>> GetAllActiveBusinessTypeAsync(
        [FromQuery] PageParam? pagination = null,
        [FromQuery] Guid? selectedId = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(BusinessTypeDto.BusinessTypeShort);
            }

            var items = await _service.GetAllAsync(
                x => x.BusinessTypeStatus == true || (selectedId.HasValue && x.BusinessTypeID == selectedId.Value),
                null,
                pagination);
            return Ok(items);
        }
        /// <summary>
        /// Gets a business type by ID.
        /// </summary>
        /// <param name="id">The business type ID.</param>
        /// <returns>The business type if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BusinessTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<ActionResult<BusinessTypeDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"BusinessType with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a business type.
        /// If BusinessTypeID is provided and exists, updates the record.
        /// If BusinessTypeID is not provided or empty, creates a new record.
        /// </summary>
        /// <param name="dto">The business type data. Include BusinessTypeID to update, omit it to create.</param>
        /// <returns>The created or updated business type.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(BusinessTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<ActionResult<BusinessTypeDto>> UpsertAsync([FromBody] BusinessTypeDto dto)
        {
            var result = await _service.UpsertAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a business type by ID.
        /// </summary>
        /// <param name="id">The business type ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound($"BusinessType with ID {id} not found.");

            return NoContent();
        }
    }
}

