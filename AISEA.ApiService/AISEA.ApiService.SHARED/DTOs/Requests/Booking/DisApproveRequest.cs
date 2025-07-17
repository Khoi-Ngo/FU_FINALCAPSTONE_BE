namespace AISEA.ApiService.SHARED.DTOs.Requests.Booking;

public class DisApproveRequest
{
    public required List<long> MeetingIds { get; set; }
    public string? Note { get; set; }
}