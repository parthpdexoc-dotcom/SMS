using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing login record operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.Authenticated))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class LoginController : ControllerBase
    {
        private readonly ILoginService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginController"/> class.
        /// </summary>
        /// <param name="service">The login service.</param>
        public LoginController(ILoginService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all login records.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of login records.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LoginRecordDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LoginRecordDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a login record by ID.
        /// </summary>
        /// <param name="id">The login record ID.</param>
        /// <returns>The login record if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LoginRecordDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LoginRecordDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Login with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new login record.
        /// </summary>
        /// <param name="dto">The login record data to create.</param>
        /// <returns>The created login record.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(LoginRecordDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoginRecordDto>> UpsertAsync([FromBody] LoginRecordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing login record.
        /// </summary>
        /// <param name="id">The login record ID.</param>
        /// <param name="dto">The login record data to update.</param>
        /// <returns>The updated login record.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(LoginRecordDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LoginRecordDto>> UpdateAsync(int id, [FromBody] LoginRecordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.LoID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"Login with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a login record by ID.
        /// </summary>
        /// <param name="id">The login record ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"Login with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

