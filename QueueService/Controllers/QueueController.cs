using Microsoft.AspNetCore.Mvc;
using QueueService.TaskQueue;
using QueueService.Models;

namespace QueueService.Controllers;

[ApiController]
[Route("[controller]")]
public class QueueController : ControllerBase
{
    private readonly ILogger<QueueController> _logger;
    private readonly IBackgroundTaskQueue _taskQueue;

    public QueueController(ILogger<QueueController> logger, IBackgroundTaskQueue taskQueue)
    {
        _logger = logger;
        _taskQueue = taskQueue;
    }

    [HttpPost(Name = "PostQueue")]
    public async Task<ActionResult> Post(QueueItem item)
    {
        _logger.LogInformation("Received Post {name}", item.Name);

        var workItem = async (CancellationToken token) => {
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

            string status = delayLoop switch
            {
                3 => "is complete",
                _ => "was cancelled."
            };

            _logger.LogInformation("Queued Background Task {name} {status}.", name, status);
        };

        await _taskQueue.QueueBackgroundWorkItemAsync(workItem);
        return Ok();
    }

}
