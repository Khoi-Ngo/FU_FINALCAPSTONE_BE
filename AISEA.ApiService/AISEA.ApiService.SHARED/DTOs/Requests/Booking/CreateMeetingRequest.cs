namespace AISEA.ApiService.SHARED.DTOs.Requests.Booking;

public class CreateMeetingRequest
{
    public long StaffProfileId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string TitleStudentIssue { get; set; }
    public string ContentIssue { get; set; }

}