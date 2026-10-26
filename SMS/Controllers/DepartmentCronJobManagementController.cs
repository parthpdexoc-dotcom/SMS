using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers;

/// <summary>Per-department background job schedules (CRUD for <c>DepartmentCronJobManagement</c> UI). Super User only — not Department Admin.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = nameof(AuthorizationPolicyType.SuperUserOnly))]
public class DepartmentCronJobManagementController : ControllerBase
{
    private readonly IDepartmentCronJobManagementService _service;

    public DepartmentCronJobManagementController(IDepartmentCronJobManagementService service)
    {
        _service = service;
    }

    /// <summary>Saved cron schedules for the department (empty if none; no placeholder rows for unconfigured jobs).</summary>
    [HttpGet("department/{departmentId:guid}/schedules")]
    [ProducesResponseType(typeof(IReadOnlyList<DepartmentCronScheduleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DepartmentCronScheduleDto>>> GetSchedulesByDepartmentIdAsync(Guid departmentId,CancellationToken cancellationToken)
    {
        var list = await _service.GetSchedulesByDepartmentIdAsync(departmentId, cancellationToken).ConfigureAwait(false);
        return Ok(list);
    }

    /// <summary>Job types that are not yet configured for the department (for the Add-cron flow).</summary>
    [HttpGet("department/{departmentId:guid}/unconfigured-job-types")]
    [ProducesResponseType(typeof(IReadOnlyList<DepartmentCronJobTypeOptionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DepartmentCronJobTypeOptionDto>>> GetUnconfiguredJobTypeOptionsAsync(
        Guid departmentId,
        CancellationToken cancellationToken)
    {
        var list = await _service.GetUnconfiguredJobTypeOptionsForDepartmentAsync(departmentId, cancellationToken)
            .ConfigureAwait(false);
        return Ok(list);
    }

    /// <summary>Insert or update one (department, job) schedule; backend builds the cron string.</summary>
    [HttpPost("upsert")]
    [ProducesResponseType(typeof(DepartmentCronScheduleDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DepartmentCronScheduleDto>> UpsertAsync([FromBody] DepartmentCronScheduleUpsertDto dto,CancellationToken cancellationToken)
    {
        var result = await _service.UpsertScheduleAsync(dto, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }
}
