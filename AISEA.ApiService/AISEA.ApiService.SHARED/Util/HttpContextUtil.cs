using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.PropConfigs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AISEA.ApiService.SHARED.Util
{
    public static class HttpContextUtil
    {
        public static string GetAccessTokenRaw(HttpContext context, EndpointSettings settings)
        {
            try
            {
                return context.Request.Headers[settings.AccessTokenPropName].ToString().Replace("Bearer ", "");
            }
            catch
            {
                throw new NotFoundTokenFromClient("No access token found in the request headers.");
            }
        }

        public static string GetRefreshTokenRaw(HttpContext context, EndpointSettings settings)
        {
            try
            {
                return context.Request.Headers[settings.RefreshTokenPropName].ToString();
            }
            catch
            {
                throw new NotFoundTokenFromClient("No refresh token found in the request headers.");
            }
        }

        public static string GetAccessTokenRaw(ActionExecutingContext context, EndpointSettings endpointSettings)
        {
            try
            {
                return context.HttpContext.Request.Headers[endpointSettings.AccessTokenPropName].ToString().Replace("Bearer ", "");
            }
            catch (Exception e)
            {
                throw new NotFoundTokenFromClient("No token found in the request headers. Please provide a valid token.");
            }
        }

    }
}