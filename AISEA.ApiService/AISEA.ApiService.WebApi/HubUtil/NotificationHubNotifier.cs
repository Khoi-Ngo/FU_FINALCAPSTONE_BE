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

    public async Task NotifyUser(string accessToken, string title, string content, string link = "Undefined")
    {
        var (notification, userId) = await _notificationService.CreateAsync(accessToken, title, content, link);
        var groupName = GetGroupName(userId);
        await _hubContext.Clients.Group(groupName)
            .SendAsync(_notificationSettings.NotificationCreatedMethod, notification);
    }

    public async Task NotifyUser(long userToNotify, string title, string content, string link = "Undefined")
    {
        var notification = await _notificationService.CreateAsync(userToNotify, title, content, link);
        var groupName = GetGroupName(userToNotify);
        await _hubContext.Clients.Group(groupName)
            .SendAsync(_notificationSettings.NotificationCreatedMethod, notification);
    }


    private string GetGroupName(long userId) => $"{_notificationSettings.IndividualUserGroupPrefix}{userId}";
}