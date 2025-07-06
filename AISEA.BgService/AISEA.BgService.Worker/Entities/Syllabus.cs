using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.BgService.Worker.Abstract;

namespace AISEA.BgService.Worker.Entities;

[Table("Syllabus")]
public partial class Syllabus : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    
    public long SubjectId { get; set; }
    
    [Column(TypeName = "text")]
    public string Content { get; set; } = null!;
    
    [ForeignKey("SubjectId")]
    [InverseProperty("Syllabi")]
    public virtual Subject Subject { get; set; } = null!;
    
    public virtual ICollection<SyllabusAssessment> SyllabusAssessments { get; set; } = new List<SyllabusAssessment>();  
    
    public virtual ICollection<SyllabusLearningMaterial> SyllabusLearningMaterials {get; set; } = new List<SyllabusLearningMaterial>();
    
    public virtual ICollection<SyllabusLearningOutcome> SyllabusLearningOutcomes { get; set; } = new List<SyllabusLearningOutcome>();
    
    public virtual ICollection<SyllabusSession> SyllabusSessions { get; set; } = new List<SyllabusSession>();
}