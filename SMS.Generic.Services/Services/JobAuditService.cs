using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using SMS.Generic.Common.Audit;
using SMS.Generic.Common.Enums;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Audit;
using SMS.Generic.Dtos.Dtos.Audit;
using SMS.Generic.Dtos.Interfaces.Audit;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services;

public class JobAuditService : IJobAuditService
{
    /// <summary>
    /// <see cref="PtaeoCode"/> properties with <see cref="AuditedPropertyAttribute"/> (same selection as SaveChanges audit for attributed-only entities).
    /// </summary>
    private static readonly Lazy<(PropertyInfo Prop, AuditedPropertyAttribute Attr)[]> PtaeoCodeJobAuditFields = new(ResolvePtaeoCodeJobAuditFields);

    private readonly IAuditQueueService _auditQueue;

    private static (PropertyInfo Prop, AuditedPropertyAttribute Attr)[] ResolvePtaeoCodeJobAuditFields()
    {
        var t = typeof(PtaeoCode);
        var entityAttr = t.GetCustomAttribute<AuditedEntityAttribute>(inherit: false)
            ?? throw new InvalidOperationException($"{t.Name} must define [AuditedEntity] for PTAEO job audit field discovery.");
        if (!entityAttr.OnlyAttributedProperties)
        {
            throw new InvalidOperationException(
                $"{t.Name} is expected to use {nameof(AuditedEntityAttribute.OnlyAttributedProperties)} = true for PTAEO job audit.");
        }

        return t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetIndexParameters().Length == 0)
            .Select(p => (Prop: p, Attr: p.GetCustomAttribute<AuditedPropertyAttribute>(inherit: false)))
            .Where(x => x.Attr is not null)
            .OrderBy(x => x.Prop.MetadataToken)
            .Select(x => (x.Prop, x.Attr!))
            .ToArray();
    }

    public JobAuditService(IAuditQueueService auditQueue)
    {
        _auditQueue = auditQueue ?? throw new ArgumentNullException(nameof(auditQueue));
    }

    private static string BuildJson(List<AuditFieldChange> changes)
    {
        return JsonSerializer.Serialize(changes, AuditJsonSerializerOptions.ChangedFields);
    }

    private static AuditFieldChange Field(string prop, object? oldVal, object? newVal)
    {
        return new AuditFieldChange
        {
            Property = prop,
            Old = oldVal,
            New = newVal
        };
    }

    public async Task EnqueueAuditAsync<T>(T audit, CancellationToken cancellationToken) where T : AuditBase
    {
        await _auditQueue.EnqueueAsync(audit, cancellationToken).ConfigureAwait(false);
    }

    public WaitingPostPositionJobRunAudit CreateWaitingPostPositionJobAudit(
        WaitingPostPositionDto tenureRow,
        Guid departmentId,
        Guid? employeeId,
        DateOnly dateOfWork,
        AuditAction action,
        DateTime changedOn,
        string? correlationId,
        ProcessingStatus processingStatus,
        string? failureReason,
        string? entityIdOverride = null)
    {
        var postTitle = string.IsNullOrWhiteSpace(tenureRow.Post?.PostTitle)
            ? "(no title)"
            : tenureRow.Post.PostTitle.Trim();

        var changes = new List<AuditFieldChange>(string.IsNullOrWhiteSpace(failureReason) ? 4 : 5)
        {
            Field("Assigned Employee Name", null, string.IsNullOrWhiteSpace(tenureRow.AssignedEmployeeName) ? null : tenureRow.AssignedEmployeeName),
            Field("Post Date", null, dateOfWork.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
            Field("Assigned Date", null, changedOn.ToString("O", CultureInfo.InvariantCulture)),
            Field("Status", null, processingStatus.ToString()),
        };

        if (!string.IsNullOrWhiteSpace(failureReason))
        {
            changes.Add(Field("Message", null, failureReason));
        }

        return new WaitingPostPositionJobRunAudit(
            tenureRow.Post?.PostID?.ToString() ?? string.Empty,
            postTitle,
            BuildJson(changes),
            action,
            changedOn,
            string.Empty,
            departmentId,
            tenureRow.WaitingPostPositionID?.ToString(),
            employeeId);
    }

    public UpdateReleasedOnToReleasedJobRunAudit CreateUpdateReleasedOnToReleasedAudit(
        Guid requestId,
        string displayName,
        Guid? departmentId,
        DateTime processedTime,
        bool hadOnToReleased,
        bool hadToComplete)
    {
        var runSummary = new List<AuditFieldChange>(1 + (hadOnToReleased ? 1 : 0) + (hadToComplete ? 1 : 0))
        {
            Field("Processed Time", null, processedTime.ToString("O")),
        };

        if (hadOnToReleased)
        {
            runSummary.Add(Field(
                "Request Status",
                nameof(RequestStatus.ReleasedOn).ToLowerInvariant(),
                nameof(RequestStatus.ReleaseNow).ToLowerInvariant()));
        }

        if (hadToComplete)
        {
            runSummary.Add(Field(
                "Request Status",
                nameof(RequestStatus.ReleaseNow).ToLowerInvariant(),
                nameof(RequestStatus.Completed).ToLowerInvariant()));
        }

        return new UpdateReleasedOnToReleasedJobRunAudit(
            requestId.ToString(),
            displayName,
            BuildJson(runSummary),
            AuditAction.UPDATE,
            DateTime.UtcNow,
            string.Empty,
            departmentId,
            Activity.Current?.Id,
            null);
    }

    public UpdatePreferredToManagedJobRunAudit CreateUpdatePreferredToManagedAudit(
        Guid requestId,
        string displayName,
        Guid? departmentId,
        DateTime processedUtc,
        bool success,
        string? message)
    {
        var runSummary = new List<AuditFieldChange>(success || !string.IsNullOrWhiteSpace(message) ? 2 : 1)
        {
            Field("Processed Time", null, processedUtc.ToString("O")),
        };

        if (success)
        {
            runSummary.Add(Field(
                "ReleaseAs",
                nameof(ReleaseAs.Preferred).ToLowerInvariant(),
                nameof(ReleaseAs.Managed).ToLowerInvariant()));
        }
        else if (!string.IsNullOrWhiteSpace(message))
        {
            runSummary.Add(Field("Message", null, message));
        }

        return new UpdatePreferredToManagedJobRunAudit(
            requestId.ToString(),
            displayName,
            BuildJson(runSummary),
            AuditAction.UPDATE,
            DateTime.UtcNow,
            string.Empty,
            departmentId,
            requestId.ToString(),
            null);
    }

    public PtaeoJobRunAudit CreatePtaeoAudit(
        PtaeoCodeDto dto,
        AuditAction action,
        bool isDelete,
        DateTime changedOn,
        PtaeoCodeDto? previousDto = null)
    {
        var id = dto.PtaeoCodeID ?? Guid.Empty;
        var entityId = id == Guid.Empty ? Guid.NewGuid().ToString() : id.ToString();
        var ptaeo = MapPtaeoFromDto(dto);

        string changedFieldsJson;
        if (isDelete)
        {
            changedFieldsJson = BuildPtaeoFieldsJson(ptaeo, isDelete: true);
        }
        else if (action == AuditAction.UPDATE && previousDto is not null)
        {
            changedFieldsJson = BuildPtaeoFieldsJsonDiff(MapPtaeoFromDto(previousDto), ptaeo);
        }
        else
        {
            changedFieldsJson = BuildPtaeoFieldsJson(ptaeo, isDelete: false);
        }

        return new PtaeoJobRunAudit(
            entityId,
            BuildPtaeoDisplayName(ptaeo),
            changedFieldsJson,
            action,
            changedOn,
            string.Empty,
            dto.DepartmentID == Guid.Empty ? null : dto.DepartmentID,
            null,
            null);
    }

    private static PtaeoCode MapPtaeoFromDto(PtaeoCodeDto dto) =>
        new()
        {
            PtaeoCodeID = dto.PtaeoCodeID ?? Guid.Empty,
            DepartmentID = dto.DepartmentID,
            ProjectNumber = dto.ProjectNumber,
            TaskNumber = dto.TaskNumber,
            AwardNumber = dto.AwardNumber,
            FundNumber = dto.FundNumber,
            OrganizationName = dto.OrganizationName,
            OrganizationNumber = dto.OrganizationNumber,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = dto.Status,
        };

    private static string BuildPtaeoDisplayName(PtaeoCode ptaeo)
    {
        static string Seg(string? s) => string.IsNullOrWhiteSpace(s) ? string.Empty : s.Trim();

        var parts = new[]
        {
            Seg(ptaeo.ProjectNumber),
            Seg(ptaeo.TaskNumber),
            Seg(ptaeo.AwardNumber),
            Seg(ptaeo.FundNumber),
            Seg(ptaeo.OrganizationNumber),
        };
        var joined = string.Join(".", parts.Where(p => p.Length > 0));
        if (string.IsNullOrEmpty(joined))
            joined = ptaeo.PtaeoCodeID == Guid.Empty ? Guid.Empty.ToString() : ptaeo.PtaeoCodeID.ToString();

        return joined;
    }

    private static string? FormatPtaeoAuditValue(object? value) =>
        value switch
        {
            null => null,
            string s => s,
            DateOnly d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            bool flag => flag ? "Active" : "Inactive",
            _ => value?.ToString(),
        };

    private static string BuildPtaeoFieldsJson(PtaeoCode ptaeo, bool isDelete)
    {
        var fieldDefs = PtaeoCodeJobAuditFields.Value;
        var list = new List<AuditFieldChange>(fieldDefs.Length);
        foreach (var (pi, attr) in fieldDefs)
        {
            var displayName = attr.DisplayName;
            var raw = FormatPtaeoAuditValue(pi.GetValue(ptaeo));
            list.Add(Field(displayName, raw, isDelete));
        }

        return BuildJson(list);
    }

    /// <summary>Old/new pairs for attributed columns that differ (formatted values).</summary>
    private static string BuildPtaeoFieldsJsonDiff(PtaeoCode previous, PtaeoCode current)
    {
        var fieldDefs = PtaeoCodeJobAuditFields.Value;
        var list = new List<AuditFieldChange>(fieldDefs.Length);
        foreach (var (pi, attr) in fieldDefs)
        {
            var oldFormatted = FormatPtaeoAuditValue(pi.GetValue(previous));
            var newFormatted = FormatPtaeoAuditValue(pi.GetValue(current));
            if (string.Equals(oldFormatted, newFormatted, StringComparison.Ordinal))
                continue;

            list.Add(Field(attr.DisplayName, oldFormatted, newFormatted));
        }

        return BuildJson(list);
    }

    private static AuditFieldChange Field(string property, string? value, bool isDelete) =>
        isDelete
            ? Field(property, value, null)
            : Field(property, null, value);
}
