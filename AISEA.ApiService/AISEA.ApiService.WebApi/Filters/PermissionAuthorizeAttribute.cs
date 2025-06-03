using AISEA.ApiService.SHARED.PropConfigs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace AISEA.ApiService.WebApi.Filters;

public class PermissionAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly int[] _roles;

    public PermissionAuthorizeAttribute(params int[] roles)
    {
        _roles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var endpointSettings = context.HttpContext.RequestServices
            .GetRequiredService<IOptions<EndpointSettings>>().Value;

        var strRole =  context.HttpContext.User.FindFirst(endpointSettings.AuthorPropName)?.Value;
        if (String.IsNullOrEmpty(strRole) || !_roles.Contains(Int32.Parse(strRole)))
        {
            context.Result = new ForbidResult();
        }
    }
}