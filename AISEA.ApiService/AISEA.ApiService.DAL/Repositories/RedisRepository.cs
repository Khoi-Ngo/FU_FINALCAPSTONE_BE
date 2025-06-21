using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.SHARED.PropConfigs;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AISEA.ApiService.DAL.Repositories;

public interface IRedisRepository
{
    Task<bool> SetValueAsync(string key, string value, TimeSpan expiry);
    Task<bool> RemoveByKeyAsync(string key);
    Task<bool> KeyExistsAsync(string key);
    Task<bool> ValueExistsAsync(string value);
    Task<string> GetValueAsync(string key);
    Task<bool> SetValueAsync<T>(string key, T value, TimeSpan expiry);
    Task<T?> GetValueAsync<T>(string key);
}

public class AppRedisRepository : IRedisRepository
{


    private readonly IDatabase _database;
    private readonly JsonSerializerOptions _jsonOptions;

    public AppRedisRepository(IDatabase database, IOptions<JsonSerializerOptions> jsonOptions)
    {
        _database = database;
        _jsonOptions = jsonOptions.Value;
    }
    public async Task<bool> SetValueAsync<T>(string key, T value, TimeSpan expiry)
    {
        var json = JsonSerializer.Serialize(value, _jsonOptions); // Use custom options
        return await _database.StringSetAsync(key, json, expiry);
    }

    public async Task<T?> GetValueAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(key);
        return value.HasValue ? JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions) : default;
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

}