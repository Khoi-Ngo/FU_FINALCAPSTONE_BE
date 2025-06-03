using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.PropConfigs;

namespace AISEA.ApiService.BAL.Services.Auth
{
    public class TokenService
    {
        private readonly AuthTokenSettings _authTokenSettings;
        private readonly IAppRedisRepository _appRedisRepository;

        public TokenService(AuthTokenSettings authTokenSettings, IAppRedisRepository appRedisRepository)
        {
            _authTokenSettings = authTokenSettings;
            _appRedisRepository = appRedisRepository;
        }


        //validate access token
        public async Task<bool> IsValidAccessTokenAsync(string accessToken)
        {

            var key = $"{_authTokenSettings.KeyPrefExpireAccessToken}:{accessToken}";
            var isExisted = await _appRedisRepository.KeyExistsAsync(key);
            return !isExisted;
        }

        //validate refresh token
        public async Task<bool> IsValidRefreshTokenAsync(string userName, string refreshToken)
        {
            var key = $"{_authTokenSettings.KeyPrefRefreshToken}:{userName}";
            var isExisted = await _appRedisRepository.KeyExistsAsync(key);
            return isExisted && (await _appRedisRepository.GetValueAsync(key)) == refreshToken;
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
        public async Task<string> GetRefreshTokenAsync(string userName)
        {
            var key = $"{_authTokenSettings.KeyPrefRefreshToken}:{userName}";
            return await _appRedisRepository.GetValueAsync(key);
        }

        //save refresh token
        public async Task StoreRefreshTokenAsync(string userName, string refreshToken)
        {
            var key = $"{_authTokenSettings.KeyPrefRefreshToken}:{userName}";
            await _appRedisRepository.SetValueAsync(key, refreshToken, TimeSpan.FromDays(_authTokenSettings.ExpireRefreshTokenDay));
        }

        //add access token to the black list
        public async Task BlacklistAccessTokenAsync(string accessToken)
        {
            var key = $"{_authTokenSettings.KeyPrefExpireAccessToken}:{accessToken}";
            await _appRedisRepository.SetValueAsync(key, _authTokenSettings.FormatValueExpireToken, TimeSpan.FromMilliseconds(_authTokenSettings.ExpireAccTokenMilli));
        }

    }
}