using AISEA.ApiService.BAL.Services.AuditLog;
using AISEA.ApiService.BAL.Services.CourseTracker;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.PropConfigs;

namespace AISEA.ApiService.WebApi.BgJob;

public class SemesterReferBgService : BackgroundService
{
    private readonly ILogger<SemesterReferBgService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly CourseTrackSettings _courseTrackSettings;

    public SemesterReferBgService(
        ILogger<SemesterReferBgService> logger,
        IServiceProvider serviceProvider,
        CourseTrackSettings courseTrackSettings)
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
                using (var scope = _serviceProvider.CreateScope())
                {
                    var semesterReferService = scope.ServiceProvider.GetRequiredService<SemesterReferService>();
                    var auditLogService = scope.ServiceProvider.GetRequiredService<AuditLogService>();

                    if (semesterReferService == null)
                    {
                        _logger.LogError("SemesterReferService not found in DI container.");
                        return;
                    }

                    var now = DateTime.Now;
                    string? semesterName = null;

                    // Helper to check if now is in a range (ignoring year)
                    bool InRange(DateOnly start, DateOnly end)
                    {
                        var d = new DateOnly(now.Year, now.Month, now.Day);
                        if (start <= end)
                            return d >= start && d <= end;
                        //! For ranges that cross year boundary
                        return d >= start || d <= end;
                    }

                    if (InRange(_courseTrackSettings.SpringSemesterStartDate, _courseTrackSettings.SpringSemesterEndDate))
                        semesterName = $"Spring{now.Year}";
                    else if (InRange(_courseTrackSettings.SummerSemesterStartDate, _courseTrackSettings.SummerSemesterEndDate))
                        semesterName = $"Summer{now.Year}";
                    else if (InRange(_courseTrackSettings.FallSemesterStartDate, _courseTrackSettings.FallSemesterEndDate))
                        semesterName = $"Fall{now.Year}";

                    if (!string.IsNullOrEmpty(semesterName))
                    {
                        var exists = await semesterReferService.SemesterExistsAsync(semesterName);
                        if (!exists)
                        {
                            await semesterReferService.AddSemesterAsync(semesterName, now);
                            _logger.LogInformation($"Added new semester: {semesterName}");


                            // Audit log for adding new semester
                            await auditLogService.CreateAsync(new AuditLog
                            {
                                Tag = "SEMESTER_ADDED",
                                Description = $"Added new semester '{semesterName}' on {now:yyyy-MM-dd HH:mm:ss}.",
                                UserId = null, // system process, or use a system user ID if you have one
                                CreatedAt = DateTime.UtcNow
                            });

                        }
                        else
                        {
                            _logger.LogInformation($"Semester {semesterName} already exists. Skipping.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Current date does not match any semester range.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in SemesterReferBgService.");
            }

            await Task.Delay(TimeSpan.FromDays(_courseTrackSettings.AddSemesterNameIntervalDays), stoppingToken);
        }
    }
}