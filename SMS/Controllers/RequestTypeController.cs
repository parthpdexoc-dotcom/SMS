using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing RequestType (formerly RequestRecurrence) operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    public class RequestTypeController : ControllerBase
    {
        private readonly IRequestTypeService _service;

        public RequestTypeController(IRequestTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequestTypeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequestTypeDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(RequestTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequestTypeDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound($"RequestType with ID {id} not found.");

            return Ok(item);
        }

        [HttpPost]
        [ProducesResponseType(typeof(RequestTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RequestTypeDto>> UpsertAsync([FromBody] RequestTypeDto dto)
        {
            var result = await _service.UpsertAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound($"RequestType with ID {id} not found.");

            return NoContent();
        }
    }
}
