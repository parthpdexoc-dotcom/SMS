using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;
using SMS.Filters;
using SMS.Generic.Services.Extensions;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing pay code operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PayCodeController : ControllerBase
    {
        private readonly IPayCodeService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayCodeController"/> class.
        /// </summary>
        /// <param name="service">The pay code service.</param>
        public PayCodeController(IPayCodeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all pay codes.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <param name="isActive">When true, only active; when false, only inactive; when omitted, no status filter.</param>
        /// <returns>Paged pay codes.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<PayCodeDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        public async Task<ActionResult<PagedList<PayCodeDto>>> GetAllAsync(
            [FromQuery] PageParam? pagination = null,
            [FromQuery] bool? isActive = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(PayCodeDto.PayCodeValue);
            }

            var result = await _service.GetAllPagedAsyncWithIsActive(
                isActive,
                pagination,
                x => x.PayCodeStatus,
                x => !x.PayCodeStatus);
            return Ok(result);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<PayCodeDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        public async Task<ActionResult<PagedList<PayCodeDto>>> SearchAsync(
            [FromQuery] string? searchTerm,
            [FromQuery] bool? isActive,
            [FromQuery] PageParam? pageParam = null)
        {
            pageParam ??= new PageParam();

            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
            {
                pageParam.SortBy = nameof(PayCodeDto.PayCodeValue);
            }

            var result = await _service.SearchAsync(
                searchTerm,
                pageParam,
                isActive.HasValue ? x => x.PayCodeStatus == isActive.Value : null
            );

            return Ok(result);
        }

        /// <summary>
        /// Gets lightweight pay code options for dropdowns (payCodeId + payCodeValue only).
        /// </summary>
        [HttpPost("paycodeoptions")]
        [ProducesResponseType(typeof(IReadOnlyList<SelectOptionDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        public async Task<ActionResult<IReadOnlyList<SelectOptionDto>>> GetPayCodeOptionsAsync([FromBody] SelectOptionsRequestDto? body)
        {
            var request = SelectOptionsRequestDto.OrDefault(body);
            var selectedId = request.SelectedId;
            return Ok(await _service.GetOptionsAsync(
                p => p.PayCodeValue!,
                x => x.PayCodeStatus == true || (selectedId.HasValue && x.PayCodeID == selectedId.Value),
                request.SearchTerm));
        }

        /// <summary>
        /// Gets all active pay codes.
        /// </summary>
        [HttpGet("getAllActivePayCodes")]
        [ProducesResponseType(typeof(IEnumerable<PayCodeDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        public async Task<ActionResult<IEnumerable<PayCodeDto>>> GetAllActiveAsync(
            [FromQuery] PageParam? pagination = null,
            [FromQuery] Guid? selectedId = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(PayCodeDto.PayCodeValue);
            }

            var items = await _service.GetAllAsync(
                x => x.PayCodeStatus == true || (selectedId.HasValue && x.PayCodeID == selectedId.Value),
                null,
                pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a pay code by ID.
        /// </summary>
        /// <param name="id">The pay code ID.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PayCodeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<PayCodeDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"PayCode with ID {id} not found.");
            }

            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a pay code.
        /// If PayCodeID is provided and exists, updates the record.
        /// If PayCodeID is not provided or empty, creates a new record.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PayCodeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<PayCodeDto>> UpsertAsync([FromBody] PayCodeDto dto)
        {
            var result = await _service.UpsertAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a pay code by ID.
        /// </summary>
        /// <param name="id">The pay code ID.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound($"PayCode with ID {id} not found.");
            }

            return NoContent();
        }
    }
}

