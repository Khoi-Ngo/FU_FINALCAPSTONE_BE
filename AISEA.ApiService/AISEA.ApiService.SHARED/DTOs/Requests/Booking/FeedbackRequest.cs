using System.ComponentModel.DataAnnotations;

namespace AISEA.ApiService.SHARED.DTOs.Requests.Booking;

public class FeedbackRequest
{
    [Required(ErrorMessage = "Feedback is required.")]
    public string Feedback { get; set; }

    public string? SuggestionFromAdvisor { get; set; }
}
