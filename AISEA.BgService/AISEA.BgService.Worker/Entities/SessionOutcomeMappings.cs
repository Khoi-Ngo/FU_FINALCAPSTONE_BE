using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.BgService.Worker.Abstract;

namespace AISEA.BgService.Worker.Entities;

[Table("SessionOutcomeMapping")]
public partial class SessionOutcomeMapping : BaseEntity
{
    [Key]
    [Column("session_id")]
    public long SessionId { get; set; }
    
    [Key]
    [Column("outcome_id")]
    public long OutcomeId { get; set; }
    
    [ForeignKey("SessionId")]
    [InverseProperty("SessionOutcomeMappings")]
    public virtual SyllabusSession Session { get; set; } = null!;

    [ForeignKey("OutcomeId")]
    [InverseProperty("SessionOutcomeMappings")]
    public virtual SyllabusLearningOutcome Outcome { get; set; } = null!;
    
}