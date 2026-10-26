using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing department policy operations.
    /// </summary>
    [ApiController]
    [Route("api/department-policy")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    public class DepartmentPolicyController : ControllerBase
    {
        private readonly IDepartmentPolicyService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentPolicyController"/> class.
        /// </summary>
        /// <param name="service">The department policy service.</param>
        public DepartmentPolicyController(IDepartmentPolicyService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all department policies.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of department policies.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DepartmentPolicyDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DepartmentPolicyDto>>> GetAllAsync(
            [FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a department policy by ID.
        /// </summary>
        /// <param name="id">The department policy ID.</param>
        /// <returns>The department policy if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DepartmentPolicyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DepartmentPolicyDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"DepartmentPolicy with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a department policy.
        /// If DepartmentPolicyID is provided and exists, updates the record.
        /// If DepartmentPolicyID is not provided or empty, creates a new record.
        /// </summary>
        /// <param name="dto">The department policy data. Include DepartmentPolicyID to update, omit it to create.</param>
        /// <returns>The created or updated department policy.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(DepartmentPolicyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DepartmentPolicyDto>> UpsertAsync([FromBody] DepartmentPolicyDto dto)
        {
            var result = await _service.UpsertAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a department policy by ID.
        /// </summary>
        /// <param name="id">The department policy ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound($"DepartmentPolicy with ID {id} not found.");

            return NoContent();
        }
    }
}
