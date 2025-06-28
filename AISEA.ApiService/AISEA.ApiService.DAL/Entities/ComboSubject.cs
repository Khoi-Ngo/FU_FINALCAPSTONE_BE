using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("ComboSubject")]
public partial class ComboSubject : BaseEntity
{
    [Column("combo_id")]
    public long ComboId { get; set; }
    
    [Column("subject_id")]
    public long SubjectId { get; set; }
    
    [ForeignKey("ComboId")]
    public virtual Combo Combo { get; set; } = null!;
    
    [ForeignKey("SubjectId")]
    public virtual Subject Subject { get; set; } = null!;
}