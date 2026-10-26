using System.Globalization;
using AutoMapper;
using SMS.Generic.Dtos.Dtos;

namespace SMS.Mappings;

/// <summary>
/// Converts an Excel row (Dictionary with keys like PROJECT_NUMBER, TASK_NUMBER, etc.) to PtaeoCodeDto.
/// START_DATE / END_DATE from PTAEO CSV use compact <c>YYYYMMDD</c> (e.g. 20231001, 20260930).
/// <see cref="PtaeoCodeDto.Status"/> is not set from the row; assign from application logic when needed.
/// </summary>
public class ExcelRowToPtaeoCodeDtoConverter : ITypeConverter<Dictionary<string, string>, PtaeoCodeDto>
{
    private static readonly StringComparison KeyComparison = StringComparison.OrdinalIgnoreCase;

    public PtaeoCodeDto Convert(Dictionary<string, string> source, PtaeoCodeDto destination, ResolutionContext context)
    {
        if (source == null)
            return destination ?? new PtaeoCodeDto();

        return new PtaeoCodeDto
        {
            ProjectNumber = GetValue(source, "PROJECT_NUMBER"),
            TaskNumber = GetValue(source, "TASK_NUMBER"),
            AwardNumber = GetValue(source, "AWARD_NUMBER"),
            FundNumber = GetValue(source, "FUNDNUMBER"),
            OrganizationName = GetValue(source, "ORGANIZATION_NAME"),
            OrganizationNumber = GetValue(source, "ORGANIZATION_NUMBER"),
            StartDate = ParseOptionalDate(GetValue(source, "START_DATE")),
            EndDate = ParseOptionalDate(GetValue(source, "END_DATE")),
        };
    }

    private static string? GetValue(Dictionary<string, string> row, string key)
    {
        var pair = row.FirstOrDefault(p => string.Equals(p.Key, key, KeyComparison));
        return pair.Value?.Trim();
    }

    private static DateOnly? ParseOptionalDate(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var t = raw.Trim();

        // PTAEO export: YYYYMMDD only digits (no separators).
        if (t.Length == 8 && IsAllAsciiDigits(t) &&
            DateOnly.TryParseExact(t, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var compact))
            return compact;

        if (DateOnly.TryParse(t, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
            return d;

        return null;
    }

    private static bool IsAllAsciiDigits(string s)
    {
        for (var i = 0; i < s.Length; i++)
        {
            var c = s[i];
            if (c < '0' || c > '9')
                return false;
        }

        return true;
    }
}
