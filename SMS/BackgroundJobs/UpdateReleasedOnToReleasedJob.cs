using System.Diagnostics;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Dtos.Audit;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Dtos.Interfaces.Audit;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Enums;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;

namespace SMS.BackgroundJobs;

/// <summary>
/// Scheduled job that updates requests from ReleasedOn to ReleaseNow
/// when the scheduled date/time has passed. Schedule is configured only via appsettings (ScheduledJobs:UpdateReleasedOnToReleasedCron).
/// Request schedule updates go through EF <c>SaveChanges</c> so <see cref="SMS.Generic.Repositories.Audit.AuditSaveChangesInterceptor"/> writes per-row changes to <c>RequestSchedule_Audit</c>.
/// When at least one schedule row is updated, enqueues one summary row per affected <see cref="SMS.Generic.Entities.Entities.Request"/> to <c>UpdateReleasedOnToReleased_Audit</c>.
/// </summary>
public class UpdateReleasedOnToReleasedJob : ScheduledJobBase
{

    private readonly IJobAuditService _jobAuditService;

    public UpdateReleasedOnToReleasedJob(
        IServiceScopeFactory scopeFactory,
        ILogger<UpdateReleasedOnToReleasedJob> logger,
        IConfiguration configuration,
        IJobAuditService jobAuditService)
        : base(
            scopeFactory,
            logger,
            configuration,
            JobType.UpdateReleasedOnToReleased)
    {
        _jobAuditService = jobAuditService ?? throw new ArgumentNullException(nameof(jobAuditService));
    }

    protected override async Task ExecuteAsync(IServiceProvider serviceProvider, DepartmentCronJobScheduleDto department, CancellationToken cancellationToken)
    {
        var requestScheduleService = serviceProvider.GetRequiredService<IRequestScheduleService>();
        var currentDepartmentTime = department.GetDepartmentNow();
        var onToReleased = await requestScheduleService.UpdateReleasedOnToReleasedAsync(currentDepartmentTime).ConfigureAwait(false);
        var toComplete = await requestScheduleService.UpdateReleasedToCompleteAsync(currentDepartmentTime).ConfigureAwait(false);

        if (onToReleased.RowCount > 0)
            Logger.LogInformation(
                "Updated {Count} request(s) from {From} to {To} (scheduled before {AsOf:O}).",
                onToReleased.RowCount, nameof(RequestStatus.ReleasedOn), nameof(RequestStatus.ReleaseNow), currentDepartmentTime);

        if (toComplete.RowCount > 0)
            Logger.LogInformation(
                "Updated {Count} request(s) from {From} to {To} (completed before {AsOf:O}).",
                toComplete.RowCount, nameof(RequestStatus.ReleaseNow), nameof(RequestStatus.Completed), currentDepartmentTime);

        if (onToReleased.RowCount == 0 && toComplete.RowCount == 0)
        {
            Logger.LogDebug("No requests updated (as of {AsOf:O}).", currentDepartmentTime);
            return;
        }

        var aggregated = new Dictionary<Guid, (string Title, Guid? DepartmentId, bool HadOnToReleased, bool HadToComplete)>();

        foreach (var a in onToReleased.AffectedRequests)
        {
            if (aggregated.TryGetValue(a.RequestId, out var cur))
            {
                aggregated[a.RequestId] = (
                    PreferTitle(cur.Title, a.RequestTitle),
                    cur.DepartmentId ?? a.DepartmentId,
                    true,
                    cur.HadToComplete);
            }
            else
            {
                aggregated[a.RequestId] = (a.RequestTitle, a.DepartmentId, true, false);
            }
        }

        foreach (var a in toComplete.AffectedRequests)
        {
            if (aggregated.TryGetValue(a.RequestId, out var cur))
            {
                aggregated[a.RequestId] = (
                    PreferTitle(cur.Title, a.RequestTitle),
                    cur.DepartmentId ?? a.DepartmentId,
                    cur.HadOnToReleased,
                    true);
            }
            else
            {
                aggregated[a.RequestId] = (a.RequestTitle, a.DepartmentId, false, true);
            }
        }

        var postService = serviceProvider.GetRequiredService<IPostService>();
        bool emailSent = false;

        foreach (var kvp in aggregated)
        {
            var requestId = kvp.Key;
            var x = kvp.Value;
            var displayName = x.Title;
            
            var jobRunAudit = _jobAuditService.CreateUpdateReleasedOnToReleasedAudit(
                requestId,
                displayName,
                x.DepartmentId,
                currentDepartmentTime,
                x.HadOnToReleased,
                x.HadToComplete);

            await _jobAuditService.EnqueueAuditAsync(jobRunAudit, cancellationToken).ConfigureAwait(false);

            if (x.HadOnToReleased && requestId != Guid.Empty && !emailSent)
            {
                try
                {
                    await postService.NotifyJobsPostedAsync(requestId).ConfigureAwait(false);
                    emailSent = true;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to send jobs posted notification for Request ID {RequestId}", requestId);
                }
            }
        }
    }

    private static string PreferTitle(string existing, string incoming) =>
        !string.IsNullOrWhiteSpace(existing) ? existing : incoming;
}
