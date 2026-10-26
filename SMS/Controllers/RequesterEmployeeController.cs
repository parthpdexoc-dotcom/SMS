using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing requester employee operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RequesterEmployeeController : ControllerBase
    {
        private readonly IRequesterEmployeeService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterEmployeeController"/> class.
        /// </summary>
        /// <param name="service">The requester employee service.</param>
        public RequesterEmployeeController(IRequesterEmployeeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all requester employees.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of requester employees.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RequesterEmployeeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequesterEmployeeDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        [HttpGet("getallRequestEmployeewithdetails")]
         public async Task<IActionResult> GetAllManagerWithDetails([FromQuery] PageParam? pageParam = null)
        {
            var result = await _service.GetAllRequestEmployeeWithDetailsAsync(pageParam);
            return Ok(result);
        }
        /// <summary>
        /// Gets a requester employee by ID.
        /// </summary>
        /// <param name="id">The requester employee ID (RequesterEmployeeID as Guid).</param>
        /// <returns>The requester employee if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RequesterEmployeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequesterEmployeeDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Requester employee with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a requester employee.
        /// </summary>
        /// <param name="dto">The requester employee data to create or update.</param>
        /// <returns>The created or updated requester employee.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RequesterEmployeeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
        public async Task<ActionResult<RequesterEmployeeDto>> UpsertAsync([FromBody] RequesterEmployeeDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Deletes a requester employee by ID.
        /// </summary>
        /// <param name="id">The requester employee ID (RequesterEmployeeID as Guid).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]

        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Requester employee with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
