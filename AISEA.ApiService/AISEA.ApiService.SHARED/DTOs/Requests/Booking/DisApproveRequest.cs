namespace AISEA.ApiService.SHARED.DTOs.Requests.Booking;

public class DisApproveRequest
{
    public required long MeetingId { get; set; }
    public string? Note { get; set; }
}