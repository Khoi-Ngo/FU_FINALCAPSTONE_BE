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
            var key = $"{_authTokenSettings.KeyPrefExpireAccessToken}:{accessToken}";
            var isKeyExisted = await _redisRepository.KeyExistsAsync(key);
            return !isKeyExisted;
        }

        //validate refresh token
        public async Task<bool> IsValidRefreshTokenAsync(string username, string refreshToken)
        {
            var key = $"{_authTokenSettings.KeyPrefRefreshToken}:{username}";
            var isKeyExisted = await _redisRepository.KeyExistsAsync(key);
            return isKeyExisted && (await _redisRepository.GetValueAsync(key)) == refreshToken;
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
            var key = $"{_authTokenSettings.KeyPrefRefreshToken}:{username}";
            return await _redisRepository.GetValueAsync(key);
        }

        //save refresh token
        public async Task StoreRefreshTokenAsync(string username, string refreshToken)
        {
            var key = $"{_authTokenSettings.KeyPrefRefreshToken}:{username}";
            await _redisRepository.SetValueAsync(key, refreshToken, TimeSpan.FromDays(_authTokenSettings.ExpireRefreshTokenDay));
        }

        //add access token to the black list
        public async Task BlacklistAccessTokenAsync(string accessToken)
        {
            var key = $"{_authTokenSettings.KeyPrefExpireAccessToken}:{accessToken}";
            await _redisRepository.SetValueAsync(key, accessToken, TimeSpan.FromMilliseconds(_authTokenSettings.ExpireAccTokenMilli));
        }

    }
}