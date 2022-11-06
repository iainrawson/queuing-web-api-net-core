using Microsoft.AspNetCore.Mvc;
using QueueService.TaskQueue;
using QueueService.Models;
using DataSources.TidalData;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;
using QueueService.FormattingExtensions;

namespace QueueService.Controllers;

using TidalEvents = List<TidalEvent>;

[ApiController]
[Route("[controller]")]
public class QueueController : ControllerBase
{
    private readonly ILogger<QueueController> _logger;
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly AdmiraltyTidalApiClient _admiraltyTidalApiClient;

    public QueueController(ILogger<QueueController> logger, IBackgroundTaskQueue taskQueue, AdmiraltyTidalApiClient admiraltyTidalApiClient)
    {
        _logger = logger;
        _taskQueue = taskQueue;
        _admiraltyTidalApiClient = admiraltyTidalApiClient;
    }

    private readonly string _stationId = "0134";

    [HttpGet(Name = "GetQueue")]
    public async Task<TidalEvents> Get() {

        var result = await _admiraltyTidalApiClient.GetTidalEventsByStationId(stationId: _stationId);
        return result;
    }
    
    [Route(template: "PrintTidalEvents")]
    [HttpPost(Name = "PrintTidalEvents")]
    public async Task<ActionResult> PrintTidalEvents()
    {
        var tidalEvents = await _admiraltyTidalApiClient.GetTidalEventsByStationId(stationId: _stationId);
        var queueItem = tidalEvents.ToQueueItem();
        await _taskQueue.QueueBackgroundWorkItemAsync(CreateWorkItem(queueItem));
        return Ok();
    }

    [HttpPost(Name = "PostQueue")]
    public async Task<ActionResult> Post(QueueItem item)
    {
        _logger.LogInformation("Received Post {name}", item.Id);

        await _taskQueue.QueueBackgroundWorkItemAsync(CreateWorkItem(item));
        return Ok();
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
