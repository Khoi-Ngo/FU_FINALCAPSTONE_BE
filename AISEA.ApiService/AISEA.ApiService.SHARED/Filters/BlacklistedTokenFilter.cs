using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.SHARED.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace AISEA.ApiService.SHARED.Filters;

public class BlacklistedTokenFilter : IAsyncActionFilter
{
    private readonly ITokenService _tokenService;
    private readonly EndpointSettings _endpointSettings;

    public BlacklistedTokenFilter(ITokenService tokenService, EndpointSettings endpointSettings)
    {
        _tokenService = tokenService;
        _endpointSettings = endpointSettings;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var endpoint = context.HttpContext.GetEndpoint();

        // Get route template from ActionDescriptor
        var routeTemplate = context.ActionDescriptor.AttributeRouteInfo?.Template;

        // Check if this is the refresh-token endpoint
        bool isRefreshTokenEndpoint = routeTemplate != null &&
            routeTemplate.Contains(_endpointSettings.RefreshTokenEndpointName, StringComparison.OrdinalIgnoreCase);

        // Skip filter if endpoint allows anonymous access, except for refresh-token
        if (!isRefreshTokenEndpoint && endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
        {
            await next();
            return;
        }

        var accessToken = context.HttpContext.Request.Headers[_endpointSettings.AccessTokenPropName].FirstOrDefault()?.Replace("Bearer ", "");
        if (!string.IsNullOrEmpty(accessToken))
        {
            var isValidToken = await _tokenService.IsValidAccessTokenAsync(accessToken);
            if (!isValidToken)
            {
                context.Result = new UnauthorizedObjectResult("Access token is blacklisted.");
                return;
            }
        }

        await next();
    }
}