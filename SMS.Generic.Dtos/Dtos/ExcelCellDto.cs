using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;

namespace SMS.Generic.Dtos.Dtos;

/// <summary>
/// Data transfer object for a single Excel cell; holds the value and cell-get logic.
/// </summary>
public class ExcelCellDto
{
    /// <summary>
    /// The cell value (string, number, bool, or null).
    /// </summary>
    public object? Value { get; set; }

    /// <summary>
    /// Gets the value from an OpenXml Cell (shared string, number, boolean, or text).
    /// </summary>
    public static object? GetValue(Cell? cell, SharedStringTablePart? sharedStringPart)
    {
        if (cell?.CellValue == null)
            return null;

        var text = cell.CellValue.Text;
        if (string.IsNullOrEmpty(text))
            return null;

        if (cell.DataType?.Value == CellValues.SharedString)
            return ResolveSharedString(text, sharedStringPart);
        if (cell.DataType?.Value == CellValues.Boolean)
            return text == "1" || string.Equals(text, "TRUE", StringComparison.OrdinalIgnoreCase);
        if (cell.DataType?.Value == CellValues.Number || cell.DataType == null)
        {
            if (double.TryParse(text, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var num))
                return num;
        }
        return text;
    }

    /// <summary>
    /// Gets the column letter(s) for zero-based index: 0 -> A, 1 -> B, 26 -> AA.
    /// </summary>
    public static string GetColumnLetter(int index)
    {
        if (index < 0)
            return string.Empty;
        var letters = new List<char>();
        var n = index + 1;
        while (n > 0)
        {
            n--;
            letters.Add((char)('A' + (n % 26)));
            n /= 26;
        }
        letters.Reverse();
        return new string(letters.ToArray());
    }

    /// <summary>
    /// Gets the column index from a cell reference (e.g. "A1" -> 0, "B2" -> 1).
    /// </summary>
    public static int GetColumnIndex(string? cellReference)
    {
        if (string.IsNullOrEmpty(cellReference))
            return -1;
        var letters = TakeLetters(cellReference.AsSpan());
        if (letters.Length == 0)
            return -1;
        var index = 0;
        foreach (var c in letters)
            index = index * 26 + (char.ToUpperInvariant(c) - 'A' + 1);
        return index - 1;
    }

    private static string? ResolveSharedString(string indexText, SharedStringTablePart? sharedStringPart)
    {
        if (sharedStringPart == null || !int.TryParse(indexText, out var idx))
            return indexText;
        var items = sharedStringPart.SharedStringTable.Elements<SharedStringItem>().ToList();
        if (idx < 0 || idx >= items.Count)
            return indexText;
        return items[idx].InnerText;
    }

    private static ReadOnlySpan<char> TakeLetters(ReadOnlySpan<char> s)
    {
        int i = 0;
        while (i < s.Length && char.IsLetter(s[i]))
            i++;
        return s[..i];
    }
}
