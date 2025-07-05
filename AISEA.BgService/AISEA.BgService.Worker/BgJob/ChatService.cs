using AISEA.BgService.Worker.PropConfig;
using AISEA.BgService.Worker.Repositories;

namespace AISEA.BgService.Worker.BgJob;

public class ChatService : BackgroundService
{
    private readonly ILogger<ChatService> _logger;
    private readonly AdvisorySession1to1Repository _advisorySession1To1Repository;
    private readonly ChatSettings _chatSettings;

    public ChatService(ILogger<ChatService> logger, AdvisorySession1to1Repository advisorySession1To1Repository, ChatSettings chatSettings)
    {
        _logger = logger;
        _advisorySession1To1Repository = advisorySession1To1Repository;
        _chatSettings = chatSettings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var removedSessionIds = await _advisorySession1To1Repository.RemoveAllExistedOverDaysAsync(_chatSettings.SessionExpiryDays);

            if (removedSessionIds.Any())
            {
                _logger.LogInformation("Removed expired advisory sessions with IDs: {SessionIds}", string.Join(", ", removedSessionIds));
            }
            else
            {
                _logger.LogInformation("No expired advisory sessions found to remove.");
            }

            await Task.Delay(_chatSettings.IntervalMillis, stoppingToken);
        }
    }
}
