using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;

namespace SMS.BackgroundJobs;

/// <summary>
/// Scheduled job that restores <see cref="PostPositionStatus.Assigned"/> for post positions still in
/// <see cref="PostPositionStatus.CancellationPending"/> after their shift start time (department-local).
/// Schedule: <c>ScheduledJobs:RevertCancellationPendingOnShiftStartCron</c> (default every 5 minutes, UTC-based tick with per-department timezone).
/// </summary>
public class RevertCancellationPendingOnShiftStartJob : ScheduledJobBase
{
    public RevertCancellationPendingOnShiftStartJob(
        IServiceScopeFactory scopeFactory,
        ILogger<RevertCancellationPendingOnShiftStartJob> logger,
        IConfiguration configuration)
        : base(
            scopeFactory,
            logger,
            configuration,
            JobType.RevertCancellationPendingOnShiftStart)
    {
    }

    protected override async Task ExecuteAsync(
        IServiceProvider serviceProvider,
        DepartmentCronJobScheduleDto department,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation(
            "RevertCancellationPendingOnShiftStartJob started for Department {DepartmentId} | TimeZone {TimeZoneId}",
            department.DepartmentId,
            department.TimeZoneId);

        var postPositionService = serviceProvider.GetRequiredService<IPostPositionService>();
        var departmentNow = department.GetDepartmentNow();

        var revertedPostPositionIds = await postPositionService.RevertCancellationPendingToAssignedAsync(department.DepartmentId,departmentNow,cancellationToken).ConfigureAwait(false);
        if (revertedPostPositionIds.Count > 0)
        {
            Logger.LogInformation(
                "Reverted {Count} CancellationPending post position(s) to Assigned for Department {DepartmentId} (as of {DepartmentNow:O}). PostPositionIds: {PostPositionIds}",
                revertedPostPositionIds.Count,
                department.DepartmentId,
                departmentNow,
                string.Join(", ", revertedPostPositionIds));
        }
        else
        {
            Logger.LogDebug(
                "No CancellationPending post positions due for revert for Department {DepartmentId} (as of {DepartmentNow:O}).",
                department.DepartmentId,
                departmentNow);
        }
    }
}
