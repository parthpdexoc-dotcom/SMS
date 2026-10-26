namespace SMS.Generic.Dtos.Dtos;

/// <summary>
/// Data transfer object for a single Excel sheet.
/// </summary>
public class ExcelSheetDto
{
    /// <summary>
    /// Display name of the sheet.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Header names from the first row.
    /// </summary>
    public IReadOnlyList<string> Headers { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Data rows.
    /// </summary>
    public IReadOnlyList<ExcelRowDto> Rows { get; set; } = Array.Empty<ExcelRowDto>();

    /// <summary>
    /// Builds a sheet from name and rows (first row = headers). Skips rows where all cells are empty.
    /// </summary>
    public static ExcelSheetDto FromRows(string name, IEnumerable<IEnumerable<object?>> rows)
    {
        var rowsList = rows?.ToList() ?? new List<IEnumerable<object?>>();
        if (rowsList.Count == 0)
            return new ExcelSheetDto { Name = name ?? string.Empty };

        var headers = rowsList[0].Select(v => v?.ToString() ?? string.Empty).ToList();
        var dataRows = rowsList
            .Skip(1)
            .Where(row => !IsEmptyRow(row))
            .Select(row => ExcelRowDto.FromCells(headers, row))
            .ToList();

        return new ExcelSheetDto
        {
            Name = name ?? string.Empty,
            Headers = headers,
            Rows = dataRows
        };
    }

    private static bool IsEmptyRow(IEnumerable<object?> row)
    {
        if (row == null)
            return true;
        return row.All(v => v == null || (v is string s && string.IsNullOrWhiteSpace(s)) || string.IsNullOrWhiteSpace(v.ToString()));
    }
}
