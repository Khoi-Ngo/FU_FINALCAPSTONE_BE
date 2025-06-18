using AISEA.ApiService.SHARED.PropConfigs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AISEA.ApiService.SHARED
{
    public static class DependenciesInjection
    {
        public static IServiceCollection AddSharedConfig(this IServiceCollection services, IConfiguration configuration)
        {
            //adding properties configuration
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));
            services.Configure<RedisSettings>(configuration.GetSection(RedisSettings.Section));
            services.Configure<EndpointSettings>(configuration.GetSection(EndpointSettings.Section));
            services.Configure<AuthTokenSettings>(configuration.GetSection(AuthTokenSettings.Section));
            services.Configure<SqlSettings>(configuration.GetSection(SqlSettings.Section));
            services.Configure<GoogleAuthSettings>(configuration.GetSection(GoogleAuthSettings.Section));
            services.Configure<MailSettings>(configuration.GetSection(MailSettings.Section));
            services.Configure<VerifyResetPassCodeSettings>(configuration.GetSection(VerifyResetPassCodeSettings.Section));


            services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<RedisSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<EndpointSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<AuthTokenSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<SqlSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<GoogleAuthSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<MailSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<VerifyResetPassCodeSettings>>().Value);

            return services;
        }
    }
}