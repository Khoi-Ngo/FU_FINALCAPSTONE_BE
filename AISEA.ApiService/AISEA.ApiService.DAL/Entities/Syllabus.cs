using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

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

    [InverseProperty("Syllabus")]
    public virtual ICollection<SyllabusAssessment> SyllabusAssessments { get; set; } = new List<SyllabusAssessment>();

    [InverseProperty("Syllabus")]
    public virtual ICollection<SyllabusLearningMaterial> SyllabusLearningMaterials { get; set; } = new List<SyllabusLearningMaterial>();

    [InverseProperty("Syllabus")]
    public virtual ICollection<SyllabusLearningOutcome> SyllabusLearningOutcomes { get; set; } = new List<SyllabusLearningOutcome>();

    [InverseProperty("Syllabus")]
    public virtual ICollection<SyllabusSession> SyllabusSessions { get; set; } = new List<SyllabusSession>();
}