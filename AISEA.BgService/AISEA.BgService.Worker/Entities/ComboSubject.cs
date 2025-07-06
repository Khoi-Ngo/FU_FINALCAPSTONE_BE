using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.BgService.Worker.Abstract;

namespace AISEA.BgService.Worker.Entities;

[Table("ComboSubject")]
public partial class ComboSubject : BaseEntity
{
    [Key]
    [Column("combo_id")]
    public long ComboId { get; set; }
    
    [Key]
    [Column("subject_id")]
    public long SubjectId { get; set; }
    
    [ForeignKey("ComboId")]
    public virtual Combo Combo { get; set; } = null!;
    
    [ForeignKey("SubjectId")]
    public virtual Subject Subject { get; set; } = null!;
}