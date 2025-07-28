using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.DTOs.Responses.Booking;

public class OverdueMeetingDTO
{
    public long Id { get; set; }
    public long StaffUserId { get; set; }
    public long StudentUserId { get; set; }
    public DateTime StartDateTime { get; set; }
    public EBookingStatus Status { get; set; }
}