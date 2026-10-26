using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;
using SMS.Filters;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing employee skill operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
    public class EmployeeSkillController : ControllerBase
    {
        private readonly IEmployeeSkillService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeSkillController"/> class.
        /// </summary>
        /// <param name="service">The employee skill service.</param>
        public EmployeeSkillController(IEmployeeSkillService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all employee skills.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of employee skills.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmployeeSkillDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeSkillDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets all skills for a specific employee.
        /// </summary>
        /// <param name="employeeId">The employee ID.</param>
        /// <returns>A collection of employee skills for the given employee.</returns>
        [HttpGet("employee/{employeeId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeSkillDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeSkillDto>>> GetByEmployeeIdAsync(Guid employeeId)
        {
            var items = await _service.GetAllAsync(
                es => es.EmployeeID == employeeId,
                new System.Linq.Expressions.Expression<Func<EmployeeSkill, object>>[] { es => es.Skill },
                null);
            return Ok(items);
        }

        /// <summary>
        /// Gets an employee skill by ID.
        /// </summary>
        /// <param name="id">The employee skill ID.</param>
        /// <returns>The employee skill if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EmployeeSkillDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeSkillDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"EmployeeSkill with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates an employee skill.
        /// </summary>
        /// <param name="dto">The employee skill data to create or update.</param>
        /// <returns>The created or updated employee skill.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeSkillDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeeSkillDto>> UpsertAsync(
            [FromBody] EmployeeSkillDto dto,
            [FromHeader] bool isSuperUser = false)
        {
            var result = await _service.UpsertAsync(dto, isSuperUser);
            return Ok(result);
        }

        /// <summary>
        /// Creates or updates multiple employee skill records in one call.
        /// </summary>
        /// <param name="dtos">The collection of employee skill data to create or update.</param>
        /// <returns>The created or updated employee skill records.</returns>
        [HttpPost("bulk")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeSkillDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<EmployeeSkillDto>>> BulkUpsertAsync(
            [FromBody] IEnumerable<EmployeeSkillDto> dtos,
            [FromHeader] bool isSuperUser = false)
        {
            var results = await _service.BulkUpsertAsync(dtos, isSuperUser);
            return Ok(results);
        }

        /// <summary>
        /// Deletes an employee skill by ID.
        /// </summary>
        /// <param name="id">The employee skill ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"EmployeeSkill with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

