using System.Diagnostics;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using SMS.Generic.Common.Enums;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Dtos.Audit;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Dtos.Interfaces.Audit;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Enums;
using SMS.Generic.Services.Helpers;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Services;

namespace SMS.BackgroundJobs;

/// <summary>
/// Tenure / waiting-list processing for test scheduling. DB updates for audited entities (e.g. <c>PostPosition</c>, <c>RequestSchedule</c>) follow the standard <c>AuditSaveChangesInterceptor</c> path with job correlation.
/// Job-run summary and per-employee rows are written to <c>WaitingPostPosition_Audit</c> via <see cref="IAuditQueueService.EnqueueAsync"/> (same <see cref="AuditFieldChange"/> JSON shape for all enqueued rows).
/// </summary>
public class TenureWaitingPostPositionJob : ScheduledJobBase
{
    private readonly IJobAuditService _jobAuditService;

    public TenureWaitingPostPositionJob(
        IServiceScopeFactory scopeFactory,
        ILogger<TenureWaitingPostPositionJob> logger,
        IConfiguration configuration,
        IJobAuditService jobAuditService)
        : base(
            scopeFactory,
            logger,
            configuration,
            JobType.TenureWaitingList)
    {
        _jobAuditService = jobAuditService ?? throw new ArgumentNullException(nameof(jobAuditService));
    }

    protected override async Task ExecuteAsync(IServiceProvider serviceProvider, DepartmentCronJobScheduleDto department, CancellationToken cancellationToken)
    {
        Logger.LogInformation("WaitingPostPositionListTestJob started");

        var requestService = serviceProvider.GetRequiredService<IRequestService>();
        var requestScheduleService = serviceProvider.GetRequiredService<IRequestScheduleService>();
        var waitingPostPositionService = serviceProvider.GetRequiredService<IWaitingPostPositionService>();
        var postPositionService = serviceProvider.GetRequiredService<IPostPositionService>();
        var departmentPolicyService = serviceProvider.GetRequiredService<IDepartmentPolicyService>();
        var tenureEmployeeStatisticsService = serviceProvider.GetRequiredService<ITenureEmployeeStatisticsService>();

        var currentDepartmentTime = department.GetDepartmentNow();

        var request = await requestService.GetTenureBasedRequestsDueForProcessingAsync(currentDepartmentTime).ConfigureAwait(false);

        if (request == null)
            return;

        var result = new RequestScheduleProcessingResultDto(request.RequestID);
        var employeeResults = new Dictionary<Guid, EmployeeResultDto>();
        var correlationId = Activity.Current?.Id;

            try
            {
                var rows = await waitingPostPositionService.GetForPostsForJobAsync(request.PostIDs, cancellationToken).ConfigureAwait(false);
                var departmentPolicy = await departmentPolicyService.GetDepartmentPolicyConfigurationByDepartmentId(request.DepartmentID).ConfigureAwait(false);

                if (departmentPolicy == null)
                {
                    var skipReason = $"Department policy is not configured for DepartmentID {request.DepartmentID}. Processing skipped.";
                    result.MarkFailed(skipReason);
                    await requestScheduleService.UpsertRequestProcessingResultAsync(request.RequestID, result, cancellationToken).ConfigureAwait(false);
                    var policySkipChangedOn = DateTime.UtcNow;
                    var policyPlaceholder = BuildRequestScopePlaceholderRow(request);
                    var policyAudit = _jobAuditService.CreateWaitingPostPositionJobAudit(
                        policyPlaceholder,
                        request.DepartmentID,
                        employeeId: null,
                        policyPlaceholder.DateOfWork,
                        AuditAction.UPDATE,
                        policySkipChangedOn,
                        correlationId,
                        ProcessingStatus.Failed,
                        skipReason,
                        entityIdOverride: request.RequestID.ToString("D"));
                    await _jobAuditService.EnqueueAuditAsync(policyAudit, cancellationToken).ConfigureAwait(false);
                    return;
                }
                var byDateThenPost = rows.Where(r => r.PostID is { } postId && postId != Guid.Empty &&
                                r.EmployeeID != Guid.Empty &&
                                r.DateOfWork != null)
                    .GroupBy(r => r.DateOfWork!)
                    .OrderBy(g => g.Key);

                foreach (var dateGroup in byDateThenPost)
                {
                    var byPost = dateGroup.GroupBy(x => x.PostID);
                    var employeeIdsForDate = dateGroup.Select(x => x.EmployeeID)
                        .Distinct()
                        .ToList();
                    var tenureSchedules = (await requestScheduleService.GetAllAsync(
                            s => s.RequestId == request.RequestID &&
                                 s.ReleaseAs == ReleaseAs.TenureBased &&
                                 s.ProcessDate.HasValue)
                        .ConfigureAwait(false)).ToList();
                    var statisticsWindowEnd = tenureSchedules
                        .Where(s =>
                            (s.StartDate == null || dateGroup.Key >= s.StartDate) &&
                            (s.EndDate == null || dateGroup.Key <= s.EndDate))
                        .OrderByDescending(s => s.StartDate)
                        .ThenByDescending(s => s.ProcessDate)
                        .ThenByDescending(s => s.ProcessTime)
                        .Select(s => s.ProcessDate!.Value)
                        .FirstOrDefault(dateGroup.Key);

                    foreach (var postGroup in byPost)
                    {
                        var tenureRowsForPost = postGroup.ToList();
                        if (!postGroup.Key.HasValue || postGroup.Key.Value == Guid.Empty)
                            continue;
                        var postId = postGroup.Key.Value;
                        var dateOfWork = dateGroup.Key;
                        var alreadyAppliedForThisPost = new HashSet<Guid>();
                        List<TenureEmployeeStatisticsDto>? lastOrderedEmployees = null;

                        while (true)
                        {
                            var remainingEmployees = tenureRowsForPost
                                .Where(r => r.ProcessingStatus is ProcessingStatus.Pending or ProcessingStatus.InProgress or ProcessingStatus.Failed)
                                .Select(r => r.EmployeeID)
                                .Where(id => !alreadyAppliedForThisPost.Contains(id))
                                .Distinct()
                                .ToList();

                            if (remainingEmployees.Count == 0)
                                break;

                            var orderedEmployeesForDate = await tenureEmployeeStatisticsService
                                .GetTenureEmployeeStatisticsAsync(departmentPolicy, employeeIdsForDate, statisticsWindowEnd, request.DepartmentID, cancellationToken)
                                .ConfigureAwait(false);
                            lastOrderedEmployees = orderedEmployeesForDate;

                            var copiedAnyThisIteration = false;

                            foreach (var emp in orderedEmployeesForDate)
                            {
                                if (!remainingEmployees.Contains(emp.EmployeeID))
                                    continue;

                                var tenureRow = TenureEmployeeStatisticsHelper.GetPreferredWaitingRowForEmployee(
                                    tenureRowsForPost,
                                    emp.EmployeeID);

                                if (tenureRow == null)
                                    continue;

                                if (!tenureRow.WaitingPostPositionID.HasValue)
                                    continue;

                                var waitingPostPositionId = tenureRow.WaitingPostPositionID.Value;

                                // ✅ MARK IN PROGRESS
                                await waitingPostPositionService.UpdateProcessingStatusAsync(waitingPostPositionId, ProcessingStatus.InProgress, "Processing started", markProcessed: false);
                                var available = await requestService.CheckAvailablePostForJobAsync(postId, dateOfWork, emp.EmployeeID, request.DepartmentID, request.TimeZoneId, departmentPolicy, false).ConfigureAwait(false);
                                // ❌ FAILURE
                                if (available.HasError)
                                {
                                    result.AddFailure(emp.EmployeeID, available.Message);
                                    await waitingPostPositionService.UpdateProcessingStatusAsync(waitingPostPositionId, ProcessingStatus.Failed, available.Message, markProcessed: true);
                                    // Eligibility failed: take the row off the waiting queue unless the only issue is "no slot"
                                    // (post full / waiting list off path) — then keep status Waiting for a later run or manual handling.
                                    if (!available.HasNoSlot)
                                    {
                                        await waitingPostPositionService.MarkWaitingRowDeclinedForJobAsync(waitingPostPositionId, cancellationToken).ConfigureAwait(false);
                                    }

                                    var failChangedOn = DateTime.UtcNow;
                                    var audit = _jobAuditService.CreateWaitingPostPositionJobAudit(
                                        tenureRow, 
                                        request.DepartmentID, 
                                        emp.EmployeeID, 
                                        dateOfWork, 
                                        AuditAction.UPDATE, 
                                        failChangedOn, 
                                        correlationId, 
                                        ProcessingStatus.Failed, 
                                        available.Message);
                                    await _jobAuditService.EnqueueAuditAsync(audit, cancellationToken).ConfigureAwait(false);
                                    tenureRowsForPost.RemoveAll(r => r.WaitingPostPositionID == tenureRow.WaitingPostPositionID);
                                    continue;
                                }
                                // ✅ SUCCESS
                                var copied = await postPositionService.CopyTenureWaitingToPostPositionsAsync(new List<WaitingPostPositionDto> { tenureRow }, cancellationToken).ConfigureAwait(false);

                                if (copied > 0)
                                {
                                    if (!employeeResults.ContainsKey(emp.EmployeeID))
                                    {
                                        employeeResults[emp.EmployeeID] = EmployeeResultDto.Success(emp.EmployeeID);
                                    }
                                    await waitingPostPositionService.UpdateProcessingStatusAsync(
                                        waitingPostPositionId,
                                        ProcessingStatus.Complete,
                                        "job assigned successfully",
                                        markProcessed: true);
                                    await waitingPostPositionService.MarkWaitingRowAssignedAsync(waitingPostPositionId, cancellationToken: cancellationToken).ConfigureAwait(false);
                                    await requestService.SendAssignedPostNotificationAsync(
                                            postId,
                                            dateOfWork,
                                            emp.EmployeeID,
                                            request.DepartmentID,
                                            cancellationToken)
                                        .ConfigureAwait(false);
                                    tenureRowsForPost.RemoveAll(r => r.WaitingPostPositionID == tenureRow.WaitingPostPositionID);
                                    var assignOkChangedOn = DateTime.UtcNow;
                                    var assignOkAudit = _jobAuditService.CreateWaitingPostPositionJobAudit(
                                        tenureRow,
                                        request.DepartmentID,
                                        emp.EmployeeID,
                                        dateOfWork,
                                        AuditAction.UPDATE,
                                        assignOkChangedOn,
                                        correlationId,
                                        ProcessingStatus.Complete,
                                        string.Empty);
                                    await _jobAuditService.EnqueueAuditAsync(assignOkAudit, cancellationToken).ConfigureAwait(false);
                                }
                                else
                                {
                                    var copyFailDetail = string.IsNullOrWhiteSpace(available.Message)
                                        ? "CopyTenureWaitingToPostPositionsAsync returned 0 rows."
                                        : available.Message;
                                    if (!employeeResults.ContainsKey(emp.EmployeeID))
                                    {
                                        employeeResults[emp.EmployeeID] = EmployeeResultDto.Failure(emp.EmployeeID, copyFailDetail);
                                    }
                                    await waitingPostPositionService.UpdateProcessingStatusAsync(waitingPostPositionId, ProcessingStatus.Failed, copyFailDetail, markProcessed: true);
                                    var copyFailChangedOn = DateTime.UtcNow;
                                    var audit = _jobAuditService.CreateWaitingPostPositionJobAudit(
                                        tenureRow, 
                                        request.DepartmentID, 
                                        emp.EmployeeID, 
                                        dateOfWork, 
                                        AuditAction.UPDATE, 
                                        copyFailChangedOn, 
                                        correlationId, 
                                        ProcessingStatus.Failed, 
                                        copyFailDetail);
                                    await _jobAuditService.EnqueueAuditAsync(audit, cancellationToken).ConfigureAwait(false);
                                }
                                alreadyAppliedForThisPost.Add(emp.EmployeeID);
                                copiedAnyThisIteration = copied > 0;

                                // Ensure remaining `Waiting` rows are renumbered (1..n)
                                // according to the last computed tenure order.
                                await ReorderWaitingPositionsFromTenureOrderAsync(
                                        waitingPostPositionService,
                                        postId,
                                        dateOfWork,
                                        lastOrderedEmployees,
                                        cancellationToken)
                                    .ConfigureAwait(false);
                                break;
                            }
                            if (!copiedAnyThisIteration)
                                break;
                        }

                        await ReorderWaitingPositionsFromTenureOrderAsync(
                                waitingPostPositionService,
                                postId,
                                dateOfWork,
                                lastOrderedEmployees,
                                cancellationToken)
                            .ConfigureAwait(false);
                    }
                }
                // map dictionary → DTO once
                foreach (var empResult in employeeResults.Values)
                {
                    if (empResult.IsAssigned)
                        result.AddSuccess(empResult.EmployeeId);
                    else
                        result.AddFailure(empResult.EmployeeId, empResult.Reason);
                }
                result.FinalizeResult();
                var completionMessage = $"Processing completed. Assigned: {result.AssignedCount}, Failed: {result.FailedCount}, Total: {result.TotalEmployees}.";
                if (result.AssignedCount > 0)
                {
                    completionMessage += " Assigned requests were moved from tenure to open.";
                }
                result.MarkSuccessed(completionMessage);
                // ✅ SAVE RESULT JSON
                await requestScheduleService.UpsertRequestProcessingResultAsync(request.RequestID, result, cancellationToken, ReleaseAs.Open).ConfigureAwait(false);
                var completionChangedOn = DateTime.UtcNow;
                var completionDetail = $"{completionMessage} ReleaseAs: {nameof(ReleaseAs.TenureBased)} → {nameof(ReleaseAs.Open)}.";
                var completionPlaceholder = BuildRequestScopePlaceholderRow(request);
                var completionAudit = _jobAuditService.CreateWaitingPostPositionJobAudit(
                    completionPlaceholder,
                    request.DepartmentID,
                    employeeId: null,
                    completionPlaceholder.DateOfWork,
                    AuditAction.UPDATE,
                    completionChangedOn,
                    correlationId,
                    ProcessingStatus.Complete,
                    completionDetail,
                    entityIdOverride: request.RequestID.ToString("D"));
                await _jobAuditService.EnqueueAuditAsync(completionAudit, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                Logger.LogWarning(
                    "Tenure processing cancelled for Request {RequestID}. Schedule stays InProgress so the next run can retry it.",
                    request.RequestID);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Tenure processing failed for Request {RequestID}", request.RequestID);

                result.MarkFailed(ex.Message);

                await requestScheduleService.UpsertRequestProcessingResultAsync(request.RequestID, result, cancellationToken).ConfigureAwait(false);
                
                var exceptionChangedOn = DateTime.UtcNow;
                var exceptionPlaceholder = BuildRequestScopePlaceholderRow(request);
                var exceptionAudit = _jobAuditService.CreateWaitingPostPositionJobAudit(
                    exceptionPlaceholder,
                    request.DepartmentID,
                    employeeId: null,
                    exceptionPlaceholder.DateOfWork,
                    AuditAction.UPDATE,
                    exceptionChangedOn,
                    correlationId,
                    ProcessingStatus.Failed,
                    ex.Message,
                    entityIdOverride: request.RequestID.ToString("D"));
                await _jobAuditService.EnqueueAuditAsync(exceptionAudit, cancellationToken).ConfigureAwait(false);
            }
    }

    private static Task ReorderWaitingPositionsFromTenureOrderAsync(
        IWaitingPostPositionService waitingPostPositionService,
        Guid postId,
        DateOnly dateOfWork,
        IReadOnlyList<TenureEmployeeStatisticsDto>? orderedEmployees,
        CancellationToken cancellationToken)
    {
        if (orderedEmployees == null || orderedEmployees.Count == 0)
            return Task.CompletedTask;

        var finalOrderedIds = orderedEmployees.Select(e => e.EmployeeID).ToList();
        return waitingPostPositionService.ReorderWaitingPositionsByGivenOrderNumbersAsync(postId, dateOfWork, finalOrderedIds, cancellationToken);
    }

    /// <summary>Minimal <see cref="WaitingPostPositionDto"/> so request-level job audits reuse <see cref="CreateWaitingPostPositionJobAudit"/>.</summary>
    private static WaitingPostPositionDto BuildRequestScopePlaceholderRow(RequestScheduleProcessDueRequestDto request)
    {
        var title = string.IsNullOrWhiteSpace(request.RequestTitle) ? "(no title)" : request.RequestTitle.Trim();
        return new WaitingPostPositionDto
        {
            Post = new PostDto { PostTitle = title },
            DateOfWork = DateOnly.FromDateTime(request.ProcessLocalDateTime),
            RequestId = request.RequestID,
        };
    }

}