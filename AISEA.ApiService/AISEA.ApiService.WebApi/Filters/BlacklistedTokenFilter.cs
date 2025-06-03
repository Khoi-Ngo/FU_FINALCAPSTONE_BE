using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.BAL.Services.Auth;
using AISEA.ApiService.SHARED.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AISEA.ApiService.WebApi.Filters;

public class BlacklistedTokenFilter : IAsyncActionFilter
{
    private readonly TokenService _tokenService;
    private readonly HttpContextUserService _httpContextUserService;

    public BlacklistedTokenFilter(TokenService tokenService, HttpContextUserService httpContextUserService)
    {
        _tokenService = tokenService;
        _httpContextUserService = httpContextUserService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Skip filter if endpoint allows anonymous access
        var endpoint = context.HttpContext.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
        {
            await next();
            return;
        }

        var accessToken = _httpContextUserService.GetAccessTokenRaw();
        if (!string.IsNullOrEmpty(accessToken))
        {
            var isValidToken = await _tokenService.IsValidAccessTokenAsync(accessToken);
            if (!isValidToken)
            {
                context.Result = new UnauthorizedObjectResult("Access token is blacklisted.");
                return;
            }
        }

        //logic never touch below
        await next();
    }
}