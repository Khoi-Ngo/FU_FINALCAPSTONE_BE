using AISEA.ApiService.SHARED.PropConfigs;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Base;

public abstract class BaseController : ControllerBase
{
    private readonly EndpointSettings _endpointSettings;

    protected BaseController(EndpointSettings endpointSettings)
    {
        _endpointSettings = endpointSettings;
    }

    protected string AccessToken
    {
        get
        {
            if (Request.Headers.TryGetValue(_endpointSettings.AccessTokenPropName, out var accessToken))
            {
                return accessToken.ToString().Replace("Bearer ", "");
            }
            return string.Empty;
        }
    }

    protected string RefreshToken
    {
        get
        {
            if (Request.Headers.TryGetValue(_endpointSettings.RefreshTokenPropName, out var refreshToken))
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
            if (Request.Headers.TryGetValue(_endpointSettings.GoogleAuthTokenPropName, out var authHeader))
            {
                return authHeader.ToString().Replace("Bearer ", "");
            }
            return string.Empty;
        }
    }
    
    protected string GetAccessTokenFromHeader()
    {
        if (Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var token = authHeader.ToString();
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return token.Substring("Bearer ".Length).Trim();
            }
        }
        return null;
    }
}
