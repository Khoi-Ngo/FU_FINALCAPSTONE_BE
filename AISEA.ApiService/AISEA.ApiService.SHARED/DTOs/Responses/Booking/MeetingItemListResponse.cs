using System;
using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.DTOs.Responses.Booking;

public class MeetingItemListResponse
{
    public long Id { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public EBookingStatus Status { get; set; }
    public string TitleStudentIssue { get; set; }
    public DateTime? CreatedAt { get; set; }

    // Staff Profile Details
    public long StaffProfileId { get; set; }
    public string StaffFirstName { get; set; }
    public string StaffLastName { get; set; }
    public string StaffEmail { get; set; }

    // Student Profile Details
    public long StudentProfileId { get; set; }
    public string StudentFirstName { get; set; }
    public string StudentLastName { get; set; }
    public string StudentEmail { get; set; }
}