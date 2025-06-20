using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("AdvisorAvailabilitySlot")]
public partial class AdvisorAvailabilitySlot : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public long AdvisorId { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset EndTime { get; set; }

    [StringLength(10)]
    public string MeetingType { get; set; } = null!; // Online or Offline

    [StringLength(255)]
    public string? Location { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!; // Available, Booked

    [ForeignKey("AdvisorId")]
    [InverseProperty("AdvisorAvailabilitySlots")]
    public virtual User Advisor { get; set; } = null!;

    [InverseProperty("Slot")]
    public virtual ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();
}