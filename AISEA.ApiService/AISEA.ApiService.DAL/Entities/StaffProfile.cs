using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("StaffProfile")]
public partial class StaffProfile
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [StringLength(255)]
    public string Campus { get; set; } = null!;

    [StringLength(255)]
    public string Department { get; set; } = null!;

    [StringLength(255)]
    public string Position { get; set; } = null!;

    public DateTimeOffset? StartWorkAt { get; set; }

    public DateTimeOffset? EndWorkAt { get; set; }

    public long UserId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("StaffProfile")]
    public virtual User User { get; set; } = null!;

    [InverseProperty("Staff")]
    public virtual ICollection<AdvisorySession1to1> AdvisorySessions1to1 { get; set; } = new List<AdvisorySession1to1>();

    [InverseProperty("StaffProfile")]
    public virtual ICollection<BookingAvailability> BookingAvailabilities { get; set; } = new List<BookingAvailability>();

    [InverseProperty("StaffProfile")]
    public virtual ICollection<LeaveSchedule> LeaveSchedules { get; set; } = new List<LeaveSchedule>();

    [InverseProperty("StaffProfile")]
    public virtual ICollection<BookedMeeting> BookedMeetings { get; set; } = new List<BookedMeeting>();
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}