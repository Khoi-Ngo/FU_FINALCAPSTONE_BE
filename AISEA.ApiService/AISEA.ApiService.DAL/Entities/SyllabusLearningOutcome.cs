using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("SyllabusLearningOutcome")]
public partial class SyllabusLearningOutcome : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public long SyllabusId { get; set; }

    [StringLength(20)]
    public string OutcomeCode { get; set; } = null!; // LO1, LO2

    [Column(TypeName = "text")]
    public string Description { get; set; } = null!;

    [ForeignKey("SyllabusId")]
    [InverseProperty("SyllabusLearningOutcomes")]
    public virtual Syllabus Syllabus { get; set; } = null!;

    [InverseProperty("Outcome")]
    public virtual ICollection<SessionOutcomeMapping> SessionOutcomeMappings { get; set; } = new List<SessionOutcomeMapping>();
}