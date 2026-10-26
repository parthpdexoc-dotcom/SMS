using System.Threading.Channels;
using SMS.Generic.Dtos.Interfaces.Audit;
using SMS.Generic.Entities.Entities.Audit;

namespace SMS.Generic.Services.Services.Audit;

/// <summary>
/// Thread-safe in-memory channel queue. Single reader (background worker), multiple writers (API threads).
/// </summary>
public sealed class AuditQueueService : IAuditQueueService, IDisposable
{
    private readonly Channel<AuditBase> _channel;
    private bool _writerCompleted;

    public AuditQueueService()
    {
        _channel = Channel.CreateUnbounded<AuditBase>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        });
    }

    public ChannelReader<AuditBase> Reader => _channel.Reader;

    /// <inheritdoc />
    public ValueTask EnqueueAsync(AuditBase row, CancellationToken cancellationToken = default)
    {
        if (_writerCompleted)
            return ValueTask.CompletedTask;
        return _channel.Writer.WriteAsync(row, cancellationToken);
    }

    /// <inheritdoc />
    public bool TryEnqueue(AuditBase row)
    {
        if (_writerCompleted)
            return false;
        return _channel.Writer.TryWrite(row);
    }

    /// <inheritdoc />
    public async ValueTask EnqueueManyAsync(IReadOnlyList<AuditBase> rows, CancellationToken cancellationToken = default)
    {
        if (_writerCompleted || rows.Count == 0)
            return;

        for (var i = 0; i < rows.Count; i++)
            await _channel.Writer.WriteAsync(rows[i], cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Signals no further messages; reader continues until drained.</summary>
    public void CompleteWriter()
    {
        _writerCompleted = true;
        _channel.Writer.TryComplete();
    }

    public void Dispose() => CompleteWriter();
}
