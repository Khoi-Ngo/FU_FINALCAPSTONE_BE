using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AISEA.ApiService.BAL.Services;
using AISEA.ApiService.BAL.Services.Auth;
using AISEA.ApiService.BAL.Services.Chat;
using AISEA.ApiService.BAL.Services.Role;
using AISEA.ApiService.BAL.Services.SystemProfile;
using AISEA.ApiService.BAL.Services.User;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddScoped<ChatBotService>();
            services.AddScoped<StudentProfileService>();
            services.AddScoped<StaffProfileService>();

            //adding business logic mappings profiles
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Add BotMessageQueueService

            //adding business logic validators
            services.AddValidatorsFromAssembly(
             Assembly.GetExecutingAssembly(),
             includeInternalTypes: true
            );
            services.AddFluentValidationAutoValidation();

            return services;
        }
    }
}