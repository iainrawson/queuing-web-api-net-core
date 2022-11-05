using System.Threading.Channels;

namespace QueueService.TaskQueue;

public class DefaultBackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, Task>> _queue;
    private readonly ILogger<DefaultBackgroundTaskQueue> _logger;

    public DefaultBackgroundTaskQueue(ILogger<DefaultBackgroundTaskQueue> logger, int capacity)
    {
        _logger = logger;
        BoundedChannelOptions options = new(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<Func<CancellationToken, Task>>(options);
    }

    public async Task QueueBackgroundWorkItemAsync(Func<CancellationToken, Task> workItem)
    {
        if (workItem is null)
        {
            throw new ArgumentNullException(nameof(workItem));
        }
        _logger.LogDebug($"QueueBackgroundWorkItemAsync");
        await _queue.Writer.WriteAsync(workItem);
    }

    public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
    {
        Func<CancellationToken, Task>? workItem = await _queue.Reader.ReadAsync(cancellationToken);
        return workItem;
    }
}