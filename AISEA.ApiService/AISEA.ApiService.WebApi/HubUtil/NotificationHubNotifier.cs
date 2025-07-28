using AISEA.ApiService.BAL.Services.Notification;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AISEA.ApiService.WebApi.HubUtil;

public class NotificationHubNotifier
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly NotificationService _notificationService;
    private readonly NotificationSettings _notificationSettings;

    public NotificationHubNotifier(
        IHubContext<NotificationHub> hubContext,
        NotificationService notificationService,
        NotificationSettings notificationSettings)
    {
        _hubContext = hubContext;
        _notificationService = notificationService;
        _notificationSettings = notificationSettings;
    }

    public async Task NotifyUserAsync(string accessToken, string title, string content, string link = "Undefined")
    {
        var (notification, userId) = await _notificationService.CreateAsync(accessToken, title, content, link);
        var groupName = GetGroupName(userId);
        await _hubContext.Clients.Group(groupName)
            .SendAsync(_notificationSettings.NotificationCreatedMethod, notification);
    }

    public async Task NotifyUserAsync(long userToNotify, string title, string content, string link = "Undefined")
    {
        var notification = await _notificationService.CreateAsync(userToNotify, title, content, link);
        var groupName = GetGroupName(userToNotify);
        await _hubContext.Clients.Group(groupName)
            .SendAsync(_notificationSettings.NotificationCreatedMethod, notification);
    }

    public async Task NotifyUsersAsync(IEnumerable<(long UserId, string Title, string Content)> notifications)
    {
        var groupedNotifications = notifications
            .GroupBy(n => n.UserId)
            .Select(g => (UserId: g.Key, Notifications: g.Select(n => (n.Title, n.Content, Link: "Undefined")).ToList()))
            .ToList();

        var tasks = new List<Task>();
        foreach (var group in groupedNotifications)
        {
            var groupName = GetGroupName(group.UserId);
            var notificationTasks = group.Notifications.Select(n =>
                _notificationService.CreateAsync(group.UserId, n.Title, n.Content, n.Link))
                .ToList();
            var createdNotifications = await Task.WhenAll(notificationTasks);

            tasks.Add(_hubContext.Clients.Group(groupName)
                .SendAsync(_notificationSettings.NotificationCreatedMethod, createdNotifications));
        }

        await Task.WhenAll(tasks);
    }


    public async Task NotifyUsersAsync(IEnumerable<(long UserId, string Title, string Content, string link)> notifications)
    {
        throw new NotImplementedException();
    }



    private string GetGroupName(long userId) => $"{_notificationSettings.IndividualUserGroupPrefix}{userId}";
}