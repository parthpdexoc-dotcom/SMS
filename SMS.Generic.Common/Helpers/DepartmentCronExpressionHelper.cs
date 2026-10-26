using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using SMS.Generic.Enums;

namespace SMS.Generic.Common.Helpers;

/// <summary>
/// Standard 5-field crontab (minute, hour, day of month, month, day of week) used by department cron config
/// with Cronos (supports <c>#</c> and <c>L</c> in the day-of-week field).
/// </summary>
public static class DepartmentCronExpressionHelper
{
    private static readonly Regex NthDowInMonth =
        new(@"^(?<dow>\d+)#(?<n>[1-5])$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(200));

    private static readonly Regex LastDowInMonth =
        new(@"^(?<dow>\d+)L$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(200));

    public static string Build(
        ScheduleFrequency frequency,
        int minute,
        int hour,
        int? dayOfWeek,
        int? dayOfMonth,
        MonthlySubPattern monthlySubPattern = MonthlySubPattern.ByCalendarDay,
        int? monthWeekOrdinal = null,
        int? intervalMinutes = null,
        IReadOnlyList<int>? weekDays = null)
    {
        if (hour is < 0 or > 23) throw new ArgumentOutOfRangeException(nameof(hour));
        if (minute is < 0 or > 59) throw new ArgumentOutOfRangeException(nameof(minute));

        return frequency switch
        {
            ScheduleFrequency.Daily => $"{minute} {hour} * * *",
            ScheduleFrequency.Weekly => $"{minute} {hour} * * {FormatDayOfWeekForCronOrList(dayOfWeek, weekDays)}",
            ScheduleFrequency.Monthly => BuildMonthly(
                minute,
                hour,
                dayOfWeek,
                dayOfMonth,
                monthlySubPattern,
                monthWeekOrdinal),
            ScheduleFrequency.EveryNMinutes => BuildEveryNMinutes(
                intervalMinutes
                ?? throw new ArgumentException("IntervalMinutes (1-59) is required for an every-N-minutes schedule.", nameof(intervalMinutes))),
            ScheduleFrequency.Custom => throw new InvalidOperationException("Custom schedules use a raw expression; do not use Build for Custom."),
            _ => throw new ArgumentOutOfRangeException(nameof(frequency))
        };
    }

    /// <summary>Builds <c>*/N * * * *</c> (every <paramref name="n" /> minutes).</summary>
    public static string BuildEveryNMinutes(int n)
    {
        if (n is < 1 or > 59)
            throw new ArgumentOutOfRangeException(nameof(n), "N must be between 1 and 59.");
        return $"*/{n} * * * *";
    }

    /// <summary>Detects the every-N-minutes form produced by <see cref="BuildEveryNMinutes" />.</summary>
    public static bool TryParseEveryNMinutes(string? cron, out int n)
    {
        n = 0;
        if (string.IsNullOrWhiteSpace(cron))
        {
            return false;
        }

        var p = cron.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (p.Length != 5)
        {
            return false;
        }

        if (p[1] != "*" || p[2] != "*" || p[3] != "*" || p[4] != "*")
        {
            return false;
        }

        var minuteField = p[0];
        if (!minuteField.StartsWith("*/", StringComparison.Ordinal))
        {
            return false;
        }

        if (!int.TryParse(minuteField.AsSpan(2), NumberStyles.None, CultureInfo.InvariantCulture, out n) || n is < 1 or > 59)
        {
            return false;
        }

        return true;
    }

    private static string BuildMonthly(
        int minute,
        int hour,
        int? dayOfWeek,
        int? dayOfMonth,
        MonthlySubPattern pattern,
        int? monthWeekOrdinal)
    {
        return pattern switch
        {
            MonthlySubPattern.ByCalendarDay =>
                $"{minute} {hour} {FormatDayOfMonth(dayOfMonth)} * *",
            MonthlySubPattern.ByWeekdayOrdinal => $"{minute} {hour} ? * {FormatDowWithOrdinal(dayOfWeek, monthWeekOrdinal)}",
            MonthlySubPattern.ByLastWeekday => $"{minute} {hour} ? * {FormatDowLastInMonth(dayOfWeek)}",
            _ => throw new ArgumentOutOfRangeException(nameof(pattern))
        };
    }

    /// <summary>
    /// Parses a 5-field cron we produced or a compatible expression.
    /// </summary>
    public static bool TryParse(
        string? cron,
        out int minute,
        out int hour,
        out ScheduleFrequency frequency,
        out int? dayOfWeek,
        out int? dayOfMonth,
        out MonthlySubPattern monthlySubPattern,
        out int? monthWeekOrdinal,
        out IReadOnlyList<int>? weekDays)
    {
        minute = 0;
        hour = 0;
        frequency = ScheduleFrequency.Daily;
        dayOfWeek = null;
        dayOfMonth = null;
        monthlySubPattern = MonthlySubPattern.ByCalendarDay;
        monthWeekOrdinal = null;
        weekDays = null;

        if (string.IsNullOrWhiteSpace(cron))
            return false;

        var p = cron.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (p.Length != 5)
            return false;

        if (!int.TryParse(p[0], NumberStyles.None, CultureInfo.InvariantCulture, out var m) || m is < 0 or > 59)
            return false;
        if (!int.TryParse(p[1], NumberStyles.None, CultureInfo.InvariantCulture, out var h) || h is < 0 or > 23)
            return false;

        var dayOfMonthField = p[2];
        var dayOfWeekField = p[4];

        // Nth weekday (must run before 1-31 in dom, e.g. 1 * 1#1 is not "day 1" monthly)
        if (dayOfWeekField.Contains('#', StringComparison.Ordinal) && IsAnyOrDayWildcard(dayOfMonthField))
        {
            var match = NthDowInMonth.Match(dayOfWeekField);
            if (match.Success
                && int.TryParse(match.Groups["dow"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var d)
                && int.TryParse(match.Groups["n"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var n)
                && n is >= 1 and <= 5
                && d is >= 0 and <= 7)
            {
                if (d == 7) d = 0;
                if (d is >= 0 and <= 6)
                {
                    minute = m;
                    hour = h;
                    frequency = ScheduleFrequency.Monthly;
                    dayOfWeek = d;
                    dayOfMonth = null;
                    monthlySubPattern = MonthlySubPattern.ByWeekdayOrdinal;
                    monthWeekOrdinal = n;
                    return true;
                }
            }
        }

        // Last weekday in month: 1L, 5L, … (before parsing calendar day, so 1L is not confused with "day 1 of month")
        if (!dayOfWeekField.Contains('#', StringComparison.Ordinal) && LastDowInMonth.IsMatch(dayOfWeekField))
        {
            var lm = LastDowInMonth.Match(dayOfWeekField);
            if (lm.Success
                && int.TryParse(lm.Groups["dow"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var d)
                && d is >= 0 and <= 7)
            {
                if (d == 7) d = 0;
                if (d is >= 0 and <= 6)
                {
                    minute = m;
                    hour = h;
                    frequency = ScheduleFrequency.Monthly;
                    dayOfWeek = d;
                    dayOfMonth = null;
                    monthlySubPattern = MonthlySubPattern.ByLastWeekday;
                    monthWeekOrdinal = null;
                    return true;
                }
            }
        }

        if (int.TryParse(dayOfMonthField, NumberStyles.None, CultureInfo.InvariantCulture, out var dom) && dom is >= 1 and <= 31)
        {
            minute = m;
            hour = h;
            frequency = ScheduleFrequency.Monthly;
            dayOfMonth = dom;
            dayOfWeek = null;
            monthWeekOrdinal = null;
            monthlySubPattern = MonthlySubPattern.ByCalendarDay;
            return true;
        }

        if (IsAnyCron(dayOfMonthField) && IsAnyCron(dayOfWeekField))
        {
            minute = m;
            hour = h;
            frequency = ScheduleFrequency.Daily;
            return true;
        }

        if (IsAnyCron(dayOfMonthField) && !IsAnyCron(dayOfWeekField) && TryParseDayOfWeekFieldList(dayOfWeekField, out var wds) && wds is { Count: > 0 })
        {
            minute = m;
            hour = h;
            frequency = ScheduleFrequency.Weekly;
            weekDays = wds;
            dayOfWeek = wds[0];
            return true;
        }

        minute = m;
        hour = h;
        frequency = ScheduleFrequency.Daily;
        return true;
    }

    private static bool IsAnyOrDayWildcard(string field) => field is "*" or "?";

    private static string FormatDayOfWeekForCronOrList(int? dayOfWeek, IReadOnlyList<int>? weekDays)
    {
        if (weekDays is { Count: > 0 })
        {
            return string.Join(
                ",",
                NormalizeAndSortDayOfWeekList(weekDays, nameof(weekDays)).Select(n => n.ToString(CultureInfo.InvariantCulture)));
        }

        if (dayOfWeek is null or < 0 or > 6)
            throw new ArgumentException("Day of week must be 0 (Sunday) through 6 (Saturday).", nameof(dayOfWeek));
        return dayOfWeek.Value.ToString(CultureInfo.InvariantCulture);
    }

    private static int[] NormalizeAndSortDayOfWeekList(IReadOnlyList<int> weekDays, string paramName)
    {
        if (weekDays is { Count: < 1 } or { Count: > 7 })
            throw new ArgumentException("Provide 1 to 7 distinct days of week (0–6).", paramName);
        var set = new HashSet<int>();
        foreach (var d in weekDays)
        {
            if (d is < 0 or > 6)
                throw new ArgumentException("Each day of week must be 0 (Sunday) through 6 (Saturday).", paramName);
            if (!set.Add(d))
                throw new ArgumentException("Days of week must be unique.", paramName);
        }
        return set.OrderBy(n => n).ToArray();
    }

    private static int FormatDayOfMonth(int? dayOfMonth)
    {
        var d = dayOfMonth ?? throw new ArgumentException("Day of month is required for this monthly schedule.", nameof(dayOfMonth));
        if (d is < 1 or > 31)
            throw new ArgumentException("Day of month must be between 1 and 31.", nameof(dayOfMonth));
        return d;
    }

    private static string FormatDowWithOrdinal(int? dayOfWeek, int? n)
    {
        if (dayOfWeek is null or < 0 or > 6)
            throw new ArgumentException("Day of week is required (0–6).", nameof(dayOfWeek));
        var o = n ?? throw new ArgumentException("Week ordinal (1–5) is required for this monthly pattern.", nameof(n));
        if (o is < 1 or > 5)
            throw new ArgumentException("Week ordinal must be 1 (first) through 5 (fifth).", nameof(n));
        return $"{dayOfWeek.Value}#{o}";
    }

    private static string FormatDowLastInMonth(int? dayOfWeek)
    {
        if (dayOfWeek is null or < 0 or > 6)
            throw new ArgumentException("Day of week is required (0–6).", nameof(dayOfWeek));
        return $"{dayOfWeek.Value}L";
    }

    private static bool IsAnyCron(string field) => field is "*" or "?" or "";

    /// <summary>Comma-separated DOW field (0–6 or 7 for Sunday) with 1+ distinct days.</summary>
    private static bool TryParseDayOfWeekFieldList(string field, out IReadOnlyList<int>? list)
    {
        list = null;
        if (string.IsNullOrWhiteSpace(field))
            return false;
        var parts = field.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0)
            return false;
        var set = new HashSet<int>();
        foreach (var part in parts)
        {
            if (!TryParseSingleDayOfWeekToken(part, out var wd))
                return false;
            if (!set.Add(wd))
                return false;
        }
        if (set.Count == 0)
            return false;
        list = set.OrderBy(n => n).ToArray();
        return true;
    }

    private static bool TryParseSingleDayOfWeekToken(string s, out int value)
    {
        value = 0;
        if (!int.TryParse(s, NumberStyles.None, CultureInfo.InvariantCulture, out var n))
            return false;
        if (n is >= 0 and <= 6)
        {
            value = n;
            return true;
        }
        if (n == 7) // 7 = Sunday in some crons
        {
            value = 0;
            return true;
        }
        return false;
    }
}
