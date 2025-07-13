using AISEA.ApiService.SHARED.PropConfigs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AISEA.ApiService.WebApi.Base;

[Authorize]
public class BaseHub : Hub
{
    private readonly EndpointSettings _endpointSettings;

    public BaseHub(EndpointSettings endpointSettings)
    {
        _endpointSettings = endpointSettings;
    }

    protected string AccessToken
    {
        get
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext != null)
            {
                // For SignalR, token is usually passed via query string
                if (httpContext.Request.Query.TryGetValue("access_token", out var queryToken))
                {
                    return queryToken.ToString();
                }
            }
            return string.Empty;
        }
    }


    public override async Task OnConnectedAsync()
    {
        // Optionally handle connection logic here
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // Optionally handle disconnection logic here
        await base.OnDisconnectedAsync(exception);
    }
}