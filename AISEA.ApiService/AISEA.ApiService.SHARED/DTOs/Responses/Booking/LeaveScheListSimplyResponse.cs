namespace AISEA.ApiService.SHARED.DTOs.Responses.Booking;

public class LeaveScheListSimplyResponse
{
    public long Id { get; set; }
    public long StaffProfileId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public DateTime? CreatedAt { get; set; }
}