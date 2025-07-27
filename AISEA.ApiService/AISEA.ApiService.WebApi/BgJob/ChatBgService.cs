using AISEA.ApiService.BAL.Services.Chat;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.PropConfigs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AISEA.ApiService.WebApi.BgJob;

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
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var advisorySession1to1Service = scope.ServiceProvider.GetRequiredService<AdvisorySession1to1Service>();

                var removedSessionIds = await advisorySession1to1Service.RemoveAllExistedOverDaysAsync(_chatSessionSettings.SessionExpiryDays);

                if (removedSessionIds.Any())
                {
                    _logger.LogInformation("Removed expired chat sessions: {SessionIds}", string.Join(", ", removedSessionIds));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing expired chat sessions.");
            }

            await Task.Delay(_chatSessionSettings.IntervalMillis, stoppingToken);
        }
    }
}
