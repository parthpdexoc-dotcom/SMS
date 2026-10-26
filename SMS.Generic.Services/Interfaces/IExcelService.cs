using SMS.Generic.Dtos.Dtos;
using static SMS.Generic.Dtos.Dtos.RosterDto;

namespace SMS.Generic.Services.Interfaces;

/// <summary>
/// Service for reading Excel files into DTO or flat rows, and creating simple .xlsx workbooks.
/// </summary>
public interface IExcelService
{
    /// <summary>
    /// Reads an Excel file from a stream and returns sheet/row data as DTO.
    /// </summary>
    ExcelFileDto Read(Stream stream, string? fileName = null);

    /// <summary>
    /// Reads all .xlsx files from the given folder (first sheet per file) and returns flat rows plus optional error.
    /// </summary>
    ExcelFolderReadResult ReadFromFolder(string folderPath);

    /// <summary>
    /// Creates a new .xlsx with a single sheet: first row is <paramref name="headers"/>, then one row per entry in <paramref name="dataRows"/>.
    /// Caller owns the returned stream (dispose after use).
    /// </summary>
    MemoryStream CreateWorkbookSingleSheet(
        string sheetName,
        IReadOnlyList<string> headers,
        IEnumerable<IReadOnlyList<object?>> dataRows);

    /// <summary>
    /// Creates a single-sheet workbook: row 1 is a merged centered <paramref name="reportTitle"/>, row 2 is <paramref name="headers"/> with a solid blue header style, then data rows.
    /// </summary>
    MemoryStream CreateWorkbookSingleSheetWithTitleAndStyledHeader(
        string sheetName,
        string reportTitle,
        IReadOnlyList<string> headers,
        IEnumerable<IReadOnlyList<object?>> dataRows);

    /// <summary>
    /// Creates a roster export workbook from backend-preformatted roster tables.
    /// </summary>
    MemoryStream CreateRosterInformationWorkbook(DateOnly dateOfWork, RosterResponseDto roster);

}

/// <summary>
/// Result of reading Excel files from a folder.
/// </summary>
public class ExcelFolderReadResult
{
    public List<Dictionary<string, string>> Rows { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Provides the configured Excel folder path.
/// </summary>
public interface IExcelFolderPathProvider
{
    string GetPath();
}
