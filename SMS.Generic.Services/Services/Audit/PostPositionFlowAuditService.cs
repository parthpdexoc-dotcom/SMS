using System.Globalization;
using System.Linq.Expressions;
using System.Text.Json;
using SMS.Data;
using SMS.Generic.Common.Audit;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services.Audit;

public sealed class PostPositionFlowAuditService : IPostPositionFlowAuditService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    private readonly AuditDbContext _auditDbContext;
    private readonly IPostRepository _postRepository;
    private readonly IPostPositionRepository _postPositionRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentEquipmentService _departmentEquipmentService;
    private readonly IAuditUserContext _auditUserContext;
    private readonly ITokenParser _tokenParser;

    public PostPositionFlowAuditService(
        AuditDbContext auditDbContext,
        IPostRepository postRepository,
        IPostPositionRepository postPositionRepository,
        IEmployeeRepository employeeRepository,
        IDepartmentEquipmentService departmentEquipmentService,
        IAuditUserContext auditUserContext,
        ITokenParser tokenParser)
    {
        _auditDbContext = auditDbContext;
        _postRepository = postRepository;
        _postPositionRepository = postPositionRepository;
        _employeeRepository = employeeRepository;
        _departmentEquipmentService = departmentEquipmentService;
        _auditUserContext = auditUserContext;
        _tokenParser = tokenParser;
    }

    public async Task LogAsync(
        PostPositionFlowEventType eventType,
        PostPositionFlowAuditContext context,
        CancellationToken cancellationToken = default)
    {
        var row = await BuildRowAsync(eventType, context, employeeNames: null, cancellationToken).ConfigureAwait(false);
        if (row != null)
            await PersistRowsAsync([row], cancellationToken).ConfigureAwait(false);
    }

    public async Task LogCloseFlowAsync(
        SplitPostPositionRequestDto request,
        PostPositionDto closedPosition,
        PostPositionDto closeResult,
        PostPositionStatus status,
        bool createSplitPostPosition,
        CancellationToken cancellationToken = default)
    {
        var sourcePostPositionId = request.PostPositionID!.Value;
        var splitChildId = createSplitPostPosition
            && closeResult.PostPositionID is { } resultId
            && resultId != Guid.Empty
            && resultId != sourcePostPositionId
                ? resultId
                : (Guid?)null;

        var postId = request.PostID ?? closedPosition.PostID;
        var closedEmployeeId = closedPosition.EmployeeID;
        var employeeId = closedEmployeeId == Guid.Empty ? null : (Guid?)closedEmployeeId;

        var employeeIds = CollectCloseFlowEmployeeIds(request, closedEmployeeId, createSplitPostPosition);
        var closerId = string.IsNullOrWhiteSpace(request.CloseBy)
            ? NormalizeGuid(_tokenParser.GetEmployeeID())
            : null;
        if (closerId.HasValue)
            employeeIds.Add(closerId.Value);

        var employeeNames = await ResolveEmployeeDisplayNamesAsync(employeeIds, cancellationToken).ConfigureAwait(false);
        var enrichedEquipment = await EnrichEquipmentDetailsAsync(request.EquipmentDetails, cancellationToken)
            .ConfigureAwait(false);

        var rows = new List<PostPositionFlowAudit>(splitChildId.HasValue ? 2 : 1)
        {
            (await BuildRowAsync(
                status == PostPositionStatus.ClosedAndSplit
                    ? PostPositionFlowEventType.ClosedAndSplit
                    : PostPositionFlowEventType.Closed,
                new PostPositionFlowAuditContext
                {
                    PostPositionId = sourcePostPositionId,
                    RelatedPostPositionId = splitChildId,
                    PostId = postId,
                    EmployeeId = employeeId,
                    RelatedEmployeeId = createSplitPostPosition ? request.SplitEmployeeID : null,
                    Details = BuildCloseFlowDetails(request, closedEmployeeId, createSplitPostPosition, closerId, employeeNames, enrichedEquipment)
                },
                employeeNames,
                cancellationToken).ConfigureAwait(false))!
        };

        if (splitChildId is { } childId
            && request.SplitEmployeeID is { } splitEmployeeId
            && splitEmployeeId != Guid.Empty)
        {
            rows.Add((await BuildRowAsync(
                PostPositionFlowEventType.SplitPending,
                new PostPositionFlowAuditContext
                {
                    PostPositionId = childId,
                    RelatedPostPositionId = sourcePostPositionId,
                    PostId = postId,
                    EmployeeId = splitEmployeeId,
                    Details = BuildSplitPendingDetails(closedEmployeeId, splitEmployeeId, employeeNames)
                },
                employeeNames,
                cancellationToken).ConfigureAwait(false))!);
        }

        await PersistRowsAsync(rows, cancellationToken).ConfigureAwait(false);
    }

    public async Task LogSplitDeclinedFlowAsync(
        PostPositionDto declined,
        Guid parentId,
        CancellationToken cancellationToken = default)
    {
        if (declined.PostPositionID is not { } childId || childId == Guid.Empty)
            return;

        var postId = declined.PostID;
        var employeeId = NormalizeEmployeeId(declined.EmployeeID);
        var details = DecisionDetails("Declined");

        var rows = new List<PostPositionFlowAudit>(parentId != Guid.Empty ? 2 : 1)
        {
            (await BuildRowAsync(
                PostPositionFlowEventType.SplitDeclined,
                new PostPositionFlowAuditContext
                {
                    PostPositionId = childId,
                    RelatedPostPositionId = parentId != Guid.Empty ? parentId : null,
                    PostId = postId,
                    EmployeeId = employeeId,
                    Description = "Split assignment declined",
                    Details = details
                },
                employeeNames: null,
                cancellationToken).ConfigureAwait(false))!
        };

        if (parentId != Guid.Empty)
        {
            rows.Add((await BuildRowAsync(
                PostPositionFlowEventType.SplitDeclined,
                new PostPositionFlowAuditContext
                {
                    PostPositionId = parentId,
                    RelatedPostPositionId = childId,
                    PostId = postId,
                    EmployeeId = employeeId,
                    Description = "Split assignment declined by recipient",
                    Details = details
                },
                employeeNames: null,
                cancellationToken).ConfigureAwait(false))!);
        }

        await PersistRowsAsync(rows, cancellationToken).ConfigureAwait(false);
    }

    public async Task LogSplitAcceptedFlowAsync(
        PostPositionDto accepted,
        CancellationToken cancellationToken = default)
    {
        if (accepted.PostPositionID is not { } childId || childId == Guid.Empty)
            return;

        var postId = accepted.PostID;
        var employeeId = NormalizeEmployeeId(accepted.EmployeeID);
        var parentId = accepted.ParentPostPositionId;
        var details = DecisionDetails("Accepted");

        var rows = new List<PostPositionFlowAudit>(parentId is { } pid && pid != Guid.Empty ? 2 : 1)
        {
            (await BuildRowAsync(
                PostPositionFlowEventType.SplitAccepted,
                new PostPositionFlowAuditContext
                {
                    PostPositionId = childId,
                    RelatedPostPositionId = parentId,
                    PostId = postId,
                    EmployeeId = employeeId,
                    Description = "Split assignment accepted",
                    Details = details
                },
                employeeNames: null,
                cancellationToken).ConfigureAwait(false))!
        };

        if (parentId is { } parent && parent != Guid.Empty)
        {
            rows.Add((await BuildRowAsync(
                PostPositionFlowEventType.SplitAccepted,
                new PostPositionFlowAuditContext
                {
                    PostPositionId = parent,
                    RelatedPostPositionId = childId,
                    PostId = postId,
                    EmployeeId = employeeId,
                    Description = "Split assignment accepted by recipient",
                    Details = details
                },
                employeeNames: null,
                cancellationToken).ConfigureAwait(false))!);
        }

        await PersistRowsAsync(rows, cancellationToken).ConfigureAwait(false);
    }

    public async Task LogStatusUpdateFlowAsync(
        PostPositionDto existingBeforeUpdate,
        PostPositionStatus newStatus,
        CancellationToken cancellationToken = default)
    {
        if (existingBeforeUpdate.PostPositionID is not { } postPositionId || postPositionId == Guid.Empty)
            return;

        var wasSplitAccept = newStatus == PostPositionStatus.Assigned
            && existingBeforeUpdate.PostPositionStatus == PostPositionStatus.SplitDecisionPending
            && existingBeforeUpdate.ParentPostPositionId is { } parentId
            && parentId != Guid.Empty;

        if (wasSplitAccept)
        {
            await LogSplitAcceptedFlowAsync(existingBeforeUpdate, cancellationToken).ConfigureAwait(false);
            return;
        }

        var flowEvent = newStatus switch
        {
            PostPositionStatus.Unclosed => PostPositionFlowEventType.Unclosed,
            PostPositionStatus.Assigned => PostPositionFlowEventType.AssignedByAdmin,
            _ => (PostPositionFlowEventType?)null
        };

        if (!flowEvent.HasValue)
            return;

        await LogAsync(
            flowEvent.Value,
            PostPositionFlowAuditContext.ForPosition(
                postPositionId,
                postId: existingBeforeUpdate.PostID,
                employeeId: NormalizeEmployeeId(existingBeforeUpdate.EmployeeID)),
            cancellationToken).ConfigureAwait(false);
    }

    public Task LogCancellationFlowAsync(
        Guid postPositionId,
        Guid? postId,
        Guid? employeeId,
        bool isApproved,
        string? reason,
        CancellationToken cancellationToken = default) =>
        LogAsync(
            isApproved ? PostPositionFlowEventType.Cancelled : PostPositionFlowEventType.CancellationPending,
            new PostPositionFlowAuditContext
            {
                PostPositionId = postPositionId,
                PostId = postId,
                EmployeeId = employeeId,
                Details = string.IsNullOrWhiteSpace(reason) ? null : ReasonDetails(reason)
            },
            cancellationToken);

    public Task LogCancellationRevokedFlowAsync(
        Guid postPositionId,
        Guid? postId,
        Guid? employeeId,
        CancellationToken cancellationToken = default) =>
        LogAsync(
            PostPositionFlowEventType.CancellationRevoked,
            PostPositionFlowAuditContext.ForPosition(postPositionId, postId, employeeId),
            cancellationToken);

    private async Task<PostPositionFlowAudit?> BuildRowAsync(
        PostPositionFlowEventType eventType,
        PostPositionFlowAuditContext context,
        IReadOnlyDictionary<Guid, string?>? employeeNames,
        CancellationToken cancellationToken)
    {
        try
        {
            if (context.PostPositionId == Guid.Empty)
                return null;

            var employeeId = NormalizeGuid(context.EmployeeId);
            var employeeNameHint = context.EmployeeName;
            var postId = NormalizeGuid(context.PostId);
            var requestId = NormalizeGuid(context.RequestId);

            // When EmployeeId/Name are missing, resolve from PostPosition (employee or placeholder).
            if (employeeId is null || string.IsNullOrWhiteSpace(employeeNameHint))
            {
                var (positionEmployeeId, positionEmployeeName, positionPostId) =
                    await ResolvePositionSubjectAsync(context.PostPositionId, cancellationToken).ConfigureAwait(false);

                employeeId ??= positionEmployeeId;
                if (string.IsNullOrWhiteSpace(employeeNameHint))
                    employeeNameHint = positionEmployeeName;
                postId ??= positionPostId;
            }

            var performedById = NormalizeGuid(context.PerformedByEmployeeId)
                ?? NormalizeGuid(_tokenParser.GetEmployeeID());

            var (resolvedPostId, resolvedRequestId, postTitle, employeeName, performedByName) =
                await ResolveDisplayAndKeysAsync(
                    postId,
                    requestId,
                    employeeId,
                    performedById,
                    context.PostTitle,
                    employeeNameHint,
                    context.PerformedByName,
                    employeeNames,
                    cancellationToken).ConfigureAwait(false);

            postId = resolvedPostId ?? postId;
            requestId = resolvedRequestId ?? requestId;

            string? assignmentType = null;

            // Assign / self-take / system: classify from PostPosition.CreatedUserID vs EmployeeID.
            if (eventType is PostPositionFlowEventType.SelfTaken
                or PostPositionFlowEventType.AssignedByAdmin
                or PostPositionFlowEventType.AssignedByOther
                or PostPositionFlowEventType.AssignedBySystem)
            {
                var (createdUserId, positionEmployeeId, adminTypeShort) =
                    await ResolveAssignClassificationAsync(context.PostPositionId, performedById, employeeId, cancellationToken)
                        .ConfigureAwait(false);

                performedById = createdUserId ?? performedById;
                if (performedById.HasValue && string.IsNullOrWhiteSpace(performedByName))
                    performedByName = await ResolveEmployeeDisplayNameAsync(performedById.Value, employeeNames)
                        .ConfigureAwait(false);

                if (!performedById.HasValue)
                {
                    eventType = PostPositionFlowEventType.AssignedBySystem;
                    assignmentType = "System";
                    performedByName = "System";
                }
                else if (positionEmployeeId.HasValue && performedById.Value == positionEmployeeId.Value)
                {
                    eventType = PostPositionFlowEventType.SelfTaken;
                    assignmentType = "Self take";
                }
                else if (!string.IsNullOrWhiteSpace(adminTypeShort))
                {
                    eventType = PostPositionFlowEventType.AssignedByAdmin;
                    assignmentType = adminTypeShort;
                }
                else
                {
                    eventType = PostPositionFlowEventType.AssignedByOther;
                    assignmentType = "Assigned by other employee";
                }
            }

            var isSystemAssign = eventType == PostPositionFlowEventType.AssignedBySystem;

            var description = string.IsNullOrWhiteSpace(context.Description)
                ? BuildDefaultDescription(eventType, employeeName, performedByName, context.RelatedEmployeeName)
                : context.Description.Trim();

            var details = context.Details != null
                ? context.Details
                    .Where(d => d != null && !string.Equals(d.Label, "Request", StringComparison.OrdinalIgnoreCase))
                    .ToList()
                : BuildDefaultDetails(eventType, employeeName, performedByName, context.RelatedEmployeeName, assignmentType);

            return new PostPositionFlowAudit
            {
                EventType = eventType,
                PostPositionId = context.PostPositionId,
                RelatedPostPositionId = NormalizeGuid(context.RelatedPostPositionId),
                PostId = postId,
                RequestId = requestId,
                DepartmentID = _auditUserContext.DepartmentID ?? _tokenParser.GetDepartmentID(),
                EmployeeId = employeeId,
                EmployeeName = employeeName,
                PerformedByEmployeeId = isSystemAssign ? null : performedById,
                PerformedByName = isSystemAssign ? "System" : performedByName,
                Description = Truncate(description, 512) ?? string.Empty,
                DetailsJson = details.Count == 0 ? null : JsonSerializer.Serialize(details, JsonOptions),
                PostTitle = Truncate(postTitle, 200),
                RecordedDate = DateTime.UtcNow
            };
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Loads the position subject: real employee when present; otherwise placeholder display name.
    /// </summary>
    private async Task<(Guid? EmployeeId, string? EmployeeName, Guid? PostId)> ResolvePositionSubjectAsync(
        Guid postPositionId,
        CancellationToken cancellationToken)
    {
        var position = await _postPositionRepository.GetByIdAsync(postPositionId,nameof(PostPosition.PostPositionID),
                x => x.Employee!,
                x => x.Placeholder!).ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();

        if (position is null)
            return (null, null, null);

        var postId = position.PostID == Guid.Empty ? null : (Guid?)position.PostID;
        var employeeId = NormalizeGuid(position.EmployeeID);

        if (employeeId is not null)
        {
            var name = position.Employee != null ? FormatEmployeeName(position.Employee) : null;
            return (employeeId, Truncate(name, 200), postId);
        }

        // No employee on the position ? use placeholder name when present.
        if (position.Placeholder != null)
        {
            var placeholderName = BaseDto.FormatDisplayName(position.Placeholder.LastName,position.Placeholder.FirstName);
            return (null, Truncate(placeholderName, 200), postId);
        }

        return (null, null, postId);
    }

    private async Task PersistRowsAsync(
        IReadOnlyList<PostPositionFlowAudit> rows,
        CancellationToken cancellationToken)
    {
        if (rows.Count == 0)
            return;

        try
        {
            foreach (var row in rows)
                _auditDbContext.PostPositionFlowAudits.Add(row);

            await _auditDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            // Intentionally swallow audit failures to avoid breaking the business flow.
        }
    }

    private static HashSet<Guid> CollectCloseFlowEmployeeIds(
        SplitPostPositionRequestDto request,
        Guid closedEmployeeId,
        bool isSplit)
    {
        var ids = new HashSet<Guid>();
        if (closedEmployeeId != Guid.Empty)
            ids.Add(closedEmployeeId);

        if (isSplit && request.SplitEmployeeID is { } splitId && splitId != Guid.Empty)
            ids.Add(splitId);

        return ids;
    }

    private static List<JobPositionFlowDetailItemDto> BuildCloseFlowDetails(
        SplitPostPositionRequestDto request,
        Guid closedEmployeeId,
        bool isSplit,
        Guid? closerId,
        IReadOnlyDictionary<Guid, string?> names,
        IReadOnlyList<SplitPostPositionEquipmentDto>? equipmentDetails = null)
    {
        var details = new List<JobPositionFlowDetailItemDto>();

        var closerName = request.CloseBy;
        if (string.IsNullOrWhiteSpace(closerName) && closerId.HasValue)
            closerName = LookupName(names, closerId.Value);

        details.Add(Item("Closed by", closerName));

        if (closedEmployeeId != Guid.Empty)
            details.Add(Item("Employee", LookupName(names, closedEmployeeId)));

        var billableHours = request.GetBillableHours();
        if (billableHours > 0)
            details.Add(Item("Hours worked", billableHours.ToString("0.##", CultureInfo.InvariantCulture)));
        else if (request.HoursWorked is int minutes && minutes > 0)
            details.Add(Item("Hours worked", (minutes / 60m).ToString("0.##", CultureInfo.InvariantCulture)));

        if (isSplit && request.SplitEmployeeID is { } splitId && splitId != Guid.Empty)
            details.Add(Item("Split with", LookupName(names, splitId)));

        AppendEquipmentDetails(details, equipmentDetails ?? request.EquipmentDetails);
        return details;
    }

    private async Task<IReadOnlyList<SplitPostPositionEquipmentDto>?> EnrichEquipmentDetailsAsync(
        List<SplitPostPositionEquipmentDto>? equipmentDetails,
        CancellationToken cancellationToken)
    {
        if (equipmentDetails == null || equipmentDetails.Count == 0)
            return equipmentDetails;

        var departmentEquipmentIds = equipmentDetails
            .Where(e => e.DepartmentEquipmentID is { } id && id != Guid.Empty)
            .Select(e => e.DepartmentEquipmentID!.Value)
            .Distinct()
            .ToList();

        if (departmentEquipmentIds.Count == 0)
            return equipmentDetails;

        Expression<Func<DepartmentEquipment, object>>[] includes = { x => x.EquipmentType! };
        var deptEquips = await _departmentEquipmentService
            .GetAllAsync(
                de => departmentEquipmentIds.Contains(de.DepartmentEquipmentID),
                includes,
                null)
            .ConfigureAwait(false);

        var deptEquipDict = (deptEquips ?? Enumerable.Empty<DepartmentEquipmentDto>())
            .Where(de => de.DepartmentEquipmentID.HasValue)
            .GroupBy(de => de.DepartmentEquipmentID!.Value)
            .ToDictionary(g => g.Key, g => g.First());

        foreach (var item in equipmentDetails)
        {
            if (!item.DepartmentEquipmentID.HasValue
                || !deptEquipDict.TryGetValue(item.DepartmentEquipmentID.Value, out var deptEquip))
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(item.DepartmentEquipmentName))
            {
                item.DepartmentEquipmentName =
                    deptEquip.EquipmentType?.EquipementTypeShort
                    ?? deptEquip.EquipmentType?.EquipementTypeLong;
            }

            var isVehicle = deptEquip.EquipmentType?.IsVehicle == true;
            item.DepartmentEquipmentIsVehicle ??= isVehicle;
            if (isVehicle && item.EquipmentUsedIsVehicle != true)
                item.EquipmentUsedIsVehicle = true;
        }

        cancellationToken.ThrowIfCancellationRequested();
        return equipmentDetails;
    }

    private static List<JobPositionFlowDetailItemDto> BuildSplitPendingDetails(
        Guid splitFromEmployeeId,
        Guid splitToEmployeeId,
        IReadOnlyDictionary<Guid, string?> names) =>
    [
        Item("Split from", LookupName(names, splitFromEmployeeId)),
        Item("Offered to", LookupName(names, splitToEmployeeId)),
        Item("Status", "Split decision pending")
    ];

    private static void AppendEquipmentDetails(
        List<JobPositionFlowDetailItemDto> details,
        IReadOnlyList<SplitPostPositionEquipmentDto>? equipmentDetails)
    {
        if (equipmentDetails == null || equipmentDetails.Count == 0)
            return;

        foreach (var item in equipmentDetails)
        {
            var name = string.IsNullOrWhiteSpace(item.DepartmentEquipmentName)
                ? "Equipment"
                : item.DepartmentEquipmentName.Trim();
            var isVehicle = IsVehicleEquipment(item);
            var nested = new List<JobPositionFlowDetailItemDto> { Item("type", name) };

            if (isVehicle)
            {
                if (!string.IsNullOrWhiteSpace(item.VehNumber))
                    nested.Add(Item("number", item.VehNumber.Trim()));

                var mileage = FormatMileage(item.TotalMileage, item.StartMileage, item.EndMileage);
                if (mileage != null)
                    nested.Add(Item("mileage", mileage));

                if (item.VehicleQuantity is > 0)
                    nested.Add(Item("quantity", item.VehicleQuantity.Value.ToString(CultureInfo.InvariantCulture)));

                if (item.VehicleOwnedBy.HasValue)
                    nested.Add(Item("owned by", item.VehicleOwnedBy.Value.ToString()));

                details.Add(new JobPositionFlowDetailItemDto { Label = "Vehicle", Items = nested });
            }
            else
            {
                if (item.UsedEquipmentQuantity is > 0)
                    nested.Add(Item("quantity", item.UsedEquipmentQuantity.Value.ToString(CultureInfo.InvariantCulture)));
                if (item.EquipmentUsedHours is > 0)
                    nested.Add(Item("hours", item.EquipmentUsedHours.Value.ToString("0.##", CultureInfo.InvariantCulture)));

                details.Add(new JobPositionFlowDetailItemDto { Label = "Equipment", Items = nested });
            }
        }
    }

    private static bool IsVehicleEquipment(ClosedPostPositionEquipmentDto item) =>
        item.DepartmentEquipmentIsVehicle == true
        || item.EquipmentUsedIsVehicle == true
        || item.StartMileage.HasValue
        || item.EndMileage.HasValue
        || item.TotalMileage.HasValue
        || !string.IsNullOrWhiteSpace(item.VehNumber)
        || item.VehicleOwnedBy.HasValue
        || item.VehicleQuantity is > 0
        || item.VehicleUsedHours is > 0;

    private async Task<(Guid? PostId, Guid? RequestId, string? PostTitle, string? EmployeeName, string? PerformedByName)> ResolveDisplayAndKeysAsync(
        Guid? postId,
        Guid? requestId,
        Guid? employeeId,
        Guid? performedById,
        string? postTitleHint,
        string? employeeNameHint,
        string? performedByNameHint,
        IReadOnlyDictionary<Guid, string?>? employeeNames,
        CancellationToken cancellationToken)
    {
        string? postTitle = Truncate(postTitleHint, 200);
        string? employeeName = Truncate(employeeNameHint, 200);
        string? performedByName = Truncate(performedByNameHint, 200);

        if (postId is { } pid && pid != Guid.Empty && (postTitle is null || requestId is null))
        {
            var post = await _postRepository.GetByIdAsync(pid, nameof(Post.PostID)).ConfigureAwait(false);
            if (post is not null)
            {
                postTitle ??= post.PostTitle;
                if (requestId is null && post.RequestID is { } rid && rid != Guid.Empty)
                    requestId = rid;
            }
        }

        if (employeeId is { } eid && employeeName is null)
            employeeName = await ResolveEmployeeDisplayNameAsync(eid, employeeNames).ConfigureAwait(false);

        if (performedById is { } pbid && performedByName is null)
        {
            performedByName = pbid == employeeId
                ? employeeName
                : await ResolveEmployeeDisplayNameAsync(pbid, employeeNames).ConfigureAwait(false);
        }

        cancellationToken.ThrowIfCancellationRequested();
        return (postId, requestId, postTitle, employeeName, performedByName);
    }

    private async Task<IReadOnlyDictionary<Guid, string?>> ResolveEmployeeDisplayNamesAsync(
        IEnumerable<Guid> employeeIds,
        CancellationToken cancellationToken)
    {
        var ids = employeeIds.Where(id => id != Guid.Empty).Distinct().ToList();
        if (ids.Count == 0)
            return new Dictionary<Guid, string?>();

        var employees = await _employeeRepository
            .GetAllAsync(filter: e => ids.Contains(e.EmployeeID), includes: null, pageParam: null)
            .ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();

        return (employees ?? Enumerable.Empty<Employee>())
            .GroupBy(e => e.EmployeeID)
            .ToDictionary(g => g.Key, g => FormatEmployeeName(g.First()));
    }

    private async Task<string?> ResolveEmployeeDisplayNameAsync(
        Guid employeeId,
        IReadOnlyDictionary<Guid, string?>? employeeNames = null)
    {
        if (employeeNames != null && employeeNames.TryGetValue(employeeId, out var cached))
            return cached;

        var map = await ResolveEmployeeDisplayNamesAsync([employeeId], CancellationToken.None).ConfigureAwait(false);
        return map.TryGetValue(employeeId, out var name) ? name : null;
    }

    private static string? LookupName(IReadOnlyDictionary<Guid, string?> names, Guid employeeId) =>
        names.TryGetValue(employeeId, out var name) ? name : null;

    private static string? FormatEmployeeName(Employee employee) =>
        BaseDto.FormatDisplayName(employee.LastName, employee.FirstName) ?? employee.FullName;

    private static string? FormatMileage(decimal? total, decimal? start, decimal? end)
    {
        if (total.HasValue)
            return total.Value.ToString("0.##", CultureInfo.InvariantCulture);
        if (start.HasValue && end.HasValue)
            return (end.Value - start.Value).ToString("0.##", CultureInfo.InvariantCulture);
        return null;
    }

    /// <summary>
    /// Self take when <c>CreatedUserID == EmployeeID</c>.
    /// Otherwise: AdminTypeShort from EmployeeAdmin / DepartmentAdmin ? AssignedByAdmin;
    /// no admin role ? AssignedByOther.
    /// </summary>
    private async Task<(Guid? CreatedUserId, Guid? PositionEmployeeId, string? AdminTypeShort)> ResolveAssignClassificationAsync(
        Guid postPositionId,
        Guid? fallbackCreatedUserId,
        Guid? fallbackEmployeeId,
        CancellationToken cancellationToken)
    {
        var position = await _postPositionRepository
            .GetByIdAsync(postPositionId, nameof(PostPosition.PostPositionID))
            .ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();

        var createdUserId = NormalizeGuid(position?.CreatedUserID) ?? fallbackCreatedUserId;
        var positionEmployeeId = NormalizeGuid(position?.EmployeeID) ?? fallbackEmployeeId;

        string? adminTypeShort = null;
        if (createdUserId.HasValue
            && (!positionEmployeeId.HasValue || createdUserId.Value != positionEmployeeId.Value))
        {
            adminTypeShort = await ResolveAssignerAdminTypeShortAsync(createdUserId.Value, cancellationToken)
                .ConfigureAwait(false);
        }

        return (createdUserId, positionEmployeeId, adminTypeShort);
    }

    /// <summary>
    /// Assigner's role label(s) from EmployeeAdmin + DepartmentAdmin (e.g. "Department Admin, Operator").
    /// </summary>
    private async Task<string?> ResolveAssignerAdminTypeShortAsync(Guid createdUserId, CancellationToken cancellationToken)
    {
        var loginInfo = await _employeeRepository
            .GetEmployeeWithDetailsByIdAsync(createdUserId)
            .ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();

        var roles = loginInfo?.AdminTypes?
            .Select(at => at.AdminTypeShort?.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return roles is { Count: > 0 } ? string.Join(", ", roles!) : null;
    }

    private static string BuildDefaultDescription(
        PostPositionFlowEventType eventType,
        string? employeeName,
        string? performedByName,
        string? relatedEmployeeName)
    {
        var name = string.IsNullOrWhiteSpace(performedByName) ? "System" : performedByName;

        return eventType switch
        {
            PostPositionFlowEventType.Created => "Job position created from request schedule",
            PostPositionFlowEventType.SelfTaken => string.IsNullOrWhiteSpace(employeeName)
                ? "Self taken"
                : $"Self taken by {employeeName}",
            PostPositionFlowEventType.AssignedByAdmin
                or PostPositionFlowEventType.AssignedByOther => string.IsNullOrWhiteSpace(employeeName)
                ? $"Assigned by {name}"
                : $"Assigned by {name} to {employeeName}",
            PostPositionFlowEventType.AssignedBySystem => string.IsNullOrWhiteSpace(employeeName)
                ? "Assigned by System"
                : $"Assigned by System to {employeeName}",
            PostPositionFlowEventType.Transferred =>
                $"Transferred by admin from {relatedEmployeeName ?? "previous employee"} to {employeeName ?? "employee"}",
            PostPositionFlowEventType.UnassignedByAdmin =>
                $"Unassigned by admin {name}",
            PostPositionFlowEventType.Closed => "Position closed",
            PostPositionFlowEventType.ClosedAndSplit => "Position closed and split",
            PostPositionFlowEventType.CancellationPending => "Cancellation requested",
            PostPositionFlowEventType.Cancelled => "Position cancelled",
            PostPositionFlowEventType.SplitPending => "Split assignment pending decision",
            PostPositionFlowEventType.SplitAccepted => "Split assignment accepted",
            PostPositionFlowEventType.SplitDeclined => "Split assignment declined",
            PostPositionFlowEventType.Unclosed => "Position unclosed",
            PostPositionFlowEventType.CancellationRevoked => "Cancellation revoked",
            _ => eventType.ToString()
        };
    }

    private static List<JobPositionFlowDetailItemDto> BuildDefaultDetails(
        PostPositionFlowEventType eventType,
        string? employeeName,
        string? performedByName,
        string? relatedEmployeeName,
        string? assignmentType = null) =>
        eventType switch
        {
            PostPositionFlowEventType.SelfTaken =>
            [
                Item("Taken by", employeeName),
                Item("Assignment type", "Self take")
            ],
            PostPositionFlowEventType.AssignedByAdmin =>
            [
                Item("Assigned by", performedByName),
                Item("Assigned to", employeeName),
                Item("Assignment type", string.IsNullOrWhiteSpace(assignmentType) ? "Admin" : assignmentType)
            ],
            PostPositionFlowEventType.AssignedByOther =>
            [
                Item("Assigned by", performedByName),
                Item("Assigned to", employeeName),
                Item("Assignment type", "Assigned by other employee")
            ],
            PostPositionFlowEventType.AssignedBySystem =>
            [
                Item("Assigned by", "System"),
                Item("Assigned to", employeeName),
                Item("Assignment type", "System")
            ],
            PostPositionFlowEventType.Transferred =>
            [
                Item("Transferred by", performedByName),
                Item("From", relatedEmployeeName),
                Item("To", employeeName)
            ],
            PostPositionFlowEventType.UnassignedByAdmin =>
            [
                Item("Unassigned by", performedByName),
                Item("Employee", employeeName)
            ],
            PostPositionFlowEventType.Closed or PostPositionFlowEventType.ClosedAndSplit =>
            [
                Item("Closed by", performedByName),
                Item("Employee", employeeName)
            ],
            PostPositionFlowEventType.SplitPending =>
            [
                Item("Offered to", employeeName),
                Item("Status", "Split decision pending")
            ],
            PostPositionFlowEventType.SplitAccepted =>
            [
                Item("Decision", "Accepted"),
                Item("Employee", employeeName)
            ],
            PostPositionFlowEventType.SplitDeclined =>
            [
                Item("Decision", "Declined"),
                Item("Employee", employeeName)
            ],
            PostPositionFlowEventType.Cancelled or PostPositionFlowEventType.CancellationPending =>
            [
                Item("Cancelled by", performedByName),
                Item("Employee", employeeName)
            ],
            _ => []
        };

    private static List<JobPositionFlowDetailItemDto> DecisionDetails(string decision) =>
        [Item("Decision", decision)];

    private static List<JobPositionFlowDetailItemDto> ReasonDetails(string reason) =>
        [Item("Reason", reason)];

    private static Guid? NormalizeEmployeeId(Guid employeeId) =>
        employeeId == Guid.Empty ? null : employeeId;

    private static JobPositionFlowDetailItemDto Item(string label, string? value) =>
        new() { Label = label, Value = value ?? string.Empty };

    private static Guid? NormalizeGuid(Guid? value) =>
        value is { } g && g != Guid.Empty ? g : null;

    private static string? Truncate(string? value, int max)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        var trimmed = value.Trim();
        return trimmed.Length <= max ? trimmed : trimmed[..max];
    }
}
