using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("ComboPrerequisite")]
public partial class ComboPrerequisite : BaseEntity
{
    [Key]
    [Column("combo_id")]
    public long ComboId { get; set; }
    
    [Key]
    [Column("subject_id")]
    public long SubjectId { get; set; }
    
    public bool IsRequired { get; set; } = true;
    
    [ForeignKey("ComboId")]
    public virtual Combo Combo { get; set; } = null!;
    
    [ForeignKey("SubjectId")]
    public virtual Subject Subject { get; set; } = null!;
}