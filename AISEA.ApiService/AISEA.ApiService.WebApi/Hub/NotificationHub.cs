using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace AISEA.ApiService.Services;

public class NotificationHub : Hub
{
    public async Task SubscribeToNotifications(long userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
    }
}