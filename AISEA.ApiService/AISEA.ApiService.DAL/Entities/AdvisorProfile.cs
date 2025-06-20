using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("AdvisorProfile")]
public partial class AdvisorProfile : BaseEntity
{
    [Key]
    [Column("user_id")]
    public long UserId { get; set; }

    [StringLength(255)]
    public string? Specialization { get; set; }

    public int YearsOfExperience { get; set; }

    [Column(TypeName = "text")]
    public string? Bio { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("AdvisorProfiles")]
    public virtual User User { get; set; } = null!;

    [InverseProperty("Advisor")]
    public virtual ICollection<AdvisorAvailabilitySlot> AdvisorAvailabilitySlots { get; set; } = new List<AdvisorAvailabilitySlot>();
}