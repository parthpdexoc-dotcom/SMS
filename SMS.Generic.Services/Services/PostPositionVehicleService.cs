using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Helpers;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing post position vehicle operations using the generic service pattern.
    /// </summary>
    public class PostPositionVehicleService : GenericService<PostPositionVehicleDto, PostPositionVehicle>, IPostPositionVehicleService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PostPositionVehicleService> _logger;
        private readonly ITokenParser? _tokenParser;
        private readonly IPostPositionVehicleRepository _postPositionVehicleRepository;
        private readonly IPostPositionService _postPositionService;
        private readonly IDepartmentEquipmentService _departmentEquipmentService;
        private readonly IViewDetailsEquipmentRepository _viewDetailsEquipmentRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostPositionVehicleService"/> class.
        /// </summary>
        public PostPositionVehicleService(
            IPostPositionVehicleRepository repository,
            IMapper mapper,
            ILogger<PostPositionVehicleService> logger,
            ITokenParser tokenParser,
            IPostPositionService postPositionService,
            IDepartmentEquipmentService departmentEquipmentService,
            IViewDetailsEquipmentRepository viewDetailsEquipmentRepository)
            : base(repository, mapper, "PostPositionVehicleID", tokenParser)
        {
            _mapper = mapper;
            _logger = logger;
            _tokenParser = tokenParser;
            _postPositionVehicleRepository = repository;
            _postPositionService = postPositionService;
            _departmentEquipmentService = departmentEquipmentService;
            _viewDetailsEquipmentRepository = viewDetailsEquipmentRepository;
        }

        /// <inheritdoc />
        public override async Task<PostPositionVehicleDto> UpsertAsync(PostPositionVehicleDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            await ValidateForeignKeyAsync<IPostPositionService, PostPositionDto>(_postPositionService, dto.PostPositionID, "PostPosition");

            await ValidateForeignKeyAsync<IDepartmentEquipmentService, DepartmentEquipmentDto>(_departmentEquipmentService, dto.DepartmentEquipmentID, "DepartmentEquipment");

            await ValidateCompositeKeyDuplicateAsync(
                dto: dto,
                filterExpression: e => e.PostPositionID == dto.PostPositionID && e.DepartmentEquipmentID == dto.DepartmentEquipmentID,
                errorMessage: ModelValidationErrorCodes.DUPLICATE_POST_POSITION_VEHICLE_ASSIGNMENT);

            return await base.UpsertAsync(dto);
        }

        /// <inheritdoc />
        public async Task CopyAllocationsAsync(Guid sourcePostPositionId, Guid targetPostPositionId)
        {
            if (sourcePostPositionId == Guid.Empty || targetPostPositionId == Guid.Empty)
                return;

            if (sourcePostPositionId == targetPostPositionId)
            {
                if (!await IsAllocationFinalizedAsync(sourcePostPositionId).ConfigureAwait(false))
                    await PersistLiveAllocationsIfMissingAsync(sourcePostPositionId).ConfigureAwait(false);
                return;
            }

            var sourceRows = (await GetAllAsync(
                ppv => !ppv.IsDeleted && ppv.PostPositionID == sourcePostPositionId,
                null,
                null).ConfigureAwait(false)).ToList();

            if (sourceRows.Count == 0
                && !await IsAllocationFinalizedAsync(sourcePostPositionId).ConfigureAwait(false))
            {
                await PersistLiveAllocationsIfMissingAsync(sourcePostPositionId).ConfigureAwait(false);
                sourceRows = (await GetAllAsync(
                    ppv => !ppv.IsDeleted && ppv.PostPositionID == sourcePostPositionId,
                    null,
                    null).ConfigureAwait(false)).ToList();
            }

            if (sourceRows.Count == 0)
            {
                await CopyAllocationFinalizedFlagAsync(sourcePostPositionId, targetPostPositionId).ConfigureAwait(false);
                return;
            }

            var targetHasRows = (await GetAllAsync(
                ppv => !ppv.IsDeleted && ppv.PostPositionID == targetPostPositionId,
                null,
                null).ConfigureAwait(false)).Any();
            if (targetHasRows)
                await ReleaseAllocationsAsync(new[] { targetPostPositionId }).ConfigureAwait(false);

            var createdDate = DateTime.UtcNow;
            var createdUserId = _tokenParser?.GetEmployeeID() ?? Guid.Empty;
            var createdBy = _tokenParser?.GetUsername() ?? string.Empty;

            var clones = sourceRows
                .Where(r => r.DepartmentEquipmentID.HasValue && r.DepartmentEquipmentID.Value != Guid.Empty)
                .Select(r =>
                {
                    var clone = _mapper.Map<PostPositionVehicleDto>(r);
                    clone.PostPositionVehicleID = Guid.NewGuid();
                    clone.PostPositionID = targetPostPositionId;
                    clone.CreatedDate = createdDate;
                    clone.CreatedUserID = createdUserId;
                    clone.CreatedBy = createdBy;
                    clone.EditedDate = null;
                    clone.EditedUserID = null;
                    clone.EditedBy = null;
                    clone.EquipmentQuantity = r.EquipmentQuantity;
                    clone.EquipmentPerPostPosition = r.EquipmentPerPostPosition;
                    clone.IsDeleted = false;
                    return clone;
                })
                .ToList();

            if (clones.Count > 0)
                await BulkUpsertAsync(clones, isSuperUser: true).ConfigureAwait(false);

            await CopyAllocationFinalizedFlagAsync(sourcePostPositionId, targetPostPositionId).ConfigureAwait(false);
        }

        private async Task PersistLiveAllocationsIfMissingAsync(Guid postPositionId)
        {
            var allocated = await GetAllocatedPositionIdsAsync(new[] { postPositionId }).ConfigureAwait(false);
            if (allocated.Count > 0)
                return;

            var (lines, departmentEquipmentById) = await LoadLiveAllocationLinesAsync(
                    postPositionId,
                    applyExistingAllocationCounts: false)
                .ConfigureAwait(false);
            if (lines.Count == 0)
                return;

            var dtos = BuildFinalizedDtos(
                lines,
                departmentEquipmentById,
                postPositionId,
                _tokenParser?.GetEmployeeID() ?? Guid.Empty,
                _tokenParser?.GetUsername() ?? string.Empty,
                DateTime.UtcNow);

            if (dtos.Count > 0)
                await BulkUpsertAsync(dtos, isSuperUser: true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HashSet<Guid>> GetAllocatedPositionIdsAsync(IReadOnlyCollection<Guid> postPositionIds)
        {
            var ids = postPositionIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();
            if (ids.Count == 0)
                return new HashSet<Guid>();

            return (await GetAllAsync(
                    ppv => !ppv.IsDeleted && ids.Contains(ppv.PostPositionID),
                    null,
                    null)
                .ConfigureAwait(false))
                .Select(ppv => ppv.PostPositionID.GetValueOrDefault())
                .Where(id => id != Guid.Empty)
                .ToHashSet();
        }

        /// <inheritdoc />
        public async Task ReleaseAllocationsAsync(IReadOnlyCollection<Guid> postPositionIds)
        {
            var ids = postPositionIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();
            if (ids.Count == 0)
                return;

            await UpdateAsync(
                ppv => !ppv.IsDeleted && ids.Contains(ppv.PostPositionID),
                ppv => ppv.IsDeleted = true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<(string Vehicles, string Equipment)> GetAssignedDisplayNamesAsync(Guid postPositionId)
        {
            if (postPositionId == Guid.Empty)
                return (string.Empty, string.Empty);

            var assigned = await _postPositionVehicleRepository
                .GetAssignedEquipmentTypeNamesAsync(postPositionId)
                .ConfigureAwait(false);

            if (assigned.Count > 0)
                return FormatDisplayNames(assigned);

            return await GetLiveAssignedDisplayNamesAsync(postPositionId).ConfigureAwait(false);
        }

        private static (string Vehicles, string Equipment) FormatDisplayNames(
            IReadOnlyList<(bool IsVehicle, string Name)> rows)
        {
            var vehicles = rows
                .Where(r => r.IsVehicle && !string.IsNullOrWhiteSpace(r.Name))
                .Select(r => r.Name)
                .Distinct(StringComparer.OrdinalIgnoreCase);
            var equipment = rows
                .Where(r => !r.IsVehicle && !string.IsNullOrWhiteSpace(r.Name))
                .Select(r => r.Name)
                .Distinct(StringComparer.OrdinalIgnoreCase);
            return (string.Join(", ", vehicles), string.Join(", ", equipment));
        }

        private async Task<(string Vehicles, string Equipment)> GetLiveAssignedDisplayNamesAsync(Guid postPositionId)
        {
            var (lines, _) = await LoadLiveAllocationLinesAsync(postPositionId).ConfigureAwait(false);
            if (lines.Count == 0)
                return (string.Empty, string.Empty);

            return FormatDisplayNames(
                lines
                    .Where(l => !string.IsNullOrWhiteSpace(l.EquipmentTypeName))
                    .Select(l => (l.IsVehicle, Name: l.EquipmentTypeName!.Trim()))
                    .ToList());
        }

        private async Task<(List<ViewDetailsEquipmentLine> Lines, Dictionary<Guid, DepartmentEquipmentDto> DepartmentEquipmentById)>
            LoadLiveAllocationLinesAsync(Guid postPositionId, bool applyExistingAllocationCounts = true)
        {
            var empty = (new List<ViewDetailsEquipmentLine>(), new Dictionary<Guid, DepartmentEquipmentDto>());

            var position = await _postPositionService
                .GetByIdAsync(postPositionId, p => p.Post!)
                .ConfigureAwait(false);
            if (position?.DateOfWork is not DateOnly dateOfWork
                || position.Post?.RequestID is not Guid requestId
                || requestId == Guid.Empty)
                return empty;

            var loaded = await _viewDetailsEquipmentRepository
                .LoadAsync(requestId, postId: null, dateOfWork, loadRankContext: true)
                .ConfigureAwait(false);

            if (!HasActiveEquipmentConfiguration(loaded, dateOfWork))
                return empty;

            var requestEquipment = _mapper.Map<List<RequestEquipmentDto>>(loaded.RequestEquipment);
            var postEquipment = _mapper.Map<List<PostEquipmentDto>>(loaded.PostEquipment);
            var requestPositions = _mapper.Map<List<PostPositionDto>>(loaded.RequestPositions);
            var hireDateByEmployeeId = EmployeeHighestRankHelper.BuildHireDateMapFromTenureEntities(loaded.EmployeeTenures);
            var lookup = ViewDetailsDepartmentEquipmentLookupBuilder.FromLoadedEquipment(
                loaded.RequestEquipment,
                loaded.PostEquipment);

            IReadOnlyDictionary<(Guid PostId, DateOnly DateOfWork, Guid DepartmentEquipmentId), int>? allocationCounts = null;
            if (applyExistingAllocationCounts)
            {
                var postIds = requestPositions
                    .Select(p => p.PostID)
                    .Append(position.PostID)
                    .Where(id => id != Guid.Empty)
                    .Distinct()
                    .ToList();
                allocationCounts = await _postPositionVehicleRepository
                    .GetAllocationCountsByPostAndDateAsync(postIds, new[] { dateOfWork })
                    .ConfigureAwait(false);
            }

            var byPosition = ViewDetailsEquipmentLines.ResolveEquipmentLinesByPostPositionId(
                requestEquipment,
                postEquipment,
                lookup,
                showVehicle: true,
                showEquipment: true,
                requestPositions,
                hireDateByEmployeeId,
                dateOfWork,
                loaded.PostDepartmentEquipmentIdsByPostId,
                loaded.EmployeeTenures,
                allocationCounts: allocationCounts);

            if (!byPosition.TryGetValue(postPositionId, out var lines) || lines.Count == 0)
                return empty;

            var departmentEquipmentById = new Dictionary<Guid, DepartmentEquipmentDto>();
            MergeDepartmentEquipmentFromLoaded(departmentEquipmentById, loaded);
            return (lines, departmentEquipmentById);
        }

        private static List<ViewDetailsEquipmentLine> FilterLinesByRemainingQuantity(
            IReadOnlyList<ViewDetailsEquipmentLine> lines,
            Guid postId,
            DateOnly dateOfWork,
            IReadOnlyList<RequestEquipmentDto> requestEquipment,
            IReadOnlyList<PostEquipmentDto> postEquipment,
            Dictionary<(Guid PostId, DateOnly DateOfWork, Guid DepartmentEquipmentId), int> allocationCounts)
        {
            var eligibleLines = new List<ViewDetailsEquipmentLine>(lines.Count);
            foreach (var line in lines)
            {
                var departmentEquipmentId = line.DepartmentEquipmentId.GetValueOrDefault();
                if (departmentEquipmentId == Guid.Empty)
                    continue;

                if (!IsPerJobLine(line))
                {
                    eligibleLines.Add(line);
                    continue;
                }

                var configuredQuantity = GetConfiguredQuantity(line, requestEquipment, postEquipment);
                if (configuredQuantity <= 0)
                    continue;

                var key = (postId, dateOfWork, departmentEquipmentId);
                allocationCounts.TryGetValue(key, out var usedCount);
                if (usedCount >= configuredQuantity)
                    continue;

                allocationCounts[key] = usedCount + 1;
                eligibleLines.Add(line);
            }

            return eligibleLines;
        }

        private static bool IsPerJobLine(ViewDetailsEquipmentLine line) =>
            line.PostEquipmentId.HasValue
                ? !line.EquipmentPerPostPosition
                : line.EquipmentPerPost;

        private static int GetConfiguredQuantity(
            ViewDetailsEquipmentLine line,
            IReadOnlyList<RequestEquipmentDto> requestEquipment,
            IReadOnlyList<PostEquipmentDto> postEquipment)
        {
            if (line.PostEquipmentId is Guid postEquipmentId && postEquipmentId != Guid.Empty)
            {
                var postLine = postEquipment.FirstOrDefault(item => item.PostEquipmentID == postEquipmentId);
                return postLine?.EquipmentQuantity ?? 0;
            }

            if (line.RequestEquipmentId is Guid requestEquipmentId && requestEquipmentId != Guid.Empty)
            {
                var requestLine = requestEquipment.FirstOrDefault(item => item.RequestEquipmentID == requestEquipmentId);
                return requestLine?.EquipmentQuantity ?? 0;
            }

            return line.EquipmentQuantity;
        }

        /// <inheritdoc />
        public async Task<int> FinalizeStartedAllocationsForDepartmentAsync(
            Guid departmentId,
            DateTime departmentNow,
            Guid masterEmployeeId,
            string masterEmployeeDisplayName,
            CancellationToken cancellationToken = default)
        {
            var targetStatuses = new[]
            {
                PostPositionStatus.Assigned,
                PostPositionStatus.Unclosed,
                PostPositionStatus.SplitDecisionPending,
                PostPositionStatus.Closed,
                PostPositionStatus.ClosedAndSplit
            };

            var eligibleStarted = await _postPositionService
                .GetPostPositionsPastScheduledStartAsync(
                    departmentId,
                    departmentNow,
                    statuses: targetStatuses,
                    excludePlaceholders: true,
                    requireEmployee: true,
                    requireRequestId: true,
                    onlyUnfinalizedAllocations: true,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (eligibleStarted.Count == 0)
                return 0;

            var postIds = eligibleStarted.Select(p => p.PostID).Distinct().ToList();
            var workDates = eligibleStarted.Select(p => p.DateOfWork!.Value).Distinct().ToList();
            var postScopeByPostPositionId = eligibleStarted.ToDictionary(
                p => p.PostPositionID!.Value,
                p => (p.PostID, p.DateOfWork!.Value));

            var existingAllocations = (await GetAllAsync(
                ppv => !ppv.IsDeleted
                       && ppv.PostPosition != null
                       && ppv.PostPosition.DateOfWork.HasValue
                       && postIds.Contains(ppv.PostPosition.PostID)
                       && workDates.Contains(ppv.PostPosition.DateOfWork.Value),
                null,
                null).ConfigureAwait(false)).ToList();

            var postPositionIdsWithRecords = existingAllocations
                .Select(allocation => allocation.PostPositionID.GetValueOrDefault())
                .Where(id => id != Guid.Empty)
                .ToHashSet();

            var postDatesWithAllocations = existingAllocations
                .Select(allocation => allocation.PostPositionID.GetValueOrDefault())
                .Where(id => id != Guid.Empty && postScopeByPostPositionId.ContainsKey(id))
                .Select(id => postScopeByPostPositionId[id])
                .ToHashSet();

            var allocationCounts = (await _postPositionVehicleRepository
                .GetAllocationCountsByPostAndDateAsync(postIds, workDates, cancellationToken)
                .ConfigureAwait(false))
                .ToDictionary(entry => entry.Key, entry => entry.Value);

            var requestWorkGroups = eligibleStarted
                .GroupBy(p => (RequestId: p.Post!.RequestID!.Value, DateOfWork: p.DateOfWork!.Value))
                .ToList();

            var departmentEquipmentById = new Dictionary<Guid, DepartmentEquipmentDto>();
            var pendingDtos = new List<PostPositionVehicleDto>();
            var siblingFinalizedCount = 0;
            var createdDate = DateTime.UtcNow;

            foreach (var group in requestWorkGroups)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var loaded = await _viewDetailsEquipmentRepository
                    .LoadAsync(group.Key.RequestId, postId: null, group.Key.DateOfWork, loadRankContext: true, cancellationToken)
                    .ConfigureAwait(false);

                if (!HasActiveEquipmentConfiguration(loaded, group.Key.DateOfWork))
                    continue;

                MergeDepartmentEquipmentFromLoaded(departmentEquipmentById, loaded);

                var requestEquipment = _mapper.Map<List<RequestEquipmentDto>>(loaded.RequestEquipment);
                var postEquipment = _mapper.Map<List<PostEquipmentDto>>(loaded.PostEquipment);
                var requestPositions = _mapper.Map<List<PostPositionDto>>(loaded.RequestPositions);
                var hireDateByEmployeeId = EmployeeHighestRankHelper.BuildHireDateMapFromTenureEntities(loaded.EmployeeTenures);
                var lookup = ViewDetailsDepartmentEquipmentLookupBuilder.FromLoadedEquipment(
                    loaded.RequestEquipment,
                    loaded.PostEquipment);

                var allocationsByPostPositionId = ViewDetailsEquipmentLines.ResolveEquipmentLinesByPostPositionId(
                    requestEquipment,
                    postEquipment,
                    lookup,
                    showVehicle: true,
                    showEquipment: true,
                    requestPositions,
                    hireDateByEmployeeId,
                    group.Key.DateOfWork,
                    loaded.PostDepartmentEquipmentIdsByPostId,
                    loaded.EmployeeTenures,
                    allocationCounts: allocationCounts);

                foreach (var position in group.Where(p => !postPositionIdsWithRecords.Contains(p.PostPositionID!.Value)))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var postPositionId = position.PostPositionID!.Value;
                    if (!allocationsByPostPositionId.TryGetValue(postPositionId, out var lines) || lines.Count == 0)
                        continue;

                    var eligibleLines = FilterLinesByRemainingQuantity(
                        lines,
                        position.PostID,
                        group.Key.DateOfWork,
                        requestEquipment,
                        postEquipment,
                        allocationCounts);

                    if (eligibleLines.Count == 0)
                        continue;

                    var dtos = BuildFinalizedDtos(
                        eligibleLines,
                        departmentEquipmentById,
                        postPositionId,
                        masterEmployeeId,
                        masterEmployeeDisplayName,
                        createdDate);

                    if (dtos.Count == 0)
                        continue;

                    pendingDtos.AddRange(dtos);
                    postPositionIdsWithRecords.Add(postPositionId);
                    postDatesWithAllocations.Add((position.PostID, group.Key.DateOfWork));
                }

                siblingFinalizedCount += group.Count(position =>
                {
                    var postPositionId = position.PostPositionID!.Value;
                    if (postPositionIdsWithRecords.Contains(postPositionId))
                        return false;

                    if (allocationsByPostPositionId.TryGetValue(postPositionId, out var lines) && lines.Count > 0)
                        return false;

                    if (!postDatesWithAllocations.Contains((position.PostID, group.Key.DateOfWork)))
                        return false;

                    postPositionIdsWithRecords.Add(postPositionId);
                    return true;
                });
            }

            _logger.LogInformation("Found {Count} eligible started post positions for department {DepartmentId} as of {DepartmentNow}", eligibleStarted.Count, departmentId, departmentNow);

            var insertFinalizedCount = 0;
            if (pendingDtos.Count > 0)
            {
                var dtosToInsert = await ExcludeAlreadyFinalizedAsync(pendingDtos, cancellationToken).ConfigureAwait(false);
                insertFinalizedCount = dtosToInsert
                    .Select(dto => dto.PostPositionID!.Value)
                    .Distinct()
                    .Count();

                if (dtosToInsert.Count > 0)
                {
                    _logger.LogInformation("Inserting {Count} PostPositionVehicle records for {PositionCount} post positions.", dtosToInsert.Count, insertFinalizedCount);
                    await BulkUpsertAsync(dtosToInsert, isSuperUser: true).ConfigureAwait(false);
                }
            }

            _logger.LogInformation("Finalized allocations complete. Total finalized positions: {Total}", insertFinalizedCount + siblingFinalizedCount);

            await MarkAllocationsFinalizedAsync(
                    eligibleStarted
                        .Select(position => position.PostPositionID.GetValueOrDefault())
                        .Where(id => id != Guid.Empty)
                        .Distinct()
                        .ToList())
                .ConfigureAwait(false);

            return insertFinalizedCount + siblingFinalizedCount;
        }

        private async Task MarkAllocationsFinalizedAsync(IReadOnlyCollection<Guid> postPositionIds)
        {
            var ids = postPositionIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();
            if (ids.Count == 0)
                return;

            await _postPositionService.UpdateAsync(
                position => ids.Contains(position.PostPositionID) && !position.IsAllocationFinalized,
                position => position.IsAllocationFinalized = true).ConfigureAwait(false);
        }

        private async Task CopyAllocationFinalizedFlagAsync(Guid sourcePostPositionId, Guid targetPostPositionId)
        {
            if (sourcePostPositionId == Guid.Empty
                || targetPostPositionId == Guid.Empty
                || sourcePostPositionId == targetPostPositionId)
                return;

            if (!await IsAllocationFinalizedAsync(sourcePostPositionId).ConfigureAwait(false))
                return;

            await _postPositionService.UpdateAsync(
                position => position.PostPositionID == targetPostPositionId && !position.IsAllocationFinalized,
                position => position.IsAllocationFinalized = true).ConfigureAwait(false);
        }

        private async Task<bool> IsAllocationFinalizedAsync(Guid postPositionId)
        {
            if (postPositionId == Guid.Empty)
                return false;

            var position = await _postPositionService.GetByIdAsync(postPositionId).ConfigureAwait(false);
            return position?.IsAllocationFinalized == true;
        }

        private async Task<List<PostPositionVehicleDto>> ExcludeAlreadyFinalizedAsync(
            IReadOnlyList<PostPositionVehicleDto> pendingDtos,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var postPositionIds = pendingDtos
                .Select(dto => dto.PostPositionID!.Value)
                .Distinct()
                .ToList();

            var alreadyFinalizedPostPositionIds = (await GetAllAsync(
                ppv => !ppv.IsDeleted
                       && postPositionIds.Contains(ppv.PostPositionID),
                null,
                null).ConfigureAwait(false))
                .Select(ppv => ppv.PostPositionID)
                .ToHashSet();

            if (alreadyFinalizedPostPositionIds.Count == 0)
                return pendingDtos.ToList();

            return pendingDtos
                .Where(dto => !alreadyFinalizedPostPositionIds.Contains(dto.PostPositionID!.Value))
                .ToList();
        }

        private void MergeDepartmentEquipmentFromLoaded(
            IDictionary<Guid, DepartmentEquipmentDto> cache,
            ViewDetailsEquipmentQueryResult loaded)
        {
            var departmentEquipment = loaded.PostEquipment
                .Select(pe => pe.DepartmentEquipment)
                .Where(de => de != null && de.DepartmentEquipmentID != Guid.Empty);

            foreach (var equipment in departmentEquipment)
            {
                cache.TryAdd(equipment!.DepartmentEquipmentID, _mapper.Map<DepartmentEquipmentDto>(equipment));
            }
        }

        private static bool HasActiveEquipmentConfiguration(
            ViewDetailsEquipmentQueryResult loaded,
            DateOnly dateOfWork)
        {
            return loaded.PostEquipment.Any(pe =>
                ViewDetailsEquipmentLines.IsEquipmentActiveOnDate(pe.StartDate, pe.EndDate, dateOfWork));
        }

        private List<PostPositionVehicleDto> BuildFinalizedDtos(
            IReadOnlyList<ViewDetailsEquipmentLine> lines,
            IReadOnlyDictionary<Guid, DepartmentEquipmentDto> departmentEquipmentById,
            Guid postPositionId,
            Guid masterEmployeeId,
            string masterEmployeeDisplayName,
            DateTime createdDate)
        {
            var lineByDeptEquipId = lines
                .Where(l => l.DepartmentEquipmentId.HasValue && l.DepartmentEquipmentId.Value != Guid.Empty)
                .GroupBy(l => l.DepartmentEquipmentId!.Value)
                .ToDictionary(g => g.Key, g => g.First());

            return lines
                .Select(line => line.DepartmentEquipmentId.GetValueOrDefault())
                .Where(departmentEquipmentId => departmentEquipmentId != Guid.Empty)
                .Distinct()
                .Select(departmentEquipmentId =>
                {
                    if (!departmentEquipmentById.TryGetValue(departmentEquipmentId, out var departmentEquipment))
                    {
                        _logger.LogWarning(
                            "Skipping PostPositionVehicle insert: DepartmentEquipment {DepartmentEquipmentId} not found for PostPosition {PostPositionId}.",
                            departmentEquipmentId,
                            postPositionId);
                        return null;
                    }

                    var isVehicle = departmentEquipment.EquipmentType?.IsVehicle == true;
                    int? qty = null;
                    var equipmentPerPostPosition = false;
                    if (lineByDeptEquipId.TryGetValue(departmentEquipmentId, out var line))
                    {
                        qty = line.IsVehicle ? 1 : (line.EquipmentQuantity > 0 ? line.EquipmentQuantity : 1);
                        isVehicle = line.IsVehicle;
                        // Post lines: EquipmentPerPostPosition as-is. Request lines: EquipmentPerPost true = Per Job → false.
                        equipmentPerPostPosition = line.PostEquipmentId.HasValue
                            ? line.EquipmentPerPostPosition
                            : !line.EquipmentPerPost;
                    }

                    return CreateFinalizedDto(
                        departmentEquipment,
                        postPositionId,
                        departmentEquipmentId,
                        masterEmployeeId,
                        masterEmployeeDisplayName,
                        createdDate,
                        qty,
                        isVehicle,
                        equipmentPerPostPosition);
                })
                .Where(dto => dto != null)
                .Cast<PostPositionVehicleDto>()
                .ToList();
        }

        private PostPositionVehicleDto CreateFinalizedDto(
            DepartmentEquipmentDto departmentEquipment,
            Guid postPositionId,
            Guid departmentEquipmentId,
            Guid masterEmployeeId,
            string masterEmployeeDisplayName,
            DateTime createdDate,
            int? equipmentQuantity = null,
            bool isVehicle = false,
            bool equipmentPerPostPosition = false)
        {
            var entity = _mapper.Map<PostPositionVehicle>(departmentEquipment);
            entity.PostPositionVehicleID = Guid.NewGuid();
            entity.PostPositionID = postPositionId;
            entity.DepartmentEquipmentID = departmentEquipmentId;
            entity.EquipmentQuantity = isVehicle ? 1 : (equipmentQuantity.HasValue && equipmentQuantity.Value > 0 ? equipmentQuantity.Value : 1);
            entity.EquipmentPerPostPosition = equipmentPerPostPosition;
            entity.CreatedDate = createdDate;
            entity.CreatedUserID = masterEmployeeId;
            entity.CreatedBy = masterEmployeeDisplayName;
            entity.IsDeleted = false;
            var dto = _mapper.Map<PostPositionVehicleDto>(entity);
            dto.EquipmentQuantity = entity.EquipmentQuantity;
            dto.EquipmentPerPostPosition = entity.EquipmentPerPostPosition;
            return dto;
        }
    }
}
