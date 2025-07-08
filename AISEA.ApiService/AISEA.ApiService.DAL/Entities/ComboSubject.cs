using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("ComboSubject")]
public partial class ComboSubject
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
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}