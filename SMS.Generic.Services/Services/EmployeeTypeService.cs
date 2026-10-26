using AutoMapper;
using SMS.Generic.Common;
using SMS.Generic.Common.Constants;
using SMS.Generic.Common.Enums;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Interfaces;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Net;
using SMS.Generic.Common.Implementations.Paginations;
using System.Linq.Expressions;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing employee type operations using the generic service pattern.
    /// </summary>
    public class EmployeeTypeService : GenericService<EmployeeTypeDto, EmployeeType>, IEmployeeTypeService
    {
        private readonly IEmployeeTypeRepository _repository;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeTypeService"/> class.
        /// </summary>
        /// <param name="repository">The employee type repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="tokenParser">The token parser for accessing current user information for audit tracking.</param>
        public EmployeeTypeService(
            IEmployeeTypeRepository repository,
            IMapper mapper,
            ILogger<EmployeeTypeService> logger,
            ITokenParser tokenParser) : base(repository, mapper, "EmployeeTypeID", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _tokenParser = tokenParser;
        }

        /// <summary>
        /// Upserts an employee type. On update, if the client sends <see cref="EmployeeTypeDto.SameRankIds"/> (including empty array),
        /// saves it on the parent and in one transaction sets every listed type's <see cref="EmployeeTypeDto.Rank"/> to the parent's rank.
        /// Types no longer listed keep their rank. If <see cref="EmployeeTypeDto.SameRankIds"/> is omitted on update, the stored list is preserved and peer ranks are not changed.
        /// </summary>
        public override async Task<EmployeeTypeDto> UpsertAsync(EmployeeTypeDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            var isNew = dto.EmployeeTypeID == Guid.Empty || await GetByIdAsync(dto.EmployeeTypeID) == null;
            if (isNew)
            {
                var activeTypes = await GetActiveEmployeeTypesAsync(true);
                var maxRank = activeTypes.Where(x => x.Rank > 0).Select(x => x.Rank).DefaultIfEmpty(0).Max();
                dto.Rank = maxRank + 1;
            }

            if (!isNew && dto.SameRankIds == null)
            {
                var existingForMerge = await GetByIdAsync(dto.EmployeeTypeID).ConfigureAwait(false);
                if (existingForMerge?.SameRankIds != null)
                    dto.SameRankIds = existingForMerge.SameRankIds.ToList();
            }

            // Use dto.SameRankIds after merge so omitted JSON still runs symmetric same-rank sync when DB has links.
            if (!isNew && dto.SameRankIds != null)
            {
                if (dto is IDepartmentScoped scoped && scoped.DepartmentID == Guid.Empty)
                {
                    var deptId = _tokenParser?.GetDepartmentID();
                    if (!deptId.HasValue)
                        throw new UnauthorizedAccessException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_DEPARTMENT_ID_MISSING_FROM_TOKEN));
                    scoped.DepartmentID = deptId.Value;
                }

                if (!isSuperUser)
                    await ApplyEditedAuditFieldsForEmployeeTypeAsync(dto).ConfigureAwait(false);

                // Parent rank drives SameRankIds sync. JSON often omits rank (defaults to 0) — do not wipe DB rank.
                var existingParent = await GetByIdAsync(dto.EmployeeTypeID).ConfigureAwait(false);
                if (existingParent != null && dto.Rank == 0 && existingParent.Rank != 0)
                    dto.Rank = existingParent.Rank;

                var peerIds = dto.SameRankIds.Distinct().Where(id => id != dto.EmployeeTypeID).ToList();
                await ValidateSameRankPeersSameDepartmentAsync(dto.DepartmentID, peerIds).ConfigureAwait(false);

                var entity = _mapper.Map<EmployeeType>(dto);
                await _repository.UpsertWithSameRankPeersAsync(entity, peerIds).ConfigureAwait(false);

                await RenormalizeEmployeeTypeRanksForDepartmentAsync(dto.DepartmentID).ConfigureAwait(false);
                var saved = await GetByIdAsync(dto.EmployeeTypeID).ConfigureAwait(false);
                return saved ?? _mapper.Map<EmployeeTypeDto>(entity);
            }

            var upserted = await base.UpsertAsync(dto, isSuperUser).ConfigureAwait(false);
            if (upserted.DepartmentID != Guid.Empty)
                await RenormalizeEmployeeTypeRanksForDepartmentAsync(upserted.DepartmentID).ConfigureAwait(false);
            return (await GetByIdAsync(upserted.EmployeeTypeID).ConfigureAwait(false)) ?? upserted;
        }

        /// <summary>
        /// Active employee types in the department with <see cref="EmployeeTypeDto.Rank"/> &gt; 0 are ordered by current rank (same-rank clusters via <see cref="EmployeeTypeDto.SameRankIds"/>),
        /// then reassigned ranks 1, 2, … so there are no gaps; each cluster keeps one shared rank.
        /// Types with rank 0 are left unchanged.
        /// </summary>
        private async Task RenormalizeEmployeeTypeRanksForDepartmentAsync(Guid departmentId)
        {
            if (departmentId == Guid.Empty)
                return;

            var list = await GetAllAsync(
                filter: x => x.EmployeeTypeActive == true && x.DepartmentID == departmentId && x.Rank > 0,
                includes: null,
                pageParam: null).ConfigureAwait(false);
            var ranked = list.ToList();
            if (ranked.Count == 0)
                return;

            var slots = GetOrderedSameRankSlots(ranked);
            for (var i = 0; i < slots.Count; i++)
            {
                var rank = i + 1;
                foreach (var id in slots[i])
                {
                    await UpdateAsync(
                        et => et.EmployeeTypeID == id,
                        et => et.Rank = rank).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Same as <see cref="UpsertAsync"/>; when <paramref name="includeAffectedSameRankTypes"/> is true and same-rank sync ran, returns refreshed DTOs for linked types.
        /// </summary>
        public async Task<EmployeeTypeUpsertResultDto> UpsertEmployeeTypeWithResultAsync(EmployeeTypeDto dto, bool includeAffectedSameRankTypes, bool isSuperUser = false)
        {
            var hadClientSameRankIds = dto.SameRankIds != null;
            var isNewBefore = dto.EmployeeTypeID == Guid.Empty || await GetByIdAsync(dto.EmployeeTypeID).ConfigureAwait(false) == null;

            var parent = await UpsertAsync(dto, isSuperUser).ConfigureAwait(false);

            IReadOnlyList<EmployeeTypeDto>? affected = null;
            if (includeAffectedSameRankTypes && hadClientSameRankIds && !isNewBefore)
            {
                var peerIds = (parent.SameRankIds ?? new List<Guid>()).Distinct().Where(id => id != parent.EmployeeTypeID).ToList();
                var list = new List<EmployeeTypeDto>();
                foreach (var id in peerIds)
                {
                    var p = await GetByIdAsync(id).ConfigureAwait(false);
                    if (p != null)
                        list.Add(p);
                }
                affected = list;
            }

            return new EmployeeTypeUpsertResultDto { EmployeeType = parent, AffectedSameRankTypes = affected };
        }

        private async Task ApplyEditedAuditFieldsForEmployeeTypeAsync(EmployeeTypeDto dto)
        {
            if (_tokenParser == null)
                return;

            var userTypeString = _tokenParser.GetClaimValue(ClaimNames.UserType);
            Guid? currentUserId = Enum.TryParse<UserType>(userTypeString, ignoreCase: true, out var userType) && userType == UserType.Admin
                ? _tokenParser.GetAdminID()
                : _tokenParser.GetEmployeeID();

            if (!currentUserId.HasValue)
                return;

            var fullName = PersonNameFormatHelper.FormatLastNameCommaFirstName(
                _tokenParser.GetClaimValue(ClaimNames.LastName),
                _tokenParser.GetClaimValue(ClaimNames.FirstName));

            var existing = await GetByIdAsync(dto.EmployeeTypeID).ConfigureAwait(false);
            if (existing != null)
            {
                dto.CreatedDate = existing.CreatedDate;
                dto.CreatedUserID = existing.CreatedUserID;
                dto.CreatedBy = existing.CreatedBy;
            }

            dto.EditedDate = DateTime.UtcNow;
            dto.EditedUserID = currentUserId.Value;
            dto.EditedBy = fullName;
        }

        private async Task ValidateSameRankPeersSameDepartmentAsync(Guid departmentId, IReadOnlyList<Guid> peerIds)
        {
            foreach (var id in peerIds)
            {
                var peer = await GetByIdAsync(id).ConfigureAwait(false);
                if (peer == null)
                {
                    throw new WebApiException(
                        HttpStatusCode.BadRequest,
                        ((int)HttpStatusCode.BadRequest).ToString(),
                        $"Employee type '{id}' was not found.");
                }

                if (peer.DepartmentID != departmentId)
                {
                    throw new WebApiException(
                        HttpStatusCode.BadRequest,
                        ((int)HttpStatusCode.BadRequest).ToString(),
                        $"Employee type '{id}' must belong to the same department as the type being updated.");
                }
            }
        }
        /// <summary>
        /// Moves the target type—and every other active type in the same rank group (same numeric rank, linked via <see cref="EmployeeTypeDto.SameRankIds"/> in either direction)—to slot <paramref name="newRank"/>.
        /// All types in that group get the same new rank; other types are renumbered by slot so existing same-rank groups (e.g. E and F both at 5) stay tied and become the next slot (e.g. both 3).
        /// </summary>
        public async Task UpdateRankAsync(Guid employeeTypeId, int newRank)
        {
            if (newRank <= 0)
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    "Rank must be a positive number greater than zero.");

            var activeEmployeeTypes = await GetActiveEmployeeTypesAsync(true);
            var targetEmployeeType = activeEmployeeTypes.FirstOrDefault(et => et.EmployeeTypeID == employeeTypeId);
            if (targetEmployeeType == null)
                throw new WebApiException(
                    HttpStatusCode.NotFound,
                    ((int)HttpStatusCode.NotFound).ToString(),
                    $"Employee type with ID {employeeTypeId} not found or is not active.");

            // Partition all active types into same-rank clusters (slots). Order slots by current rank.
            var orderedSlots = GetOrderedSameRankSlots(activeEmployeeTypes);
            var slotCount = orderedSlots.Count;

            var targetSlotIndex = orderedSlots.FindIndex(slot => slot.Contains(employeeTypeId));
            if (targetSlotIndex < 0)
                throw new WebApiException(
                    HttpStatusCode.NotFound,
                    ((int)HttpStatusCode.NotFound).ToString(),
                    $"Employee type with ID {employeeTypeId} not found in active list.");

            if (newRank < 1 || newRank > slotCount)
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    $"Invalid slot {newRank}. Use a value from 1 to {slotCount}.");

            // Remove target slot and insert at newRank (1-based) => index newRank - 1
            var targetSlot = orderedSlots[targetSlotIndex];
            orderedSlots.RemoveAt(targetSlotIndex);
            orderedSlots.Insert(newRank - 1, targetSlot);

            // Assign rank = slot index + 1 to every type in that slot (so same-rank groups stay tied)
            var rankById = new Dictionary<Guid, int>();
            for (var i = 0; i < orderedSlots.Count; i++)
            {
                var rank = i + 1;
                foreach (var id in orderedSlots[i])
                    rankById[id] = rank;
            }

            foreach (var kv in rankById)
            {
                await UpdateAsync(
                    et => et.EmployeeTypeID == kv.Key,
                    et => et.Rank = kv.Value).ConfigureAwait(false);
            }

            foreach (var deptId in rankById.Keys
                         .Select(id => activeEmployeeTypes.FirstOrDefault(e => e.EmployeeTypeID == id)?.DepartmentID ?? Guid.Empty)
                         .Where(d => d != Guid.Empty)
                         .Distinct())
            {
                await RenormalizeEmployeeTypeRanksForDepartmentAsync(deptId).ConfigureAwait(false);
            }
        }

        /// <inheritdoc cref="IEmployeeTypeService.UpdateRankAtDisplayRowAsync" />
        public async Task UpdateRankAtDisplayRowAsync(Guid employeeTypeId, int displayRow)
        {
            var activeEmployeeTypes = await GetActiveEmployeeTypesAsync(true);
            var target = activeEmployeeTypes.FirstOrDefault(et => et.EmployeeTypeID == employeeTypeId);
            if (target == null)
                throw new WebApiException(
                    HttpStatusCode.NotFound,
                    ((int)HttpStatusCode.NotFound).ToString(),
                    $"Employee type with ID {employeeTypeId} not found or is not active.");

            var clusterIds = GetSameRankClusterEmployeeTypeIds(target, activeEmployeeTypes);
            var withoutCluster = OrderEmployeeTypesForDisplay(activeEmployeeTypes)
                .Where(et => !clusterIds.Contains(et.EmployeeTypeID))
                .ToList();

            var row = Math.Min(displayRow, withoutCluster.Count + 1);
            var prefix = withoutCluster.Take(row - 1).ToList();
            var targetSlot = 1 + CountDistinctRankSlotsInDisplayOrder(prefix, activeEmployeeTypes);

            await UpdateRankAsync(employeeTypeId, targetSlot).ConfigureAwait(false);
        }

        private static IEnumerable<EmployeeTypeDto> OrderEmployeeTypesForDisplay(IEnumerable<EmployeeTypeDto> all)
        {
            return all
                .OrderBy(et => et.Rank == 0 ? int.MaxValue : et.Rank)
                .ThenBy(et => et.EmployeeTypeShort ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(et => et.EmployeeTypeLong ?? string.Empty, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Counts how many same-rank slots appear in <paramref name="sequence"/> (in display order), using full <paramref name="all"/> for cluster links.
        /// </summary>
        private static int CountDistinctRankSlotsInDisplayOrder(IReadOnlyList<EmployeeTypeDto> sequence, List<EmployeeTypeDto> all)
        {
            var alreadyCountedClusterMember = new HashSet<Guid>();
            var count = 0;
            foreach (var et in sequence)
            {
                if (alreadyCountedClusterMember.Contains(et.EmployeeTypeID))
                    continue;
                var cluster = GetSameRankClusterEmployeeTypeIds(et, all);
                count++;
                foreach (var id in cluster)
                    alreadyCountedClusterMember.Add(id);
            }

            return count;
        }

        /// <summary>
        /// Partitions active types into same-rank clusters (each cluster = same rank + linked via SameRankIds). Returns clusters ordered by current rank (0 last).
        /// </summary>
        private static List<HashSet<Guid>> GetOrderedSameRankSlots(List<EmployeeTypeDto> all)
        {
            var assigned = new HashSet<Guid>();
            var slots = new List<HashSet<Guid>>();
            var ordered = OrderEmployeeTypesForDisplay(all).ToList();

            foreach (var et in ordered)
            {
                if (assigned.Contains(et.EmployeeTypeID))
                    continue;
                var cluster = GetSameRankClusterEmployeeTypeIds(et, all);
                foreach (var id in cluster)
                    assigned.Add(id);
                slots.Add(cluster);
            }

            return slots;
        }

        /// <summary>
        /// Types that share the target's rank and are connected via <see cref="EmployeeTypeDto.SameRankIds"/> (undirected: A lists B or B lists A). Rank 0: only the target unless linked to other rank-0 peers.
        /// </summary>
        private static HashSet<Guid> GetSameRankClusterEmployeeTypeIds(EmployeeTypeDto target, List<EmployeeTypeDto> all)
        {
            var targetId = target.EmployeeTypeID;
            var r = target.Rank;
            var byId = all.ToDictionary(x => x.EmployeeTypeID, x => x);
            var cluster = new HashSet<Guid> { targetId };
            var queue = new Queue<Guid>();
            queue.Enqueue(targetId);

            while (queue.Count > 0)
            {
                var id = queue.Dequeue();
                if (!byId.TryGetValue(id, out var et) || et.Rank != r)
                    continue;

                foreach (var peerId in et.SameRankIds ?? Enumerable.Empty<Guid>())
                {
                    if (!byId.TryGetValue(peerId, out var peer) || peer.Rank != r)
                        continue;
                    if (cluster.Add(peerId))
                        queue.Enqueue(peerId);
                }

                foreach (var other in all)
                {
                    if (other.EmployeeTypeID == id || other.Rank != r)
                        continue;
                    if (other.SameRankIds != null && other.SameRankIds.Contains(id) && cluster.Add(other.EmployeeTypeID))
                        queue.Enqueue(other.EmployeeTypeID);
                }
            }

            return cluster;
        }
        /// <summary>
        /// Gets all active employee types.
        /// </summary>
        /// <returns>A list of active EmployeeTypeDto.</returns>
        public async Task<List<EmployeeTypeDto>> GetActiveEmployeeTypesAsync()
        {
            var employeeTypes = await GetAllAsync(
                filter: x => x.EmployeeTypeActive == true );

            return employeeTypes.ToList();
        }

        /// <summary>
        /// Validates if an EmployeeTypeID exists in the EmployeeType table and is active.
        /// </summary>
        /// <param name="employeeTypeID">The EmployeeType ID to validate.</param>
        /// <exception cref="WebApiException">Thrown when EmployeeType does not exist or is not active.</exception>
        public async Task ValidateEmployeeTypeExistsAsync(Guid employeeTypeID)
        {
            var activeEmployeeTypes = await GetActiveEmployeeTypesAsync(true);
            var employeeType = activeEmployeeTypes.FirstOrDefault(x => x.EmployeeTypeID == employeeTypeID);

            if (employeeType == null)
            {
                throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.EMPLOYEE_TYPE_NOT_AVAILABLE));
            }
        }

        /// <summary>
        /// Validates if an employee can be promoted from current EmployeeType to a new EmployeeType based on rank.
        /// </summary>
        /// <param name="currentEmployeeTypeID">The current EmployeeType ID (nullable).</param>
        /// <param name="newEmployeeTypeID">The new EmployeeType ID to validate.</param>
        /// <exception cref="WebApiException">Thrown when validation fails.</exception>
        public async Task ValidateRankPromotionAsync(Guid? currentEmployeeTypeID, Guid newEmployeeTypeID)
        {
            // Validate that the new EmployeeType exists and is active
            await ValidateEmployeeTypeExistsAsync(newEmployeeTypeID);

            // Get all active EmployeeTypes for rank comparison
            var allActiveEmployeeTypes = await GetActiveEmployeeTypesAsync(true);

            // Get new EmployeeType
            var newEmployeeType = allActiveEmployeeTypes.FirstOrDefault(x => x.EmployeeTypeID == newEmployeeTypeID);

            // If there's a current EmployeeType, validate rank comparison
            if (currentEmployeeTypeID.HasValue)
            {
                var currentEmployeeType = allActiveEmployeeTypes.FirstOrDefault(x => x.EmployeeTypeID == currentEmployeeTypeID.Value);

                // Validate rank comparison - new rank must be higher than current rank (only if EmployeeType is active)
                if (currentEmployeeType != null && newEmployeeType != null)
                {
                    if (currentEmployeeType.Rank != 0 && newEmployeeType.Rank != 0)
                    {
                        if (newEmployeeType.Rank > currentEmployeeType.Rank)
                        {
                            throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(),
                                ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.EMPLOYEE_CAN_ONLY_BE_PROMOTED_TO_HIGHER_RANK));
                        }
                    }
                }
            }
        }
        public async Task<EmployeeTypeDto?> GetByNameAsync(string name,Guid departmentId)
        {
            var trimmed = name?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                return null;

            var lower = trimmed.ToLowerInvariant();
            var list = await GetAllAsync(
                x => ((x.EmployeeTypeShort != null && x.EmployeeTypeShort.ToLower() == lower)
                     || (x.EmployeeTypeLong != null && x.EmployeeTypeLong.ToLower() == lower)) && x.DepartmentID == departmentId ,
                null,
                null).ConfigureAwait(false);
            return list.FirstOrDefault();
        }
        public async Task<List<EmployeeTypeDto>> GetActiveEmployeeTypesAsync(bool isactive)
        {
            var activeEmployeeTypes = await GetAllAsync(
                     filter: x => x.EmployeeTypeActive == true,
                     includes: null,
                     pageParam: null);
            return activeEmployeeTypes.ToList();

        }

        /// <inheritdoc />
        public async Task<Guid> UpsertMasterTypeAsync(Guid departmentId)
        {
            const string masterTypeShort = "MasterType";
            var existing = await GetByNameAsync(masterTypeShort,departmentId).ConfigureAwait(false);
            if (existing != null)
                return existing.EmployeeTypeID;

            var dto = new EmployeeTypeDto(
                Guid.Empty,
                departmentId,
                masterTypeShort,
                "Master Type",
                employeeTypeActive: true,
                rank: 0,
                isSystemUser: true);

            await PopulateAuditFieldsForCreateAsync(dto).ConfigureAwait(false);

            var entity = _mapper.Map<EmployeeType>(dto);
            await _repository.UpsertAsync(entity, "EmployeeTypeID").ConfigureAwait(false);

            return entity.EmployeeTypeID;
        }

        /// <inheritdoc />
        public async Task EnsureLeoEmployeeTypeExistsAsync(Guid departmentId)
        {
            var existing = (await _repository.GetAllAsync(
                    x => x.DepartmentID == departmentId &&
                         ((x.EmployeeTypeShort != null && x.EmployeeTypeShort.ToLower() == "leo") ||
                          (x.EmployeeTypeLong != null && x.EmployeeTypeLong.ToLower() == "leo")),
                    null,
                    null).ConfigureAwait(false))
                .FirstOrDefault();
            if (existing != null)
                return;

            var leoTypeDto = new EmployeeTypeDto(
                Guid.Empty,
                departmentId,
                "LEO",
                "LEO",
                employeeTypeActive: true,
                rank: 0,
                isSystemUser: false,
                isSupervisor: false,
                isEligible: false);
            await UpsertAsync(leoTypeDto, isSuperUser: true).ConfigureAwait(false);
        }

        private Task PopulateAuditFieldsForCreateAsync(EmployeeTypeDto dto)
        {
            var now = DateTime.UtcNow;
            dto.CreatedDate = now;
            dto.EditedDate = null;
            dto.EditedBy = null;
            dto.EditedUserID = null;

            // CreatedUserID has FK to Employee - only use EmployeeID, never Admin ID
            if (_tokenParser != null)
            {
                var employeeId = _tokenParser.GetEmployeeID();
                dto.CreatedUserID = employeeId;

                var fullName = PersonNameFormatHelper.FormatLastNameCommaFirstName(
                    _tokenParser.GetClaimValue(ClaimNames.LastName),
                    _tokenParser.GetClaimValue(ClaimNames.FirstName));
                dto.CreatedBy = fullName ?? "System";
            }
            else
            {
                dto.CreatedUserID = null;
                dto.CreatedBy = "System";
            }

            return Task.CompletedTask;
        }

        public override Task<PagedList<EmployeeTypeDto>> SearchAsync(
            string? searchTerm,
            PageParam? pageParam = null,
            Expression<Func<EmployeeType, bool>>? filter = null,
            Expression<Func<EmployeeType, object>>[]? includes = null,
            params string[] searchableProperties)
        {
            // ✅ Keep business rule, remove active-only restriction
            Expression<Func<EmployeeType, bool>> baseFilter = x => !x.IsSystemUser;

            if (filter != null)
            {
                var parameter = Expression.Parameter(typeof(EmployeeType), "x");
                var combined = Expression.AndAlso(
                    Expression.Invoke(baseFilter, parameter),
                    Expression.Invoke(filter, parameter));

                baseFilter = Expression.Lambda<Func<EmployeeType, bool>>(combined, parameter);
            }

            var effectiveSearchableProperties =
                (searchableProperties == null || searchableProperties.Length == 0)
                    ? new[] { nameof(EmployeeType.EmployeeTypeShort), nameof(EmployeeType.EmployeeTypeLong) }
                    : searchableProperties;

            return base.SearchAsync(searchTerm, pageParam, baseFilter, includes, effectiveSearchableProperties);
        }
    }
}

