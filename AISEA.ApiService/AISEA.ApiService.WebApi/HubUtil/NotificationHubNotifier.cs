using AISEA.ApiService.BAL.Services.Notification;
using AISEA.ApiService.SHARED.DTOs.Requests.Noti;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AISEA.ApiService.WebApi.HubUtil;

public class NotificationHubNotifier
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ILogger<NotificationHubNotifier> _logger;

    public NotificationHubNotifier(
        IBackgroundTaskQueue taskQueue,
        ILogger<NotificationHubNotifier> logger)
    {
        _taskQueue = taskQueue;
        _logger = logger;
    }

    public async Task NotifyUserAsync(string accessToken, NotificationDTO notificationDTO)
    {
        try
        {
            _taskQueue.QueueBackgroundWorkItem(async (sp, token) =>
            {
                using var scope = sp.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();
                var notificationSettings = scope.ServiceProvider.GetRequiredService<NotificationSettings>();

                var (notification, userId) = await notificationService.CreateAsync(accessToken, notificationDTO);
                var groupName = GetGroupName(userId, notificationSettings);
                await hubContext.Clients.Group(groupName)
                    .SendAsync(notificationSettings.NotificationCreatedMethod, notification);
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while queuing notification for user");
        }
    }

    public async Task NotifyUserAsync(long userToNotify, NotificationDTO notificationDTO)
    {
        try
        {
            _taskQueue.QueueBackgroundWorkItem(async (sp, token) =>
            {
                using var scope = sp.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();
                var notificationSettings = scope.ServiceProvider.GetRequiredService<NotificationSettings>();

                var notification = await notificationService.CreateAsync(userToNotify, notificationDTO);
                var groupName = GetGroupName(userToNotify, notificationSettings);
                await hubContext.Clients.Group(groupName)
                    .SendAsync(notificationSettings.NotificationCreatedMethod, notification);
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while queuing notification for user");
        }
    }

    public async Task NotifyUsersAsync(IEnumerable<(long UserId, NotificationDTO Notification)> notifications)
    {
        try
        {
            _taskQueue.QueueBackgroundWorkItem(async (sp, token) =>
            {
                using var scope = sp.CreateScope();
                var notificationSettings = scope.ServiceProvider.GetRequiredService<NotificationSettings>();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

                var groupedNotifications = notifications
                    .GroupBy(n => n.UserId)
                    .Select(g => (UserId: g.Key, Notifications: g.Select(n => n.Notification).ToList()))
                    .ToList();

                var allTasks = groupedNotifications.Select(async group =>
                {
                    var groupName = GetGroupName(group.UserId, notificationSettings);

                    // Run all notification saves in parallel, each with its own DbContext
                    var createdNotifications = await Task.WhenAll(
                        group.Notifications.Select(async n =>
                        {
                            using var innerScope = sp.CreateScope();
                            var scopedNotificationService = innerScope.ServiceProvider
                                .GetRequiredService<NotificationService>();
                            return await scopedNotificationService.CreateAsync(group.UserId, n);
                        })
                    );

                    // Send them all at once to SignalR
                    await hubContext.Clients.Group(groupName)
                        .SendAsync(notificationSettings.NotificationCreatedMethod, createdNotifications);
                });

                await Task.WhenAll(allTasks);
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while queuing notifications for users");
        }
    }

    private string GetGroupName(long userId, NotificationSettings notificationSettings)
        => $"{notificationSettings.IndividualUserGroupPrefix}{userId}";
}