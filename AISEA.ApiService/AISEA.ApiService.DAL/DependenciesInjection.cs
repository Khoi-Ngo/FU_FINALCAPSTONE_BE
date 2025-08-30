using System.Text.Json;
using System.Text.Json.Serialization;
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
            options.EnableSensitiveDataLogging(true);
        });
        #endregion

        #region Repositories

        services.AddScoped<RoleRepository>();
        services.AddScoped<UserRepository>();
        services.AddScoped<AdvisorySession1to1Repository>();
        services.AddScoped<MessageRepository>();
        services.AddScoped<NotificationRepository>();
        services.AddScoped<StaffProfileRepository>();
        services.AddScoped<StudentProfileRepository>();
        services.AddScoped<SubjectRepository>();
        services.AddScoped<SubjectVersionRepository>();
        services.AddScoped<SubjectVersionPrerequisiteRepository>();
        services.AddScoped<SyllabusRepository>();
        services.AddScoped<SyllabusAssessmentRepository>();
        services.AddScoped<SyllabusLearningMaterialRepository>();
        services.AddScoped<SyllabusLearningOutcomeRepository>();
        services.AddScoped<SyllabusSessionRepository>();
        services.AddScoped<SessionOutcomeMappingRepository>();
        services.AddScoped<CurriculumRepository>();
        services.AddScoped<CurriculumSubjectRepository>();
        services.AddScoped<ComboRepository>();
        services.AddScoped<ComboSubjectRepository>();
        services.AddScoped<ProgramRepository>();
        services.AddScoped<AuditLogRepository>();
        services.AddScoped<BookingAvailabilityRepository>();
        services.AddScoped<LeaveScheduleRepository>();
        services.AddScoped<BookedMeetingRepository>();
        services.AddScoped<SemesterRepository>();
        services.AddScoped<JoinedSubjectRepository>();
        services.AddScoped<JoinedSubjectCheckPointRepository>();

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
        services.AddScoped<IHolidayService, HolidayService>();
        services.AddHttpClient<IHolidayService, HolidayService>();



        services.Configure<JsonSerializerOptions>(options =>
        {
            options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            // options.WriteIndented = false;
        });


        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

    

        return services;
    }
}