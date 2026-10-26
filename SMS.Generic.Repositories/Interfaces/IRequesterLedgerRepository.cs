using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Repositories.Interfaces
{
    public interface IRequesterLedgerRepository : IGenericRepository<RequesterLedger>
    {
        /// <summary>
        /// Latest non-deleted ledger row for a requester. When <paramref name="beforeDateTime"/> is set, only rows before that date are considered.
        /// </summary>
        Task<RequesterLedger?> GetLatestLedgerEntryByRequesterIdAsync(
            Guid requesterId,
            DateTime? beforeDateTime = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Period entries plus the latest entry before <paramref name="startDateTime"/> when provided (single query).
        /// </summary>
        Task<IReadOnlyList<RequesterLedger>> GetLedgerEntriesForStatementAsync(
            Guid requesterId,
            DateTime? startDateTime,
            DateTime? endDateTime,
            CancellationToken cancellationToken = default);
    }
}
