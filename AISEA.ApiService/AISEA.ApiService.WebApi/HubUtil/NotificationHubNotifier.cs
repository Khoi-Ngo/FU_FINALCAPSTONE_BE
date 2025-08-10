using AISEA.ApiService.BAL.Services.Notification;
using AISEA.ApiService.SHARED.DTOs.Requests.Noti;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AISEA.ApiService.WebApi.HubUtil;

public class NotificationHubNotifier
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly NotificationService _notificationService;
    private readonly NotificationSettings _notificationSettings;
    private readonly ILogger<NotificationHubNotifier> _logger;

    public NotificationHubNotifier(
        IHubContext<NotificationHub> hubContext,
        NotificationService notificationService,
        NotificationSettings notificationSettings,
        ILogger<NotificationHubNotifier> logger)
    {
        _hubContext = hubContext;
        _notificationService = notificationService;
        _notificationSettings = notificationSettings;
        _logger = logger;
    }

    public async Task NotifyUserAsync(string accessToken, NotificationDTO notificationDTO)
    {
        try
        {
            var (notification, userId) = await _notificationService.CreateAsync(accessToken, notificationDTO);
            var groupName = GetGroupName(userId);
            await _hubContext.Clients.Group(groupName)
                .SendAsync(_notificationSettings.NotificationCreatedMethod, notification);
        }
        catch (Exception e)
        {
            // Log the error
            _logger.LogError(e, "Error while notifying user");
        }
    }

    public async Task NotifyUserAsync(long userToNotify, NotificationDTO notificationDTO)
    {

        try
        {
            var notification = await _notificationService.CreateAsync(userToNotify, notificationDTO);
            var groupName = GetGroupName(userToNotify);
            await _hubContext.Clients.Group(groupName)
                .SendAsync(_notificationSettings.NotificationCreatedMethod, notification);

        }
        catch (Exception e)
        {
            // Log the error
            _logger.LogError(e, "Error while notifying user");
        }
    }

    public async Task NotifyUsersAsync(IEnumerable<(long UserId, NotificationDTO Notification)> notifications)
    {
        try
        {
            var groupedNotifications = notifications
                .GroupBy(n => n.UserId)
                .Select(g => (UserId: g.Key, Notifications: g.Select(n => n.Notification).ToList()))
                .ToList();

            var allTasks = groupedNotifications.Select(async group =>
            {
                var groupName = GetGroupName(group.UserId);

                // Create notifications for this user in parallel
                var createdNotifications = await Task.WhenAll(
                    group.Notifications.Select(n => _notificationService.CreateAsync(group.UserId, n))
                );

                // Send them to the SignalR group
                await _hubContext.Clients.Group(groupName)
                    .SendAsync(_notificationSettings.NotificationCreatedMethod, createdNotifications);
            });

            // Run everything in parallel
            await Task.WhenAll(allTasks);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while notifying users");
        }
    }





    private string GetGroupName(long userId) => $"{_notificationSettings.IndividualUserGroupPrefix}{userId}";
}