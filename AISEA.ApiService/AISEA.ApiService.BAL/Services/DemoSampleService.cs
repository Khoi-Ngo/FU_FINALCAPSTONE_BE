using System.Security.Authentication;
using AISEA.ApiService.BAL.Interfaces;
using AISEA.ApiService.DAL.Infrastructure;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.SHARED.Util;
using Microsoft.Extensions.Options;

namespace AISEA.ApiService.BAL.Services
{
    public class DemoSampleService : IDemoSampleService
    {

        private readonly IJWTService _jwtService;
        private readonly ITokenService _tokenService;
        private readonly EndpointSettings _endpointSettings;
        private readonly JwtSettings _jwtSettings;

        public DemoSampleService(
            IJWTService jwtService,
            ITokenService tokenService,
            IOptions<EndpointSettings> endpointSettings,
            IOptions<JwtSettings> jwtSettings)
        {
            _jwtService = jwtService;
            _tokenService = tokenService;
            _endpointSettings = endpointSettings.Value;
            _jwtSettings = jwtSettings.Value;
        }

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

        public async Task<object?> DemoRefreshTokenWithRedis(string expiredToken, string refreshToken)
        {
         
            //get the user info from expired access token
            var principal = JWTTokenUtil.GetPrincipalFromExpiredToken(expiredToken, _jwtSettings);
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

        public async Task DemoLogoutWithRedis(string accessToken)
        {
            //simply adding the access token to the blacklist
            await _tokenService.BlacklistAccessTokenAsync(accessToken);

        }
    }
}