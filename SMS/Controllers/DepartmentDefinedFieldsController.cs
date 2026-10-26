using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing department defined fields.
    /// Only accessible to super users (AdminType with IsSuperUser = true).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentDefinedFieldsController : ControllerBase
    {
        private readonly IDepartmentDefinedFieldsService _service;

        public DepartmentDefinedFieldsController(IDepartmentDefinedFieldsService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DepartmentDefinedFieldsDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DepartmentDefinedFieldsDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        [HttpGet("department/{departmentId:guid}")]
        [ProducesResponseType(typeof(DepartmentDefinedFieldsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DepartmentDefinedFieldsDto>> GetByDepartmentIdAsync(Guid departmentId)
        {
            var item = await _service.GetByDepartmentIdAsync(departmentId);
            if (item == null)
                return NotFound($"Department defined fields for department ID {departmentId} not found.");
            return Ok(item);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(DepartmentDefinedFieldsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DepartmentDefinedFieldsDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound($"DepartmentEquipment with ID {id} not found.");

            return Ok(item);
        }

        [HttpPost]
        [ProducesResponseType(typeof(DepartmentDefinedFieldsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<DepartmentDefinedFieldsDto>> UpsertAsync([FromBody] DepartmentDefinedFieldsDto dto)
        {
            var result = await _service.UpsertAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound($"DepartmentEquipment with ID {id} not found.");

            return NoContent();
        }
    }
}


