using SMS.Generic.Entities.Entities.Audit;

namespace SMS.Generic.Dtos.Interfaces.Audit;

/// <summary>Queues audit rows for asynchronous persistence to the audit database.</summary>
public interface IAuditQueueService
{
    /// <summary>Non-blocking enqueue (async wait only if the channel buffer is full for bounded channels).</summary>
    ValueTask EnqueueAsync(AuditBase row, CancellationToken cancellationToken = default);

    /// <summary>Synchronous enqueue for synchronous SaveChanges paths (channel TryWrite).</summary>
    bool TryEnqueue(AuditBase row);

    /// <summary>Enqueues many audit rows in order (e.g. job bulk delete/insert summaries).</summary>
    ValueTask EnqueueManyAsync(IReadOnlyList<AuditBase> rows, CancellationToken cancellationToken = default);
}
