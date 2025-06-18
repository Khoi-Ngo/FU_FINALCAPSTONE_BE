using System.Security.Cryptography;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;

namespace AISEA.ApiService.DAL.Infrastructure
{
    public class TokenService : ITokenService
    {
        private readonly AuthTokenSettings _authTokenSettings;
        private readonly IRedisRepository _redisRepository;

        public TokenService(AuthTokenSettings authTokenSettings, IRedisRepository redisRepository)
        {
            _authTokenSettings = authTokenSettings;
            _redisRepository = redisRepository;
        }


        //validate access token
        public async Task<bool> IsValidAccessTokenAsync(string accessToken)
        {
            var isExisted = await _redisRepository.IsAccessTokenExisted(accessToken);
            return !isExisted;
        }

        //validate refresh token
        public async Task<bool> IsValidRefreshTokenAsync(string username, string refreshToken)
        {
            var isUserNameExisted = await _redisRepository.IsUsernameExisted(username);
            return isUserNameExisted && (await _redisRepository.GetRefreshTokenAsync(username)) == refreshToken;
        }

        //Generate the refresh token
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        //get refresh token
        public async Task<string> GetRefreshTokenAsync(string username)
        {
            return await _redisRepository.GetRefreshTokenAsync(username);
        }

        //save refresh token
        public async Task StoreRefreshTokenAsync(string username, string refreshToken)
        {
            await _redisRepository.StoreRefreshTokenAsync(username, refreshToken, TimeSpan.FromDays(_authTokenSettings.ExpireRefreshTokenDay));
        }

        //add access token to the black list
        public async Task BlacklistAccessTokenAsync(string accessToken)
        {
            await _redisRepository.BlacklistAccessTokenAsync(accessToken, TimeSpan.FromMilliseconds(_authTokenSettings.ExpireAccTokenMilli));
        }

    }
}