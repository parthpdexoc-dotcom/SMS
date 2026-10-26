using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing HR request class operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class HrRequestClassController : ControllerBase
    {
        private readonly IHrRequestClassService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="HrRequestClassController"/> class.
        /// </summary>
        /// <param name="service">The HR request class service.</param>
        public HrRequestClassController(IHrRequestClassService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all HR request classes.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of HR request classes.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<HrRequestClassDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<HrRequestClassDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets an HR request class by ID.
        /// </summary>
        /// <param name="id">The HR request class ID.</param>
        /// <returns>The HR request class if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(HrRequestClassDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HrRequestClassDto>> GetByIdAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"HrRequestClass with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new HR request class.
        /// </summary>
        /// <param name="dto">The HR request class data to create.</param>
        /// <returns>The created HR request class.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(HrRequestClassDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<HrRequestClassDto>> UpsertAsync([FromBody] HrRequestClassDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }

        /// <summary>
        /// Updates an existing HR request class.
        /// </summary>
        /// <param name="id">The HR request class ID.</param>
        /// <param name="dto">The HR request class data to update.</param>
        /// <returns>The updated HR request class.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(HrRequestClassDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HrRequestClassDto>> UpdateAsync(int id, [FromBody] HrRequestClassDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.HrRequestClassID)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            // Check if entity exists
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound($"HrRequestClass with ID {id} not found.");
            }

            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes an HR request class by ID.
        /// </summary>
        /// <param name="id">The HR request class ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _service.DeleteAsync<int>(id);
            if (!deleted)
            {
                return NotFound($"HrRequestClass with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

