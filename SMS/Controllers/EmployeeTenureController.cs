using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeTenureController : ControllerBase
    {
        private readonly IEmployeeTenureService _service;

        public EmployeeTenureController(IEmployeeTenureService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmployeeTenureDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeTenureDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets all tenure records for a specific employee (promotion / type history).
        /// </summary>
        /// <param name="employeeId">Employee ID.</param>
        /// <param name="pagination">Optional paging and sort (default sort: <c>dateOfPromotion</c> ascending — oldest first, latest last).</param>
        [HttpGet("by-employee/{employeeId:guid}")]
        [ProducesResponseType(typeof(PagedList<EmployeeTenureDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedList<EmployeeTenureDto>>> GetByEmployeeIdAsync(
            Guid employeeId,
            [FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetByEmployeeIdAsync(employeeId, pagination);
            return Ok(items);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(EmployeeTenureDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeTenureDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound($"EmployeeTenure with ID {id} not found.");
            return Ok(item);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound($"EmployeeTenure with ID {id} not found.");
            return NoContent();
        }
    }
}


