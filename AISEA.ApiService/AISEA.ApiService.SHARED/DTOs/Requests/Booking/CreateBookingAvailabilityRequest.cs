namespace AISEA.ApiService.SHARED.DTOs.Requests.Booking;

public class CreateBookingAvailabilityRequest
{
    public required TimeSpan StartTime { get; set; }

    public required TimeSpan EndTime { get; set; }

    public required DayOfWeek DayInWeek { get; set; }
}