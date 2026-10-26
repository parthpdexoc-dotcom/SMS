using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Helpers;

/// <summary>
/// Job / post-position scheduling date helpers. Extend with shared scheduling logic over time.
/// </summary>
public static class JobDateHelper
{

    /// <summary>
    /// True when this entity’s work date/time is at or before <paramref name="now"/> (uses <see cref="PostPosition.StartTime"/> or post default start).
    /// </summary>
    public static bool IsPostPositionDateTimePassed(PostPosition postPosition, DateTime now)
    {
        if (!postPosition.DateOfWork.HasValue)
            return false;

        var startTime = postPosition.StartTime ?? postPosition.Post?.PostStartTime;
        return IsPostPositionDateTimePassed(postPosition.DateOfWork.Value, startTime, now);
    }

    /// <summary>
    /// Core comparison: work calendar date plus optional start time vs <paramref name="now"/>.
    /// Without <paramref name="startTime"/>, only the date is used (passed when strictly before <paramref name="now"/>'s date).
    /// </summary>
    private static bool IsPostPositionDateTimePassed(DateOnly dateOfWork, TimeOnly? startTime, DateTime now)
    {
        if (!startTime.HasValue)
            return dateOfWork < DateOnly.FromDateTime(now);

        return now >= dateOfWork.ToDateTime(startTime.Value);
    }
}
