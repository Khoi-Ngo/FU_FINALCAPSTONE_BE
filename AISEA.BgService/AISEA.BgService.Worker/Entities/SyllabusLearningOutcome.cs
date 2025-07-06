using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.BgService.Worker.Abstract;

namespace AISEA.BgService.Worker.Entities;

[Table("SyllabusLearningOutcome")]
public partial class SyllabusLearningOutcome : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    
    public long SyllabusId { get; set; }
    
    [StringLength(20)]
    public string OutcomeCode { get; set; } = null!; //LO1, LO2
    
    [Column(TypeName = "text")]
    public string Description { get; set; } = null!;
    
    [ForeignKey("SyllabusId")]
    public virtual Syllabus Syllabus { get; set; } = null!;
    
    public virtual ICollection<SessionOutcomeMapping> SessionOutcomeMappings { get; set; } = new List<SessionOutcomeMapping>(); 
}