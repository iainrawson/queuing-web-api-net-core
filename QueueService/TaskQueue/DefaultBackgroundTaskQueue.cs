using System.Threading.Channels;

namespace App.QueueService;

public class DefaultBackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue;
    private readonly ILogger<DefaultBackgroundTaskQueue> _logger;

    public DefaultBackgroundTaskQueue(ILogger<DefaultBackgroundTaskQueue> logger, int capacity)
    {
        _logger = logger;
        BoundedChannelOptions options = new(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
    }

    public async ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem)
    {
        if (workItem is null)
        {
            throw new ArgumentNullException(nameof(workItem));
        }
        _logger.LogDebug($"QueueBackgroundWorkItemAsync");
        await _queue.Writer.WriteAsync(workItem);
    }

    public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
    {
        Func<CancellationToken, ValueTask>? workItem = await _queue.Reader.ReadAsync(cancellationToken);
        return workItem;
    }
}