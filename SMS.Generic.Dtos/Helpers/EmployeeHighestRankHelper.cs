using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Helpers
{
    /// <summary>
    /// Orders post-position employees by rank, hire date, current-rank promotion date, then post-position created date/time.
    /// </summary>
    public static class EmployeeHighestRankHelper
    {
        private static readonly IReadOnlyDictionary<Guid, DateOnly?> EmptyHireDates =
            new Dictionary<Guid, DateOnly?>();

        /// <summary>
        /// All employees tied for highest rank after hire-date tiebreak (multiple when rank and hire date match).
        /// </summary>
        public static IReadOnlyList<Guid> SelectHighestRankEmployeeIds(
            IEnumerable<PostPositionDto> positions,
            IReadOnlyDictionary<Guid, DateOnly?>? hireDateByEmployeeId = null,
            IEnumerable<EmployeeTenure>? tenures = null,
            PostPositionStatus? historicalViewerStatus = null)
        {
            var ordered = GetOrderedCandidates(positions, hireDateByEmployeeId, tenures, historicalViewerStatus);
            if (ordered.Count == 0)
                return Array.Empty<Guid>();

            var best = ordered[0];
            return ordered
                .Where(c =>
                    InvalidRankSortTier(c.Rank) == InvalidRankSortTier(best.Rank)
                    && PositiveRankSortValue(c.Rank) == PositiveRankSortValue(best.Rank)
                    && (c.HireDate ?? DateOnly.MaxValue) == (best.HireDate ?? DateOnly.MaxValue))
                .Select(c => c.EmployeeId)
                .ToList();
        }

        /// <summary>
        /// Assigns one unit per position, highest rank first, then earlier hire date.
        /// Count is min(quantity, eligible positions); extra quantity is not stacked on one position.
        /// </summary>
        public static IReadOnlyList<Guid> SelectAssignedEmployeeIds(
            IEnumerable<PostPositionDto> positions,
            int quantity,
            IReadOnlyDictionary<Guid, DateOnly?>? hireDateByEmployeeId = null,
            IEnumerable<EmployeeTenure>? tenures = null,
            PostPositionStatus? historicalViewerStatus = null)
        {
            if (quantity <= 0)
                return Array.Empty<Guid>();

            return GetOrderedCandidates(positions, hireDateByEmployeeId, tenures, historicalViewerStatus)
                .Take(quantity)
                .Select(c => c.EmployeeId)
                .ToList();
        }

        public static Guid? SelectHighestRankEmployeeId(
            IEnumerable<PostPositionDto> positions,
            IReadOnlyDictionary<Guid, DateOnly?>? hireDateByEmployeeId = null,
            IEnumerable<EmployeeTenure>? tenures = null,
            PostPositionStatus? historicalViewerStatus = null)
        {
            var ids = SelectHighestRankEmployeeIds(positions, hireDateByEmployeeId, tenures, historicalViewerStatus);
            return ids.Count > 0 ? ids[0] : null;
        }

        public static bool IsAmongHighestRankEmployees(
            Guid employeeId,
            IEnumerable<PostPositionDto> positions,
            IReadOnlyDictionary<Guid, DateOnly?>? hireDateByEmployeeId = null,
            IEnumerable<EmployeeTenure>? tenures = null,
            PostPositionStatus? historicalViewerStatus = null)
        {
            if (employeeId == Guid.Empty)
                return false;
            return SelectHighestRankEmployeeIds(positions, hireDateByEmployeeId, tenures, historicalViewerStatus)
                .Contains(employeeId);
        }

        /// <summary>Builds hire date per employee from tenure rows (current record first, else earliest hire).</summary>
        public static IReadOnlyDictionary<Guid, DateOnly?> BuildHireDateMapFromTenures(
            IEnumerable<EmployeeTenureDto> tenures) =>
            BuildHireDateMapFromTenureEntities(tenures.Select(ToTenureShape));

        public static IReadOnlyDictionary<Guid, DateOnly?> BuildHireDateMapFromTenureEntities(
            IEnumerable<EmployeeTenure> tenures) =>
            BuildHireDateMapFromTenureEntities(tenures.Select(t => (t.EmployeeID, t.DateOfHire, t.IsCurrent)));

        private static (Guid EmployeeId, DateOnly? DateOfHire, bool IsCurrent) ToTenureShape(EmployeeTenureDto t) =>
            (t.EmployeeID ?? Guid.Empty, t.DateOfHire, t.IsCurrent);

        private static IReadOnlyDictionary<Guid, DateOnly?> BuildHireDateMapFromTenureEntities(
            IEnumerable<(Guid EmployeeId, DateOnly? DateOfHire, bool IsCurrent)> tenures)
        {
            return tenures
                .Where(t => t.EmployeeId != Guid.Empty)
                .GroupBy(t => t.EmployeeId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Where(t => t.IsCurrent && t.DateOfHire.HasValue).Select(t => t.DateOfHire).FirstOrDefault()
                        ?? g.Where(t => t.DateOfHire.HasValue).Min(t => t.DateOfHire));
        }

        private static List<Candidate> GetOrderedCandidates(
            IEnumerable<PostPositionDto> positions,
            IReadOnlyDictionary<Guid, DateOnly?>? hireDateByEmployeeId,
            IEnumerable<EmployeeTenure>? tenures,
            PostPositionStatus? historicalViewerStatus)
        {
            hireDateByEmployeeId ??= EmptyHireDates;
            var promotionDateByEmployeeId = CurrentPromotionDates(tenures);

            return positions
                .Where(p => IsFilledRankCandidate(p, historicalViewerStatus))
                .Select(p => new Candidate(
                    p.EmployeeID,
                    p.Employee?.EmployeeType?.Rank,
                    hireDateByEmployeeId.TryGetValue(p.EmployeeID, out var hire) ? hire : null,
                    promotionDateByEmployeeId.TryGetValue(p.EmployeeID, out var promoted) ? promoted : null,
                    p.CreatedDate))
                .OrderBy(c => InvalidRankSortTier(c.Rank))
                .ThenBy(c => PositiveRankSortValue(c.Rank))
                .ThenBy(c => c.HireDate ?? DateOnly.MaxValue)
                .ThenBy(c => c.PromotionDate ?? DateOnly.MaxValue)
                .ThenBy(c => c.PositionCreatedDate ?? DateTime.MaxValue)
                .ToList();
        }

        private static Dictionary<Guid, DateOnly?> CurrentPromotionDates(IEnumerable<EmployeeTenure>? tenures) =>
            (tenures ?? Enumerable.Empty<EmployeeTenure>())
                .Where(t => t.IsCurrent && t.EmployeeID != Guid.Empty)
                .GroupBy(t => t.EmployeeID)
                .ToDictionary(g => g.Key, g => g.First().DateOfPromotion);

        /// <summary>
        /// Live assignment pool: filled holders that still consume Per Job quantity.
        /// Closed keeps quantity. Cancelled / Declined / Transferred free the slot.
        /// </summary>
        public static bool IsFilledCurrentAssignment(PostPositionDto position) =>
            IsFilledRankCandidate(position, historicalViewerStatus: null);

        /// <summary>
        /// Live ranking excludes Cancelled/Declined/Transferred.
        /// History View Details for Cancelled or Declined also includes peers with that same status
        /// so the viewer can still see the vehicle they ranked for — without changing live quantity.
        /// Transferred never consumes quantity.
        /// </summary>
        public static bool IsFilledRankCandidate(
            PostPositionDto position,
            PostPositionStatus? historicalViewerStatus = null)
        {
            if (position.IsPlaceholder || position.EmployeeID == Guid.Empty)
                return false;

            if (position.PostPositionStatus == PostPositionStatus.Transferred)
                return false;

            // Live / roster / close-count path: cancel and decline free quantity; closed does not.
            if (historicalViewerStatus is not { } viewerStatus
                || !IsHistoryTerminalStatus(viewerStatus))
            {
                return position.PostPositionStatus is not (
                    PostPositionStatus.Cancelled
                    or PostPositionStatus.Declined);
            }

            // History View Details for Cancelled / Declined: keep live holders + same-status peers.
            if (position.PostPositionStatus is not (
                    PostPositionStatus.Cancelled
                    or PostPositionStatus.Declined))
                return true;

            return position.PostPositionStatus == viewerStatus;
        }

        private static bool IsHistoryTerminalStatus(PostPositionStatus status) =>
            status is PostPositionStatus.Cancelled or PostPositionStatus.Declined;

        private static int InvalidRankSortTier(int? rank) =>
            !rank.HasValue || rank.Value <= 0 ? 1 : 0;

        private static int PositiveRankSortValue(int? rank) =>
            rank.HasValue && rank.Value > 0 ? rank.Value : int.MaxValue;

        private readonly record struct Candidate(
            Guid EmployeeId,
            int? Rank,
            DateOnly? HireDate,
            DateOnly? PromotionDate,
            DateTime? PositionCreatedDate);
    }
}
