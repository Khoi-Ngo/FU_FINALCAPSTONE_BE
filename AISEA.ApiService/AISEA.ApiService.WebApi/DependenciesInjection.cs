using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using AISEA.ApiService.SHARED.PropConfigs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using AISEA.ApiService.SHARED.Filters;
using FluentValidation.AspNetCore;
using System.Collections.Immutable;

namespace AISEA.ApiService.WebApi
{
    public static class DependenciesInjection
    {
        public static IServiceCollection AddWebApiConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.GetSection(JwtSettings.Section)["Issuer"],
                    ValidAudience = configuration.GetSection(JwtSettings.Section)["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration.GetSection(JwtSettings.Section)["SecretKey"]))
                };

                // Support SignalR token from query string
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(token) &&
                            context.HttpContext.Request.Path.StartsWithSegments("/advisoryChat1to1Hub"))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            // Swagger setup
            services.AddSwaggerGen(options =>
            {
                var swaggerSection = configuration.GetSection("Swagger");
                var securitySchemeName = swaggerSection["SecuritySchemeName"];
                var headerName = swaggerSection["HeaderName"];
                var type = Enum.TryParse<SecuritySchemeType>(swaggerSection["Type"], out var parsedType)
                    ? parsedType
                    : SecuritySchemeType.ApiKey;
                var scheme = swaggerSection["Scheme"];
                var bearerFormat = swaggerSection["BearerFormat"];
                var description = swaggerSection["Description"];

                options.AddSecurityDefinition(securitySchemeName, new OpenApiSecurityScheme
                {
                    Name = headerName,
                    Type = type,
                    Scheme = scheme,
                    BearerFormat = bearerFormat,
                    In = ParameterLocation.Header,
                    Description = description
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = securitySchemeName
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // XML comments for Swagger
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            // CORS
            var corsPolicyName = configuration.GetSection(EndpointSettings.Section)["CORSPolicy"];
            var prodClientOrigin = configuration.GetSection(EndpointSettings.Section)["ProdClientOrigin"];
            var devClientOrigin = configuration.GetSection(EndpointSettings.Section)["DevClientOrigin"];

            services.AddCors(options =>
            {
                options.AddPolicy(corsPolicyName, builder =>
                {
                    builder.WithOrigins(prodClientOrigin, devClientOrigin)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services.AddEndpointsApiExplorer();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<BlacklistedTokenFilter>();

            services.AddControllers(opt =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
                opt.Filters.Add<BlacklistedTokenFilter>();
                opt.Filters.Add<ModelStateValidationFilter>();
            })
            .AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
            })
            .ConfigureApiBehaviorOptions(opt =>
            {
                opt.SuppressModelStateInvalidFilter = true;
            });

            // SignalR
            services.AddSignalR();

            return services;
        }
    }
}