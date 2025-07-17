using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISEA.ApiService.DAL.Entities;

[Table("AttendanceChecklist")]
public partial class AttendanceChecklist
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [ForeignKey("ScheduleId")]
    public long ScheduleId { get; set; }

    public string Username { get; set; } = null!; // Indirect reference to StudentProfile.Username
    public bool Status { get; set; } // 1 for present, 0 for absent
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual Schedule Schedule { get; set; } = null!;
}