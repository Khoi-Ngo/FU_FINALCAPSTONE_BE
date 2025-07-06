using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Enums;
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
            using (var scope = _serviceProvider.CreateScope())
            {
                var notificationRepository = scope.ServiceProvider.GetRequiredService<NotificationRepository>();
                var auditLogRepository = scope.ServiceProvider.GetRequiredService<AuditLogRepository>();

                var removedNotis = await notificationRepository.RemoveAllExistedOverDaysAsync(_notificationSettings.ExpiredDays);

                if (removedNotis.Any())
                {
                    _logger.LogInformation("Removed expired notifications with IDs: {NotificationIds}", string.Join(", ", removedNotis));

                    var auditLogs = removedNotis.Select(id => new DAL.Entities.AuditLog
                    {
                        Tag = EAuditLogTag.REMOVE_NOTI,
                        Description = $"Removed expired notification with ID: {id}",
                        CreatedAt = DateTime.UtcNow
                    }).ToList();

                    await auditLogRepository.AddRangeAsync(auditLogs);
                }
                else
                {
                    _logger.LogInformation("No expired notifications found to remove.");
                }
            }

            await Task.Delay(_notificationSettings.IntervalMillis, stoppingToken);
        }
    }
}