namespace SMS.Generic.Enums;

/// <summary>
/// How often a background job is scheduled in department cron settings.
/// </summary>
public enum ScheduleFrequency
{
    /// <summary>Each day at the same local time (minute hour * * *).</summary>
    Daily = 0,

    /// <summary>Once a week (minute hour * * dayOfWeek).</summary>
    Weekly = 1,

    /// <summary>Once a month (minute hour day * *).</summary>
    Monthly = 2,

    /// <summary>Every <c>N</c> minutes, <c>*/N * * * *</c> (N = 1–59).</summary>
    EveryNMinutes = 3,

    /// <summary>Full 5-field expression (user-controlled fields); stored as-is after Cronos validation.</summary>
    Custom = 4
}
