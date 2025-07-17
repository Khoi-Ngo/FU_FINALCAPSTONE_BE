using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.DTOs.Requests.Booking;

public class CreateBookingAvailabilityRequest
{
    public  TimeSpan StartTime { get; set; }

    public  TimeSpan EndTime { get; set; }

    public  DayOfWeekAISEA DayInWeek { get; set; }
}