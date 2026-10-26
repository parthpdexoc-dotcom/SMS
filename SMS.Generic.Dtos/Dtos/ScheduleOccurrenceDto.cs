namespace SMS.Generic.Dtos.Dtos;

/// <summary>A scheduled job run occurrence, with how late the host is in reaching it.</summary>
public readonly record struct ScheduleOccurrenceDto(DateTime LocalTime, DateTime Utc, DateTime LocalNow)
{
    public DateOnly BusinessDate => DateOnly.FromDateTime(LocalTime);

    public TimeSpan Delay => LocalNow - LocalTime;
}
