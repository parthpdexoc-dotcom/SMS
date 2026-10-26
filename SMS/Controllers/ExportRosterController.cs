using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing export roster operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.RequesterManagerOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class ExportRosterController : ControllerBase
    {
        private readonly IExportRosterService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportRosterController"/> class.
        /// </summary>
        /// <param name="service">The export roster service.</param>
        public ExportRosterController(IExportRosterService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all export roster records.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of export roster records.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ExportRosterDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ExportRosterDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an export roster record by ID.
        /// </summary>
        /// <param name="id">The export roster ID (SL).</param>
        /// <returns>The export roster record if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ExportRosterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExportRosterDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"ExportRoster with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new export roster record.
        /// </summary>
        /// <param name="dto">The export roster data to create.</param>
        /// <returns>The created export roster record.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ExportRosterDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ExportRosterDto>> UpsertAsync([FromBody] ExportRosterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing export roster record.
        /// </summary>
        /// <param name="id">The export roster ID (SL).</param>
        /// <param name="dto">The export roster data to update.</param>
        /// <returns>The updated export roster record.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ExportRosterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExportRosterDto>> UpdateAsync(int id, [FromBody] ExportRosterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.SL)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"ExportRoster with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an export roster record by ID.
        /// </summary>
        /// <param name="id">The export roster ID (SL).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"ExportRoster with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

