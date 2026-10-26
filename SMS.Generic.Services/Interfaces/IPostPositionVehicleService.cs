using System.Collections.Generic;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for post position vehicle service operations.
    /// </summary>
    public interface IPostPositionVehicleService : IGenericService<PostPositionVehicleDto, PostPositionVehicle>
    {
        /// <summary>
        /// Creates historical vehicle/equipment allocation records for started assigned post positions
        /// in the department that have not yet been finalized.
        /// </summary>
        Task<int> FinalizeStartedAllocationsForDepartmentAsync(
            Guid departmentId,
            DateTime departmentNow,
            Guid masterEmployeeId,
            string masterEmployeeDisplayName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Copies stored vehicle/equipment from <paramref name="sourcePostPositionId"/> to
        /// <paramref name="targetPostPositionId"/> with no rank check.
        /// Same ids: persist the current live kit if none is stored yet (call while Assigned).
        /// </summary>
        Task CopyAllocationsAsync(Guid sourcePostPositionId, Guid targetPostPositionId);

        /// <summary>
        /// Position ids among <paramref name="postPositionIds"/> that already have stored allocations.
        /// </summary>
        Task<HashSet<Guid>> GetAllocatedPositionIdsAsync(IReadOnlyCollection<Guid> postPositionIds);

        /// <summary>
        /// Soft-deletes stored allocations for the given positions.
        /// </summary>
        Task ReleaseAllocationsAsync(IReadOnlyCollection<Guid> postPositionIds);

        /// <summary>
        /// Comma-separated vehicle and equipment type names for the post position
        /// (frozen <see cref="PostPositionVehicle"/> when present; otherwise live assignment).
        /// </summary>
        Task<(string Vehicles, string Equipment)> GetAssignedDisplayNamesAsync(Guid postPositionId);
    }
}
