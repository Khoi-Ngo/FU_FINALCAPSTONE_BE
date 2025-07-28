using AISEA.ApiService.BAL.Services.Booking;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.HubUtil;

namespace AISEA.ApiService.WebApi.BgJob;

public class MeetingOverdueBgService : BackgroundService
{
    private readonly ILogger<MeetingOverdueBgService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly BookingSettings _bookingSettings;

    public MeetingOverdueBgService(
        ILogger<MeetingOverdueBgService> logger,
        IServiceProvider serviceProvider,
        BookingSettings bookingSettings)
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
                var notifier = scope.ServiceProvider.GetRequiredService<NotificationHubNotifier>();

                // Get overdue meetings with user IDs in a single query
                var overdueMeetings = await bookedMeetingService.GetPendingOverdueMeetingsWithUserIdsAsync();

                if (overdueMeetings.Any())
                {
                    // Bulk update statuses to OVERDUE
                    await bookedMeetingService.UpdateMeetingStatusesAsync(overdueMeetings.Select(m => m.Id).ToList(), EBookingStatus.OVERDUE);

                    // Batch notifications to reduce SignalR overhead
                    var notificationTasks = new List<Task>();
                    var studentNotifications = new List<(long UserId, string Title, string Content)>();
                    var staffNotifications = new List<(long UserId, string Title, string Content)>();

                    foreach (var meeting in overdueMeetings)
                    {
                        try
                        {
                            // Prepare student notification
                            studentNotifications.Add((
                                meeting.StudentUserId,
                                "Meeting Overdue",
                                $"The meeting starting at {meeting.StartDateTime:yyyy-MM-dd HH:mm} is now overdue."
                            ));

                            // Prepare staff notification
                            staffNotifications.Add((
                                meeting.StaffUserId,
                                "Alert: Meeting Overdue",
                                $"The meeting starting at {meeting.StartDateTime:yyyy-MM-dd HH:mm} is now overdue. Please provide a reason."
                            ));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Failed to prepare notification for meeting ID: {meeting.Id}");
                        }
                    }

                    // Batch notify students and staff
                    if (studentNotifications.Any())
                    {
                        notificationTasks.Add(notifier.NotifyUsersAsync(studentNotifications));
                    }
                    if (staffNotifications.Any())
                    {
                        notificationTasks.Add(notifier.NotifyUsersAsync(staffNotifications));
                    }

                    await Task.WhenAll(notificationTasks);

                    _logger.LogInformation("Processed {Count} overdue meetings", overdueMeetings.Count);
                }
                else
                {
                    _logger.LogInformation("No overdue meetings found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing overdue meetings");
            }

            await Task.Delay((int)_bookingSettings.GeneralPurposeIntervalMillis, stoppingToken);
        }
    }
}