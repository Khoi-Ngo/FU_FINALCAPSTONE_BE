using AISEA.BgService.DAL.Repositories;
using AISEA.BgService.Worker.BgJob;
using AISEA.BgService.Worker.Persistence;
using AISEA.BgService.Worker.PropConfig;
using AISEA.BgService.Worker.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);
#region Register Service
builder.Services.AddHostedService<ChatService>();

builder.Services.Configure<SqlSettings>(builder.Configuration.GetSection(SqlSettings.Section));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<SqlSettings>>().Value);

builder.Services.Configure<ChatSettings>(builder.Configuration.GetSection(ChatSettings.Section));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<ChatSettings>>().Value);


builder.Services.Configure<NotiSettings>(builder.Configuration.GetSection(NotiSettings.Section));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<NotiSettings>>().Value);

#endregion

#region DAL
// DBContext
builder.Services.AddDbContext<AiseaContext>(options =>
{
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Error);

//Repositories
builder.Services.AddScoped<AdvisorySession1to1Repository>();
builder.Services.AddScoped<NotificationRepository>();
builder.Services.AddScoped<AuditLogRepository>();
#endregion


var host = builder.Build();
host.Run();
