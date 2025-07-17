namespace AISEA.ApiService.SHARED.DTOs.Requests.Booking;

public class ReasonOverdueRequest
{
    public required long Id { get; set; }
    public required string Note { get; set; }
}