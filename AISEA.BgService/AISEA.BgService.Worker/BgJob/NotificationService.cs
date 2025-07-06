using AISEA.BgService.DAL.Repositories;
using AISEA.BgService.Worker.Entities;
using AISEA.BgService.Worker.Enums;
using AISEA.BgService.Worker.PropConfig;
using AISEA.BgService.Worker.Repositories;

namespace AISEA.BgService.Worker.BgJob;

public class NotificationService : BackgroundService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly NotificationRepository _notificationRepository;
    private readonly NotiSettings _notiSettings;
    private readonly AuditLogRepository _auditLogRepository;

    public NotificationService(ILogger<NotificationService> logger, NotificationRepository notificationRepository, NotiSettings notiSettings, AuditLogRepository auditLogRepository)
    {
        _logger = logger;
        _notificationRepository = notificationRepository;
        _notiSettings = notiSettings;
        _auditLogRepository = auditLogRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var removedNotis = await _notificationRepository.RemoveAllExistedOverDaysAsync(_notiSettings.ExpiredDays);

            if (removedNotis.Any())
            {
                _logger.LogInformation("Removed expired notifications with IDs: {NotificationIds}", string.Join(", ", removedNotis));

                // Bulk insert audit logs
                var auditLogs = removedNotis.Select(id => new AuditLog
                {
                    Tag = EAuditLogTag.REMOVE_NOTI,
                    Description = $"Removed expired notification with ID: {id}",
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                await _auditLogRepository.AddRangeAsync(auditLogs);
            }
            else
            {
                _logger.LogInformation("No expired notifications found to remove.");
            }

            await Task.Delay(_notiSettings.IntervalMillis, stoppingToken);
        }
    }
}