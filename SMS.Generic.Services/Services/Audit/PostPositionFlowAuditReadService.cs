using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SMS.Data;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services.Audit;

public sealed class PostPositionFlowAuditReadService : IPostPositionFlowAuditReadService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly AuditDbContext _auditDbContext;
    private readonly IPostPositionRepository _postPositionRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ITokenParser _tokenParser;

    public PostPositionFlowAuditReadService(
        AuditDbContext auditDbContext,
        IPostPositionRepository postPositionRepository,
        IEmployeeRepository employeeRepository,
        ITokenParser tokenParser)
    {
        _auditDbContext = auditDbContext;
        _postPositionRepository = postPositionRepository;
        _employeeRepository = employeeRepository;
        _tokenParser = tokenParser;
    }

    public async Task<PagedList<JobPositionFlowStoryDto>> GetPagedAsync(
        PageParam? pageParam,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        pageParam ??= new PageParam();
        var departmentId = _tokenParser.GetDepartmentID();
        var searchTerm = string.IsNullOrWhiteSpace(search) ? null : search.Trim();

        var baseQuery = _auditDbContext.PostPositionFlowAudits
            .AsNoTracking()
            .Where(x => x.EventType != PostPositionFlowEventType.Created
                && (!departmentId.HasValue || x.DepartmentID == null || x.DepartmentID == departmentId));

        if (searchTerm != null)
        {
            var like = searchTerm.ToLowerInvariant();
            baseQuery = baseQuery.Where(x =>
                (x.PostTitle != null && x.PostTitle.ToLower().Contains(like)) ||
                (x.EmployeeName != null && x.EmployeeName.ToLower().Contains(like)) ||
                (x.PerformedByName != null && x.PerformedByName.ToLower().Contains(like)) ||
                (x.Description != null && x.Description.ToLower().Contains(like)) ||
                (x.DetailsJson != null && x.DetailsJson.ToLower().Contains(like)) ||
                x.PostPositionId.ToString().Contains(like));
        }

        var rootQuery = baseQuery
            .GroupBy(x => x.PostPositionId)
            .Select(g => new
            {
                PostPositionId = g.Key,
                Latest = g.Max(x => x.RecordedDate)
            })
            .OrderByDescending(x => x.Latest);

        var total = await rootQuery.CountAsync(cancellationToken).ConfigureAwait(false);

        // Use Offset/Limit properties (they derive skip from page/pageSize).
        // GetOffset()/GetLimit() only return raw offset/limit fields and ignore page/pageSize.
        var offset = pageParam.Offset ?? 0;
        var limit = pageParam.Limit ?? PageParam.MinPageSize;

        var pageIds = await rootQuery
            .Skip(offset)
            .Take(limit)
            .Select(x => x.PostPositionId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var stories = new List<JobPositionFlowStoryDto>(pageIds.Count);
        foreach (var id in pageIds)
        {
            stories.Add(await BuildStoryAsync(id, cancellationToken).ConfigureAwait(false));
        }

        return new PagedList<JobPositionFlowStoryDto>(stories, pageParam, total);
    }

    public async Task<JobPositionFlowStoryDto> GetByPostPositionIdAsync(
        Guid postPositionId,
        CancellationToken cancellationToken = default)
    {
        if (postPositionId == Guid.Empty)
            throw new InvalidOperationException(
                ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_POSITION_NOT_FOUND_BY_ID, postPositionId));

        return await BuildStoryAsync(postPositionId, cancellationToken).ConfigureAwait(false);
    }

    private async Task<JobPositionFlowStoryDto> BuildStoryAsync(
        Guid postPositionId,
        CancellationToken cancellationToken)
    {
        var departmentId = _tokenParser.GetDepartmentID();

        var rows = await _auditDbContext.PostPositionFlowAudits
            .AsNoTracking()
            .Where(x => x.PostPositionId == postPositionId
                && x.EventType != PostPositionFlowEventType.Created
                && (!departmentId.HasValue || x.DepartmentID == null || x.DepartmentID == departmentId))
            .OrderBy(x => x.RecordedDate)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var position = await _postPositionRepository
            .GetByIdForFlowAuditAsync(postPositionId, cancellationToken)
            .ConfigureAwait(false);

        string employeeName = string.Empty;
        if (position?.Employee != null)
            employeeName = BaseDto.FormatDisplayName(position.Employee.LastName, position.Employee.FirstName) ?? string.Empty;
        else if (position?.EmployeeID is { } eid && eid != Guid.Empty)
        {
            var employee = await _employeeRepository
                .GetByIdForFlowAuditAsync(eid, cancellationToken)
                .ConfigureAwait(false);
            employeeName = employee == null
                ? string.Empty
                : BaseDto.FormatDisplayName(employee.LastName, employee.FirstName) ?? string.Empty;
        }
        else if (rows.LastOrDefault()?.EmployeeName is { } lastName)
        {
            employeeName = lastName;
        }

        if (string.IsNullOrWhiteSpace(employeeName))
            employeeName = "—";

        var latest = rows.LastOrDefault();
        var workDate = position?.DateOfWork?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty;
        var start = FormatTime(position?.StartTime ?? position?.Post?.PostStartTime);
        var end = FormatTime(position?.EndTime ?? position?.Post?.PostEndTime);
        var status = position?.PostPositionStatus.ToString()
            ?? latest?.EventType switch
            {
                PostPositionFlowEventType.Closed => nameof(PostPositionStatus.Closed),
                PostPositionFlowEventType.ClosedAndSplit => nameof(PostPositionStatus.ClosedAndSplit),
                PostPositionFlowEventType.Cancelled => nameof(PostPositionStatus.Cancelled),
                PostPositionFlowEventType.CancellationPending => nameof(PostPositionStatus.CancellationPending),
                PostPositionFlowEventType.Transferred => nameof(PostPositionStatus.Transferred),
                _ => nameof(PostPositionStatus.Assigned)
            };

        return new JobPositionFlowStoryDto
        {
            Id = $"flow-{postPositionId:N}",
            PostPositionId = postPositionId,
            JobTitle = position?.Post?.PostTitle ?? latest?.PostTitle ?? string.Empty,
            WorkDate = workDate,
            StartTime = start,
            EndTime = end,
            CurrentStatus = status,
            EmployeeName = employeeName,
            Events = rows.Select(MapEvent).ToList()
        };
    }

    private static JobPositionFlowEventDto MapEvent(PostPositionFlowAudit row)
    {
        var details = ParseDetails(row.DetailsJson);
        return new JobPositionFlowEventDto
        {
            Id = row.Id,
            Action = ToAction(row.EventType),
            OccurredAt = DateTime.SpecifyKind(row.RecordedDate, DateTimeKind.Utc).ToString("o", CultureInfo.InvariantCulture),
            PerformedBy = string.IsNullOrWhiteSpace(row.PerformedByName)
                ? (row.EventType == PostPositionFlowEventType.AssignedBySystem ? "System" : string.Empty)
                : row.PerformedByName,
            Role = ToRole(row.EventType),
            EmployeeName = row.EmployeeName,
            Summary = row.Description,
            Details = details.Count == 0 ? null : details
        };
    }

    private static List<JobPositionFlowDetailItemDto> ParseDetails(string? detailsJson)
    {
        var result = new List<JobPositionFlowDetailItemDto>();
        if (string.IsNullOrWhiteSpace(detailsJson))
            return result;

        try
        {
            using var doc = JsonDocument.Parse(detailsJson);
            if (doc.RootElement.ValueKind == JsonValueKind.Array)
            {
                var items = JsonSerializer.Deserialize<List<JobPositionFlowDetailItemDto>>(detailsJson, JsonOptions);
                if (items == null)
                    return result;

                foreach (var item in items)
                {
                    if (item == null || string.IsNullOrWhiteSpace(item.Label))
                        continue;
                    if (string.Equals(item.Label, "Request", StringComparison.OrdinalIgnoreCase))
                        continue;
                    result.Add(item);
                }

                return result;
            }

            // Legacy flat dictionary: { "Closed by": "...", "Vehicle": "Bike · #32" }
            var map = JsonSerializer.Deserialize<Dictionary<string, string?>>(detailsJson, JsonOptions);
            if (map == null)
                return result;

            foreach (var pair in map)
            {
                if (string.IsNullOrWhiteSpace(pair.Key))
                    continue;
                if (string.Equals(pair.Key, "Request", StringComparison.OrdinalIgnoreCase))
                    continue;
                result.Add(new JobPositionFlowDetailItemDto
                {
                    Label = pair.Key,
                    Value = pair.Value ?? string.Empty
                });
            }
        }
        catch
        {
            // Ignore malformed detail payloads.
        }

        return result;
    }

    /// <summary>Maps stored enum → frontend JobPositionFlowAction string.</summary>
    private static string ToAction(PostPositionFlowEventType eventType) => eventType switch
    {
        PostPositionFlowEventType.Created => "created",
        PostPositionFlowEventType.SelfTaken => "self_taken",
        PostPositionFlowEventType.AssignedByAdmin => "assigned_by_admin",
        PostPositionFlowEventType.AssignedByOther => "assigned_by_other",
        PostPositionFlowEventType.AssignedBySystem => "assigned_by_system",
        PostPositionFlowEventType.Transferred => "transferred",
        PostPositionFlowEventType.UnassignedByAdmin => "unassigned_by_admin",
        PostPositionFlowEventType.Closed => "closed",
        PostPositionFlowEventType.ClosedAndSplit => "split",
        PostPositionFlowEventType.CancellationPending => "cancellation_pending",
        PostPositionFlowEventType.Cancelled => "cancelled",
        PostPositionFlowEventType.SplitPending => "split_pending",
        PostPositionFlowEventType.SplitAccepted => "split_accepted",
        PostPositionFlowEventType.SplitDeclined => "split_declined",
        PostPositionFlowEventType.Unclosed => "unclosed",
        PostPositionFlowEventType.CancellationRevoked => "cancellation_revoked",
        _ => eventType.ToString().ToLowerInvariant()
    };

    private static string ToRole(PostPositionFlowEventType eventType) => eventType switch
    {
        PostPositionFlowEventType.AssignedByAdmin
            or PostPositionFlowEventType.UnassignedByAdmin
            or PostPositionFlowEventType.Transferred => "admin",
        PostPositionFlowEventType.AssignedBySystem
            or PostPositionFlowEventType.Created => "system",
        _ => "employee"
    };

    private static string FormatTime(TimeOnly? time) =>
        time?.ToString("HH:mm", CultureInfo.InvariantCulture) ?? string.Empty;
}
