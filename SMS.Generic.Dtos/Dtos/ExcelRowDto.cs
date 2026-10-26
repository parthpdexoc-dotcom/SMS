namespace SMS.Generic.Dtos.Dtos;

/// <summary>
/// Data transfer object for a single Excel row (column name to value).
/// </summary>
public class ExcelRowDto
{
    /// <summary>
    /// Cell values keyed by column header name.
    /// </summary>
    public IReadOnlyDictionary<string, object> Cells { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Builds a row from headers and cell values.
    /// Empty headers use column letter (A, B, ..., AA) as key so each column stays unique.
    /// </summary>
    public static ExcelRowDto FromCells(IReadOnlyList<string> headers, IEnumerable<object?> values)
    {
        var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        var index = 0;

        foreach (var value in values ?? Enumerable.Empty<object?>())
        {
            if (index >= headers.Count)
                break;
            dict[GetKey(headers, index)] = value ?? string.Empty;
            index++;
        }

        for (var i = index; i < headers.Count; i++)
        {
            var key = GetKey(headers, i);
            if (!dict.ContainsKey(key))
                dict[key] = string.Empty;
        }

        return new ExcelRowDto { Cells = dict };
    }

    private static string GetKey(IReadOnlyList<string> headers, int index)
    {
        var key = headers[index];
        return string.IsNullOrWhiteSpace(key) ? ExcelCellDto.GetColumnLetter(index) : key;
    }
}
