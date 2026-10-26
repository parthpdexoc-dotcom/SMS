using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Employee type-specific repository implementation.
    /// </summary>
    public class EmployeeTypeRepository : GenericRepository<EmployeeType, SMSDbContext>, IEmployeeTypeRepository
    {
        private readonly SMSDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeTypeRepository"/> class.
        /// </summary>
        public EmployeeTypeRepository(
            SMSDbContext context,
            ILogger<EmployeeTypeRepository> logger) : base(context, logger)
        {
            _db = context;
        }

        /// <inheritdoc />
        public async Task UpsertWithSameRankPeersAsync(EmployeeType parent, IReadOnlyList<Guid> peerEmployeeTypeIds, CancellationToken cancellationToken = default)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            // SqlServerRetryingExecutionStrategy requires transactions inside CreateExecutionStrategy().ExecuteAsync
            var strategy = _db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    var set = _db.Set<EmployeeType>();
                    var parentId = parent.EmployeeTypeID;
                    var peerIds = peerEmployeeTypeIds.Where(id => id != parentId).Distinct().ToList();

                    var existing = await set.FindAsync(new object[] { parentId }, cancellationToken).ConfigureAwait(false);
                    var sameRankIdsBeforeUpsert = existing?.SameRankIds?.ToList();

                    if (existing != null)
                    {
                        _db.Entry(existing).CurrentValues.SetValues(parent);
                    }
                    else
                        await set.AddAsync(parent, cancellationToken).ConfigureAwait(false);

                    var targetRank = parent.Rank;
                    if (existing != null && targetRank == 0 && existing.Rank != 0)
                        targetRank = existing.Rank;
                    var departmentId = parent.DepartmentID;

                    // Old cluster before parent's row was overwritten (SetValues).
                    var oldFullGroup = await ExpandClusterFromParentSnapshotAsync(
                        set, parentId, sameRankIdsBeforeUpsert, departmentId, targetRank, cancellationToken).ConfigureAwait(false);

                    var peersBeforeOnParent = (sameRankIdsBeforeUpsert ?? new List<Guid>())
                        .Where(x => x != parentId)
                        .ToHashSet();
                    var peerIdsSet = peerIds.ToHashSet();
                    // If this row's same-rank selection changed vs DB, trust the client list only (e.g. Senior HR removes Plexoc).
                    // If unchanged, expand via BFS so a member row that only stored partial links still picks up the full group.
                    var peerListChangedOnParent =
                        !peerIdsSet.SetEquals(peersBeforeOnParent)
                        || (peerIds.Count == 0 && peersBeforeOnParent.Count > 0);

                    HashSet<Guid> newFullGroup;
                    if (peerListChangedOnParent)
                    {
                        newFullGroup = new HashSet<Guid> { parentId };
                        foreach (var p in peerIds)
                            newFullGroup.Add(p);
                    }
                    else
                    {
                        newFullGroup = await ExpandNewGroupFromPeerSeedsAsync(
                            set, parentId, peerIds, departmentId, targetRank, cancellationToken).ConfigureAwait(false);
                    }

                    var removedIds = oldFullGroup.Where(id => !newFullGroup.Contains(id)).ToList();
                    // Solo type: no same-rank peers — store null, not [self].
                    List<Guid>? symmetricSameRankIds = newFullGroup.Count > 1
                        ? newFullGroup.OrderBy(x => x).ToList()
                        : null;

                    if (existing != null)
                        existing.SameRankIds = symmetricSameRankIds;
                    else
                        parent.SameRankIds = symmetricSameRankIds;

                    var removedFromGroup = removedIds.Where(id => id != parentId).OrderBy(x => x).ToList();
                    var removedCount = removedFromGroup.Count;
                    if (removedCount > 0 && targetRank > 0)
                    {
                        var baseRank = targetRank + 1;
                        var removedSet = removedFromGroup.ToHashSet();
                        await set
                            .Where(e =>
                                e.DepartmentID == departmentId
                                && e.Rank >= baseRank
                                && !removedSet.Contains(e.EmployeeTypeID))
                            .ExecuteUpdateAsync(
                                s => s.SetProperty(e => e.Rank, e => e.Rank + removedCount),
                                cancellationToken)
                            .ConfigureAwait(false);

                        for (var i = 0; i < removedFromGroup.Count; i++)
                        {
                            var rid = removedFromGroup[i];
                            var removedEntity = await set.FindAsync(new object[] { rid }, cancellationToken).ConfigureAwait(false);
                            if (removedEntity == null || removedEntity.DepartmentID != departmentId)
                                continue;
                            removedEntity.SameRankIds = null;
                            removedEntity.Rank = baseRank + i;
                        }
                    }
                    else if (removedCount > 0)
                    {
                        foreach (var rid in removedFromGroup)
                        {
                            var removedEntity = await set.FindAsync(new object[] { rid }, cancellationToken).ConfigureAwait(false);
                            if (removedEntity == null || removedEntity.DepartmentID != departmentId)
                                continue;
                            removedEntity.SameRankIds = null;
                        }
                    }

                    // Current group members: same rank + full symmetric SameRankIds list
                    foreach (var memberId in newFullGroup)
                    {
                        if (memberId == parentId)
                            continue;
                        var member = await set.FindAsync(new object[] { memberId }, cancellationToken).ConfigureAwait(false);
                        if (member == null || member.DepartmentID != departmentId)
                            continue;
                        member.Rank = targetRank;
                        member.SameRankIds = symmetricSameRankIds;
                    }

                    await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                    throw;
                }
            }).ConfigureAwait(false);
        }

        private static async Task<HashSet<Guid>> ExpandClusterFromParentSnapshotAsync(
            DbSet<EmployeeType> set,
            Guid parentId,
            List<Guid>? parentPeersBeforeUpsert,
            Guid departmentId,
            int targetRank,
            CancellationToken cancellationToken)
        {
            var g = new HashSet<Guid> { parentId };
            var queue = new Queue<Guid>();
            queue.Enqueue(parentId);
            if (parentPeersBeforeUpsert != null)
            {
                foreach (var x in parentPeersBeforeUpsert)
                {
                    if (g.Add(x))
                        queue.Enqueue(x);
                }
            }

            return await RunSameRankBfsAsync(set, g, queue, departmentId, targetRank, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<HashSet<Guid>> ExpandNewGroupFromPeerSeedsAsync(
            DbSet<EmployeeType> set,
            Guid parentId,
            IReadOnlyList<Guid> peerIds,
            Guid departmentId,
            int targetRank,
            CancellationToken cancellationToken)
        {
            var g = new HashSet<Guid> { parentId };
            var queue = new Queue<Guid>();
            foreach (var p in peerIds.Distinct())
            {
                g.Add(p);
                queue.Enqueue(p);
            }

            return await RunSameRankBfsAsync(set, g, queue, departmentId, targetRank, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<HashSet<Guid>> RunSameRankBfsAsync(
            DbSet<EmployeeType> set,
            HashSet<Guid> g,
            Queue<Guid> queue,
            Guid departmentId,
            int targetRank,
            CancellationToken cancellationToken)
        {
            while (queue.Count > 0)
            {
                var id = queue.Dequeue();
                var e = await set.FindAsync(new object[] { id }, cancellationToken).ConfigureAwait(false);
                if (e == null || e.DepartmentID != departmentId)
                    continue;
                if (e.Rank != targetRank)
                    continue;
                foreach (var x in e.SameRankIds ?? Enumerable.Empty<Guid>())
                {
                    if (g.Add(x))
                        queue.Enqueue(x);
                }
            }

            return g;
        }
    }
}

