using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.SHARED.PropConfigs;
using Microsoft.AspNetCore.Http;

namespace AISEA.ApiService.SHARED.Services.Auth
{
    public class HttpContextUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EndpointSettings _endpointSettings;

        public HttpContextUserService(IHttpContextAccessor httpContextAccessor, EndpointSettings endpointSettings)
        {
            _httpContextAccessor = httpContextAccessor;
            _endpointSettings = endpointSettings;
        }

        //get the access token
        public string GetAccessTokenRaw()
        {
            // token = "Bearer <actual_token>"
            return _httpContextAccessor.HttpContext.Request.Headers[_endpointSettings.AccessTokenPropName].ToString().Replace("Bearer ", "");
        }

        //get the refresh token
        public string GetRefreshTokenRaw()
        {
            return _httpContextAccessor.HttpContext.Request.Headers[_endpointSettings.RefreshTokenPropName].ToString();
        }
    }
}