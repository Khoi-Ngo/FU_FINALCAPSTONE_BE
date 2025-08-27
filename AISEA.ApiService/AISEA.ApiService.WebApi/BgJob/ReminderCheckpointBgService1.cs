using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Noti;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.HubUtil;

namespace AISEA.ApiService.WebApi.BgJob;

public class ReminderCheckpointBgService1 : BackgroundService
{
    private readonly ILogger<ReminderCheckpointBgService1> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly CourseTrackSettings _courseTrackSettings;

    public ReminderCheckpointBgService1(
        ILogger<ReminderCheckpointBgService1> logger,
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

                List<(long userId, string email, List<JoinedSubjectCheckPoint>)> reminds1 = await checkpointRepo.GetRemindAsync(_courseTrackSettings.DeadlineReminderThresholdHours1, nameof(JoinedSubjectCheckPoint.ReminderSentHours1));
                if (reminds1.Any())
                {
                    foreach (var (userId, email, checkpoints) in reminds1)
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

                    var checkpointIds = reminds1
                        .SelectMany(r => r.Item3)
                        .Select(cp => cp.Id)
                        .ToList();

                    if (checkpointIds.Any())
                    {
                        await checkpointRepo.MarkRemind1SentAsync(checkpointIds);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running ReminderCheckpointBgService");
            }

            await Task.Delay(TimeSpan.FromMinutes(_courseTrackSettings.ReminderIntervalMins), stoppingToken);
        }
    }
}


