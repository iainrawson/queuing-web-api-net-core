using Microsoft.AspNetCore.Hosting;
using QueueService.Services;
using QueueService.TaskQueue;

namespace QueueService;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<ConsoleMonitor>();
                services.AddHostedService<QueuedHostedService>();
                services.AddSingleton<IBackgroundTaskQueue>(services => 
                {
                    if (!int.TryParse(context.Configuration["QueueCapacity"], out var queueCapacity))
                    {
                        queueCapacity = 10;
                    }

                    return new DefaultBackgroundTaskQueue(services.GetRequiredService<ILogger<DefaultBackgroundTaskQueue>>(), queueCapacity);
                });
            })
            .Build();

        ConsoleMonitor monitorLoop = host.Services.GetRequiredService<ConsoleMonitor>()!;
        monitorLoop.StartMonitorLoop();

        await host.RunAsync();
    }
}
