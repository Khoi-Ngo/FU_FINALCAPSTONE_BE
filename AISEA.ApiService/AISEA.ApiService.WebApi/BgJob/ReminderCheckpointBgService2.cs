using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Noti;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.HubUtil;

namespace AISEA.ApiService.WebApi.BgJob;

public class ReminderCheckpointBgService2 : BackgroundService
{
    private readonly ILogger<ReminderCheckpointBgService2> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly CourseTrackSettings _courseTrackSettings;

    public ReminderCheckpointBgService2(
        ILogger<ReminderCheckpointBgService2> logger,
        IServiceProvider serviceProvider,
        CourseTrackSettings courseTrackSettings)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _courseTrackSettings = courseTrackSettings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var checkpointRepo = scope.ServiceProvider.GetRequiredService<JoinedSubjectCheckPointRepository>();
                var notifier = scope.ServiceProvider.GetRequiredService<NotificationHubNotifier>();
                var mailService = scope.ServiceProvider.GetRequiredService<IMailService>();

                var notifications = new List<(long userId, NotificationDTO dto)>();

                // Use the second reminder threshold
                var reminds2 = await checkpointRepo.GetRemindAsync(
                    _courseTrackSettings.DeadlineReminderThresholdHours2,
                    nameof(JoinedSubjectCheckPoint.ReminderSentHours2)
                );

                if (reminds2.Any())
                {
                    foreach (var (userId, email, checkpoints) in reminds2)
                    {
                        foreach (var cp in checkpoints)
                        {
                            notifications.Add((userId, new NotificationDTO
                            {
                                Title = "Checkpoint Reminder",
                                Content = $"Your checkpoint \"{cp.Title}\" is due at {cp.Deadline}."
                            }));
                        }
                    }
                }

                if (notifications.Any())
                {
                    await notifier.NotifyUsersAsync(notifications);

                    var checkpointIds = reminds2
                        .SelectMany(r => r.Item3)
                        .Select(cp => cp.Id)
                        .ToList();

                    if (checkpointIds.Any())
                    {
                        // Mark the second reminder as sent
                        await checkpointRepo.MarkRemind2SentAsync(checkpointIds);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running ReminderCheckpointBgService2");
            }

            await Task.Delay(TimeSpan.FromMinutes(_courseTrackSettings.ReminderIntervalMins), stoppingToken);
        }
    }
}
