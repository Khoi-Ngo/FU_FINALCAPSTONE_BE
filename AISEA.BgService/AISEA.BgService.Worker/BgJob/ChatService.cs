using AISEA.BgService.DAL.Repositories;
using AISEA.BgService.Worker.Entities;
using AISEA.BgService.Worker.Enums;
using AISEA.BgService.Worker.PropConfig;
using AISEA.BgService.Worker.Repositories;

namespace AISEA.BgService.Worker.BgJob;

public class ChatService : BackgroundService
{
    private readonly ILogger<ChatService> _logger;
    private readonly AdvisorySession1to1Repository _advisorySession1To1Repository;
    private readonly ChatSettings _chatSettings;
    private readonly AuditLogRepository _auditLogRepository;

    public ChatService(ILogger<ChatService> logger, AdvisorySession1to1Repository advisorySession1To1Repository, ChatSettings chatSettings, AuditLogRepository auditLogRepository)
    {
        _logger = logger;
        _advisorySession1To1Repository = advisorySession1To1Repository;
        _chatSettings = chatSettings;
        _auditLogRepository = auditLogRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var removedSessionIds = await _advisorySession1To1Repository.RemoveAllExistedOverDaysAsync(_chatSettings.SessionExpiryDays);

            if (removedSessionIds.Any())
            {
                _logger.LogInformation("Removed expired advisory sessions with IDs: {SessionIds}", string.Join(", ", removedSessionIds));

                // Bulk insert audit logs
                var auditLogs = removedSessionIds.Select(id => new AuditLog
                {
                    Tag = EAuditLogTag.REMOVE_CHATSESSION,
                    Description = $"Removed expired advisory session with ID: {id}",
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                await _auditLogRepository.AddRangeAsync(auditLogs);
            }
            else
            {
                _logger.LogInformation("No expired advisory sessions found to remove.");
            }

            await Task.Delay(_chatSettings.IntervalMillis, stoppingToken);
        }
    }
}
