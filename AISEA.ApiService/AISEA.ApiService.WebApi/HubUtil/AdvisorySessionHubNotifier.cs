using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AISEA.ApiService.WebApi.HubUtil;

public class AdvisorySessionHubNotifier
{
    private readonly IHubContext<AdvisoryChat1to1Hub> _hubContext;
    private readonly ChatSessionSettings _chatSessionSettings;
    private readonly StaffUserSettings _staffUserSettings;

    public AdvisorySessionHubNotifier(
        IHubContext<AdvisoryChat1to1Hub> hubContext,
        ChatSessionSettings chatSessionSettings,
        StaffUserSettings staffUserSettings)
    {
        _hubContext = hubContext;
        _chatSessionSettings = chatSessionSettings;
        _staffUserSettings = staffUserSettings;
    }

    public Task NotifySessionDeletedAsync(long sessionId, long staffId, long studentId)
    {
        return Task.WhenAll(
            _hubContext.Clients.Group($"{_chatSessionSettings.MulDataSessionsPrefixStaff}{staffId}")
                .SendAsync(_chatSessionSettings.SessionDeletedMethod, sessionId),
            _hubContext.Clients.Group($"{_chatSessionSettings.MulDataSessionsPrefixStudent}{studentId}")
                .SendAsync(_chatSessionSettings.SessionDeletedMethod, sessionId),
            _hubContext.Clients.Group($"{_chatSessionSettings.GroupChatADVssPrefix}{sessionId}")
                .SendAsync(_chatSessionSettings.SessionDeletedMethod, sessionId)
        );
    }

    public Task NotifySessionCreatedAsync(long studentProfileId, object hubRes)
    {
        return Task.WhenAll(
            _hubContext.Clients.Group($"{_chatSessionSettings.MulDataSessionsPrefixStaff}{_staffUserSettings.EmptyStaffProfileId}")
                .SendAsync(_chatSessionSettings.SessionCreatedMethod, hubRes),
            _hubContext.Clients.Group($"{_chatSessionSettings.MulDataSessionsPrefixStudent}{studentProfileId}")
                .SendAsync(_chatSessionSettings.SessionCreatedMethod, hubRes)
        );
    }
}