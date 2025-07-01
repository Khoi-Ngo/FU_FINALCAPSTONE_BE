using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("CurriculumVersion")]
public partial class CurriculumVersion : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    
    public long CurriculumId { get; set; }
    
    [StringLength(20)]
    public string Version { get; set; } = null!;
    
    public DateTimeOffset EffectiveDate { get; set; }
    
    [Column(TypeName = "nvarchar(max)")]
    public string? ChangeDescription { get; set; }
    
    [ForeignKey("CurriculumId")]
    public virtual Curriculum Curriculum { get; set; } = null!;
}