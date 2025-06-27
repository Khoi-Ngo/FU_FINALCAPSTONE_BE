using AISEA.ApiService.WebApi;
using AISEA.ApiService.BAL;
using AISEA.ApiService.DAL;
using AISEA.ApiService.SHARED;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.SHARED.Middleware;
using AISEA.ApiService.WebApi.Hubs;

var builder = WebApplication.CreateBuilder(args);
{
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
    var AdvisoryHubEndpoint = app.Configuration.GetSection(EndpointSettings.Section)["AdvisoryHubEndpoint"];

    // app.UseHttpsRedirection();
    app.UseCors(corsPolicyName);
    app.UseAuthentication();
    app.UseAuthorization();
    //mapping signalR hubs
    app.MapHub<AdvisoryChat1to1Hub>(AdvisoryHubEndpoint).RequireAuthorization();
    app.MapControllers();
    app.Run();
}
