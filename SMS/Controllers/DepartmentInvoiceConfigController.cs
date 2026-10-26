using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Filters;
using SMS.Generic.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing department invoice config operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentInvoiceConfigController : ControllerBase
    {
        private readonly IDepartmentInvoiceConfigService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentInvoiceConfigController"/> class.
        /// </summary>
        /// <param name="service">The department invoice config service.</param>
        public DepartmentInvoiceConfigController(IDepartmentInvoiceConfigService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all department invoice configs.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of department invoice configs.</returns>
        [HttpGet]
        [AuthorizeAny(nameof(AuthorizationPolicyType.DepartmentAdminOnly), nameof(AuthorizationPolicyType.OperatorOnly))]
        [ProducesResponseType(typeof(IEnumerable<DepartmentInvoiceConfigDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DepartmentInvoiceConfigDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a department invoice config by ID.
        /// </summary>
        /// <param name="id">The department invoice config ID.</param>
        /// <returns>The department invoice config if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [ProducesResponseType(typeof(DepartmentInvoiceConfigDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DepartmentInvoiceConfigDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"DepartmentInvoiceConfig with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new department invoice config.
        /// </summary>
        /// <param name="dto">The department invoice config data to create.</param>
        /// <returns>The created department invoice config.</returns>
        [HttpPost]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [ProducesResponseType(typeof(DepartmentInvoiceConfigDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DepartmentInvoiceConfigDto>> UpsertAsync([FromBody] DepartmentInvoiceConfigDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdItem = await _service.UpsertAsync(dto);
            return createdItem;
        }


        /// <summary>
        /// Deletes a department invoice config by ID.
        /// </summary>
        /// <param name="id">The department invoice config ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = nameof(AuthorizationPolicyType.DepartmentAdminOnly))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"DepartmentInvoiceConfig with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
