using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Entities;

[Table("Curriculum")]
[Index("CurriculumCode", Name = "curriculum_code_unique", IsUnique = true)]
public partial class Curriculum : BaseEntity
{
    [Key]
    [Column("id")]  
    public long Id { get; set; }
    
    public long ProgramId { get; set; }
    
    [StringLength(50)]
    public string CurriculumCode { get; set; } = null;
    
    [StringLength(255)]
    public string CurriculumName { get; set; } = null;
    
    public DateTimeOffset EffectiveDate { get; set; }

    [ForeignKey("ProgramId")]
    public virtual Program Program { get; set; } = null;
    
    public virtual ICollection<CurriculumSubject> CurriculumSubjects  { get; set; } = new List<CurriculumSubject>();
}