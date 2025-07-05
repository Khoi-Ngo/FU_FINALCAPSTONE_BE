using AISEA.ApiService.BAL.Services.Notification;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.SignalR;

namespace AISEA.ApiService.WebApi.Hubs;

public class NotificationHub : BaseHub
{
    private readonly NotificationService _notificationService;
    private readonly NotificationSettings _notificationSettings;

    public NotificationHub(
        EndpointSettings endpointSettings,
        NotificationService notificationService,
        NotificationSettings notificationSettings) : base(endpointSettings)
    {
        _notificationService = notificationService;
        _notificationSettings = notificationSettings;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = await _notificationService.ConnectUserNotificationGroupAsync(AccessToken);
        var groupName = GetGroupName(userId);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await base.OnConnectedAsync();
    }
    public async Task GetNotifications(PaginationRequest request)
    {
        var (notifications, userId) = await _notificationService.GetNotificationsAsync(AccessToken, request);
        var groupName = GetGroupName(userId);
        await Clients.Group(groupName).SendAsync(_notificationSettings.NotificationReceivedMethod, notifications);
    }
    public async Task MarkAsRead(long notificationId)
    {
        var (broadcastedNotiId, userId) = await _notificationService.MarkAsReadAsync(notificationId);
        var groupName = GetGroupName(userId);
        await Clients.Group(groupName).SendAsync(_notificationSettings.NotificationReadMethod, broadcastedNotiId);
    }
    private string GetGroupName(long userId) => $"{_notificationSettings.IndividualUserGroupPrefix}{userId}";
}