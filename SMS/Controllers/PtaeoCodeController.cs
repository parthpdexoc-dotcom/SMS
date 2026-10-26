using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Services.Interfaces;
using SMS.Filters;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing PTAEO_Code (Project/Task/Award/Fund/Organization) operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PtaeoCodeController : ControllerBase
    {
        private readonly IPtaeoCodeService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PtaeoCodeController"/> class.
        /// </summary>
        public PtaeoCodeController(IPtaeoCodeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets active PTAEO codes only (<see cref="PtaeoCode.Status"/> is true).
        /// </summary>
        /// <returns>A collection of PTAEO codes.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PtaeoCodeDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<IEnumerable<PtaeoCodeDto>>> GetAllAsync()
        {
            var items = await _service.GetAllAsync(p => p.Status, null, null);
            return Ok(items);
        }

        /// <summary>
        /// Gets lightweight PTAEO code options for dropdowns (composite label, searchable).
        /// </summary>
        [HttpPost("ptaeocodeoptions")]
        [ProducesResponseType(typeof(IReadOnlyList<SelectOptionDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<IReadOnlyList<SelectOptionDto>>> GetPtaeoCodeOptionsAsync([FromBody] SelectOptionsRequestDto? body)
        {
            var request = SelectOptionsRequestDto.OrDefault(body);
            return Ok(await _service.GetOptionsAsync(request.SearchTerm, request.SelectedId));
        }

        /// <summary>
        /// Gets a PTAEO code by ID.
        /// </summary>
        /// <param name="id">The PTAEO code ID (PtaeoCodeID as Guid).</param>
        /// <returns>The PTAEO code if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PtaeoCodeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        public async Task<ActionResult<PtaeoCodeDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound($"PTAEO code with ID {id} not found.");
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a PTAEO code.
        /// </summary>
        /// <param name="dto">The PTAEO code data to create or update.</param>
        /// <returns>The created or updated PTAEO code.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PtaeoCodeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<ActionResult<PtaeoCodeDto>> UpsertAsync([FromBody] PtaeoCodeDto dto)
        {
            var result = await _service.UpsertAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing PTAEO code.
        /// </summary>
        /// <param name="id">The PTAEO code ID.</param>
        /// <param name="dto">The PTAEO code data to update.</param>
        /// <returns>The updated PTAEO code.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PtaeoCodeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<ActionResult<PtaeoCodeDto>> UpdateAsync(Guid id, [FromBody] PtaeoCodeDto dto)
        {
            if (dto.PtaeoCodeID.HasValue && id != dto.PtaeoCodeID.Value)
                return BadRequest("The ID in the URL does not match the ID in the request body.");

            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
                return NotFound($"PTAEO code with ID {id} not found.");

            dto.PtaeoCodeID = id;
            var updatedItem = await _service.UpsertAsync(dto);
            return Ok(updatedItem);
        }

        /// <summary>
        /// Deletes a PTAEO code by ID.
        /// </summary>
        /// <param name="id">The PTAEO code ID (PtaeoCodeID as Guid).</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]

        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
                return NotFound($"PTAEO code with ID {id} not found.");
            return NoContent();
        }
    }
}
