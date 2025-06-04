using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.BAL.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AISEA.ApiService.BAL
{
    public static class DependenciesInjection
    {
        public static IServiceCollection AddBALConfig(this IServiceCollection services, IConfiguration configuration)
        {
            //adding business logic service
            services.AddScoped<DemoSampleService>();

            return services;
        }
    }
}