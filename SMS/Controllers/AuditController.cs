using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Filters;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Dtos.Mapping;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services.Audit;
using System.Security;

namespace SMS.Controllers;

/// <summary>Read-only access to audit trail tables.</summary>
[ApiController]
[Route("api/[controller]")]
public class AuditController : ControllerBase
{
    private readonly IAuditHistoryService _auditHistoryService;
    private readonly IPostWorkflowAuditReadService _postWorkflowAuditReadService;
    private readonly IPostPositionFlowAuditReadService _postPositionFlowAuditReadService;

    public AuditController(
        IAuditHistoryService auditHistoryService,
        IPostWorkflowAuditReadService postWorkflowAuditReadService,
        IPostPositionFlowAuditReadService postPositionFlowAuditReadService)
    {
        _auditHistoryService = auditHistoryService;
        _postWorkflowAuditReadService = postWorkflowAuditReadService;
        _postPositionFlowAuditReadService = postPositionFlowAuditReadService;
    }

    /// <summary>
    /// Paged audit rows for <paramref name="table"/> scoped to the token <c>DepartmentID</c>, newest <see cref="AuditBase.ChangedOn"/> first.
    /// For <see cref="AuditHistoryTable.ClosedPostPosition"/> (5), returns enriched rows with job/employee snapshot from the main database.
    /// </summary>
    //[AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.IAAuditViewOnly))]
    [HttpGet("getall/{table}")]
    [ProducesResponseType(typeof(PagedList<AuditBase>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedList<ClosedPostPositionAuditListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetAllByTableAsync(
        AuditHistoryTable table,
        [FromQuery] PageParam? pagination,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] DateTime? changedOn = null,
        CancellationToken cancellationToken = default)
    {
        pagination ??= new PageParam();
        pagination.SortBy = nameof(AuditBase.ChangedOn);
        pagination.SortDirection = "desc";

        try
        {
            if (table == AuditHistoryTable.ClosedPostPosition)
            {
                var enriched = await _auditHistoryService
                    .GetClosedPostPositionAuditListAsync(pagination, default, startDate, endDate, changedOn, cancellationToken)
                    .ConfigureAwait(false);
                return Ok(enriched);
            }

            var page = await _auditHistoryService
                .GetAllAsync(table, null, pagination, default, startDate, endDate, changedOn, cancellationToken)
                .ConfigureAwait(false);
            return Ok(page);
        }
        catch (NotSupportedException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Audit rows for <paramref name="entityId"/> in <paramref name="table"/>, with <see cref="PageParam"/> (offset, limit, sortBy, sortDirection).
    /// </summary>
    //[AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.IAAuditViewOnly))]
    [HttpGet("getall/{table}/{entityId}")]
    [ProducesResponseType(typeof(PagedList<AuditBase>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedList<AuditBase>>> GetAllAsync(
        AuditHistoryTable table,
        string entityId,
        [FromQuery] PageParam? pagination,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] DateTime? changedOn = null,
        CancellationToken cancellationToken = default)
    {
        pagination ??= new PageParam();
        if (string.IsNullOrWhiteSpace(pagination.SortBy))
            pagination.SortBy = nameof(AuditBase.ChangedOn);
        if (string.IsNullOrWhiteSpace(pagination.SortDirection))
            pagination.SortDirection = "desc";

        try
        {
            var page = await _auditHistoryService
                .GetAllAsync(table, entityId, pagination, default, startDate, endDate, changedOn, cancellationToken)
                .ConfigureAwait(false);
            return Ok(page);
        }
        catch (NotSupportedException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Paged audit rows for <paramref name="table"/> where <see cref="AuditBase.EmployeeId"/> matches <paramref name="employeeId"/>, scoped to the token department.
    /// For <see cref="AuditHistoryTable.ClosedPostPosition"/> (5), returns enriched rows with job/employee snapshot from the main database.
    /// </summary>
    //[AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.IAAuditViewOnly))]
    [HttpGet("getall/{table}/employee/{employeeId:guid}")]
    [ProducesResponseType(typeof(PagedList<AuditBase>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedList<ClosedPostPositionAuditListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetAllByEmployeeIdAsync(
        AuditHistoryTable table,
        Guid employeeId,
        [FromQuery] PageParam? pagination,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] DateTime? changedOn = null,
        CancellationToken cancellationToken = default)
    {
        pagination ??= new PageParam();
        if (string.IsNullOrWhiteSpace(pagination.SortBy))
            pagination.SortBy = nameof(AuditBase.ChangedOn);
        if (string.IsNullOrWhiteSpace(pagination.SortDirection))
            pagination.SortDirection = "desc";

        try
        {
            if (table == AuditHistoryTable.ClosedPostPosition)
            {
                var enriched = await _auditHistoryService
                    .GetClosedPostPositionAuditListAsync(pagination, employeeId, startDate, endDate, changedOn, cancellationToken)
                    .ConfigureAwait(false);
                return Ok(enriched);
            }

            var page = await _auditHistoryService
                .GetAllAsync(table, null, pagination, employeeId, startDate, endDate, changedOn, cancellationToken)
                .ConfigureAwait(false);
            return Ok(page);
        }
        catch (NotSupportedException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Paged rows from <c>PostWorkflow_Audit</c> for the authenticated employee by default,
    /// or for <paramref name="employeeId"/> when supplied by a department admin (same pattern as <c>PostPosition/my-post-positions/by-statuses</c>), newest first by default.
    /// </summary>
    [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.IAAuditViewOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
    [HttpGet("post-workflow")]
    [ProducesResponseType(typeof(PagedList<EmployeePostListingItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedList<EmployeePostListingItemDto>>> GetPostWorkflowAuditsByEmployeeIdAsync(
        [FromQuery] PageParam? pagination,
        [FromQuery] PostWorkflowAttemptStatus? status,
        [FromQuery] Guid? employeeId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var page = await _postWorkflowAuditReadService
                .GetPagedByEmployeeIdAsync(pagination, status, employeeId, cancellationToken)
                .ConfigureAwait(false);
            return Ok(page.ToEmployeePostListingPaged());
        }
        catch (SecurityException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Paged Job Position Flow stories for the Audit Log grid.
    /// Events within each story are oldest → newest; stories ordered by latest event descending.
    /// </summary>
    [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.IAAuditViewOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
    [HttpGet("job-position-flow")]
    [ProducesResponseType(typeof(PagedList<JobPositionFlowStoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedList<JobPositionFlowStoryDto>>> GetJobPositionFlowsAsync(
        [FromQuery] PageParam? pagination,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var page = await _postPositionFlowAuditReadService
                .GetPagedAsync(pagination, search, cancellationToken)
                .ConfigureAwait(false);
            return Ok(page);
        }
        catch (SecurityException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// One Job Position Flow story (header + timeline). Events oldest → newest.
    /// Walks transfer chains via flow root.
    /// </summary>
    [AuthorizeAny(nameof(AuthorizationPolicyType.RequesterManagerOnly), nameof(AuthorizationPolicyType.IAAuditViewOnly), nameof(AuthorizationPolicyType.RegularEmployeeOnly))]
    [HttpGet("job-position-flow/{postPositionId:guid}")]
    [ProducesResponseType(typeof(JobPositionFlowStoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<JobPositionFlowStoryDto>> GetJobPositionFlowByIdAsync(
        Guid postPositionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var flow = await _postPositionFlowAuditReadService
                .GetByPostPositionIdAsync(postPositionId, cancellationToken)
                .ConfigureAwait(false);
            return Ok(flow);
        }
        catch (SecurityException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Searches audit records by EntityId, DisplayName, or UserId with paging.
    /// When <paramref name="employeeId"/> is set, restricts to rows whose persisted <see cref="AuditBase.EmployeeId"/> matches.
    /// When <paramref name="changedOn"/> is set, filters to that calendar day on <see cref="AuditBase.ChangedOn"/>.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedList<AuditBase>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedList<ClosedPostPositionAuditListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> SearchAsync(
        [FromQuery] AuditHistoryTable? table = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] PageParam? pageParam = null,
        [FromQuery] Guid? employeeId = null,
        [FromQuery] DateTime? changedOn = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pageParam ??= new PageParam();

            if (table == AuditHistoryTable.ClosedPostPosition)
                return Ok(await _auditHistoryService.SearchClosedPostPositionAuditListAsync(searchTerm, pageParam, employeeId, changedOn, cancellationToken).ConfigureAwait(false));

            var page = await _auditHistoryService.SearchAsync(table, searchTerm, pageParam, employeeId, changedOn, cancellationToken).ConfigureAwait(false);
            return Ok(page);
        }
        catch (NotSupportedException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

}
