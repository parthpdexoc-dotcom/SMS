using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Loads request/post equipment, positions, and tenure for view-details and vehicle visibility in few SQL round-trips.
    /// </summary>
    public interface IViewDetailsEquipmentRepository
    {
        /// <summary>
        /// Loads equipment rows (with <see cref="Entities.DepartmentEquipment"/> and equipment type),
        /// optional all post positions on the request for a work date (with employee rank),
        /// and tenure rows for those employees.
        /// </summary>
        /// <param name="postId">When set, post equipment is limited to that post; positions are still all posts on the request.</param>
        /// <param name="dateOfWork">When set, only equipment whose <c>StartDate</c>–<c>EndDate</c> range includes this date is loaded.</param>
        Task<ViewDetailsEquipmentQueryResult> LoadAsync(
            Guid requestId,
            Guid? postId,
            DateOnly? dateOfWork,
            bool loadRankContext,
            CancellationToken cancellationToken = default);

        /// <summary>Tenure rows for hire-date tiebreak among employees on a post/date.</summary>
        Task<List<EmployeeTenure>> GetTenuresForEmployeesAsync(
            IEnumerable<Guid> employeeIds,
            CancellationToken cancellationToken = default);
    }
}
