using AISEA.ApiService.BAL.Services.CourseTracker;
using AISEA.ApiService.SHARED.PropConfigs;

namespace AISEA.ApiService.WebApi.BgJob;

public class CleanNonUseJoinedSubjectBgService : BackgroundService
{
    private readonly ILogger<CleanNonUseJoinedSubjectBgService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly CourseTrackSettings _courseTrackSettings;

    public CleanNonUseJoinedSubjectBgService(ILogger<CleanNonUseJoinedSubjectBgService> logger, IServiceProvider serviceProvider, CourseTrackSettings courseTrackSettings)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _courseTrackSettings = courseTrackSettings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();

                var joinedSubjectService = scope.ServiceProvider.GetRequiredService<JoinedSubjectService>();

                await joinedSubjectService.RemoveAllNonUseAsync();


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing non use joined subjects.");
            }

            await Task.Delay(TimeSpan.FromDays(_courseTrackSettings.RemoveNonUseJoinedSubjectIntervalDays), stoppingToken);
        }
    }
}
