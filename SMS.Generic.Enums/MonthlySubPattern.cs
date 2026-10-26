namespace SMS.Generic.Enums;

/// <summary>
/// How a <see cref="ScheduleFrequency.Monthly"/> job picks its day, mapped to <see href="https://github.com/HangfireIO/Cronos#special-characters">Cronos</see> (# and L in day-of-week).
/// </summary>
public enum MonthlySubPattern
{
    /// <summary>Run on a calendar day (1–31). Expression: <c>m h dom * *</c></summary>
    ByCalendarDay = 0,

    /// <summary>Nth occurrence of a weekday (1st–5th). Expression: <c>m h ? * dow#n</c> (n = 1..5)</summary>
    ByWeekdayOrdinal = 1,

    /// <summary>Last such weekday in the month. Expression: <c>m h ? * dowL</c></summary>
    ByLastWeekday = 2
}
