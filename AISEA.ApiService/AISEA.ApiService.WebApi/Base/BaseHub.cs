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
                if (httpContext.Request.Headers.TryGetValue(_endpointSettings.AccessTokenPropName, out var accessToken))
                {
                    return accessToken.ToString().Replace("Bearer ", "");
                }
            }
            return string.Empty;
        }
    }

    protected string RefreshToken
    {
        get
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext != null &&
                httpContext.Request.Headers.TryGetValue(_endpointSettings.RefreshTokenPropName, out var refreshToken))
            {
                return refreshToken.ToString();
            }
            return string.Empty;
        }
    }

    protected string AuthorizationTokenGoogle
    {
        get
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext != null &&
                httpContext.Request.Headers.TryGetValue(_endpointSettings.GoogleAuthTokenPropName, out var authHeader))
            {
                return authHeader.ToString().Replace("Bearer ", "");
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