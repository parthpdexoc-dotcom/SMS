using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using System.Collections.Concurrent;
using System.Globalization;

namespace SMS.Generic.Dtos.Extensions;
public static class DateExtensions
{
    private static readonly ConcurrentDictionary<string, TimeZoneInfo> TimeZoneCache = new(StringComparer.OrdinalIgnoreCase);
    private const string DefaultTimeZoneId = "Eastern Standard Time";

    /* =====================================================
       WEEK HELPERS
       ===================================================== */

    /// <summary>
    /// Converts a department-configured short value to DayOfWeek.
    /// </summary>
    public static DayOfWeek ToDayOfWeek(this short value)
    {
        if (value < 0 || value > 6)
            throw new ArgumentOutOfRangeException(
                nameof(value),
                "DayOfWeek must be between 0 (Sunday) and 6 (Saturday)");

        return (DayOfWeek)value;
    }

    /// <summary>
    /// Gets the department-based week range for a given date.
    /// </summary>
    public static (DateOnly WeekStart, DateOnly WeekEnd) GetDepartmentWeekRange(this DateOnly date, DayOfWeek weekStartDay)
    {
        var current = (int)date.DayOfWeek;
        var start = (int)weekStartDay;

        var diff = (7 + current - start) % 7;
        var weekStart = date.AddDays(-diff);

        return (weekStart, weekStart.AddDays(6));
    }

    /* =====================================================
       DERIVED VIEWS (FROM PER-DAY MAPS)
       ===================================================== */

    /// <summary>
    /// Converts a per-day count map into a per-week count map
    /// using department week configuration.
    /// </summary>
    public static Dictionary<DateOnly, int> ToWeeklyCountMap(this Dictionary<DateOnly, int> perDayMap,DayOfWeek weekStartDay)
    {
        return perDayMap
            .GroupBy(x =>
                x.Key.GetDepartmentWeekRange(weekStartDay).WeekStart)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(x => x.Value));
    }

    /// <summary>
    /// Converts a per-day hours map into a per-week hours map.
    /// </summary>
    public static Dictionary<DateOnly, decimal> ToWeeklyHoursMap(this Dictionary<DateOnly, decimal> perDayMap,DayOfWeek weekStartDay)
    {
        return perDayMap
            .GroupBy(x =>
                x.Key.GetDepartmentWeekRange(weekStartDay).WeekStart)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(x => x.Value));
    }

    /* =====================================================
       RANGE HELPERS (DAY / WEEK / MONTH)
       ===================================================== */
    /// Sums decimal values between two dates (inclusive).
    /// </summary>
    public static decimal SumForDateRange(this Dictionary<DateOnly, decimal> map,DateOnly start,DateOnly end)
        => map
            .Where(x => x.Key >= start && x.Key <= end)
            .Sum(x => x.Value);
    /// <summary>
    /// Returns system timezone if valid; otherwise Eastern Standard Time. Results are cached per ID.
    /// </summary>
    public static TimeZoneInfo ToSafeTimeZone(this string? timeZoneId)
    {
        var key = string.IsNullOrWhiteSpace(timeZoneId) ? DefaultTimeZoneId : timeZoneId.Trim();
        return TimeZoneCache.GetOrAdd(key, static id =>
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById(DefaultTimeZoneId);
            }
            catch (InvalidTimeZoneException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById(DefaultTimeZoneId);
            }
        });
    }
    public static DateTime TruncateToUtcMinute(this DateTime value)
    {
        return new DateTime(value.Year,value.Month,value.Day,value.Hour,value.Minute,0,DateTimeKind.Utc);
    }
    public static DateTime GetDepartmentNow(this DepartmentCronJobScheduleDto department)
    {
        var zone = department.TimeZoneId.ToSafeTimeZone();

        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,zone);
    }

    /// <summary>
    /// Converts a UTC close timestamp to an <see cref="DateTimeKind.Unspecified"/> calendar/clock value in <paramref name="departmentTimeZoneId"/>,
    /// matching how <see cref="GetDepartmentNow"/> derives department-local time from UTC.
    /// </summary>
    public static DateTime UtcInstantToDepartmentClock(this DateTime closeUtcStored, string? departmentTimeZoneId)
    {
        var utc = DateTime.SpecifyKind(closeUtcStored, DateTimeKind.Utc);
        return TimeZoneInfo.ConvertTimeFromUtc(utc, departmentTimeZoneId.ToSafeTimeZone());
    }
    public static DateOnly GetDepartmentYesterday(this DepartmentCronJobScheduleDto department)
    {
        return DateOnly.FromDateTime(department.GetDepartmentNow().AddDays(-1));
    }
    public static DateOnly GetDepartmentTodayDate(this DepartmentCronJobScheduleDto department)
    {
        return DateOnly.FromDateTime(department.GetDepartmentNow());
    }
}