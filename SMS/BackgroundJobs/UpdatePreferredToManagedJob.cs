using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Enums;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Dtos.Audit;
using SMS.Generic.Dtos.Interfaces.Audit;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;
using SMS.Generic.Dtos.Extensions;

namespace SMS.BackgroundJobs;

/// <summary>
/// Scheduled job that picks due preferred-release requests (ProcessDate + ProcessTime reached),
/// marks them in progress, and finalizes schedule by converting Preferred to Managed.
/// Persisted changes use the same audit intercept pipeline as API flows for audited entities (e.g. <c>Request</c>, <c>RequestSchedule</c>).
/// Each processed request enqueues one summary row to <c>UpdatePreferredToManaged_Audit</c> via <see cref="IAuditQueueService.EnqueueAsync"/>.
/// </summary>
public class UpdatePreferredToManagedJob : ScheduledJobBase
{

    private readonly IJobAuditService _jobAuditService;

    public UpdatePreferredToManagedJob(
        IServiceScopeFactory scopeFactory,
        ILogger<UpdatePreferredToManagedJob> logger,
        IConfiguration configuration,
        IJobAuditService jobAuditService)
        : base(
            scopeFactory,
            logger,
            configuration,
            JobType.UpdatePreferredToManaged)
    {
        _jobAuditService = jobAuditService ?? throw new ArgumentNullException(nameof(jobAuditService));
    }

    protected override async Task ExecuteAsync(IServiceProvider serviceProvider, DepartmentCronJobScheduleDto department, CancellationToken cancellationToken)
    {
        var requestService = serviceProvider.GetRequiredService<IRequestService>();
        var requestScheduleService = serviceProvider.GetRequiredService<IRequestScheduleService>();

        var currentDepartmentTime = department.GetDepartmentNow();

        Logger.LogInformation("UpdatePreferredToManagedJob started");

        var dueRequest = await requestService.GetPreferredRequestsDueForProcessingAsync(currentDepartmentTime).ConfigureAwait(false);
        if (dueRequest == null)
        {
            Logger.LogDebug("No due preferred request schedules found to convert to Managed.");
            return;
        }
        var result = new RequestScheduleProcessingResultDto(dueRequest.RequestID);
        var processedUtc = DateTime.UtcNow;

        try
        {
            result.MarkSuccessed($"Preferred process threshold reached at {dueRequest.ProcessLocalDateTime:O}; ReleaseAs updated to Managed.");
            await requestScheduleService.UpsertRequestProcessingResultAsync(dueRequest.RequestID, result, cancellationToken, ReleaseAs.Managed).ConfigureAwait(false);
            Logger.LogInformation("Processed preferred request schedule for Request {RequestID} and converted to Managed.", dueRequest.RequestID);
            var audit = _jobAuditService.CreateUpdatePreferredToManagedAudit(
                dueRequest.RequestID,
                string.IsNullOrWhiteSpace(dueRequest.RequestTitle) ? dueRequest.RequestID.ToString("D") : dueRequest.RequestTitle.Trim(),
                dueRequest.DepartmentID,
                processedUtc,
                success: true,
                message: null);
            await _jobAuditService.EnqueueAuditAsync(audit, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Preferred->Managed processing failed for Request {RequestID}", dueRequest.RequestID);

            result.MarkFailed($"Preferred->Managed transition failed. ScheduledAtLocal={dueRequest.ProcessLocalDateTime:O}. Error={ex.Message}");

            await requestScheduleService.UpsertRequestProcessingResultAsync(dueRequest.RequestID, result, cancellationToken, ReleaseAs.Managed).ConfigureAwait(false);
            var audit = _jobAuditService.CreateUpdatePreferredToManagedAudit(
                dueRequest.RequestID,
                string.IsNullOrWhiteSpace(dueRequest.RequestTitle) ? dueRequest.RequestID.ToString("D") : dueRequest.RequestTitle.Trim(),
                dueRequest.DepartmentID,
                processedUtc,
                success: false,
                message: ex.Message);
            await _jobAuditService.EnqueueAuditAsync(audit, cancellationToken).ConfigureAwait(false);
        }
    }

}
