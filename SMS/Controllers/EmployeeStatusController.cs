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
    /// Controller for managing employee status operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeStatusController : ControllerBase
    {
        private readonly IEmployeeStatusService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeStatusController"/> class.
        /// </summary>
        /// <param name="service">The employee status service.</param>
        public EmployeeStatusController(IEmployeeStatusService service)
        {
            _service = service;
        }
        /// <summary>
        /// Gets all employee statuses.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <param name="isActive">When true, only active; when false, only inactive; when omitted, no status filter.</param>
        /// <returns>Paged employee statuses.</returns>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.IAAuditViewOnly))]
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<EmployeeStatusDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<EmployeeStatusDto>>> GetAllAsync(
        [FromQuery] PageParam? pagination = null,
        [FromQuery] bool? isActive = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(EmployeeStatusDto.StatusShort);
            }

            var result = await _service.GetAllPagedAsyncWithIsActive(
                isActive,
                pagination,
                x => x.Status == true,
                x => x.Status != true);
            return Ok(result);
        }

        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.IAAuditViewOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<EmployeeStatusDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<EmployeeStatusDto>>> SearchAsync(
            [FromQuery] string? searchTerm,
            [FromQuery] bool? isActive,
            [FromQuery] PageParam? pageParam = null)
        {
            pageParam ??= new PageParam();

            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
            {
                pageParam.SortBy = nameof(EmployeeStatusDto.StatusShort);
            }

            var result = await _service.SearchAsync(
                searchTerm,
                pageParam,
                isActive.HasValue ? x => x.Status == isActive.Value : null
            );

            return Ok(result);
        }

        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.IAAuditViewOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        [HttpGet("allactiveemployeestatus")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeStatusDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeStatusDto>>> GetAllActiveEmployeeStatusAsync(
            [FromQuery] Guid? departmentId = null,
            [FromQuery] PageParam? pagination = null,
            [FromQuery] Guid? selectedId = null,
            [FromQuery] List<Guid>? selectedIds = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(EmployeeStatusDto.StatusShort);
            }

            var mergedSelectedIds = new List<Guid>();
            if (selectedIds != null)
                mergedSelectedIds.AddRange(selectedIds.Where(id => id != Guid.Empty));
            if (selectedId.HasValue && selectedId.Value != Guid.Empty)
                mergedSelectedIds.Add(selectedId.Value);

            var items = await _service.GetActiveEmployeeStatusesForDropdownAsync(
                departmentId,
                pagination,
                mergedSelectedIds.Distinct().ToList());
            return Ok(items);
        }
        /// <summary>
        /// Gets an employee status by ID.
        /// </summary>
        /// <param name="id">The employee status ID.</param>
        /// <returns>The employee status if found; otherwise, NotFound.</returns>
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.IAAuditViewOnly))]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EmployeeStatusDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeStatusDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"EmployeeStatus with ID {id} not found.");
            }
            return Ok(item);
        }
         /// <summary>
        /// Creates a new employee status.
        /// </summary>
        /// <param name="employeeStatusDto">The employee status data to create.</param>
        /// <returns>The created employee status.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeStatusDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeeStatusDto>> UpsertAsync([FromBody] EmployeeStatusDto employeeStatusDto)
        {
            try
            {
                return await _service.UpsertAsync(employeeStatusDto); 
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing employee status.
        /// </summary>
        /// <param name="id">The employee status ID.</param>
        /// <param name="employeeStatusDto">The employee status data to update.</param>
        /// <returns>The updated employee status.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EmployeeStatusDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeStatusDto>> UpdateAsync(Guid id, [FromBody] EmployeeStatusDto employeeStatusDto)
        {

            if (id != employeeStatusDto.EmployeeStatusID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"EmployeeStatus with ID {id} not found.");
            }

            try
            {
                var updatedItem = await _service.UpsertAsync(employeeStatusDto);
                return Ok(updatedItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes an employee status by ID.
        /// </summary>
        /// <param name="id">The employee status ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"EmployeeStatus with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

