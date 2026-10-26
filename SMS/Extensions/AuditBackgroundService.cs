using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Services.Services.Audit;

namespace SMS.Infrastructure.Audit;

/// <summary>
/// Drains the in-memory audit channel and persists to <see cref="AuditDbContext"/> whenever messages are available.
/// </summary>
public sealed class AuditBackgroundService : BackgroundService
{
    private const int MaxSaveAttempts = 3;

    private readonly AuditQueueService _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AuditBackgroundService> _logger;

    public AuditBackgroundService(
        AuditQueueService queue,
        IServiceScopeFactory scopeFactory,
        ILogger<AuditBackgroundService> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _queue.CompleteWriter();
        await base.StopAsync(cancellationToken);
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var batch = new List<AuditBase>();

        try
        {
            while (await _queue.Reader.WaitToReadAsync(stoppingToken).ConfigureAwait(false))
            {
                while (_queue.Reader.TryRead(out var item))
                    batch.Add(item);

                // Persist everything available in this wake-up (one message or a short burst); no minimum batch size.
                await FlushBatchAsync(batch, stoppingToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Audit background service is stopping; draining remaining messages.");
        }

        while (_queue.Reader.TryRead(out var item))
            batch.Add(item);

        await FlushBatchAsync(batch, CancellationToken.None).ConfigureAwait(false);
    }

    private async Task FlushBatchAsync(List<AuditBase> batch, CancellationToken cancellationToken)
    {
        if (batch.Count == 0)
            return;

        var snapshot = batch.ToList();
        batch.Clear();

        for (var attempt = 1; attempt <= MaxSaveAttempts; attempt++)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<AuditDbContext>();
            foreach (var row in snapshot)
                db.Add(row);

            try
            {
                await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return;
            }
            catch (Exception ex) when (attempt < MaxSaveAttempts)
            {
                _logger.LogWarning(ex, "Audit database save failed (attempt {Attempt} of {Max})", attempt, MaxSaveAttempts);
                await Task.Delay(TimeSpan.FromMilliseconds(200 * attempt), cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Audit database save failed after {Max} attempts; re-queuing messages if possible.", MaxSaveAttempts);
                foreach (var row in snapshot)
                {
                    if (!_queue.TryEnqueue(row))
                        _logger.LogError("Audit row for entity {EntityId} could not be re-queued.", row.EntityId);
                }

                return;
            }
        }
    }
}
