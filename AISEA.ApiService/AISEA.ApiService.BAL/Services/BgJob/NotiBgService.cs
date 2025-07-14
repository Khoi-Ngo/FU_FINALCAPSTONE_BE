using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.PropConfigs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AISEA.ApiService.BAL.Services.BgJob;

public class NotiBgService : BackgroundService
{
    private readonly ILogger<NotiBgService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly NotificationSettings _notificationSettings;

    public NotiBgService(
        ILogger<NotiBgService> logger,
        IServiceProvider serviceProvider,
        NotificationSettings notificationSettings)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _notificationSettings = notificationSettings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<NotificationRepository>();

                var removedNotiIds = await repository.RemoveAllExistedOverDaysAsync(_notificationSettings.ExpiredDays);

                if (removedNotiIds.Any())
                {
                    _logger.LogInformation("Removed expired notifications: {NotificationIds}", string.Join(", ", removedNotiIds));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing expired notifications.");
            }

            await Task.Delay(_notificationSettings.IntervalMillis, stoppingToken);
        }
    }
}
