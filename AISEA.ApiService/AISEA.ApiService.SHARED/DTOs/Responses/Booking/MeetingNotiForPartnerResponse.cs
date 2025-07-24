using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.DTOs.Responses.Booking;

public class MeetingNotiForPartnerResponse
{
    public long PartnerUserId { get; set; }
    public DateTime MeetingStartDateTime { get; set; }
    public DateTime MeetingEndDateTime { get; set; }
    public EBookingStatus StatusChangedTo { get; set; }

}