using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.DAL.Entities;

[Table("BookedMeeting")]
public class BookedMeeting
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public long StaffProfileId { get; set; }

    public long StudentProfileId { get; set; }

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public EBookingStatus Status { get; set; } = EBookingStatus.PENDING;

    [StringLength(1000)]
    public string? Feedback { get; set; }
    public string? SuggestionFromAdvisor { get; set; }
    public string? Note { get; set; }
    public string TitleStudentIssue { get; set; }
    public string ContentIssue { get; set; }
    public string CheckinCode { get; set; }
    public string? ConfirmCheckinCode { get; set; }

    [ForeignKey("StaffProfileId")]
    [InverseProperty("BookedMeetings")]
    public virtual StaffProfile StaffProfile { get; set; } = null!;

    [ForeignKey("StudentProfileId")]
    [InverseProperty("BookedMeetings")]
    public virtual StudentProfile StudentProfile { get; set; } = null!;
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
}
