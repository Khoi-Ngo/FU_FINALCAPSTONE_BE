namespace AISEA.ApiService.SHARED.DTOs.Requests.Booking;

public class UpdateBookingAvailabilityRequest
{
    public  TimeSpan StartTime { get; set; }

    public  TimeSpan EndTime { get; set; }

    public  DayOfWeek DayInWeek { get; set; }

}