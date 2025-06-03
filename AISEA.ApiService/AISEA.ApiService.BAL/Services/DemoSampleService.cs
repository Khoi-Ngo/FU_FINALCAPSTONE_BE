using System.Security.Authentication;
using AISEA.ApiService.BAL.Services.Auth;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.SHARED.Services.Auth;
using AISEA.ApiService.SHARED.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AISEA.ApiService.BAL.Services
{
    public class DemoSampleService(JWTService jwtService, HttpContextUserService httpContextUserService, TokenService tokenService, IOptions<EndpointSettings> endpointSettings)
    {
        private readonly JWTService _jwtService = jwtService;
        private readonly HttpContextUserService _httpContextUserService = httpContextUserService;
        private readonly TokenService _tokenService = tokenService;
        private readonly EndpointSettings _endpointSettings = endpointSettings.Value;

        public async Task<object?> DemoLoginWithRedis(string userName, string password)
        {
            //getting user via userName + password
            if (userName != "admin" && password != "admin") throw new InvalidCredentialException("Login failed, please check userName and password again");

            //generating new jwt access token
            var accessToken = _jwtService.GenerateAccessToken(userName);

            //checking the refresh token existed on redis or not via key userName
            var storedToken = await _tokenService.GetRefreshTokenAsync(userName);

            var refreshToken = String.IsNullOrEmpty(storedToken) ? _tokenService.GenerateRefreshToken() : storedToken;

            //saving into the redis no matter new or existed
            await _tokenService.StoreRefreshTokenAsync(userName, refreshToken);

            return new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<object?> DemoRefreshTokenWithRedis()
        {
            var expiredToken = _httpContextUserService.GetAccessTokenRaw();
            var refreshToken = _httpContextUserService.GetRefreshTokenRaw();

            //get the user info from expired access token
            var principal = _jwtService.GetPrincipalFromExpiredToken(expiredToken);
            string userName = JWTTokenUtil.GetValueFromPrincipal(principal, _endpointSettings.UserNameClaimName).ToString();

            // Check if refresh token exists in Redis + refresh token belong to the userName
            var isValid = await _tokenService.IsValidRefreshTokenAsync(userName, refreshToken);
            if (!isValid)
            {
                return new InvalidRefreshToken("Invalid refresh token, the refresh token may be gone or not matched with request");
            }

            // Generate new tokens
            string newRefreshToken = _tokenService.GenerateRefreshToken();
            string newAccessToken = _jwtService.GenerateAccessToken(userName);

            // Update Redis: Adding the new refresh token to overwrite in Redis
            await _tokenService.StoreRefreshTokenAsync(userName, newRefreshToken);


            return new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

        }

        public async Task DemoLogoutWithRedis()
        {
            var accessToken = _httpContextUserService.GetAccessTokenRaw();
            //simply adding the access token to the blacklist
            await _tokenService.BlacklistAccessTokenAsync(accessToken);

        }
    }
}