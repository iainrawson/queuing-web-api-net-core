

public class WorkItemBuilder {

    public WorkItemBuilder(ILogger<WorkItemBuilder> logger) {
        _logger = logger;
    }

    private readonly ILogger<WorkItemBuilder> _logger;

    public async ValueTask BuildWorkItemAsync(CancellationToken token)
    {
        // Simulate three 5-second tasks to complete
        // for each enqueued work item

        int delayLoop = 0;

        string name = "test";

        _logger.LogInformation("Queued work item {name} is starting.", name);

        while (!token.IsCancellationRequested && delayLoop < 3)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), token);
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if the Delay is cancelled
            }

            ++ delayLoop;

            _logger.LogInformation("Queued work item {name} is running. {DelayLoop}/3", name, delayLoop);
        }

        string format = delayLoop switch
        {
            3 => "Queued Background Task {name} is complete.",
            _ => "Queued Background Task {name} was cancelled."
        };

        _logger.LogInformation(format, name);
    }
}