using System;
using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.DTOs.Responses.Booking;

public class MeetingViewDetailResponse
{
    public long Id { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public EBookingStatus Status { get; set; }
    public string? Feedback { get; set; }
    public string? SuggestionFromAdvisor { get; set; }
    public string? Note { get; set; }
    public string TitleStudentIssue { get; set; }
    public string ContentIssue { get; set; }
    public string CheckInCode { get; set; }
    public DateTime? CreatedAt { get; set; }

    // Staff Profile Details
    public long StaffProfileId { get; set; }
    public string StaffCampus { get; set; }
    public string StaffDepartment { get; set; }
    public string StaffPosition { get; set; }
    public DateTimeOffset? StaffStartWorkAt { get; set; }
    public DateTimeOffset? StaffEndWorkAt { get; set; }
    public long StaffUserId { get; set; }
    public string StaffUsername { get; set; }
    public string StaffEmail { get; set; }
    public string StaffFirstName { get; set; }
    public string StaffLastName { get; set; }
    public DateTimeOffset? StaffDateOfBirth { get; set; }
    public string? StaffAvatarUrl { get; set; }
    public EUserStatus StaffStatus { get; set; }

    // Student Profile Details
    public long StudentProfileId { get; set; }
    public DateTimeOffset StudentEnrolledAt { get; set; }
    public bool StudentDoGraduate { get; set; }
    public int StudentNumberOfBan { get; set; }
    public string? StudentCareerGoal { get; set; }
    public long? StudentProgramId { get; set; }
    public long StudentUserId { get; set; }
    public string StudentUsername { get; set; }
    public string StudentEmail { get; set; }
    public string StudentFirstName { get; set; }
    public string StudentLastName { get; set; }
    public DateTimeOffset? StudentDateOfBirth { get; set; }
    public string? StudentAvatarUrl { get; set; }
    public EUserStatus StudentStatus { get; set; }
}