using AISEA.ApiService.DAL.Infrastructure;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AISEA.ApiService.DAL;

public static class DependenciesInjection
{
    public static IServiceCollection AddDALConfig(this IServiceCollection services, IConfiguration configuration)
    {
        #region DBContext
        services.AddDbContext<AiseaContext>(options =>
        {
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
        #endregion

        #region Repositories
        services.AddScoped<RoleRepository>();
        services.AddScoped<UserRepository>();
        #endregion

        #region  Redis
        services.AddSingleton<IConnectionMultiplexer>(sp =>
                    {
                        var redisSettings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
                        var redisConnectionString = redisSettings.ConnectionString;

                        if (string.IsNullOrEmpty(redisConnectionString))
                        {
                            throw new InvalidOperationException("Redis connection string is missing or empty.");
                        }

                        var configurationOptions = ConfigurationOptions.Parse(redisConnectionString);
                        configurationOptions.AbortOnConnectFail = false; // Prevent crash on connection failure
                        configurationOptions.ConnectTimeout = 10000; // Optional: Increase timeout
                        configurationOptions.ConnectRetry = 5; // Optional: Retry connection

                        return ConnectionMultiplexer.Connect(configurationOptions);
                    });

        services.AddScoped<IDatabase>(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

        services.AddScoped<IRedisRepository, AppRedisRepository>();
        #endregion

        //Service Agents

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IJWTService, JWTService>();
        services.AddScoped<IMailService, MailService>();
        services.AddScoped<IChatOpenAIService, ChatOpenAIService>();

        return services;
    }
}