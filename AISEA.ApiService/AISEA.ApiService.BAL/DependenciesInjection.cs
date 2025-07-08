using System.Reflection;
using AISEA.ApiService.BAL.Services.Auth;
using AISEA.ApiService.BAL.Services.Chat;
using AISEA.ApiService.BAL.Services.Role;
using AISEA.ApiService.BAL.Services.Subject;
using AISEA.ApiService.BAL.Services.Syllabus;
using AISEA.ApiService.BAL.Services.SystemProfile;
using AISEA.ApiService.BAL.Services.User;
using AISEA.ApiService.BAL.Services.Curriculum;
using AISEA.ApiService.BAL.Services.Combo;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AISEA.ApiService.BAL.Services.Notification;
using AISEA.ApiService.BAL.Services.AuditLog;
using AISEA.ApiService.BAL.Services.BgJob;
using AISEA.ApiService.BAL.Services.Booking;

namespace AISEA.ApiService.BAL
{
    public static class DependenciesInjection
    {
        public static IServiceCollection AddBALConfig(this IServiceCollection services, IConfiguration configuration)
        {
            //adding http client factory
            services.AddHttpClient();
            //adding business logic service for use-cases
            services.AddScoped<AuthService>();
            services.AddScoped<UserService>();
            services.AddScoped<RoleService>();
            services.AddScoped<StudentProfileService>();
            services.AddScoped<StaffProfileService>();
            services.AddScoped<AdvisorySession1to1Service>();
            services.AddScoped<SubjectService>();
            services.AddScoped<SyllabusService>();
            services.AddScoped<CurriculumService>();
            services.AddScoped<ComboService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<AuditLogService>();
            services.AddScoped<BookingAvailabilityService>();
            services.AddScoped<LeaveScheduleService>();
            services.AddScoped<BookedMeetingService>();

            //adding business logic mappings profiles
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Add BotMessageQueueService

            //adding business logic validators
            services.AddValidatorsFromAssembly(
             Assembly.GetExecutingAssembly(),
             includeInternalTypes: true
            );
            services.AddFluentValidationAutoValidation();

            //adding background jobs
            services.AddHostedService<NotiBgService>();
            services.AddHostedService<ChatBgService>();

            return services;
        }
    }
}