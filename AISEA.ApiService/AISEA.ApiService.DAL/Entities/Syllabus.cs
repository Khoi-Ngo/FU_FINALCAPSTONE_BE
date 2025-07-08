using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("Syllabus")]
public partial class Syllabus
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

    public virtual ICollection<SyllabusLearningMaterial> SyllabusLearningMaterials { get; set; } = new List<SyllabusLearningMaterial>();

    public virtual ICollection<SyllabusLearningOutcome> SyllabusLearningOutcomes { get; set; } = new List<SyllabusLearningOutcome>();

    public virtual ICollection<SyllabusSession> SyllabusSessions { get; set; } = new List<SyllabusSession>();
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}