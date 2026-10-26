using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;

namespace SMS.Generic.Services.Models;

/// <summary>
/// Result of a CYA archived report query: either JSON paging payload or an Excel download stream.
/// </summary>
public sealed class ReportRowsQueryResult
{
    private ReportRowsQueryResult(
        PagedList<ReportRowDto>? pagedJson,
        MemoryStream? excelStream,
        string? excelFileName)
    {
        PagedJson = pagedJson;
        ExcelStream = excelStream;
        ExcelFileName = excelFileName;
    }

    /// <summary>JSON grid payload when <see cref="IsExcel"/> is false.</summary>
    public PagedList<ReportRowDto>? PagedJson { get; }

    /// <summary>In-memory .xlsx when exporting; ASP.NET Core disposes the stream after the file response completes.</summary>
    public MemoryStream? ExcelStream { get; }

    public string? ExcelFileName { get; }

    public bool IsExcel => ExcelStream != null;

    public static ReportRowsQueryResult Json(PagedList<ReportRowDto> paged) =>
        new(paged, null, null);

    public static ReportRowsQueryResult Excel(MemoryStream stream, string fileName) =>
        new(null, stream, fileName);
}
