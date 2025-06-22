using AISEA.BgService.Worker;
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

#endregion

#region DAL
// DBContext
builder.Services.AddDbContext<AiseaContext>(options =>
{
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

//Repositories
builder.Services.AddScoped<RoleRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AdvisorySession1to1Repository>();
builder.Services.AddScoped<MessageRepository>();
builder.Services.AddScoped<NotificationRepository>();
builder.Services.AddScoped<StaffProfileRepository>();
builder.Services.AddScoped<StudentProfileRepository>();
#endregion


var host = builder.Build();
host.Run();
