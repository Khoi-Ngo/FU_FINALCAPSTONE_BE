using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.SHARED.Const.Enums;
using Microsoft.EntityFrameworkCore;

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

    public EBookingStatus Status { get; set; }

    [StringLength(1000)]
    public string? Feedback { get; set; }

    [ForeignKey("StaffProfileId")]
    [InverseProperty("BookedMeetings")]
    public virtual StaffProfile StaffProfile { get; set; } = null!;

    [ForeignKey("StudentProfileId")]
    [InverseProperty("BookedMeetings")]
    public virtual StudentProfile StudentProfile { get; set; } = null!;
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
}
