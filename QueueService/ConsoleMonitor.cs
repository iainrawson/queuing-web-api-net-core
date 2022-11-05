namespace App.QueueService;

public class MonitorLoop
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ILogger<MonitorLoop> _logger;
    private readonly WorkItemBuilder _workItemBuilder;
    private readonly CancellationToken _cancellationToken;

    public MonitorLoop(
        IBackgroundTaskQueue taskQueue,
        ILogger<MonitorLoop> logger,
        WorkItemBuilder workItemBuilder,
        IHostApplicationLifetime applicationLifetime)
    {
        _taskQueue = taskQueue;
        _logger = logger;
        _workItemBuilder = workItemBuilder;
        _cancellationToken = applicationLifetime.ApplicationStopping;
    }

    public void StartMonitorLoop()
    {
        _logger.LogInformation($"{nameof(MonitorAsync)} loop is starting.");

        // Run a console user input loop in a background thread
        Task.Run(async () => await MonitorAsync());
    }

    private async ValueTask MonitorAsync()
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            var line = Console.ReadLine();
            if (line != null && line.Length > 0) {

                Func<CancellationToken, ValueTask> workItem = async (CancellationToken token) => {
                    string name = line;

                    int delayLoop = 0;

                    _logger.LogInformation("Queued work item \"{name}\" is starting.", name);

                    while (!token.IsCancellationRequested && delayLoop < 3)
                    {
                        try
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1), token);
                        }
                        catch (OperationCanceledException)
                        {
                            // Prevent throwing if the Delay is cancelled
                        }

                        ++ delayLoop;

                        _logger.LogInformation("Queued work item \"{name} is running.\" {DelayLoop}/3", name, delayLoop);
                    }

                    string format = delayLoop switch
                    {
                        3 => "Queued Background Task \"{name}\" is complete.",
                        _ => "Queued Background Task \"{name}\" was cancelled."
                    };

                    _logger.LogInformation(format, name);

                };
                await _taskQueue.QueueBackgroundWorkItemAsync(workItem);
            }

        

        }
    }

    
}