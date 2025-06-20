using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("AcademicStaffProfile")]
public partial class AcademicStaffProfile : BaseEntity
{
    [Key]
    [Column("user_id")]
    public long UserId { get; set; }

    [StringLength(100)]
    public string Department { get; set; } = null!;

    [StringLength(255)]
    public string Position { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("AcademicStaffProfiles")]
    public virtual User User { get; set; } = null!;
}