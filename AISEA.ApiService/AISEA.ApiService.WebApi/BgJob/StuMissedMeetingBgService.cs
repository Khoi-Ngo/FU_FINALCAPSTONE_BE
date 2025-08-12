using AISEA.ApiService.BAL.Services.AuditLog;
using AISEA.ApiService.BAL.Services.Booking;
using AISEA.ApiService.BAL.Services.SystemProfile;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Noti;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.HubUtil;

namespace AISEA.ApiService.WebApi.BgJob;

public class StuMissedMeetingBgService : BackgroundService
{
    /*
    Description:

    - The bg service will scan all CONFIRMED meetings over EndDateTime + 1 day

    - Shift the Status into the STUDENT_MISSED

    -Increase the numberOfBan of in associated student profile by _bookingSettings.NumberOfBanWhenStuMissingTheMeeting

    */

    private readonly ILogger<StuMissedMeetingBgService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly BookingSettings _bookingSettings;

    public StuMissedMeetingBgService(ILogger<StuMissedMeetingBgService> logger, IServiceProvider serviceProvider, BookingSettings bookingSettings)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _bookingSettings = bookingSettings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var bookedMeetingService = scope.ServiceProvider.GetRequiredService<BookedMeetingService>();
                var studentProfileService = scope.ServiceProvider.GetRequiredService<StudentProfileService>();
                var notifier = scope.ServiceProvider.GetRequiredService<NotificationHubNotifier>();

                var missedMeetings = await bookedMeetingService.GetConfirmedStudentMissedMeetingsAsync(_bookingSettings.DaysToCheckStudentMissedAfterEndMeeting);

                if (missedMeetings.Any())
                {
                    // Prepare data for bulk updates and notifications
                    var meetingIds = missedMeetings.Select(m => m.Id).ToList();
                    var studentBans = missedMeetings
                            .GroupBy(m => m.StudentProfileId)
                            .ToDictionary(g => g.Key, g => g.Count() * _bookingSettings.NumberOfBanWhenStuMissingTheMeeting);


                    var studentNotifications = missedMeetings
                                .Select(m => (
                                m.StudentUserId,
                                new NotificationDTO
                                {
                                    Title = "Meeting Missed",
                                    Content = $"The meeting starting at {m.StartDateTime:yyyy-MM-dd HH:mm} was missed and marked as {EBookingStatus.STUDENT_MISSED.ToString()}.",
                                }
                                ))
                                .ToList();


                    // Bulk update meeting statuses to STUDENT_MISSED
                    await bookedMeetingService.UpdateMeetingStatusesAsync(meetingIds, EBookingStatus.STUDENT_MISSED);

                    // Bulk update NumberOfBan for student profiles
                    await studentProfileService.IncreaseNumberOfBansAsync(studentBans);

                    // Batch notify students
                    if (studentNotifications.Any())
                    {
                        await notifier.NotifyUsersAsync(studentNotifications);
                    }


                    var auditTasks = missedMeetings.Select(async missedMeeting =>
              {
                  using var scope = _serviceProvider.CreateScope();
                  var scopedAuditLogService = scope.ServiceProvider.GetRequiredService<AuditLogService>();

                  await scopedAuditLogService.CreateAsync(new AuditLog
                  {
                      Tag = "STUDENT_MISSED_MEETING",
                      Description = $"StudentProfileId {missedMeeting.StudentProfileId} missed meeting ID {missedMeeting.Id} starting at {missedMeeting.StartDateTime:yyyy-MM-dd HH:mm}.",
                      UserId = missedMeeting.StudentUserId,
                      CreatedAt = DateTime.UtcNow
                  });
              });


                    await Task.WhenAll(auditTasks);



                    _logger.LogInformation("Processed {Count} missed meetings at {Time}", missedMeetings.Count, DateTime.UtcNow);
                }
                else
                {
                    _logger.LogInformation("No confirmed meetings past due found at {Time}", DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing checking student missed meetings");
                await Task.Delay(TimeSpan.FromMinutes(_bookingSettings.ErrorRetryDelayMinutes), stoppingToken);
            }

            await Task.Delay((int)_bookingSettings.GeneralPurposeIntervalMillis, stoppingToken);
        }
    }
}