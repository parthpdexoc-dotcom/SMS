using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Helpers
{
    /// <summary>
    /// Shared Per Job / Per Position quantity and assignment rules for equipment and vehicles.
    /// </summary>
    public static class EquipmentAssignmentRules
    {
        /// <summary>
        /// Request-level: <see cref="RequestEquipmentDto.EquipmentPerPost"/> true = Per Job (rank + quantity cap).
        /// </summary>
        public static bool IsRequestPerJob(RequestEquipmentDto requestEquipment) =>
            requestEquipment.EquipmentPerPost;

        /// <summary>
        /// Post-level: <see cref="PostEquipmentDto.EquipmentPerPostPosition"/> false = Per Job (rank + quantity cap).
        /// </summary>
        public static bool IsPostPerJob(PostEquipmentDto postEquipment) =>
            !postEquipment.EquipmentPerPostPosition;

        /// <summary>
        /// Per Position vehicles allow at most one unit per position in stored configuration.
        /// Per Shift items (both vehicles and equipment) and Per Position equipment may have quantity &gt; 1.
        /// </summary>
        public static int NormalizeStoredQuantity(bool isVehicle, bool assignByRankAndQuantity, int requestedQuantity)
        {
            if (requestedQuantity <= 0)
                return 0;

            // Only Per Position vehicles are capped to 1 unit.
            if (isVehicle && !assignByRankAndQuantity)
                return 1;

            return requestedQuantity;
        }

        /// <summary>
        /// Legacy fallback for backward compatibility.
        /// </summary>
        public static int NormalizeStoredQuantity(bool assignByRankAndQuantity, int requestedQuantity) =>
            NormalizeStoredQuantity(isVehicle: true, assignByRankAndQuantity, requestedQuantity);

        /// <summary>
        /// Per Shift Vehicles: min(configured quantity, eligible filled positions) - each person gets 1.
        /// Per Shift Equipment: 1 (all equipment units go to the single highest-ranked person).
        /// Per Position: 1 recipient per eligible position (each receives their unit(s)).
        /// </summary>
        public static int GetEffectiveAssignmentQuantity(
            bool isVehicle,
            bool assignByRankAndQuantity,
            int configuredQuantity,
            int eligiblePositionCount)
        {
            if (configuredQuantity <= 0 || eligiblePositionCount <= 0)
                return 0;

            if (!assignByRankAndQuantity)
                return 1;

            return isVehicle
                ? Math.Min(configuredQuantity, eligiblePositionCount)
                : 1;
        }

        /// <summary>
        /// Legacy fallback for backward compatibility.
        /// </summary>
        public static int GetEffectiveAssignmentQuantity(
            bool assignByRankAndQuantity,
            int configuredQuantity,
            int eligiblePositionCount) =>
            GetEffectiveAssignmentQuantity(isVehicle: true, assignByRankAndQuantity, configuredQuantity, eligiblePositionCount);

        /// <summary>
        /// Count of filled positions that participate in rank/quantity assignment.
        /// </summary>
        public static int GetEligiblePositionCount(
            IReadOnlyList<PostPositionDto> positions,
            PostPositionStatus? historicalViewerStatus = null) =>
            positions.Count(p => EmployeeHighestRankHelper.IsFilledRankCandidate(p, historicalViewerStatus));

        /// <summary>
        /// Resolves employee IDs that receive equipment or vehicles under Per Job or Per Position rules.
        /// </summary>
        public static IReadOnlyList<Guid> ResolveRecipientEmployeeIds(
            bool isVehicle,
            bool assignByRankAndQuantity,
            int configuredQuantity,
            IReadOnlyList<PostPositionDto> positions,
            IReadOnlyDictionary<Guid, DateOnly?>? hireDateByEmployeeId = null,
            IEnumerable<EmployeeTenure>? tenures = null,
            PostPositionStatus? historicalViewerStatus = null)
        {
            var eligible = positions
                .Where(p => EmployeeHighestRankHelper.IsFilledRankCandidate(p, historicalViewerStatus))
                .ToList();

            if (eligible.Count == 0)
                return Array.Empty<Guid>();

            if (!assignByRankAndQuantity)
            {
                return eligible
                    .Select(p => p.EmployeeID)
                    .Where(id => id != Guid.Empty)
                    .ToList();
            }

            var effectiveQuantity = GetEffectiveAssignmentQuantity(
                isVehicle,
                assignByRankAndQuantity,
                configuredQuantity,
                eligible.Count);

            return EmployeeHighestRankHelper.SelectAssignedEmployeeIds(
                eligible,
                effectiveQuantity,
                hireDateByEmployeeId,
                tenures,
                historicalViewerStatus);
        }

        /// <summary>
        /// Maps winning employees to post position IDs on the rank pool.
        /// </summary>
        public static IReadOnlyList<Guid> 
            
            
            
            ResolveRecipientPostPositionIds(
            bool isVehicle,
            bool assignByRankAndQuantity,
            int configuredQuantity,
            IReadOnlyList<PostPositionDto> positions,
            IReadOnlyDictionary<Guid, DateOnly?>? hireDateByEmployeeId = null,
            IEnumerable<EmployeeTenure>? tenures = null,
            PostPositionStatus? historicalViewerStatus = null)
        {
            var assignedEmployeeIds = ResolveRecipientEmployeeIds(
                    isVehicle,
                    assignByRankAndQuantity,
                    configuredQuantity,
                    positions,
                    hireDateByEmployeeId,
                    tenures,
                    historicalViewerStatus)
                .ToHashSet();

            if (assignedEmployeeIds.Count == 0)
                return Array.Empty<Guid>();

            return positions
                .Where(p =>
                    assignedEmployeeIds.Contains(p.EmployeeID)
                    && p.PostPositionID is Guid id
                    && id != Guid.Empty)
                .Select(p => p.PostPositionID!.Value)
                .ToList();
        }
    }
}
