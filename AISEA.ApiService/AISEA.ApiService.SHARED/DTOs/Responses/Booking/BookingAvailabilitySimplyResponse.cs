namespace AISEA.ApiService.SHARED.DTOs.Responses.Booking
{
    public class BookingAvailabilitySimplyResponse
    {
        public long Id { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public DayOfWeek DayInWeek { get; set; }

        public long StaffProfileId { get; set; }
    }
}