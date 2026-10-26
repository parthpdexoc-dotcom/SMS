using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Filters;
using SMS.Generic.Enums;
using SMS.Generic.Common.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;
using SMS.Generic.Services.Extensions;
using System.Linq.Expressions;

namespace SMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentEquipmentController : ControllerBase
    {
        private readonly IDepartmentEquipmentService _service;

        public DepartmentEquipmentController(IDepartmentEquipmentService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets department equipment rows (paged).
        /// </summary>
        /// <param name="pagination">Paging/sort (e.g. page, pageSize).</param>
        /// <param name="isActive">When true, only active; when false, only inactive; when omitted, no status filter.</param>
        [HttpGet]
        [AuthorizeAny(nameof(AuthorizationPolicyType.DepartmentAdminOnly), nameof(AuthorizationPolicyType.OperatorOnly))]
        [ProducesResponseType(typeof(PagedList<DepartmentEquipmentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<DepartmentEquipmentDto>>> GetAllAsync(
            [FromQuery] PageParam? pagination = null,
            [FromQuery] bool? isActive = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(DepartmentEquipmentDto.EquipmentTypeID);
            }

            var result = await _service.GetAllPagedAsyncWithIsActive(
                isActive,
                pagination,
                x => x.EquipmentStatus == true,
                x => x.EquipmentStatus != true);
            return Ok(result);
        }

        [HttpGet("search")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [ProducesResponseType(typeof(PagedList<DepartmentEquipmentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<DepartmentEquipmentDto>>> SearchAsync([FromQuery] string? searchTerm, [FromQuery] bool? isActive = true, [FromQuery] PageParam? pageParam = null)
        {
            pageParam ??= new PageParam
            {
                SortBy = nameof(DepartmentEquipmentDto.EquipmentTypeID)
            };

            pageParam.SortBy ??= nameof(DepartmentEquipmentDto.EquipmentTypeID);

            var result = await _service.SearchAsync(
                searchTerm,
                pageParam,
                isActive.HasValue
                    ? x => x.EquipmentStatus == isActive.Value
                    : null
            );

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [ProducesResponseType(typeof(DepartmentEquipmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DepartmentEquipmentDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound($"DepartmentEquipment with ID {id} not found.");

            return Ok(item);
        }

        [HttpPost]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [ProducesResponseType(typeof(DepartmentEquipmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DepartmentEquipmentDto>> UpsertAsync([FromBody] DepartmentEquipmentDto dto)
        {
            var result = await _service.UpsertAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound($"DepartmentEquipment with ID {id} not found.");

            return NoContent();
        }

        /// <summary>
        /// Gets lightweight department vehicle options for dropdowns.
        /// </summary>
        [HttpPost("vehicleoptions")]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [ProducesResponseType(typeof(IReadOnlyList<DepartmentEquipmentSelectOptionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<DepartmentEquipmentSelectOptionDto>>> GetVehicleOptionsAsync(
            [FromBody] DepartmentEquipmentOptionsBody? body)
        {
            body ??= new DepartmentEquipmentOptionsBody();
            return Ok(await _service.GetDepartmentVehicleOptionsAsync(body.SelectedIds, body.SearchTerm));
        }

        /// <summary>
        /// Gets lightweight department equipment options for dropdowns.
        /// </summary>
        [HttpPost("equipmentoptions")]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [ProducesResponseType(typeof(IReadOnlyList<DepartmentEquipmentSelectOptionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<DepartmentEquipmentSelectOptionDto>>> GetEquipmentOptionsAsync(
            [FromBody] DepartmentEquipmentOptionsBody? body)
        {
            body ??= new DepartmentEquipmentOptionsBody();
            return Ok(await _service.GetDepartmentEquipmentOptionsAsync(body.SelectedIds, body.SearchTerm));
        }

        [HttpGet("department/{departmentId}/equipment")]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly),nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [ProducesResponseType(typeof(IEnumerable<DepartmentEquipmentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<DepartmentEquipmentResponseDto>>> GetDepartmentEquipmentByTypeAsync(
            Guid departmentId,
            [FromQuery] EquipmentCategory category,
            [FromQuery] List<Guid>? selectedIds = null)
        {
            var result = await _service.GetDepartmentEquipmentByTypeAsync(departmentId, category, selectedIds);
            return Ok(result);
        }
    }

    public class DepartmentEquipmentOptionsBody
    {
        public string? SearchTerm { get; set; }
        public List<Guid>? SelectedIds { get; set; }
    }
}


