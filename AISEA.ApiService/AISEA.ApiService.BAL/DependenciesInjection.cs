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
            //adding business logic service for use-cases
            services.AddScoped<DemoSampleService>();

            //adding business logic mappings profiles
            //TODO: replace with the exact and accurate folder containing mappings profiles those should be located in BAL Layer instead of scanning all assemblies
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            return services;
        }
    }
}