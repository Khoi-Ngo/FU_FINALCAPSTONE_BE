using AISEA.ApiService.SHARED.Interfaces;

namespace AISEA.ApiService.WebApi.BgJob;


public class QueuedHostedService : BackgroundService
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly IServiceProvider _serviceProvider;
    private readonly int _workerCount = 4;

    public QueuedHostedService(IBackgroundTaskQueue taskQueue, IServiceProvider serviceProvider)
    {
        _taskQueue = taskQueue;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        var workers = Enumerable.Range(0, _workerCount)
                    .Select(_ => Task.Run(() => WorkerLoop(stoppingToken), stoppingToken))
                    .ToArray();

        await Task.WhenAll(workers); // Wait for all workers


        // while (!stoppingToken.IsCancellationRequested)
        // {
        //     var workItem = await _taskQueue.DequeueAsync(stoppingToken);

        //     using var scope = _serviceProvider.CreateScope();
        //     await workItem(scope.ServiceProvider, stoppingToken);
        // }
    }

    private async Task WorkerLoop(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var workItem = await _taskQueue.DequeueAsync(stoppingToken);

                using var scope = _serviceProvider.CreateScope();
                await workItem(scope.ServiceProvider, stoppingToken);
            }
            catch (Exception ex)
            {
                // Log the exception but keep the worker alive
                Console.WriteLine($"Background task error: {ex}");
            }
        }
    }
}
