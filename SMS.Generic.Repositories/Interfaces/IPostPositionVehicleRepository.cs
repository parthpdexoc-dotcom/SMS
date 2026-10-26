using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Interface for post position vehicle repository operations, extending IGenericRepository.
    /// </summary>
    public interface IPostPositionVehicleRepository : IGenericRepository<PostPositionVehicle>
    {
        /// <summary>
        /// Returns equipment type short/long names for non-deleted allocations on the post position.
        /// </summary>
        Task<IReadOnlyList<(bool IsVehicle, string Name)>> GetAssignedEquipmentTypeNamesAsync(
            Guid postPositionId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Counts finalized allocations per post/date/equipment, excluding slots that no longer consume quantity.
        /// </summary>
        Task<IReadOnlyDictionary<(Guid PostId, DateOnly DateOfWork, Guid DepartmentEquipmentId), int>>
            GetAllocationCountsByPostAndDateAsync(
                IReadOnlyList<Guid> postIds,
                IReadOnlyList<DateOnly> workDates,
                CancellationToken cancellationToken = default);
    }
}
