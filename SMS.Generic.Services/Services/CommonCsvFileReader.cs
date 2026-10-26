using System.Text;

namespace SMS.Generic.Services;

/// <summary>
/// Reads a CSV file into key/value rows while preserving raw string values exactly as present in the file.
/// </summary>
public class CommonCsvFileReader
{
    public CommonCsvReadResult Read(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return new CommonCsvReadResult { ErrorMessage = "CSV file path is empty." };
        }

        if (!File.Exists(filePath))
        {
            return new CommonCsvReadResult { ErrorMessage = $"CSV file not found: {filePath}" };
        }

        var rows = new List<Dictionary<string, string>>();
        var headers = Array.Empty<string>();

        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);

        string? line;
        var lineNumber = 0;
        while ((line = reader.ReadLine()) != null)
        {
            lineNumber++;

            if (lineNumber == 1)
            {
                headers = ParseCsvLine(line).Select(h => h.Trim()).ToArray();
                if (headers.Length == 0)
                {
                    return new CommonCsvReadResult { ErrorMessage = "CSV header row is empty." };
                }

                continue;
            }

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var values = ParseCsvLine(line);
            var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < headers.Length; i++)
            {
                var value = i < values.Count ? values[i] : string.Empty;
                row[headers[i]] = value;
            }

            rows.Add(row);
        }

        return new CommonCsvReadResult { Rows = rows };
    }

    private static List<string> ParseCsvLine(string line)
    {
        var values = new List<string>();
        if (line.Length == 0)
        {
            values.Add(string.Empty);
            return values;
        }

        var builder = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    builder.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }

                continue;
            }

            if (c == ',' && !inQuotes)
            {
                values.Add(builder.ToString());
                builder.Clear();
                continue;
            }

            builder.Append(c);
        }

        values.Add(builder.ToString());
        return values;
    }
}

public class CommonCsvReadResult
{
    public List<Dictionary<string, string>> Rows { get; set; } = new();
    public string? ErrorMessage { get; set; }
}
