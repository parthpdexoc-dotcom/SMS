using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Interface for closed post position repository operations.
    /// </summary>
    public interface IClosedPostPositionRepository : IGenericRepository<ClosedPostPosition>
    {
        /// <summary>
        /// Single projected lookup of closed-post job/employee fields by IDs (for audit list enrichment).
        /// Returns list DTO rows with main-DB fields only; callers set audit fields.
        /// </summary>
        Task<IReadOnlyDictionary<Guid, ClosedPostPositionAuditListDto>> GetAuditEnrichmentByIdsAsync(
            IReadOnlyCollection<Guid> closedPostPositionIds,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the latest closed post position row per post position for the employee,
        /// ordered for display (matches DTO DateOfWork ordering via <see cref="ClosedPostPosition.RequestedStartDate"/>).
        /// </summary>
        /// <param name="employeeId">The employee identifier (must not be empty).</param>
        Task<IReadOnlyList<ClosedPostPosition>> GetLatestPerPostPositionForEmployeeAsync(Guid employeeId);

        /// <summary>
        /// Paged close-assignment list: filters by <see cref="ClosedPostPosition.Status"/>, post position, department, optional requester, and optional search.
        /// </summary>
        Task<(IReadOnlyList<ClosedPostPosition> Items, int TotalCount)> GetCloseAssignmentsPageAsync(
            IReadOnlyList<ClosedPostPositionStatus> statusFilter,
            string? searchTerm,
            bool filterByEmployee,
            Guid? employeeId,
            bool filterByDepartment,
            Guid? departmentId,
            DateOnly? startDate,
            DateOnly? endDate,
            PageParam pageParam,
            bool isPlaceholderOnly = false,
            Guid? requesterId = null);
    }
}
