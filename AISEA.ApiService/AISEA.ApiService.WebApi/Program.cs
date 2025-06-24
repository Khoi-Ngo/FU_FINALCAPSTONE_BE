using AISEA.ApiService.WebApi;
using AISEA.ApiService.BAL;
using AISEA.ApiService.DAL;
using AISEA.ApiService.SHARED;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.SHARED.Middleware;
using AISEA.ApiService.WebApi.Hubs;

var builder = WebApplication.CreateBuilder(args);
{
    //Adding file json to the configuration if needed

    builder.Services
    .AddWebApiConfig(builder.Configuration)
    .AddBALConfig(builder.Configuration)
    .AddDALConfig(builder.Configuration)
    .AddSharedConfig(builder.Configuration);
}

var app = builder.Build();
{

    if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "AISEA API");
        });
    }

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseStatusCodePages(async context =>
    {
        var response = context.HttpContext.Response;
        if (response.StatusCode == 404)
        {
            response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                status = 404,
                message = "Not Found: The requested resource could not be found."
            });
            await response.WriteAsync(result);
        }

        //more type status below ...
    });
    var corsPolicyName = app.Configuration.GetSection(EndpointSettings.Section)["CORSPolicy"];

    // app.UseHttpsRedirection();
    app.UseCors(corsPolicyName);
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapHub<AdvisoryChat1to1Hub>("/advisoryChat1to1Hub").RequireAuthorization();
    app.MapControllers();
    app.Run();
}

//TODO: Recheck hardcode value