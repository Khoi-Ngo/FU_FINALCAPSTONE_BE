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

    protected string AccessToken =>
        HttpContextUtil.GetAccessTokenRaw(HttpContext, _endpointSettings);

    protected string RefreshToken =>
        HttpContextUtil.GetRefreshTokenRaw(HttpContext, _endpointSettings);
}
