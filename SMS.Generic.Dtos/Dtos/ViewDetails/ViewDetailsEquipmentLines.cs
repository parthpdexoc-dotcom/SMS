using System.Collections.Generic;
using System.Linq;
using SMS.Generic.Dtos.Helpers;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>Department equipment fields used when composing view-details equipment lines.</summary>
    public readonly record struct ViewDetailsDepartmentEquipmentMeta(
        bool IsVehicle,
        string? Name,
        decimal? Charge,
        decimal? MileageCharge,
        bool? ChargeMileage,
        bool? RequiresApproval,
        int? ChargeType);


    /// <summary>Builds filtered equipment lines for <see cref="ViewDetailsDto"/>.</summary>
    public static class ViewDetailsEquipmentLines
    {
        public static bool ShouldInclude(bool isVehicle, bool showVehicle, bool showEquipment)
        {
            if (showVehicle && showEquipment)
                return true;
            if (showVehicle && !showEquipment)
                return isVehicle;
            if (!showVehicle && showEquipment)
                return !isVehicle;
            return false;
        }

        /// <summary>
        /// True when <paramref name="dateOfWork"/> falls within the equipment assignment window (inclusive).
        /// Requires a work date; rows with unset migration sentinel dates are ignored.
        /// </summary>
        public static bool IsEquipmentActiveOnDate(
            DateOnly startDate,
            DateOnly endDate,
            DateOnly? dateOfWork)
        {
            if (!dateOfWork.HasValue)
                return false;

            if (!HasConfiguredEquipmentDateRange(startDate, endDate))
                return false;

            var workDate = dateOfWork.Value;
            return workDate >= startDate && workDate <= endDate;
        }

        /// <summary>
        /// Excludes rows that still have default/unset dates from schema backfill (e.g. 0001-01-01).
        /// </summary>
        public static bool HasConfiguredEquipmentDateRange(DateOnly startDate, DateOnly endDate) => endDate >= startDate;

        /// <summary>
        /// When a post defines equipment for a department equipment ID, request-level rows for that ID are ignored
        /// for that post (post dates and rules apply).
        /// </summary>
        public static IReadOnlyList<RequestEquipmentDto> ExcludeRequestEquipmentOverriddenByPost(
            IReadOnlyList<RequestEquipmentDto> requestEquipment,
            IReadOnlySet<Guid>? postDepartmentEquipmentIds)
        {
            if (postDepartmentEquipmentIds is not { Count: > 0 })
                return requestEquipment;

            return requestEquipment
                .Where(re =>
                    !re.DepartmentEquipmentID.HasValue
                    || !postDepartmentEquipmentIds.Contains(re.DepartmentEquipmentID.Value))
                .ToList();
        }

        /// <summary>
        /// Per Job (<paramref name="assignByRankAndQuantity"/> true): one unit per position by rank then hire date,
        /// up to effective quantity (min of configured quantity and eligible positions).
        /// Per Position (false): every filled position gets it; quantity is always one per position.
        /// <summary>
        /// Per Job:
        /// - Vehicle: one unit per position by rank then hire date, up to configured quantity.
        /// - Equipment: all equipment units go to the single highest-ranked person.
        /// Per Position:
        /// - Vehicle: every filled position gets 1 unit.
        /// - Equipment: every filled position gets the set quantity.
        /// </summary>
        public static bool ShouldShowEquipmentToViewer(
            bool isVehicle,
            bool assignByRankAndQuantity,
            int equipmentQuantity,
            Guid viewerEmployeeId,
            IReadOnlyList<PostPositionDto> positions,
            IReadOnlyDictionary<Guid, DateOnly?>? hireDateByEmployeeId = null,
            IEnumerable<EmployeeTenure>? tenures = null,
            PostPositionStatus? historicalViewerStatus = null)
        {
            if (viewerEmployeeId == Guid.Empty)
                return false;

            if (EquipmentAssignmentRules.GetEligiblePositionCount(positions, historicalViewerStatus) == 0)
                return false;

            return EquipmentAssignmentRules.ResolveRecipientEmployeeIds(
                    isVehicle,
                    assignByRankAndQuantity,
                    equipmentQuantity,
                    positions,
                    hireDateByEmployeeId,
                    tenures,
                    historicalViewerStatus)
                .Contains(viewerEmployeeId);
        }

        /// <summary>
        /// Resolves equipment lines for every post position using the same rank and quantity rules as
        /// <see cref="BuildLines"/> with <c>filterByViewerRank</c> enabled.
        /// </summary>
        public static Dictionary<Guid, List<ViewDetailsEquipmentLine>> ResolveEquipmentLinesByPostPositionId(
            IReadOnlyList<RequestEquipmentDto> requestDtos,
            IReadOnlyList<PostEquipmentDto> postDtos,
            IReadOnlyDictionary<Guid, ViewDetailsDepartmentEquipmentMeta> lookup,
            bool showVehicle,
            bool showEquipment,
            IReadOnlyList<PostPositionDto> requestPositions,
            IReadOnlyDictionary<Guid, DateOnly?>? hireDateByEmployeeId = null,
            DateOnly? dateOfWork = null,
            IReadOnlyDictionary<Guid, HashSet<Guid>>? postDepartmentEquipmentIdsByPostId = null,
            IEnumerable<EmployeeTenure>? tenures = null,
            PostPositionStatus? historicalViewerStatus = null,
            IReadOnlyDictionary<(Guid PostId, DateOnly DateOfWork, Guid DepartmentEquipmentId), int>? allocationCounts = null) =>
            EnumerateRankedEquipmentAssignments(
                    requestDtos,
                    postDtos,
                    lookup,
                    showVehicle,
                    showEquipment,
                    requestPositions,
                    hireDateByEmployeeId,
                    dateOfWork,
                    postDepartmentEquipmentIdsByPostId,
                    tenures,
                    historicalViewerStatus,
                    allocationCounts)
                .GroupBy(assignment => assignment.PostPositionId)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(assignment => assignment.Line).ToList());

        public static List<ViewDetailsEquipmentLine> BuildLines(
            IReadOnlyList<RequestEquipmentDto> requestDtos,
            IReadOnlyList<PostEquipmentDto> postDtos,
            IReadOnlyDictionary<Guid, ViewDetailsDepartmentEquipmentMeta> lookup,
            bool showVehicle,
            bool showEquipment,
            Guid? viewerEmployeeId,
            IReadOnlyList<PostPositionDto>? requestPositionsForRank,
            IReadOnlyList<PostPositionDto>? postPositionsForRank,
            bool filterByViewerRank,
            IReadOnlyDictionary<Guid, DateOnly?>? hireDateByEmployeeId = null,
            DateOnly? dateOfWork = null,
            IReadOnlySet<Guid>? postOverrideDepartmentEquipmentIds = null,
            IEnumerable<EmployeeTenure>? tenures = null,
            PostPositionStatus? historicalViewerStatus = null,
            IReadOnlyDictionary<(Guid PostId, DateOnly DateOfWork, Guid DepartmentEquipmentId), int>? allocationCounts = null)
        {
            var requestPositions = requestPositionsForRank ?? Array.Empty<PostPositionDto>();
            var postPositions = postPositionsForRank ?? Array.Empty<PostPositionDto>();

            if (!filterByViewerRank)
            {
                var effectiveRequestDtos = ExcludeRequestEquipmentOverriddenByPost(
                    requestDtos,
                    postOverrideDepartmentEquipmentIds);

                return EnumerateUnfilteredEquipmentLines(
                        effectiveRequestDtos,
                        postDtos,
                        lookup,
                        showVehicle,
                        showEquipment,
                        dateOfWork)
                    .ToList();
            }

            if (!viewerEmployeeId.HasValue || viewerEmployeeId.Value == Guid.Empty)
                return new List<ViewDetailsEquipmentLine>();

            var effectiveRequestDtosForViewer = ExcludeRequestEquipmentOverriddenByPost(
                requestDtos,
                postOverrideDepartmentEquipmentIds);

            var viewerPostPositionIds = requestPositions
                .Concat(postPositions)
                .Where(position =>
                    position.EmployeeID == viewerEmployeeId.Value
                    && position.PostPositionID is Guid id
                    && id != Guid.Empty)
                .Select(position => position.PostPositionID!.Value)
                .ToHashSet();

            return EnumerateRankedEquipmentAssignments(
                    effectiveRequestDtosForViewer,
                    postDtos,
                    lookup,
                    showVehicle,
                    showEquipment,
                    requestPositions,
                    hireDateByEmployeeId,
                    dateOfWork,
                    postDepartmentEquipmentIdsByPostId: null,
                    tenures,
                    historicalViewerStatus,
                    allocationCounts)
                .Where(assignment => viewerPostPositionIds.Contains(assignment.PostPositionId))
                .Select(assignment => assignment.Line)
                .ToList();
        }

        /// <summary>
        /// Whether the viewer should see vehicle/equipment UI based on request and post equipment rows and rank rules.
        /// </summary>
        public static (bool ShowVehicle, bool ShowEquipment) ComputeShowFlagsForViewer(
            Guid viewerEmployeeId,
            IReadOnlyList<RequestEquipmentDto> requestEquipment,
            IReadOnlyList<PostEquipmentDto> postEquipment,
            IReadOnlyList<PostPositionDto> requestPositions,
            IReadOnlyList<PostPositionDto> postPositions,
            bool postRequiresVehicle = false,
            bool postRequiresEquipment = false,
            bool requestRequiresVehicle = false,
            bool requestRequiresEquipment = false,
            IReadOnlyDictionary<Guid, DateOnly?>? hireDateByEmployeeId = null,
            DateOnly? dateOfWork = null,
            IReadOnlySet<Guid>? postOverrideDepartmentEquipmentIds = null,
            IEnumerable<EmployeeTenure>? tenures = null,
            PostPositionStatus? historicalViewerStatus = null,
            IReadOnlyDictionary<(Guid PostId, DateOnly DateOfWork, Guid DepartmentEquipmentId), int>? allocationCounts = null)
        {
            var effectiveRequestEquipment = ExcludeRequestEquipmentOverriddenByPost(
                requestEquipment,
                postOverrideDepartmentEquipmentIds);

            var (showVehicle, showEquipment) = ComputeViewerEquipmentFlags(
                viewerEmployeeId,
                EnumerateViewerEquipmentRules(effectiveRequestEquipment, postEquipment, dateOfWork, allocationCounts),
                requestPositions,
                postPositions,
                hireDateByEmployeeId,
                tenures,
                historicalViewerStatus);

            return ApplyLegacyRequiresVehicleEquipmentFallback(
                showVehicle,
                showEquipment,
                viewerEmployeeId,
                effectiveRequestEquipment,
                postEquipment,
                postPositions,
                postRequiresVehicle,
                postRequiresEquipment,
                requestRequiresVehicle,
                requestRequiresEquipment,
                hireDateByEmployeeId,
                dateOfWork,
                tenures,
                historicalViewerStatus);
        }

        public static void ApplyShowFlagsFromEquipmentLines(ViewDetailsDto response)
        {
            response.ShowVehicle = response.Equipments.Any(e => e.IsVehicle);
            response.ShowEquipment = response.Equipments.Any(e => !e.IsVehicle);
        }

        /// <summary>
        /// Builds all equipment lines (both vehicles and non-vehicle equipment) from finalized <see cref="PostPositionVehicleDto"/> snapshots.
        /// </summary>
        public static List<ViewDetailsEquipmentLine> BuildLinesFromFinalizedAllocations(
            IReadOnlyList<PostPositionVehicleDto> allocations,
            IReadOnlyDictionary<Guid, DepartmentEquipmentDto> departmentEquipmentById,
            IReadOnlyDictionary<Guid, int>? configuredQuantityByDeptEquipmentId = null)
        {
            if (allocations.Count == 0 || departmentEquipmentById.Count == 0)
                return new List<ViewDetailsEquipmentLine>();

            return allocations
                .Select(allocation =>
                {
                    // Qty 0 = empty-freeze marker (transfer/split lock); do not show as issued kit.
                    if (allocation.EquipmentQuantity == 0)
                        return null;

                    var departmentEquipmentId = allocation.DepartmentEquipmentID.GetValueOrDefault();
                    if (departmentEquipmentId == Guid.Empty
                        || !departmentEquipmentById.TryGetValue(departmentEquipmentId, out var departmentEquipment))
                    {
                        return null;
                    }

                    var isVehicle = departmentEquipment.EquipmentType?.IsVehicle == true;
                    var equipmentTypeName = departmentEquipment.EquipmentType?.EquipementTypeShort
                        ?? departmentEquipment.EquipmentType?.EquipementTypeLong;

                    var quantity = isVehicle
                        ? 1
                        : (allocation.EquipmentQuantity.HasValue && allocation.EquipmentQuantity.Value > 0
                            ? allocation.EquipmentQuantity.Value
                            : (allocation.MileageCondition.HasValue && allocation.MileageCondition.Value > 0
                                ? allocation.MileageCondition.Value
                                : (configuredQuantityByDeptEquipmentId != null && configuredQuantityByDeptEquipmentId.TryGetValue(departmentEquipmentId, out var qty) && qty > 0
                                    ? qty
                                    : 1)));

                    return new ViewDetailsEquipmentLine(
                        requestEquipmentId: null,
                        postEquipmentId: null,
                        postId: null,
                        departmentEquipmentId: departmentEquipmentId,
                        equipmentPerPost: !allocation.EquipmentPerPostPosition,
                        equipmentPerPostPosition: allocation.EquipmentPerPostPosition,
                        equipmentQuantity: quantity,
                        isVehicle: isVehicle,
                        equipmentTypeName: equipmentTypeName,
                        charge: allocation.Charge ?? departmentEquipment.Charge,
                        mileageCharge: allocation.MileageCharge ?? departmentEquipment.MileageCharge,
                        chargeMileage: allocation.ChargeMileage ?? departmentEquipment.ChargeMileage,
                        requiresApproval: departmentEquipment.RequiresApproval,
                        chargeType: allocation.ChargedType ?? departmentEquipment.ChargedType);
                })
                .Where(line => line != null)
                .Cast<ViewDetailsEquipmentLine>()
                .ToList();
        }

        /// <summary>
        /// Builds vehicle equipment lines from finalized <see cref="PostPositionVehicleDto"/> snapshots.
        /// Non-vehicle rows in <paramref name="allocations"/> are ignored.
        /// </summary>
        public static List<ViewDetailsEquipmentLine> BuildVehicleLinesFromFinalizedAllocations(
            IReadOnlyList<PostPositionVehicleDto> allocations,
            IReadOnlyDictionary<Guid, DepartmentEquipmentDto> departmentEquipmentById)
        {
            if (allocations.Count == 0 || departmentEquipmentById.Count == 0)
                return new List<ViewDetailsEquipmentLine>();

            return allocations
                .Select(allocation =>
                {
                    if (allocation.EquipmentQuantity == 0)
                        return null;

                    var departmentEquipmentId = allocation.DepartmentEquipmentID.GetValueOrDefault();
                    if (departmentEquipmentId == Guid.Empty
                        || !departmentEquipmentById.TryGetValue(departmentEquipmentId, out var departmentEquipment))
                    {
                        return null;
                    }

                    if (departmentEquipment.EquipmentType?.IsVehicle != true)
                        return null;

                    var equipmentTypeName = departmentEquipment.EquipmentType.EquipementTypeShort
                        ?? departmentEquipment.EquipmentType.EquipementTypeLong;

                    return new ViewDetailsEquipmentLine(
                        requestEquipmentId: null,
                        postEquipmentId: null,
                        postId: null,
                        departmentEquipmentId: departmentEquipmentId,
                        equipmentPerPost: !allocation.EquipmentPerPostPosition,
                        equipmentPerPostPosition: allocation.EquipmentPerPostPosition,
                        equipmentQuantity: 1,
                        isVehicle: true,
                        equipmentTypeName: equipmentTypeName,
                        charge: allocation.Charge ?? departmentEquipment.Charge,
                        mileageCharge: allocation.MileageCharge ?? departmentEquipment.MileageCharge,
                        chargeMileage: allocation.ChargeMileage ?? departmentEquipment.ChargeMileage,
                        requiresApproval: departmentEquipment.RequiresApproval,
                        chargeType: allocation.ChargedType ?? departmentEquipment.ChargedType);
                })
                .Where(line => line != null)
                .Cast<ViewDetailsEquipmentLine>()
                .ToList();
        }

        private readonly record struct ViewerEquipmentRule(
            bool IsVehicle,
            bool AssignByRankAndQuantity,
            int ConfiguredQuantity,
            bool UseRequestRankPool);

        private static IEnumerable<ViewerEquipmentRule> EnumerateViewerEquipmentRules(
            IReadOnlyList<RequestEquipmentDto> requestEquipment,
            IReadOnlyList<PostEquipmentDto> postEquipment,
            DateOnly? dateOfWork,
            IReadOnlyDictionary<(Guid PostId, DateOnly DateOfWork, Guid DepartmentEquipmentId), int>? allocationCounts) =>
            postEquipment
                .Where(pe => IsEquipmentActiveOnDate(pe.StartDate, pe.EndDate, dateOfWork))
                .Select(pe => new ViewerEquipmentRule(
                    pe.IsVehicle,
                    EquipmentAssignmentRules.IsPostPerJob(pe),
                    RemainingPerJobQuantity(
                        pe.IsVehicle,
                        pe.EquipmentQuantity,
                        EquipmentAssignmentRules.IsPostPerJob(pe),
                        pe.DepartmentEquipmentID,
                        pe.PostID,
                        dateOfWork,
                        allocationCounts),
                    UseRequestRankPool: false));

        private static int RemainingPerJobQuantity(
            bool isVehicle,
            int configuredQuantity,
            bool isPerJob,
            Guid? departmentEquipmentId,
            Guid? postId,
            DateOnly? dateOfWork,
            IReadOnlyDictionary<(Guid PostId, DateOnly DateOfWork, Guid DepartmentEquipmentId), int>? allocationCounts)
        {
            if (!isPerJob
                || allocationCounts is not { Count: > 0 }
                || departmentEquipmentId is not Guid equipmentId
                || equipmentId == Guid.Empty
                || dateOfWork is not DateOnly workDate)
            {
                return configuredQuantity;
            }

            var used = 0;
            if (postId is Guid pid && pid != Guid.Empty)
                allocationCounts.TryGetValue((pid, workDate, equipmentId), out used);
            else
            {
                foreach (var entry in allocationCounts)
                {
                    if (entry.Key.DateOfWork == workDate && entry.Key.DepartmentEquipmentId == equipmentId)
                        used += entry.Value;
                }
            }

            if (used <= 0)
                return configuredQuantity;

            // Per Job Equipment: all units go to a single recipient position.
            // If any position on the post/date already has this equipment allocated, 0 units remain for other positions.
            if (!isVehicle)
                return 0;

            return Math.Max(0, configuredQuantity - used);
        }

        private static (bool ShowVehicle, bool ShowEquipment) ComputeViewerEquipmentFlags(
            Guid viewerEmployeeId,
            IEnumerable<ViewerEquipmentRule> rules,
            IReadOnlyList<PostPositionDto> requestPositions,
            IReadOnlyList<PostPositionDto> postPositions,
            IReadOnlyDictionary<Guid, DateOnly?>? hireDateByEmployeeId,
            IEnumerable<EmployeeTenure>? tenures,
            PostPositionStatus? historicalViewerStatus)
        {
            var showVehicle = false;
            var showEquipment = false;

            foreach (var rule in rules)
            {
                var rankPool = rule.UseRequestRankPool ? requestPositions : postPositions;
                if (!ShouldShowEquipmentToViewer(
                        rule.IsVehicle,
                        rule.AssignByRankAndQuantity,
                        rule.ConfiguredQuantity,
                        viewerEmployeeId,
                        rankPool,
                        hireDateByEmployeeId,
                        tenures,
                        historicalViewerStatus))
                {
                    continue;
                }

                if (rule.IsVehicle)
                    showVehicle = true;
                else
                    showEquipment = true;
            }

            return (showVehicle, showEquipment);
        }

        private static (bool ShowVehicle, bool ShowEquipment) ApplyLegacyRequiresVehicleEquipmentFallback(
            bool showVehicle,
            bool showEquipment,
            Guid viewerEmployeeId,
            IReadOnlyList<RequestEquipmentDto> effectiveRequestEquipment,
            IReadOnlyList<PostEquipmentDto> postEquipment,
            IReadOnlyList<PostPositionDto> postPositions,
            bool postRequiresVehicle,
            bool postRequiresEquipment,
            bool requestRequiresVehicle,
            bool requestRequiresEquipment,
            IReadOnlyDictionary<Guid, DateOnly?>? hireDateByEmployeeId,
            DateOnly? dateOfWork,
            IEnumerable<EmployeeTenure>? tenures,
            PostPositionStatus? historicalViewerStatus)
        {
            if (dateOfWork.HasValue
                || showVehicle
                || showEquipment
                || postEquipment.Count > 0
                || (!postRequiresVehicle && !postRequiresEquipment))
            {
                return (showVehicle, showEquipment);
            }

            var eligible = postPositions
                .Where(p => EmployeeHighestRankHelper.IsFilledRankCandidate(p, historicalViewerStatus))
                .ToList();

            if (eligible.Count == 0)
                return (showVehicle, showEquipment);

            var isHighestOnPost = EmployeeHighestRankHelper.IsAmongHighestRankEmployees(
                viewerEmployeeId,
                eligible,
                hireDateByEmployeeId,
                tenures,
                historicalViewerStatus);

            return (
                showVehicle || (isHighestOnPost && postRequiresVehicle),
                showEquipment || (isHighestOnPost && postRequiresEquipment));
        }

        private readonly record struct EquipmentAssignment(Guid PostPositionId, ViewDetailsEquipmentLine Line);

        private static IEnumerable<EquipmentAssignment> EnumerateRankedEquipmentAssignments(
            IReadOnlyList<RequestEquipmentDto> requestDtos,
            IReadOnlyList<PostEquipmentDto> postDtos,
            IReadOnlyDictionary<Guid, ViewDetailsDepartmentEquipmentMeta> lookup,
            bool showVehicle,
            bool showEquipment,
            IReadOnlyList<PostPositionDto> requestPositions,
            IReadOnlyDictionary<Guid, DateOnly?>? hireDateByEmployeeId,
            DateOnly? dateOfWork,
            IReadOnlyDictionary<Guid, HashSet<Guid>>? postDepartmentEquipmentIdsByPostId,
            IEnumerable<EmployeeTenure>? tenures,
            PostPositionStatus? historicalViewerStatus,
            IReadOnlyDictionary<(Guid PostId, DateOnly DateOfWork, Guid DepartmentEquipmentId), int>? allocationCounts)
        {
            if (requestPositions.Count == 0)
                return Enumerable.Empty<EquipmentAssignment>();

            var postPositionsByPostId = requestPositions
                .GroupBy(position => position.PostID)
                .ToDictionary(group => group.Key, group => (IReadOnlyList<PostPositionDto>)group.ToList());

            var postAssignments = postDtos
                .Where(pe => pe.PostID is Guid postId && postId != Guid.Empty)
                .Where(pe => IsActiveEquipmentRow(pe.StartDate, pe.EndDate, pe.DepartmentEquipmentID, pe.IsVehicle, lookup, showVehicle, showEquipment, dateOfWork))
                .SelectMany(pe =>
                {
                    var postId = pe.PostID!.Value;
                    if (!postPositionsByPostId.TryGetValue(postId, out var postPositions))
                        return Enumerable.Empty<EquipmentAssignment>();

                    return CreateRankedAssignmentsForRule(
                        pe.IsVehicle,
                        EquipmentAssignmentRules.IsPostPerJob(pe),
                        RemainingPerJobQuantity(
                            pe.IsVehicle,
                            pe.EquipmentQuantity,
                            EquipmentAssignmentRules.IsPostPerJob(pe),
                            pe.DepartmentEquipmentID,
                            postId,
                            dateOfWork,
                            allocationCounts),
                        postPositions,
                        hireDateByEmployeeId,
                        tenures,
                        historicalViewerStatus,
                        _ => true,
                        () =>
                        {
                            lookup.TryGetValue(pe.DepartmentEquipmentID!.Value, out var meta);
                            var displayQuantity = pe.IsVehicle ? 1 : pe.EquipmentQuantity;
                            return CreatePostEquipmentLine(pe, meta, displayQuantity: displayQuantity);
                        });
                });

            return postAssignments;
        }

        private static IEnumerable<EquipmentAssignment> CreateRankedAssignmentsForRule(
            bool isVehicle,
            bool assignByRankAndQuantity,
            int configuredQuantity,
            IReadOnlyList<PostPositionDto> rankPool,
            IReadOnlyDictionary<Guid, DateOnly?>? hireDateByEmployeeId,
            IEnumerable<EmployeeTenure>? tenures,
            PostPositionStatus? historicalViewerStatus,
            Func<Guid, bool> includePostPositionId,
            Func<ViewDetailsEquipmentLine> createLine)
        {
            var line = createLine();
            return EquipmentAssignmentRules.ResolveRecipientPostPositionIds(
                    isVehicle,
                    assignByRankAndQuantity,
                    configuredQuantity,
                    rankPool,
                    hireDateByEmployeeId,
                    tenures,
                    historicalViewerStatus)
                .Where(includePostPositionId)
                .Select(postPositionId => new EquipmentAssignment(postPositionId, line));
        }

        private static IEnumerable<ViewDetailsEquipmentLine> EnumerateUnfilteredEquipmentLines(
            IReadOnlyList<RequestEquipmentDto> requestDtos,
            IReadOnlyList<PostEquipmentDto> postDtos,
            IReadOnlyDictionary<Guid, ViewDetailsDepartmentEquipmentMeta> lookup,
            bool showVehicle,
            bool showEquipment,
            DateOnly? dateOfWork) =>
            postDtos
                .Where(pe => IsActiveEquipmentRow(pe.StartDate, pe.EndDate, pe.DepartmentEquipmentID, pe.IsVehicle, lookup, showVehicle, showEquipment, dateOfWork))
                .Select(pe =>
                {
                    lookup.TryGetValue(pe.DepartmentEquipmentID!.Value, out var meta);
                    var displayQuantity = EquipmentAssignmentRules.NormalizeStoredQuantity(
                        pe.IsVehicle,
                        EquipmentAssignmentRules.IsPostPerJob(pe),
                        pe.EquipmentQuantity);
                    return CreatePostEquipmentLine(pe, meta, displayQuantity);
                });

        private static bool IsRequestEquipmentOverriddenOnPost(
            IReadOnlyDictionary<Guid, HashSet<Guid>>? postDepartmentEquipmentIdsByPostId,
            Guid postId,
            Guid departmentEquipmentId) =>
            postDepartmentEquipmentIdsByPostId is not null
            && postDepartmentEquipmentIdsByPostId.TryGetValue(postId, out var overrides)
            && overrides.Contains(departmentEquipmentId);

        private static bool IsActiveEquipmentRow(
            DateOnly startDate,
            DateOnly endDate,
            Guid? departmentEquipmentId,
            bool isVehicle,
            IReadOnlyDictionary<Guid, ViewDetailsDepartmentEquipmentMeta> lookup,
            bool showVehicle,
            bool showEquipment,
            DateOnly? dateOfWork)
        {
            if (departmentEquipmentId is not Guid deId || deId == Guid.Empty)
                return false;
            if (!IsEquipmentActiveOnDate(startDate, endDate, dateOfWork))
                return false;
            if (!ShouldInclude(isVehicle, showVehicle, showEquipment))
                return false;

            return true;
        }

        private static ViewDetailsEquipmentLine CreateRequestEquipmentLine(
            RequestEquipmentDto requestEquipment,
            ViewDetailsDepartmentEquipmentMeta meta,
            int displayQuantity) =>
            new(
                requestEquipment.RequestEquipmentID,
                null,
                null,
                requestEquipment.DepartmentEquipmentID,
                requestEquipment.EquipmentPerPost,
                requestEquipment.EquipmentPerPost ? false : true,
                displayQuantity,
                requestEquipment.IsVehicle,
                meta.Name,
                meta.Charge,
                meta.MileageCharge,
                meta.ChargeMileage,
                meta.RequiresApproval,
                meta.ChargeType);

        private static ViewDetailsEquipmentLine CreatePostEquipmentLine(
            PostEquipmentDto postEquipment,
            ViewDetailsDepartmentEquipmentMeta meta,
            int displayQuantity) =>
            new(
                null,
                postEquipment.PostEquipmentID,
                postEquipment.PostID,
                postEquipment.DepartmentEquipmentID,
                postEquipment.EquipmentPerPostPosition ? false : true,
                postEquipment.EquipmentPerPostPosition,
                displayQuantity,
                postEquipment.IsVehicle,
                meta.Name,
                meta.Charge,
                meta.MileageCharge,
                meta.ChargeMileage,
                meta.RequiresApproval,
                meta.ChargeType);
    }
}
