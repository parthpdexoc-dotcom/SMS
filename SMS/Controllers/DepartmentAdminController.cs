using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Filters;
using SMS.Generic.Enums;
using SMS.Generic.Common.Constants;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing department admin operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentAdminController : ControllerBase
    {
        private readonly IDepartmentAdminService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentAdminController"/> class.
        /// </summary>
        /// <param name="service">The department admin service.</param>
        public DepartmentAdminController(IDepartmentAdminService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all department admins. Super user sees all; non-super user sees only rows whose AdminType has IsSuperUserType false (Super User, Customer Support, IA Admin hidden).
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of department admins.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DepartmentAdminDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.IAAuditViewOnly))]
        public async Task<ActionResult<IEnumerable<DepartmentAdminDto>>> GetAllAsync(
        [FromQuery] PageParam? pagination = null)
        {
            var isSuperUserType = User.FindFirst(ClaimNames.IsSuperUser)?.Value == "true";

            Expression<Func<DepartmentAdmin, bool>>? filter = null;
            if (!isSuperUserType)
                filter = da => da.AdminType != null && !da.AdminType.IsSuperUserType;

            var items = await _service.GetAllAsync(filter, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets employee IDs for department admins matching the given department and admin type (IDs only).
        /// </summary>
        /// <param name="departmentId">The department identifier.</param>
        /// <param name="adminTypeId">The admin type identifier.</param>
        /// <returns>A collection of employee IDs.</returns>
        [HttpGet("employees/ids")]
        [ProducesResponseType(typeof(IEnumerable<Guid>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.OperatorOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<IEnumerable<Guid>>> GetEmployeeIdsByDepartmentAndAdminTypeAsync(
            [FromQuery] Guid departmentId,
            [FromQuery] Guid adminTypeId)
        {
            var ids = await _service.GetEmployeeIdsByDepartmentAndAdminTypeAsync(departmentId, adminTypeId);
            return Ok(ids);
        }

        /// <summary>
        /// Gets a department admin by ID.
        /// </summary>
        /// <param name="id">The department admin ID.</param>
        /// <returns>The department admin if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DepartmentAdminDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<ActionResult<DepartmentAdminDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"DepartmentAdmin with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a department admin.
        /// If DepartmentAdminID is provided and exists, updates the record.
        /// If DepartmentAdminID is not provided or empty, creates a new record.
        /// </summary>
        /// <param name="dto">The department admin data. Include DepartmentAdminID to update, omit it to create.</param>
        /// <returns>The created or updated department admin.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(DepartmentAdminDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<DepartmentAdminDto>> UpsertAsync([FromBody] DepartmentAdminDto dto)
        {
            var isSuperUser = User.FindFirst(ClaimNames.IsSuperUser)?.Value == "true";
            var result = await _service.UpsertAsync(dto, isSuperUser);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a department admin by ID.
        /// </summary>
        /// <param name="id">The department admin ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AuthorizeAny(
            nameof(AuthorizationPolicyType.OperatorOnly),
            nameof(AuthorizationPolicyType.DepartmentAdminOnly),
            nameof(AuthorizationPolicyType.EmployeesViewManage))]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound($"DepartmentAdmin with ID {id} not found.");

            return NoContent();
        }
    }
}
