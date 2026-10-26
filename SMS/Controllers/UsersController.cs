using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing users operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class UsersController : ControllerBase
    {
        private readonly IUsersService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="service">The users service.</param>
        public UsersController(IUsersService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of users.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UsersDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UsersDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a user by ID.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>The user if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UsersDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsersDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"User with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="dto">The user data to create.</param>
        /// <returns>The created user.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UsersDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UsersDto>> UpsertAsync([FromBody] UsersDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="dto">The user data to update.</param>
        /// <returns>The updated user.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UsersDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsersDto>> UpdateAsync(int id, [FromBody] UsersDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.Id)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"User with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

