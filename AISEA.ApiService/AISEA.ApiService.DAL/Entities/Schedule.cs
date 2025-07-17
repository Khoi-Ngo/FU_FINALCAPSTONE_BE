using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISEA.ApiService.DAL.Entities;

[Table("Schedule")]
public partial class Schedule
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [ForeignKey("SubjectClassId")]
    public long SubjectClassId { get; set; }

    public DateTime ClassDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    public virtual SubjectClass SubjectClass { get; set; } = null!;
    public virtual ICollection<AttendanceChecklist> AttendanceChecklists { get; set; } = new List<AttendanceChecklist>();
}