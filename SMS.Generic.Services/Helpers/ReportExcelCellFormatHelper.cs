using System.Globalization;

namespace SMS.Generic.Services.Helpers;

/// <summary>
/// Shared cell string formatting for operator report Excel exports (archived <see cref="ReportRowDtoExcelTableBuilder"/> and watch commander <see cref="WatchCommanderReportDtoExcelTableBuilder"/>).
/// </summary>
public static class ReportExcelCellFormatHelper
{
    /// <summary>Slash-separated US date (<c>MM/dd/yyyy</c>).</summary>
    public static string FormatDateMmDdYy(DateOnly? date) =>
        date.HasValue ? date.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : string.Empty;

    /// <summary><c>HH:mm</c> (no seconds), aligned with <see cref="SMS.Generic.Dtos.Dtos.PostDto"/> time segments.</summary>
    public static string FormatTimeHhMm(TimeOnly? time) =>
        time.HasValue ? time.Value.ToString("HH:mm", CultureInfo.InvariantCulture) : string.Empty;

    /// <summary>Decimal without trailing fractional zeros (e.g. hours <c>6.5</c>, vehicle hours).</summary>
    public static string FormatTrimmedDecimal(decimal? value)
    {
        if (!value.HasValue)
            return string.Empty;
        var s = value.Value.ToString(CultureInfo.InvariantCulture);
        return TrimTrailingFractionalZeros(s);
    }

    /// <summary>Rounds to two decimal places for Excel numeric cells.</summary>
    public static decimal? RoundTwo(decimal? value) =>
        value.HasValue ? Math.Round(value.Value, 2, MidpointRounding.AwayFromZero) : null;

    /// <summary>Parses a zero-padded business code (e.g. <c>000267</c>) for Excel numeric storage.</summary>
    public static object? AsPaddedCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;
        var trimmed = code.Trim();
        return long.TryParse(trimmed, NumberStyles.None, CultureInfo.InvariantCulture, out var n)
            ? new ExcelPaddedCodeValue(n)
            : trimmed;
    }

    /// <summary>Parses a whole-number string (e.g. payroll, pay code) for Excel numeric storage.</summary>
    public static object? AsInteger(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        var trimmed = value.Trim();
        return long.TryParse(trimmed, NumberStyles.None, CultureInfo.InvariantCulture, out var n)
            ? new ExcelIntegerValue(n)
            : trimmed;
    }

    private static string TrimTrailingFractionalZeros(string s)
    {
        if (s.Contains('.', StringComparison.Ordinal))
            s = s.TrimEnd('0').TrimEnd('.');
        return s;
    }
}

/// <summary>Six-digit zero-padded code stored as an Excel number (format <c>000000</c>).</summary>
public readonly record struct ExcelPaddedCodeValue(long Value);

/// <summary>Whole number stored as an Excel number.</summary>
public readonly record struct ExcelIntegerValue(long Value);
