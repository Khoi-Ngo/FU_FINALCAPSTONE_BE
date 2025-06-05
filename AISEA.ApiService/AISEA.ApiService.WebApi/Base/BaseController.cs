using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.SHARED.Util;
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
            if (Request.Headers.TryGetValue(_endpointSettings.RefreshTokenPropName, out var refreshToken))
            {
                return refreshToken.ToString().Replace("Bearer ", "");
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
                return refreshToken.ToString().Replace("Bearer ", "");
            }
            return string.Empty;
        }
    }
}
