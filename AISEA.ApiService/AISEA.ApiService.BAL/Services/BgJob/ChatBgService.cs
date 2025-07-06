using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.PropConfigs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AISEA.ApiService.BAL.Services.BgJob;

public class ChatBgService : BackgroundService
{
    private readonly ILogger<ChatBgService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ChatSessionSettings _chatSessionSettings;

    public ChatBgService(
        ILogger<ChatBgService> logger,
        IServiceProvider serviceProvider,
        ChatSessionSettings chatSessionSettings)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _chatSessionSettings = chatSessionSettings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var advisorySession1To1Repository = scope.ServiceProvider.GetRequiredService<AdvisorySession1to1Repository>();
                var auditLogRepository = scope.ServiceProvider.GetRequiredService<AuditLogRepository>();

                var removedSessionIds = await advisorySession1To1Repository.RemoveAllExistedOverDaysAsync(_chatSessionSettings.SessionExpiryDays);

                if (removedSessionIds.Any())
                {
                    _logger.LogInformation("Removed expired advisory sessions with IDs: {SessionIds}", string.Join(", ", removedSessionIds));

                    var auditLogs = removedSessionIds.Select(id => new DAL.Entities.AuditLog
                    {
                        Tag = EAuditLogTag.REMOVE_CHATSESSION,
                        Description = $"Removed expired advisory session with ID: {id}",
                        CreatedAt = DateTime.UtcNow
                    }).ToList();

                    await auditLogRepository.AddRangeAsync(auditLogs);
                }
                else
                {
                    _logger.LogInformation("No expired advisory sessions found to remove.");
                }
            }

            await Task.Delay(_chatSessionSettings.IntervalMillis, stoppingToken);
        }
    }
}