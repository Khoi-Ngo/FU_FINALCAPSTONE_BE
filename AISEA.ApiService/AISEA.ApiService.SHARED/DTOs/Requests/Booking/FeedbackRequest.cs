namespace AISEA.ApiService.SHARED.DTOs.Requests.Booking;

public class FeedbackRequest
{
    public required long Id { get; set; }
    public required string Feedback { get; set; }
    public required string SuggestionFromAdvisor { get; set; }
}