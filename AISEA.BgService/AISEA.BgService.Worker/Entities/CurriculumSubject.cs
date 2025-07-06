using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.BgService.Worker.Abstract;

namespace AISEA.BgService.Worker.Entities;

[Table("CurriculumSubject")]
public partial class CurriculumSubject : BaseEntity
{
    [Key]
    [Column("curriculum_id")]
    public long CurriculumId { get; set; }
    
    [Key]
    [Column("subject_id")]
    public long SubjectId { get; set; }
    
    public int SemesterNumber { get; set; }
    
    public bool IsMandatory { get; set; }
    
    [ForeignKey("CurriculumId")]
    [InverseProperty("CurriculumSubjects")]
    public virtual Curriculum Curriculum { get; set; } = null!;
    
    [ForeignKey("SubjectId")]
    [InverseProperty("CurriculumSubjects")]
    public virtual Subject Subject { get; set; } = null!;

}