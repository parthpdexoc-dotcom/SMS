using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Interface for employee type repository operations, extending IGenericRepository.
    /// </summary>
    public interface IEmployeeTypeRepository : IGenericRepository<EmployeeType>
    {
        /// <summary>
        /// In one transaction: upsert parent (including <see cref="EmployeeType.SameRankIds"/>), then set <see cref="EmployeeType.Rank"/> on each peer to the parent's rank.
        /// IDs removed from <see cref="EmployeeType.SameRankIds"/> are not updated (rank unchanged).
        /// </summary>
        Task UpsertWithSameRankPeersAsync(EmployeeType parent, IReadOnlyList<Guid> peerEmployeeTypeIds, CancellationToken cancellationToken = default);
    }
}

