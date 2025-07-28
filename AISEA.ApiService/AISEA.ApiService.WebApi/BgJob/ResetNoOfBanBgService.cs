using AISEA.ApiService.BAL.Services.SystemProfile;
using AISEA.ApiService.SHARED.PropConfigs;

namespace AISEA.ApiService.WebApi.BgJob;

public class ResetNoOfBanBgService : BackgroundService
{
    private readonly ILogger<ResetNoOfBanBgService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly BookingSettings _bookingSettings;

    public ResetNoOfBanBgService(
        ILogger<ResetNoOfBanBgService> logger,
        IServiceProvider serviceProvider,
        BookingSettings bookingSettings)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _bookingSettings = bookingSettings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var resetInterval = TimeSpan.FromDays(_bookingSettings.ResetNumberOfBanIntervalDays);
        var checkInterval = TimeSpan.FromHours(_bookingSettings.ResetCheckIntervalHours);

        DateTime nextReset = DateTime.UtcNow.Date.Add(resetInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.UtcNow;
                var delay = nextReset - now;

                _logger.LogInformation($"The Reset Number Of Ban Worker Service Running at {now}, the delay is now {delay}");

                if (delay.TotalMilliseconds <= 0)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var studentProfileService = scope.ServiceProvider.GetRequiredService<StudentProfileService>();
                        await studentProfileService.ResetNumberOfBansAsync();
                        _logger.LogInformation("Successfully reset NumberOfBan to 0 for all student profiles at {Time}", now);
                    }

                    nextReset = now.Date.Add(resetInterval);
                    delay = nextReset - now;
                }


                var delayMs = Math.Min(delay.TotalMilliseconds, checkInterval.TotalMilliseconds);
                await Task.Delay(TimeSpan.FromMilliseconds(delayMs), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while resetting the NumberOfBan for StudentProfile table");
                await Task.Delay(TimeSpan.FromMinutes(_bookingSettings.ErrorRetryDelayMinutes), stoppingToken);

            }
        }
    }

}