using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("Meeting")]
public partial class Meeting : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public long StudentId { get; set; }

    [StringLength(255)]
    public string Topic { get; set; } = null!;

    [StringLength(20)]
    public string Status { get; set; } = null!; // Pending, Confirmed, Cancelled, Completed

    [Column(TypeName = "text")]
    public string? StudentNotes { get; set; }

    [Column(TypeName = "text")]
    public string? AdvisorNotes { get; set; }

    public long SlotId { get; set; }

    [ForeignKey("StudentId")]
    [InverseProperty("StudentMeetings")]
    public virtual User Student { get; set; } = null!;

    [ForeignKey("SlotId")]
    [InverseProperty("Meetings")]
    public virtual AdvisorAvailabilitySlot Slot { get; set; } = null!;
}