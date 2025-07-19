namespace AISEA.ApiService.SHARED.DTOs.Requests.Booking;

public class CreateMeetingRequest
{
    public long StaffProfileId { get; set; }
    public required DateTime StartDateTime { get; set; }
    public required DateTime EndDateTime { get; set; }
    public required string TitleStudentIssue { get; set; }
    public required string ContentIssue { get; set; }

}