using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;
using SMS.Filters;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing cancelled post position operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
    public class CancelledPostPositionController : ControllerBase
    {
        private readonly ICancelledPostPositionService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelledPostPositionController"/> class.
        /// </summary>
        /// <param name="service">The cancelled post position service.</param>
        public CancelledPostPositionController(ICancelledPostPositionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all cancelled post position records.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of cancelled post position records.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CancelledPostPositionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CancelledPostPositionDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a cancelled post position record by ID.
        /// </summary>
        /// <param name="id">The cancelled post position ID (GUID).</param>
        /// <returns>The record if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CancelledPostPositionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CancelledPostPositionDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Cancelled post position with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a cancelled post position record.
        /// </summary>
        /// <param name="dto">The cancelled post position data to create or update.</param>
        /// <returns>The created or updated record.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CancelledPostPositionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CancelledPostPositionDto>> UpsertAsync([FromBody] CancelledPostPositionDto dto)
        {
            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Deletes a cancelled post position record by ID.
        /// </summary>
        /// <param name="id">The cancelled post position ID (GUID).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Cancelled post position with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
