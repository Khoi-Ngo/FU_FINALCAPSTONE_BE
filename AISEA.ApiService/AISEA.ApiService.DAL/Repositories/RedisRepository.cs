using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.SHARED.PropConfigs;
using StackExchange.Redis;

namespace AISEA.ApiService.DAL.Repositories;

public interface IRedisRepository
{
    Task<bool> SetValueAsync(string key, string value, TimeSpan expiry);
    Task<bool> RemoveByKeyAsync(string key);
    Task<bool> KeyExistsAsync(string key);
    Task<bool> ValueExistsAsync(string value);
    Task<string> GetValueAsync(string key);
    Task<bool> IsAccessTokenExisted(string accessToken);
    Task<bool> IsUsernameExisted(string username);
    Task<string> GetRefreshTokenAsync(string username);
    Task BlacklistAccessTokenAsync(string accessToken, TimeSpan expiry);
    Task StoreRefreshTokenAsync(string username, string refreshToken, TimeSpan timeSpan);
    Task SaveVerifyResetCodeAsync(string email, string verificationCode, TimeSpan expiry);
    Task<bool> IsValidVerifyResetCodeAsync(string email, string verificationCode);
    Task RemoveVerifyResetCodeAsync(string email);
}

public class AppRedisRepository : IRedisRepository
{


    private readonly IDatabase _database;
    private readonly RedisSettings _redisSettings;

    public AppRedisRepository(IDatabase database, RedisSettings redisSettings)
    {
        _database = database;
        _redisSettings = redisSettings;
    }

    // Add or update a key-value pair
    public async Task<bool> SetValueAsync(string key, string value, TimeSpan expiry)
    {
        return await _database.StringSetAsync(key, value, expiry);
    }

    // Remove by key
    public async Task<bool> RemoveByKeyAsync(string key)
    {
        return await _database.KeyDeleteAsync(key);
    }

    // Check if key exists
    public async Task<bool> KeyExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }

    // Check if value exists (search all keys, not efficient for large datasets)
    public async Task<bool> ValueExistsAsync(string value)
    {
        var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints()[0]);
        foreach (var key in server.Keys())
        {
            if ((await _database.StringGetAsync(key)) == value)
                return true;
        }
        return false;
    }

    // Get value by key
    public async Task<string> GetValueAsync(string key)
    {
        var value = await _database.StringGetAsync(key);
        return value.ToString();
    }

    public async Task<bool> IsAccessTokenExisted(string accessToken)
    {
        var key = $"{_redisSettings.KeyPrefExpireAccessToken}:{accessToken}";
        return await _database.KeyExistsAsync(key);
    }
    public async Task<bool> IsUsernameExisted(string username)
    {
        var key = $"{_redisSettings.KeyPrefRefreshToken}:{username}";
        return await _database.KeyExistsAsync(key);
    }
    public async Task<string> GetRefreshTokenAsync(string username)
    {
        var key = $"{_redisSettings.KeyPrefRefreshToken}:{username}";
        return (await _database.StringGetAsync(key)).ToString();
    }

    public Task BlacklistAccessTokenAsync(string accessToken, TimeSpan expiry)
    {
        var key = $"{_redisSettings.KeyPrefExpireAccessToken}:{accessToken}";
        return _database.StringSetAsync(key, _redisSettings.FormatValueExpireToken, expiry);
    }

    public async Task StoreRefreshTokenAsync(string username, string refreshToken, TimeSpan timeSpan)
    {
        var key = $"{_redisSettings.KeyPrefRefreshToken}:{username}";
        await _database.StringSetAsync(key, refreshToken, timeSpan);
    }

    public Task SaveVerifyResetCodeAsync(string email, string verificationCode, TimeSpan expiry)
    {
        var key = $"{_redisSettings.KeyPrefVerificationResetPassCode}:{email}";
        return _database.StringSetAsync(key, verificationCode, expiry);
    }

    public Task<bool> IsValidVerifyResetCodeAsync(string email, string verificationCode)
    {
        var key = $"{_redisSettings.KeyPrefVerificationResetPassCode}:{email}";
        return _database.StringGetAsync(key).ContinueWith(task =>
        {
            var value = task.Result.ToString();
            return value == verificationCode;
        });
    }

    public Task RemoveVerifyResetCodeAsync(string email)
    {
        var key = $"{_redisSettings.KeyPrefVerificationResetPassCode}:{email}";
        return _database.KeyDeleteAsync(key);
    }
}