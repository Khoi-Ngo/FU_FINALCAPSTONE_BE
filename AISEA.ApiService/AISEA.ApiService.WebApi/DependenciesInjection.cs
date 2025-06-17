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


namespace AISEA.ApiService.WebApi
{
    public static class DependenciesInjection
    {
        public static IServiceCollection AddWebApiConfig(this IServiceCollection services, IConfiguration configuration)
        {

            //Integrate JWT into HttpContext User
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                }).AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        //TODO: Enable audience later when frontend is ready
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration.GetSection(JwtSettings.Section)["Issuer"],
                        // ValidAudience = ,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection(JwtSettings.Section)["SecretKey"]))
                    };
                });


            services.AddSwaggerGen(options =>
                {
                    var swaggerSection = configuration.GetSection("Swagger");
                    var securitySchemeName = swaggerSection["SecuritySchemeName"];
                    var headerName = swaggerSection["HeaderName"];
                    var type = Enum.TryParse<SecuritySchemeType>(swaggerSection["Type"], out var parsedType) ? parsedType : SecuritySchemeType.ApiKey;
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
                    // Add XML comments for Swagger
                    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    if (File.Exists(xmlPath))
                    {
                        options.IncludeXmlComments(xmlPath);
                    }
                });





            // Get CORS policy name from configuration
            var corsPolicyName = configuration.GetSection(EndpointSettings.Section)["CORSPolicy"];

            // Add CORS policy
            services.AddCors(options =>
            {
                options.AddPolicy(corsPolicyName,
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    }
                );
            });

            services.AddEndpointsApiExplorer();

            services.AddScoped<BlacklistedTokenFilter>();
            services.AddControllers(opt =>
                {
                    //Protect all APIs by authentication
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    opt.Filters.Add(new AuthorizeFilter(policy));
                    opt.Filters.Add<BlacklistedTokenFilter>();
                    opt.Filters.Add<ModelStateValidationFilter>();


                }).AddJsonOptions(opt =>
                {
                    //option handling json
                    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
                }).ConfigureApiBehaviorOptions(opt =>
                {
                    opt.SuppressModelStateInvalidFilter = true;
                });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();//Context helper

            //adding signalR
            services.AddSignalR();

            return services;
        }
    }
}