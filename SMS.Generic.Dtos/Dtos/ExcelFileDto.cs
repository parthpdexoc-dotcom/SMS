namespace SMS.Generic.Dtos.Dtos;

/// <summary>
/// Data transfer object for an uploaded Excel file (sheets and rows).
/// </summary>
public class ExcelFileDto
{
    /// <summary>
    /// File name or identifier.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Sheets in the file.
    /// </summary>
    public IReadOnlyList<ExcelSheetDto> Sheets { get; set; } = Array.Empty<ExcelSheetDto>();
}
