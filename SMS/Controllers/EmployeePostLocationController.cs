using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeePostLocationController : ControllerBase
    {
        private readonly IEmployeePostLocationService _service;

        public EmployeePostLocationController(IEmployeePostLocationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets GPS points for an employee on a job slot, oldest first.
        /// </summary>
        [HttpGet("by-employee/{employeeId:guid}")]
        [ProducesResponseType(typeof(List<EmployeePostLocationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        public async Task<ActionResult<List<EmployeePostLocationDto>>> GetCoordinatesByEmployeeIdAsync(
            Guid employeeId,
            [FromQuery] Guid postPositionId,
            CancellationToken cancellationToken)
        {
            if (employeeId == Guid.Empty)
                return BadRequest("EmployeeId is required.");
            if (postPositionId == Guid.Empty)
                return BadRequest("PostPositionId is required.");

            var points = await _service.GetCoordinatesByEmployeeIdAsync(employeeId, postPositionId, cancellationToken).ConfigureAwait(false);
            return Ok(points);
        }

        [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
        [HttpPost]
        public async Task<ActionResult<EmployeePostLocationDto>> SavePostLocationAsync([FromBody] EmployeePostLocationDto request)
        {
            var saved = await _service.SavePostLocationAsync(request);
            return StatusCode(StatusCodes.Status201Created, saved);
        }
    }
}
