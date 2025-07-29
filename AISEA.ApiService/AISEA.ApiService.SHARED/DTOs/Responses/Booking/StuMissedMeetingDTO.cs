namespace AISEA.ApiService.SHARED.DTOs.Responses.Booking;

public class StuMissedMeetingDTO
{
    public long Id { get; set; }
    public long StudentUserId { get; set; }
    public long StudentProfileId { get; set; }
    public DateTime StartDateTime { get; set; }
}