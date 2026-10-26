using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Enums;
using SMS.Generic.Common;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;
using System.Net;
using SMS.Filters;

namespace SMS.Controllers
{
    /// <summary>
    /// Controller for managing employee type operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeTypeController : ControllerBase
    {
        private readonly IEmployeeTypeService _service;
        private readonly ILogger<EmployeeTypeController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeTypeController"/> class.
        /// </summary>
        /// <param name="service">The employee type service.</param>
        /// <param name="logger">The logger instance.</param>
        public EmployeeTypeController(IEmployeeTypeService service, ILogger<EmployeeTypeController> logger)
        {
            _service = service;
            _logger = logger;
        }
        /// <summary>
        /// Gets all employee types.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <param name="isActive">When true, only active; when false, only inactive; when omitted, no status filter (still excludes system-user rows).</param>
        /// <returns>Paged employee types.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<EmployeeTypeDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]

        public async Task<ActionResult<PagedList<EmployeeTypeDto>>> GetAllAsync(
        [FromQuery] PageParam? pagination = null,
        [FromQuery] bool? isActive = null)
        {
            pagination ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                pagination.SortBy = nameof(EmployeeTypeDto.EmployeeTypeShort);
            }

            var result = await _service.GetAllPagedAsync(x =>
                !x.IsSystemUser &&
                (!isActive.HasValue || x.EmployeeTypeActive == isActive.Value),
                pagination);
            return Ok(result);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedList<EmployeeTypeDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]
        public async Task<ActionResult<PagedList<EmployeeTypeDto>>> SearchAsync(
            [FromQuery] string? searchTerm,
            [FromQuery] bool? isActive,
            [FromQuery] PageParam? pageParam = null)
        {
            pageParam ??= new PageParam();

            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
            {
                pageParam.SortBy = nameof(EmployeeTypeDto.EmployeeTypeShort);
            }

            var result = await _service.SearchAsync(
                searchTerm,
                pageParam,
                isActive.HasValue ? x => x.EmployeeTypeActive == isActive.Value : null
            );

            return Ok(result);
        }

        /// <summary>
        /// Gets eligible employee type options for post dropdowns.
        /// </summary>
        [HttpPost("eligibleemployeeoptions")]
        [ProducesResponseType(typeof(IReadOnlyList<SelectOptionDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        public async Task<ActionResult<IReadOnlyList<SelectOptionDto>>> GetEligibleEmployeeOptionsAsync([FromBody] SelectOptionsRequestDto? body)
        {
            var request = SelectOptionsRequestDto.OrDefault(body);
            return Ok(await _service.GetOptionsAsync(
                et => et.EmployeeTypeShort!,
                x => !x.IsSystemUser
                    && (x.EmployeeTypeActive == true || (request.SelectedId.HasValue && x.EmployeeTypeID == request.SelectedId.Value))
                    && x.IsEligible,
                request.SearchTerm));
        }

        /// <summary>
        /// Gets supervisor employee type options for post dropdowns.
        /// </summary>
        [HttpPost("supervisoremployeeoptions")]
        [ProducesResponseType(typeof(IReadOnlyList<SelectOptionDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
        public async Task<ActionResult<IReadOnlyList<SelectOptionDto>>> GetSupervisorEmployeeOptionsAsync([FromBody] SelectOptionsRequestDto? body)
        {
            var request = SelectOptionsRequestDto.OrDefault(body);
            return Ok(await _service.GetOptionsAsync(
                et => et.EmployeeTypeShort!,
                x => !x.IsSystemUser
                    && (x.EmployeeTypeActive == true || (request.SelectedId.HasValue && x.EmployeeTypeID == request.SelectedId.Value))
                    && x.IsEligible
                    && x.IsSupervisor,
                request.SearchTerm));
        }

        [HttpGet("GetAllActiveEmployeeType")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeTypeDto>), StatusCodes.Status200OK)]
        [AuthorizeAny(nameof(AuthorizationPolicyType.EmployeesViewManage), nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]

        public async Task<ActionResult<IEnumerable<EmployeeTypeDto>>> GetGetAllActiveEmployeeTypeAsync(
            [FromQuery] Guid? departmentId = null,
            [FromQuery] Guid? selectedId = null)
        {
            var items = await _service.GetAllAsync(
                x => !x.IsSystemUser && (!departmentId.HasValue || x.DepartmentID == departmentId.Value) &&
                     (x.EmployeeTypeActive == true || (selectedId.HasValue && x.EmployeeTypeID == selectedId.Value)),
                null,
                pageParam: null);
            return Ok(items.OrderBy(x => x.EmployeeTypeShort));
        }

        /// <summary>
        /// Gets an employee type by ID.
        /// </summary>
        /// <param name="id">The employee type ID.</param>
        /// <returns>The employee type if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EmployeeTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]

        public async Task<ActionResult<EmployeeTypeDto>> GetByIdAsync(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"EmployeeType with ID {id} not found.");
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates or updates an employee type. When updating with <c>sameRankIds</c>, linked types receive the parent's rank in one transaction.
        /// </summary>
        /// <param name="includeAffectedSameRankTypes">When true and update includes <c>sameRankIds</c>, response is <see cref="EmployeeTypeUpsertResultDto"/> with affected peers.</param>
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(EmployeeTypeUpsertResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]
        public async Task<IActionResult> UpsertAsync(
            [FromBody] EmployeeTypeDto employeeTypeDto,
            [FromQuery] bool includeAffectedSameRankTypes = false)
        {
            try
            {
                if (includeAffectedSameRankTypes)
                    return Ok(await _service.UpsertEmployeeTypeWithResultAsync(employeeTypeDto, true).ConfigureAwait(false));
                return Ok(await _service.UpsertAsync(employeeTypeDto).ConfigureAwait(false));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (WebApiException ex)
            {
                return StatusCode((int)ex.HttpStatusCode, new { message = ex.ErrorMessage });
            }
        }

        /// <summary>
        /// Updates an existing employee type.
        /// </summary>
        /// <param name="id">The employee type ID.</param>
        /// <param name="employeeTypeDto">The employee type data to update.</param>
        /// <returns>The updated employee type.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EmployeeTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(EmployeeTypeUpsertResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]
        public async Task<IActionResult> UpdateAsync(
            Guid id,
            [FromBody] EmployeeTypeDto employeeTypeDto,
            [FromQuery] bool includeAffectedSameRankTypes = false)
        {
            if (id != employeeTypeDto.EmployeeTypeID)
                return BadRequest("The ID in the URL does not match the ID in the request body.");

            if (await _service.GetByIdAsync(id) == null)
                return NotFound($"EmployeeType with ID {id} not found.");

            try
            {
                if (includeAffectedSameRankTypes)
                    return Ok(await _service.UpsertEmployeeTypeWithResultAsync(employeeTypeDto, true).ConfigureAwait(false));
                return Ok(await _service.UpsertAsync(employeeTypeDto).ConfigureAwait(false));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (WebApiException ex)
            {
                return StatusCode((int)ex.HttpStatusCode, new { message = ex.ErrorMessage });
            }
        }

        /// <summary>
        /// Updates the rank of an employee type.
        /// </summary>
        /// <param name="id">The employee type ID.</param>
        /// <param name="rank">Rank slot (1 = top group). Ignored when <paramref name="atDisplayRow"/> is set.</param>
        /// <param name="atDisplayRow">Optional 1-based grid row where the moved type should appear (same order as list: rank, then title). Use for drag-and-drop when the UI sends row index, not slot index.</param>
        /// <returns>Success response if rank updated successfully.</returns>
        [HttpPut("{id}/rank/{rank}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]

        public async Task<IActionResult> UpdateRankAsync(Guid id, int rank, [FromQuery] int? atDisplayRow)
        {
            try
            {
                if (atDisplayRow.HasValue)
                    await _service.UpdateRankAtDisplayRowAsync(id, atDisplayRow.Value);
                else
                    await _service.UpdateRankAsync(id, rank);
                return Ok(new { message = "Rank updated successfully" });
            }
            catch (WebApiException ex)
            {
                return StatusCode((int)ex.HttpStatusCode, new
                {
                    message = ex.ErrorMessage,
                    status = (int)ex.HttpStatusCode
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update rank for EmployeeTypeId {EmployeeTypeId}", id);
                return BadRequest(new
                {
                    message = ex.Message,
                    status = (int)HttpStatusCode.BadRequest
                });
            }
        }

        /// <summary>
        /// Deletes an employee type by ID.
        /// </summary>
        /// <param name="id">The employee type ID.</param>
        /// <returns>NoContent if deleted; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = nameof(AuthorizationPolicyType.EmployeesViewManage))]

        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _service.DeleteAsync<Guid>(id);
            if (!deleted)
            {
                return NotFound($"EmployeeType with ID {id} not found.");
            }
            return NoContent();
        }
    }
}

