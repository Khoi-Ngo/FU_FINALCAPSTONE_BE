using System.Text;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Noti;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.HubUtil;

namespace AISEA.ApiService.WebApi.BgJob;

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

                var notifications = new List<(long userId, string email, NotificationDTO dto)>();

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
                            notifications.Add((userId, email, new NotificationDTO
                            {
                                Title = "Checkpoint Reminder",
                                Content = $"Your checkpoint \"{cp.Title}\" is due at {cp.Deadline}."
                            }));
                        }
                    }
                }

                if (notifications.Any())
                {
                    var signalRNotifications = notifications.Select(n => (n.userId, n.dto)).ToList();
                    await notifier.NotifyUsersAsync(signalRNotifications);

                    var groupedNotifications = notifications
                        .GroupBy(n => new { n.userId, n.email })
                        .Where(g => !string.IsNullOrEmpty(g.Key.email));

                    foreach (var group in groupedNotifications)
                    {
                        var userEmail = group.Key.email;
                        var htmlBody = BuildNotificationTable(group.Select(n => n.dto));
                        try
                        {
                            await mailService.SendEmailAsync(userEmail, "Checkpoint Reminders", htmlBody);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send email to {Email}", userEmail);
                        }
                    }

                    var checkpointIds = reminds5.SelectMany(r => r.Item3).Select(cp => cp.Id).ToList();
                    if (checkpointIds.Any())
                        await checkpointRepo.MarkRemind5SentAsync(checkpointIds);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running ReminderCheckpointBgService5");
            }

            await Task.Delay(TimeSpan.FromMinutes(_courseTrackSettings.ReminderIntervalMins), stoppingToken);
        }
    }

    private string BuildNotificationTable(IEnumerable<NotificationDTO> notifications)
    {
        var sb = new StringBuilder(notifications.Count() * 150 + 200);
        sb.Append("<table border='1' cellpadding='5' cellspacing='0' style='border-collapse:collapse;font-family:Arial,sans-serif;font-size:14px;'>");
        sb.Append("<thead style='background-color:#f2f2f2;'><tr><th>Title</th><th>Content</th><th>Link</th></tr></thead><tbody>");
        foreach (var n in notifications)
        {
            sb.Append("<tr>");
            sb.Append("<td>").Append(System.Net.WebUtility.HtmlEncode(n.Title ?? "")).Append("</td>");
            sb.Append("<td>").Append(System.Net.WebUtility.HtmlEncode(n.Content ?? "")).Append("</td>");
            sb.Append("<td>");
            if (!string.IsNullOrEmpty(n.Link))
                sb.Append("<a href='").Append(System.Net.WebUtility.HtmlEncode(n.Link)).Append("'>Open</a>");
            sb.Append("</td>");
            sb.Append("</tr>");
        }
        sb.Append("</tbody></table>");
        return sb.ToString();
    }
}
