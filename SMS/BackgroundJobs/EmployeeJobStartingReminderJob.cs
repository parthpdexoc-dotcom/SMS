using System.Linq.Expressions;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;

namespace SMS.BackgroundJobs;

/// <summary>
/// Sends job-start reminder emails based on each post position's scheduled start time.
/// Schedule: <c>ScheduledJobs:EmployeeJobStartingReminderCron</c> (every 1 minute).
/// </summary>
public class EmployeeJobStartingReminderJob : ScheduledJobBase
{
    private const int MaxReminderMinutes = (24 * 60) + 1;

    private static readonly (int MinutesBeforeStart, EmailType EmailType, Func<EmployeeDto, bool> IsSubscribed)[] Reminders =
    [
        (24 * 60, EmailType.JobStartingIn24HoursEmail, e => e.NotifyJobStartingIn24Hours),
        (60, EmailType.JobStartingIn1HourEmail, e => e.NotifyJobStartingIn1Hour)
    ];

    public EmployeeJobStartingReminderJob(
        IServiceScopeFactory scopeFactory,
        ILogger<EmployeeJobStartingReminderJob> logger,
        IConfiguration configuration)
        : base(scopeFactory, logger, configuration, JobType.EmployeeJobStartingReminder)
    {
    }

    protected override async Task ExecuteAsync(
        IServiceProvider serviceProvider,
        DepartmentCronJobScheduleDto department,
        CancellationToken cancellationToken)
    {
        var postPositionService = serviceProvider.GetRequiredService<IPostPositionService>();
        var notificationService = serviceProvider.GetRequiredService<INotificationService>();
        var now = department.GetDepartmentNow();

        var assignments = await postPositionService.GetAllAsync(
            filter: x =>
                x.Post!.Request!.DepartmentID == department.DepartmentId
                && x.EmployeeID.HasValue
                && x.EmployeeID != Guid.Empty
                && !x.IsPlaceholder
                && x.DateOfWork.HasValue
                && x.StartTime.HasValue
                && x.PostPositionStatus == PostPositionStatus.Assigned
                && x.Employee!.IsApproved
                && (x.Employee.NotifyJobStartingIn24Hours || x.Employee.NotifyJobStartingIn1Hour),
            includes: new Expression<Func<PostPosition, object>>[]
            {
                x => x.Employee!,
                x => x.Post!
            },
            pageParam: null).ConfigureAwait(false);
        var remindersToSend = assignments
            .Select(post =>
            {
                var postStart = post.GetPostPositionStartDateTime();
                var minutesUntil = postStart.HasValue ? (postStart.Value - now).TotalMinutes : (double?)null;
                return (Post: post, MinutesUntil: minutesUntil);
            })
            .Where(x => x.MinutesUntil is > 0 and <= MaxReminderMinutes)
            .SelectMany(x => Reminders
                .Where(r => r.IsSubscribed(x.Post.Employee!)
                    && IsReminderDue(x.MinutesUntil!.Value, r.MinutesBeforeStart))
                .Select(r => (x.Post, r.EmailType)))
            .ToList();

        await remindersToSend.Aggregate(
            Task.CompletedTask,
            (chain, reminder) => chain.ContinueWith(
                _ => notificationService.SendJobStartingReminderEmailAsync(
                    reminder.Post.Employee!,
                    reminder.Post,
                    reminder.EmailType,
                    cancellationToken),
                cancellationToken,
                TaskContinuationOptions.None,
                TaskScheduler.Default).Unwrap()).ConfigureAwait(false);
    }

    private static bool IsReminderDue(double minutesUntilPostStart, int minutesBeforeStart) =>
        minutesUntilPostStart >= minutesBeforeStart
        && minutesUntilPostStart < minutesBeforeStart + 1;
}
