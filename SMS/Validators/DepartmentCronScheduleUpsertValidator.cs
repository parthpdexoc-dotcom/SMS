using System.Collections.Generic;
using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;

namespace SMS.Validators
{
    public class DepartmentCronScheduleUpsertValidator : AbstractValidator<DepartmentCronScheduleUpsertDto>
    {
        public DepartmentCronScheduleUpsertValidator()
        {
            RuleFor(x => x.DepartmentID)
                .NotEqual(Guid.Empty);

        RuleFor(x => x.Frequency)
                .IsInEnum();

        When(x => x.Frequency != ScheduleFrequency.Custom, () =>
        {
            RuleFor(x => x.Hour)
                .InclusiveBetween(0, 23)
                .WithMessage("Hour must be between 0 and 23.");

            RuleFor(x => x.Minute)
                .InclusiveBetween(0, 59)
                .WithMessage("Minute must be between 0 and 59.");
        });

        When(x => x.Frequency == ScheduleFrequency.EveryNMinutes, () =>
        {
            RuleFor(x => x.IntervalMinutes)
                .NotNull()
                .WithMessage("Interval (1–59 minutes) is required for an every-N-minutes schedule.")
                .InclusiveBetween(1, 59)
                .WithMessage("Interval must be between 1 and 59 minutes.");
        });

        When(x => x.Frequency == ScheduleFrequency.Custom, () =>
        {
            RuleFor(x => x.CustomCronExpression)
                .NotEmpty()
                .WithMessage("Enter a 5-field cron expression (e.g. 0 */2 * * * or */5 * * * *).")
                .MaximumLength(128)
                .WithMessage("Expression must be at most 128 characters.");
        });

            When(x => x.Frequency == ScheduleFrequency.Weekly, () =>
            {
                RuleFor(x => x)
                    .Must(HasValidWeeklyDaySelection)
                    .WithMessage(
                        "For a weekly schedule, set daysOfWeek to 1–7 distinct values 0–6 and/or a single dayOfWeek 0–6.");
            });

            When(x => x.Frequency == ScheduleFrequency.Monthly, () =>
            {
                RuleFor(x => x.MonthlySubPattern)
                    .IsInEnum()
                    .WithMessage("Invalid monthly sub-pattern.");

                When(x => x.MonthlySubPattern == MonthlySubPattern.ByCalendarDay, () =>
                {
                    RuleFor(x => x.DayOfMonth)
                        .NotNull()
                        .WithMessage("Day of month is required for a monthly by-calendar-day schedule.")
                        .InclusiveBetween(1, 31)
                        .WithMessage("Day of month must be between 1 and 31.");
                });

                When(x => x.MonthlySubPattern == MonthlySubPattern.ByWeekdayOrdinal, () =>
                {
                    RuleFor(x => x.DayOfWeek)
                        .NotNull()
                        .WithMessage("Day of week is required for a monthly Nth-weekday schedule.")
                        .InclusiveBetween(0, 6)
                        .WithMessage("Day of week must be 0 (Sunday) through 6 (Saturday).");
                    RuleFor(x => x.MonthWeekOrdinal)
                        .NotNull()
                        .WithMessage("Select which week (first through fifth) for a monthly Nth-weekday schedule.")
                        .InclusiveBetween(1, 5)
                        .WithMessage("Week must be 1 (first) through 5 (fifth).");
                });

                When(x => x.MonthlySubPattern == MonthlySubPattern.ByLastWeekday, () =>
                {
                    RuleFor(x => x.DayOfWeek)
                        .NotNull()
                        .WithMessage("Day of week is required for the last weekday in month schedule.")
                        .InclusiveBetween(0, 6)
                        .WithMessage("Day of week must be 0 (Sunday) through 6 (Saturday).");
                });
            });
        }

        private static bool HasValidWeeklyDaySelection(DepartmentCronScheduleUpsertDto x)
        {
            if (x.DaysOfWeek is { Length: > 0 })
            {
                if (x.DaysOfWeek.Length is < 1 or > 7)
                    return false;
                var set = new HashSet<int>();
                foreach (var d in x.DaysOfWeek)
                {
                    if (d is < 0 or > 6 || !set.Add(d))
                        return false;
                }
                return true;
            }
            return x.DayOfWeek is >= 0 and <= 6;
        }
    }
}
