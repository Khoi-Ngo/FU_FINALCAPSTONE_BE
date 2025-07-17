using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISEA.ApiService.DAL.Entities;

[Table("LeaveSchedule")]
public class LeaveSchedule
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    public long StaffProfileId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }

    [ForeignKey("StaffProfileId")]
    [InverseProperty("LeaveSchedules")]
    public virtual StaffProfile StaffProfile { get; set; } = null!;
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

}