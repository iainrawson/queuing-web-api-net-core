using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

using App.QueueService;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<MonitorLoop>();
        services.AddSingleton<WorkItemBuilder>();
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


MonitorLoop monitorLoop = host.Services.GetRequiredService<MonitorLoop>()!;
monitorLoop.StartMonitorLoop();

await host.RunAsync();

