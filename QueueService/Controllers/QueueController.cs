using Microsoft.AspNetCore.Mvc;

namespace App.QueueService.Controllers;

[ApiController]
[Route("[controller]")]
public class QueueController : ControllerBase
{
    private readonly ILogger<QueueController> _logger;
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly WorkItemBuilder _workItemBuilder;

    public QueueController(ILogger<QueueController> logger, IBackgroundTaskQueue taskQueue, WorkItemBuilder workItemBuilder)
    {
        _logger = logger;
        _taskQueue = taskQueue;
        _workItemBuilder = workItemBuilder;
    }

    [HttpPost(Name = "PostQueue")]
    public async Task<ActionResult> Post(QueueItem item)
    {
        _logger.LogInformation($"Received Post {item.Name}");

        Func<CancellationToken, ValueTask> workItem = async (CancellationToken token) => {
            // Simulate three 5-second tasks to complete
            // for each enqueued work item

            int delayLoop = 0;

            string name = item.Name.ToString();

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
        };

        await _taskQueue.QueueBackgroundWorkItemAsync(workItem);
        return Ok();
    }

}
