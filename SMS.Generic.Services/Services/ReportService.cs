using System.Globalization;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Helpers;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Models;
using System.Net;

namespace SMS.Generic.Services.Services;

/// <inheritdoc />
public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly ITokenParser _tokenParser;
    private readonly IExcelService _excelService;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        IReportRepository reportRepository,
        ITokenParser tokenParser,
        IExcelService excelService,
        ILogger<ReportService> logger)
    {
        _reportRepository = reportRepository;
        _tokenParser = tokenParser;
        _excelService = excelService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ReportRowsQueryResult> GetRowsAsync(
        DateOnly? startDate,
        DateOnly? endDate,
        Guid? postId,
        string? searchTerm,
        PageParam? pagination,
        bool shouldGenerateReport = false,
        bool? isPlaceholder = null,
        CancellationToken cancellationToken = default)
    {
        var departmentId = _tokenParser.GetDepartmentID();
        if (!departmentId.HasValue)
        {
            throw new WebApiException(
                HttpStatusCode.BadRequest,
                ((int)HttpStatusCode.BadRequest).ToString(),
                "The current token does not include a department ID (DepartmentID claim).");
        }

        var pageParam = pagination ?? new PageParam();

        _logger.LogDebug(
            "Report rows requested (department: {DepartmentId}, postId: {PostId}, searchTerm: {SearchTerm}, filterStart: {FilterStart}, filterEnd: {FilterEnd}, shouldGenerateReport: {ShouldGenerateReport}, isPlaceholder: {IsPlaceholder}, skip: {Skip}, take: {Take})",
            departmentId,
            postId,
            searchTerm,
            startDate,
            endDate,
            shouldGenerateReport,
            isPlaceholder,
            pageParam.Offset,
            pageParam.Limit);

        var paged = await _reportRepository
            .GetReportRowsAsync(
                departmentId.Value,
                startDate,
                endDate,
                postId,
                searchTerm,
                pageParam,
                shouldGenerateReport,
                isPlaceholder,
                cancellationToken)
            .ConfigureAwait(false);

        if (!shouldGenerateReport)
        {
            return ReportRowsQueryResult.Json(paged);
        }

        var fileName = $"Archived-Report-{DateTime.UtcNow.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture)}.xlsx";
        var stream = _excelService.CreateWorkbookSingleSheet(
            "Archived",
            ReportRowDtoExcelTableBuilder.Headers,
            ReportRowDtoExcelTableBuilder.ToDataRows(paged.Data));

        return ReportRowsQueryResult.Excel(stream, fileName);
    }

    /// <inheritdoc />
    public async Task<WatchCommanderReportQueryResult> GetWatchCommanderReportAsync(
        DateOnly? startDate,
        DateOnly? endDate,
        string? searchTerm,
        PageParam? pagination,
        bool isGenerateReport = false,
        CancellationToken cancellationToken = default)
    {
        var departmentId = _tokenParser.GetDepartmentID();
        if (!departmentId.HasValue)
        {
            throw new WebApiException(
                HttpStatusCode.BadRequest,
                ((int)HttpStatusCode.BadRequest).ToString(),
                "The current token does not include a department ID (DepartmentID claim).");
        }

        var page = pagination ?? new PageParam();

        _logger.LogDebug(
            "Watch commander report requested (department: {DepartmentId}, searchTerm: {SearchTerm}, filterStart: {FilterStart}, filterEnd: {FilterEnd}, isGenerateReport: {IsGenerateReport}, skip: {Skip}, take: {Take})",
            departmentId,
            searchTerm,
            startDate,
            endDate,
            isGenerateReport,
            page.Offset,
            page.Limit);

        var paged = await _reportRepository
            .GetWatchCommanderReportPagedAsync(
                departmentId.Value,
                startDate,
                endDate,
                searchTerm,
                page,
                isGenerateReport,
                cancellationToken)
            .ConfigureAwait(false);

        if (!isGenerateReport)
        {
            return WatchCommanderReportQueryResult.Json(paged);
        }

        var fileName = $"Watch-Commander-Report-{DateTime.UtcNow.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture)}.xlsx";
        var stream = _excelService.CreateWorkbookSingleSheetWithTitleAndStyledHeader(
            "Watch Commander",
            "Watch Commander Report",
            WatchCommanderReportDtoExcelTableBuilder.Headers,
            WatchCommanderReportDtoExcelTableBuilder.ToDataRows(paged.Data));

        return WatchCommanderReportQueryResult.Excel(stream, fileName);
    }
}
