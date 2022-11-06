using Microsoft.AspNetCore.Mvc;
using QueueService.TaskQueue;
using QueueService.Models;
using DataSources.TidalData;
using QueueService.FormattingExtensions;

namespace QueueService.Controllers;

[ApiController]
[Route("[controller]")]
public class PrintController : ControllerBase
{
    private readonly ILogger<PrintController> _logger;
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly AdmiraltyTidalApiClient _admiraltyTidalApiClient;

    public PrintController(
        ILogger<PrintController> logger, 
        IBackgroundTaskQueue taskQueue, 
        AdmiraltyTidalApiClient admiraltyTidalApiClient) => (_logger, _taskQueue, _admiraltyTidalApiClient) = (logger, taskQueue, admiraltyTidalApiClient);

    [HttpPost("Tides", Name = "PrintTidalEvents")]
    public async Task<ActionResult> PrintTides()
    {
        const string stationId = "0134";
        var tidalEvents = await _admiraltyTidalApiClient.GetTidalEventsByStationId(stationId: stationId);
        var queueItem = tidalEvents.ToQueueItem();
        var workItem = CreateWorkItem(queueItem);
        await _taskQueue.QueueBackgroundWorkItemAsync(workItem);
        return Ok(queueItem.Id);
    }

    [HttpPost(template: "HelloWorld", Name = "HelloWorld")]
    public async Task<ActionResult> HelloWorld()
    {
        QueueItem? queueItem = "Hello World".ToQueueItem();
        await _taskQueue.QueueBackgroundWorkItemAsync(CreateWorkItem(queueItem));
        return Ok(queueItem.Id);
    }

    private Func<CancellationToken, Task> CreateWorkItem(QueueItem queueItem) {

        var workItem = async (CancellationToken token) => {
            // Simulate three 5-second tasks to complete
            // for each enqueued work item

            int delayLoop = 0;

            string id = queueItem.Id.ToString();

            _logger.LogInformation("Queued work item {id} is starting.", id);

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

                _logger.LogInformation("Queued work item {id} is running. {DelayLoop}/3", id, delayLoop);
            }

            string status = delayLoop switch
            {
                3 => "is complete",
                _ => "was cancelled."
            };

            _logger.LogInformation("Queued Background Task {name} {status}.", id, status);
        };

        return workItem;

    }

}
