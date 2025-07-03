using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("Combo")]
public partial class Combo : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    
    [StringLength(255)]
    public string ComboName { get; set; } = null!;
    
    [Column(TypeName = "text")]
    public string? ComboDescription { get; set; }
    
    public virtual ICollection<ComboSubject> ComboSubjects { get; set; } = new List<ComboSubject>();
}