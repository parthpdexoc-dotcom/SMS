using System.Linq;
using Cronos;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Services.Mapping;

/// <summary>
/// Converts between upsert field payloads, stored cron strings, and <see cref="DepartmentCronScheduleDto"/>;
/// use instead of ad-hoc mapping in services.
/// </summary>
public static class DepartmentCronScheduleMapping
{
    /// <summary>Builds the cron string to persist from the upsert body (validates custom expressions with Cronos).</summary>
    /// <exception cref="ArgumentException">Invalid fields for the chosen frequency.</exception>
    public static string BuildStoredCron(DepartmentCronScheduleUpsertDto dto)
    {
        if (dto.Frequency == ScheduleFrequency.Custom)
        {
            var text = (dto.CustomCronExpression ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(text) || text.Length > 128)
                throw new ArgumentException("Custom cron expression is required and must be at most 128 characters.");
            if (!TryValidateStandardCronExpression(text, out var cronError))
                throw new ArgumentException(string.IsNullOrEmpty(cronError) ? "Invalid cron expression." : cronError);
            return text;
        }

        if (dto.Frequency == ScheduleFrequency.EveryNMinutes)
        {
            if (dto.IntervalMinutes is < 1 or > 59)
                throw new ArgumentException("Interval must be between 1 and 59 (minutes) for an every-N-minutes schedule.");
            return DepartmentCronExpressionHelper.BuildEveryNMinutes(dto.IntervalMinutes!.Value);
        }

        IReadOnlyList<int>? weeklyDays = null;
        if (dto.Frequency == ScheduleFrequency.Weekly && dto.DaysOfWeek is { Length: > 0 })
            weeklyDays = dto.DaysOfWeek;

        return DepartmentCronExpressionHelper.Build(
            dto.Frequency,
            dto.Minute,
            dto.Hour,
            dto.DayOfWeek,
            dto.DayOfMonth,
            dto.MonthlySubPattern,
            dto.MonthWeekOrdinal,
            null,
            weeklyDays);
    }

    /// <summary>Maps a stored cron to the API DTO, including <see cref="DepartmentCronScheduleDto.CronExpressionDescription"/>.</summary>
    public static DepartmentCronScheduleDto ToScheduleDto(
        Guid departmentId,
        JobType job,
        Guid? rowId,
        string? cron)
    {
        if (string.IsNullOrWhiteSpace(cron))
        {
            return new DepartmentCronScheduleDto
            {
                DepartmentID = departmentId,
                DepartmentCronJobManagementId = rowId,
                Job = job,
                JobName = EnumHelper.ToStringValue(job),
                Frequency = ScheduleFrequency.Daily,
                Hour = 8,
                Minute = 0,
                DayOfWeek = null,
                DayOfMonth = null,
                MonthlySubPattern = MonthlySubPattern.ByCalendarDay,
                MonthWeekOrdinal = null,
                IntervalMinutes = null,
                CustomCronExpression = null,
                CronExpression = cron,
                CronExpressionDescription = DepartmentCronExpressionDescription.ForEmpty(),
            };
        }

        if (DepartmentCronExpressionHelper.TryParseEveryNMinutes(cron, out var everyN))
        {
            return new DepartmentCronScheduleDto
            {
                DepartmentID = departmentId,
                DepartmentCronJobManagementId = rowId,
                Job = job,
                JobName = EnumHelper.ToStringValue(job),
                Frequency = ScheduleFrequency.EveryNMinutes,
                Hour = 0,
                Minute = 0,
                DayOfWeek = null,
                DayOfMonth = null,
                MonthlySubPattern = MonthlySubPattern.ByCalendarDay,
                MonthWeekOrdinal = null,
                IntervalMinutes = everyN,
                CustomCronExpression = null,
                CronExpression = cron,
                CronExpressionDescription = DepartmentCronExpressionDescription.ForEveryNMinutes(everyN),
            };
        }

        if (DepartmentCronExpressionHelper.TryParse(
                cron,
                out var minute,
                out var hour,
                out var frequency,
                out var dayOfWeek,
                out var dayOfMonth,
                out var monthlySubPattern,
                out var monthWeekOrdinal,
                out var weekDaysList))
        {
            int[]? weekDays = frequency == ScheduleFrequency.Weekly && weekDaysList is { Count: > 0 }
                ? weekDaysList is int[] arr
                    ? arr
                    : weekDaysList.ToArray()
                : null;
            return new DepartmentCronScheduleDto
            {
                DepartmentID = departmentId,
                DepartmentCronJobManagementId = rowId,
                Job = job,
                JobName = EnumHelper.ToStringValue(job),
                Frequency = frequency,
                Hour = hour,
                Minute = minute,
                DayOfWeek = dayOfWeek,
                DaysOfWeek = weekDays,
                DayOfMonth = dayOfMonth,
                MonthlySubPattern = monthlySubPattern,
                MonthWeekOrdinal = monthWeekOrdinal,
                IntervalMinutes = null,
                CustomCronExpression = null,
                CronExpression = cron,
                CronExpressionDescription = DepartmentCronExpressionDescription.ForStructured(
                    frequency,
                    hour,
                    minute,
                    dayOfWeek,
                    dayOfMonth,
                    monthlySubPattern,
                    monthWeekOrdinal,
                    weekDays),
            };
        }

        if (TryValidateStandardCronExpression(cron.Trim(), out _))
        {
            var trimmed = cron.Trim();
            return new DepartmentCronScheduleDto
            {
                DepartmentID = departmentId,
                DepartmentCronJobManagementId = rowId,
                Job = job,
                JobName = EnumHelper.ToStringValue(job),
                Frequency = ScheduleFrequency.Custom,
                Hour = 0,
                Minute = 0,
                DayOfWeek = null,
                DayOfMonth = null,
                MonthlySubPattern = MonthlySubPattern.ByCalendarDay,
                MonthWeekOrdinal = null,
                IntervalMinutes = null,
                CustomCronExpression = trimmed,
                CronExpression = cron,
                CronExpressionDescription = DepartmentCronExpressionDescription.ForCustom(trimmed),
            };
        }

        return new DepartmentCronScheduleDto
        {
            DepartmentID = departmentId,
            DepartmentCronJobManagementId = rowId,
            Job = job,
            JobName = EnumHelper.ToStringValue(job),
            Frequency = ScheduleFrequency.Daily,
            Hour = 8,
            Minute = 0,
            DayOfWeek = null,
            DayOfMonth = null,
            MonthlySubPattern = MonthlySubPattern.ByCalendarDay,
            MonthWeekOrdinal = null,
            IntervalMinutes = null,
            CustomCronExpression = null,
            CronExpression = cron,
            CronExpressionDescription = DepartmentCronExpressionDescription.ForUnrecognized(cron),
        };
    }

    /// <summary>Maps a persisted <see cref="DepartmentCronJobManagement"/> row to a schedule DTO.</summary>
    public static DepartmentCronScheduleDto ToScheduleDto(DepartmentCronJobManagement row) =>
        ToScheduleDto(
            row.DepartmentID,
            row.Job,
            row.DepartmentCronJobManagementId,
            row.CronExpression);

    private static bool TryValidateStandardCronExpression(string trimmed, out string? error)
    {
        error = null;
        try
        {
            _ = CronExpression.Parse(trimmed, CronFormat.Standard);
            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }
}
