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
    /// Controller for managing vehicle approval routing operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeAny(nameof(AuthorizationPolicyType.DepartmentAdminOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]

    public class VehicleApprovalRoutingController : ControllerBase
    {
        private readonly IVehicleApprovalRoutingService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleApprovalRoutingController"/> class.
        /// </summary>
        /// <param name="service">The vehicle approval routing service.</param>
        public VehicleApprovalRoutingController(IVehicleApprovalRoutingService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all vehicle approval routing records.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of vehicle approval routing records.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VehicleApprovalRoutingDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VehicleApprovalRoutingDto>>> GetAllAsync([FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllAsync(null, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets all vehicle approval routing records by department equipment ID with routing group information.
        /// </summary>
        /// <param name="departmentEquipmentId">The department equipment ID to filter by.</param>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of vehicle approval routing records with routing group names.</returns>
        [HttpGet("department-equipment/{departmentEquipmentId}")]
        [ProducesResponseType(typeof(IEnumerable<VehicleApprovalRoutingResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VehicleApprovalRoutingResponseDto>>> GetAllVehicleRoutingGroupByDepartmentEquipmentIdAsync(Guid departmentEquipmentId, [FromQuery] PageParam? pagination = null)
        {
            var items = await _service.GetAllVehicleRoutingGroupByDepartmentEquipmentIdAsync(departmentEquipmentId, null, pagination);
            return Ok(items);
        }

        /// <summary>
        /// Gets a vehicle approval routing record by ID.
        /// </summary>
        /// <param name="id">The vehicle approval route ID.</param>
        /// <returns>The vehicle approval routing record if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VehicleApprovalRoutingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VehicleApprovalRoutingDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Vehicle approval routing with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates a single vehicle approval routing record.
        /// </summary>
        /// <param name="dto">The vehicle approval routing data to create or update.</param>
        /// <returns>The created or updated vehicle approval routing record.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(VehicleApprovalRoutingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VehicleApprovalRoutingDto>> UpsertAsync([FromBody] VehicleApprovalRoutingDto dto)
        {
            var result = await _service.UpsertAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Creates or updates multiple vehicle approval routing records with parent-child relationships.
        /// First record: IsParent = false, ParentId = null
        /// Subsequent records: IsParent = true, ParentId = previous record's ID
        /// </summary>
        /// <param name="dtos">The collection of vehicle approval routing data to create or update.</param>
        /// <returns>The created or updated vehicle approval routing records.</returns>
        [HttpPost("bulk")]
        [ProducesResponseType(typeof(IEnumerable<VehicleApprovalRoutingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<VehicleApprovalRoutingDto>>> BulkUpsertAsync(
            [FromBody] IEnumerable<VehicleApprovalRoutingDto> dtos,
            [FromHeader] bool isSuperUser = false)
        {
            var results = await _service.BulkUpsertAsync(dtos, isSuperUser);
            return Ok(results);
        }

        /// <summary>
        /// Deletes a vehicle approval routing record by ID.
        /// </summary>
        /// <param name="id">The vehicle approval route ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"Vehicle approval routing with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

