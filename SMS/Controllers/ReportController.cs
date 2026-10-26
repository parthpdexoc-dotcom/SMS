using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;

namespace SMS.Controllers;

/// <summary>
/// CYA Archived report — operator report rows (DTO-only; no ActivityReport persistence).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Gets paged report rows for the caller's department. Optional <c>startDate</c> / <c>endDate</c> filter on work date.
    /// Optional <c>postId</c> limits rows to that post.
    /// Pass <c>shouldGenerateReport=true</c> to load all matching rows (pagination ignored) and download an Excel file (<c>.xlsx</c>).
    /// Example (paged JSON): <c>GET /api/Report/archived-report?startDate=2023-11-01&amp;endDate=2023-11-07&amp;page=1&amp;pageSize=20</c>
    /// Example (by post): <c>GET /api/Report/archived-report?postId=...</c>
    /// Example (Excel): <c>GET /api/Report/archived-report?startDate=2023-11-01&amp;endDate=2023-11-07&amp;shouldGenerateReport=true</c>
    /// </summary>
    [HttpGet("archived-report")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
    [Produces("application/json", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [ProducesResponseType(typeof(PagedList<ReportRowDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRowsAsync(
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] Guid? postId,
        [FromQuery] string? searchTerm,
        [FromQuery] PageParam? pagination = null,
        [FromQuery(Name = "isgeneratereport")] bool isGenerateReport = false,
        [FromQuery] bool? isPlaceholder = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _reportService.GetRowsAsync(
            startDate,
            endDate,
            postId,
            searchTerm,
            pagination,
            isGenerateReport,
            isPlaceholder,
            cancellationToken);

        if (result.IsExcel && result.ExcelStream != null && !string.IsNullOrEmpty(result.ExcelFileName))
        {
            return File(
                result.ExcelStream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                result.ExcelFileName);
        }

        return Ok(result.PagedJson);
    }

    /// <summary>
    /// Gets paged report rows containing ONLY placeholder records for the caller's department.
    /// Supports the same date, search, post, pagination, and Excel export (<c>isgeneratereport=true</c>) filters.
    /// Example: <c>GET /api/Report/archived-report-placeholders?startDate=2023-11-01&amp;endDate=2023-11-07</c>
    /// </summary>
    [HttpGet("archived-report-placeholders")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.OperatorOnly))]
    [Produces("application/json", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [ProducesResponseType(typeof(PagedList<ReportRowDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlaceholderRowsAsync(
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] Guid? postId,
        [FromQuery] string? searchTerm,
        [FromQuery] PageParam? pagination = null,
        [FromQuery(Name = "isgeneratereport")] bool isGenerateReport = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _reportService.GetRowsAsync(
            startDate,
            endDate,
            postId,
            searchTerm,
            pagination,
            isGenerateReport,
            isPlaceholder: true,
            cancellationToken);

        if (result.IsExcel && result.ExcelStream != null && !string.IsNullOrEmpty(result.ExcelFileName))
        {
            return File(
                result.ExcelStream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                result.ExcelFileName);
        }

        return Ok(result.PagedJson);
    }

    /// <summary>
    /// Gets watch commander report rows for the caller's department (from <c>DepartmentID</c> on the token), with an optional work-date range (paged JSON, or all rows as Excel when <c>isgeneratereport=true</c>).
    /// Optional <c>startDate</c> / <c>endDate</c> filter on work date.
    /// Example (JSON): <c>GET /api/Report/watch-commander-report?startDate=2026-03-04&amp;endDate=2026-03-14&amp;isgeneratereport=false</c>
    /// Example (Excel): <c>GET /api/Report/watch-commander-report?startDate=2026-03-04&amp;endDate=2026-03-14&amp;isgeneratereport=true</c>
    /// </summary>
    [HttpGet("watch-commander-report")]
    [Authorize(Policy = nameof(AuthorizationPolicyType.CommCenterOrDepartmentAdmin))]
    [Produces("application/json", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [ProducesResponseType(typeof(PagedList<WatchCommanderReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWatchCommanderReportAsync(
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] string? searchTerm,
        [FromQuery] PageParam? pagination,
        [FromQuery(Name = "isgeneratereport")] bool isGenerateReport = false,
        CancellationToken cancellationToken = default)
    {
        pagination ??= new PageParam();

        var result = await _reportService.GetWatchCommanderReportAsync(
            startDate,
            endDate,
            searchTerm,
            pagination,
            isGenerateReport,
            cancellationToken);

        if (result.IsExcel && result.ExcelStream != null && !string.IsNullOrEmpty(result.ExcelFileName))
        {
            return File(
                result.ExcelStream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                result.ExcelFileName);
        }

        return Ok(result.PagedJson);
    }
}
