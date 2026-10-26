using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Filters;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;
using SMS.Filters;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing department configuration operations. View: DepartmentAdmin+. Create/update: SuperUser only.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentConfigurationController : ControllerBase
    {
        private readonly IDepartmentConfigurationService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentConfigurationController"/> class.
        /// </summary>
        /// <param name="service">The department configuration service.</param>
        public DepartmentConfigurationController(IDepartmentConfigurationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all department configurations.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of department configurations.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DepartmentConfigurationDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<ActionResult<IEnumerable<DepartmentConfigurationDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a department configuration by ID.
        /// </summary>
        /// <param name="id">The department configuration DeptConfigID (Guid).</param>
        /// <returns>The department configuration if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DepartmentConfigurationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<DepartmentConfigurationDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"DepartmentConfiguration with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Gets a department configuration by DepartmentID.
        /// </summary>
        /// <param name="departmentId">The department identifier (Guid).</param>
        /// <returns>The department configuration if found; otherwise, NotFound.</returns>
        [HttpGet("department/{departmentId}")]
        [ProducesResponseType(typeof(DepartmentConfigurationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly),nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        public async Task<ActionResult<DepartmentConfigurationDto>> GetByDepartmentIdAsync(Guid departmentId)
        {
            var item = await _service.GetByDepartmentIdAsync(departmentId);
            if (item == null)
            {
                return NotFound($"DepartmentConfiguration for DepartmentID {departmentId} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new department configuration.
        /// </summary>
        /// <param name="dto">The department configuration data to create.</param>
        /// <returns>The created department configuration.</returns>
        [HttpPost]
        [Authorize(Policy = nameof(AuthorizationPolicyType.SuperUserOnly))]
        [ProducesResponseType(typeof(DepartmentConfigurationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<DepartmentConfigurationDto>> UpsertAsync([FromBody] DepartmentConfigurationDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing department configuration.
        /// </summary>
        /// <param name="id">The department configuration DeptConfigID (Guid).</param>
        /// <param name="dto">The department configuration data to update.</param>
        /// <returns>The updated department configuration.</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.SuperUserOnly))]
        [ProducesResponseType(typeof(DepartmentConfigurationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<DepartmentConfigurationDto>> UpdateAsync(Guid id, [FromBody] DepartmentConfigurationDto dto)
        {
            if (id != dto.DeptConfigID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"DepartmentConfiguration with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a department configuration by ID.
        /// </summary>
        /// <param name="id">The department configuration DeptConfigID (Guid).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [Authorize(Policy = nameof(AuthorizationPolicyType.SuperUserOnly))]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"DepartmentConfiguration with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

