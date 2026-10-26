using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SMS.Generic.Enums;

namespace SMS.Generic.Common.Helpers;

/// <summary>
/// User-facing English text for department cron settings (no time zone label — use department time zone in UI if needed).
/// </summary>
public static class DepartmentCronExpressionDescription
{
    private static readonly string[] DayNames = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

    /// <summary>Every-N minutes pattern.</summary>
    public static string ForEveryNMinutes(int n)
    {
        if (n is < 1 or > 59)
            return $"Invalid interval: {n} (use 1–59 minutes).";
        return n == 1 ? "Every minute" : $"Every {n} minutes";
    }

    /// <summary>
    /// Full sentence from resolved frequency fields (daily, weekly, monthly from <see cref="DepartmentCronExpressionHelper.TryParse"/>).
    /// For every-N minutes use <see cref="ForEveryNMinutes"/>; for custom use <see cref="ForCustom"/>.
    /// </summary>
    public static string ForStructured(
        ScheduleFrequency frequency,
        int hour,
        int minute,
        int? dayOfWeek,
        int? dayOfMonth,
        MonthlySubPattern monthlySubPattern,
        int? monthWeekOrdinal,
        IReadOnlyList<int>? weeklyDayIndices = null)
    {
        var at = FormatTime12Hour(hour, minute);
        return frequency switch
        {
            ScheduleFrequency.Daily => $"Every day at {at}",
            ScheduleFrequency.Weekly => weeklyDayIndices is { Count: > 1 } wl
                ? $"Every {FormatWeeklyDayNames(wl)} at {at}"
                : weeklyDayIndices is { Count: 1 } w1
                    ? $"Every {GetDayName(w1[0])} at {at}"
                    : $"Every {GetDayName(dayOfWeek)} at {at}",
            ScheduleFrequency.Monthly => DescribeMonthly(
                at,
                dayOfWeek,
                dayOfMonth,
                monthlySubPattern,
                monthWeekOrdinal),
            _ => throw new ArgumentOutOfRangeException(
                nameof(frequency),
                "Only Daily, Weekly, and Monthly use structured descriptions.")
        };
    }

    /// <summary>Custom 5-field expression.</summary>
    public static string ForCustom(string? cron)
    {
        if (string.IsNullOrWhiteSpace(cron))
            return "Custom schedule";
        return $"Custom: {cron.Trim()}";
    }

    public static string ForEmpty() => "No schedule";

    public static string ForUnrecognized(string? cron) =>
        string.IsNullOrWhiteSpace(cron) ? ForEmpty() : $"Unrecognized: {cron.Trim()}";

    private static string DescribeMonthly(
        string at,
        int? dayOfWeek,
        int? dayOfMonth,
        MonthlySubPattern pattern,
        int? monthWeekOrdinal)
    {
        return pattern switch
        {
            MonthlySubPattern.ByCalendarDay when dayOfMonth is >= 1 and <= 31
                => $"On day {dayOfMonth} of every month at {at}",
            MonthlySubPattern.ByWeekdayOrdinal when dayOfWeek is >= 0 and <= 6 && monthWeekOrdinal is >= 1 and <= 5
                => $"On the {OrdinalAdjective(monthWeekOrdinal!.Value)} {GetDayName(dayOfWeek)} of every month at {at}",
            MonthlySubPattern.ByLastWeekday when dayOfWeek is >= 0 and <= 6
                => $"On the last {GetDayName(dayOfWeek)} of every month at {at}",
            _ => $"Every month (at {at})"
        };
    }

    private static string GetDayName(int? dayOfWeek)
    {
        if (dayOfWeek is not (>= 0 and <= 6))
            return "week";
        return DayNames[dayOfWeek.GetValueOrDefault()];
    }

    private static string FormatWeeklyDayNames(IReadOnlyList<int> days)
    {
        if (days.Count == 0)
            return "week";
        var names = days.Select(d => GetDayName((int?)d)).ToArray();
        if (names.Length == 1)
            return names[0];
        if (names.Length == 2)
            return $"{names[0]} and {names[1]}";
        return string.Join(", ", names.Take(names.Length - 1)) + " and " + names[^1];
    }

    private static string OrdinalAdjective(int n) =>
        n switch
        {
            1 => "first",
            2 => "second",
            3 => "third",
            4 => "fourth",
            5 => "fifth",
            _ => n.ToString(CultureInfo.InvariantCulture)
        };

    private static string FormatTime12Hour(int hour, int minute)
    {
        if (hour is < 0 or > 23 || minute is < 0 or > 59)
            return string.Format(CultureInfo.InvariantCulture, "{0}:{1:00}", hour, minute);
        var h12 = hour % 12;
        if (h12 == 0)
            h12 = 12;
        var amPm = hour < 12 ? "AM" : "PM";
        return string.Format(CultureInfo.InvariantCulture, "{0}:{1:00} {2}", h12, minute, amPm);
    }
}
