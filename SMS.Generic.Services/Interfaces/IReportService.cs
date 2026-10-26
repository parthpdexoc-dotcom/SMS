using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Models;

namespace SMS.Generic.Services.Interfaces;

/// <summary>
/// Builds activity / payroll report rows without a dedicated report entity.
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Returns report rows for the current user's department (from token via <see cref="ITokenParser.GetDepartmentID"/>).
    /// Optional <paramref name="startDate"/> / <paramref name="endDate"/> filter on work date.
    /// Optional <paramref name="postId"/> limits results to that post.
    /// When <paramref name="shouldGenerateReport"/> is <c>true</c>, all matching rows are loaded (pagination ignored) and an Excel workbook is returned in <see cref="ReportRowsQueryResult.ExcelStream"/>.
    /// </summary>
    Task<ReportRowsQueryResult> GetRowsAsync(
        DateOnly? startDate,
        DateOnly? endDate,
        Guid? postId,
        string? searchTerm,
        PageParam? pagination,
        bool shouldGenerateReport = false,
        bool? isPlaceholder = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns watch commander report rows for the current user's department (from token via <see cref="ITokenParser.GetDepartmentID"/>) and an optional work-date range (paged, or all when <paramref name="isGenerateReport"/> is true).
    /// Optional <paramref name="startDate"/> / <paramref name="endDate"/> filter on work date.
    /// Optional <paramref name="searchTerm"/> filters rows by employee name (&quot;Officer Name&quot;).
    /// When <paramref name="isGenerateReport"/> is <c>true</c>, all matching rows are loaded (pagination ignored) and an Excel workbook is returned in <see cref="WatchCommanderReportQueryResult.ExcelStream"/>.
    /// </summary>
    Task<WatchCommanderReportQueryResult> GetWatchCommanderReportAsync(
        DateOnly? startDate,
        DateOnly? endDate,
        string? searchTerm,
        PageParam? pagination,
        bool isGenerateReport = false,
        CancellationToken cancellationToken = default);
}
