using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SMS.Data;
using SMS.Generic.Common.Audit;
using SMS.Generic.Dtos.Interfaces.Audit;
using SMS.Generic.Entities.Entities.Audit;

namespace SMS.Generic.Repositories.Audit;

/// <summary>
/// Captures audited entities around SaveChanges and enqueues messages after a successful save.
/// </summary>
public sealed class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IAuditQueueService _auditQueue;
    private readonly IAuditUserContext _auditUserContext;
    private readonly IJobAuditRouting _jobAuditRouting;

    private static readonly ConditionalWeakTable<DbContext, PendingCapture> Pending = new();

    public AuditSaveChangesInterceptor(
        IAuditQueueService auditQueue,
        IAuditUserContext auditUserContext,
        IJobAuditRouting jobAuditRouting)
    {
        _auditQueue = auditQueue;
        _auditUserContext = auditUserContext;
        _jobAuditRouting = jobAuditRouting;
    }

    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        StashPlanIfSmsContext(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        StashPlanIfSmsContext(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        EnqueueAfterSaveSync(eventData.Context);
        return base.SavedChanges(eventData, result);
    }

    /// <inheritdoc />
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await EnqueueAfterSaveAsync(eventData.Context, cancellationToken).ConfigureAwait(false);
        return await base.SavedChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        DiscardPlan(eventData.Context);
        base.SaveChangesFailed(eventData);
    }

    /// <inheritdoc />
    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        DiscardPlan(eventData.Context);
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    private void StashPlanIfSmsContext(DbContext? context)
    {
        if (context is not SMSDbContext sms)
            return;

        var plan = AuditChangeCapture.PlanBeforeSave(sms.ChangeTracker, _auditUserContext);
        Pending.Remove(sms);
        Pending.Add(sms, new PendingCapture(plan));
    }

    private static void DiscardPlan(DbContext? context)
    {
        if (context is not null)
            Pending.Remove(context);
    }

    private void EnqueueAfterSaveSync(DbContext? context)
    {
        if (context is not SMSDbContext || !Pending.TryGetValue(context, out var holder))
            return;

        Pending.Remove(context);
        var plan = holder.Plan;

        foreach (var row in plan.ModifiedOrDeleted)
            TryEnqueueAuditRowSync(row);

        foreach (var row in AuditChangeCapture.BuildAfterAdded(plan.AddedEntries, _auditUserContext))
            TryEnqueueAuditRowSync(row);
    }

    private async Task EnqueueAfterSaveAsync(DbContext? context, CancellationToken cancellationToken)
    {
        if (context is not SMSDbContext || !Pending.TryGetValue(context, out var holder))
            return;

        Pending.Remove(context);
        var plan = holder.Plan;

        foreach (var row in plan.ModifiedOrDeleted)
            await EnqueueAuditRowAsync(row, cancellationToken).ConfigureAwait(false);

        foreach (var row in AuditChangeCapture.BuildAfterAdded(plan.AddedEntries, _auditUserContext))
            await EnqueueAuditRowAsync(row, cancellationToken).ConfigureAwait(false);
    }

    private void TryEnqueueAuditRowSync(AuditBase row)
    {
        if (_jobAuditRouting.RouteEmployeeAuditsToJobAuditTable && row is EmployeeAudit employeeAudit)
        {
            _auditQueue.TryEnqueue(new EmployeeJobRunAudit(employeeAudit));
            return;
        }

        _auditQueue.TryEnqueue(row);
    }

    private async ValueTask EnqueueAuditRowAsync(AuditBase row, CancellationToken cancellationToken)
    {
        if (_jobAuditRouting.RouteEmployeeAuditsToJobAuditTable && row is EmployeeAudit employeeAudit)
        {
            await _auditQueue.EnqueueAsync(new EmployeeJobRunAudit(employeeAudit), cancellationToken).ConfigureAwait(false);
            return;
        }

        await _auditQueue.EnqueueAsync(row, cancellationToken).ConfigureAwait(false);
    }

    private sealed class PendingCapture
    {
        public PendingCapture(AuditChangeCapture.CapturePlan plan) => Plan = plan;
        public AuditChangeCapture.CapturePlan Plan { get; }
    }
}
