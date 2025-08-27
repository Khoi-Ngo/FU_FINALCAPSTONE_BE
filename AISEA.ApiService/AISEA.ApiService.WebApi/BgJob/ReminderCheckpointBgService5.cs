using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Noti;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.HubUtil;

namespace AISEA.ApiService.WebApi.BgJob
{
    public class ReminderCheckpointBgService5 : BackgroundService
    {
        private readonly ILogger<ReminderCheckpointBgService5> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly CourseTrackSettings _courseTrackSettings;

        public ReminderCheckpointBgService5(
            ILogger<ReminderCheckpointBgService5> logger,
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

                    var reminds5 = await checkpointRepo.GetRemindAsync(
                        _courseTrackSettings.DeadlineReminderThresholdHours5,
                        nameof(JoinedSubjectCheckPoint.ReminderSentHours5)
                    );

                    if (reminds5.Any())
                    {
                        foreach (var (userId, email, checkpoints) in reminds5)
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

                        var checkpointIds = reminds5
                            .SelectMany(r => r.Item3)
                            .Select(cp => cp.Id)
                            .ToList();

                        if (checkpointIds.Any())
                        {
                            await checkpointRepo.MarkRemind5SentAsync(checkpointIds);
                        }
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error running ReminderCheckpointBgService5");
                }

                await Task.Delay(TimeSpan.FromMinutes(_courseTrackSettings.ReminderIntervalMins), stoppingToken);
            }
        }
    }
}
