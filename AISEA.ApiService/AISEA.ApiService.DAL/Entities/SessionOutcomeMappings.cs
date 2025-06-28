using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("SessionOutcomeMapping")]
public partial class SessionOutcomeMapping : BaseEntity
{
    [Column("session_id")]
    public long SessionId { get; set; }
    
    [Column("outcome_id")]
    public long OutcomeId { get; set; }
    
    [ForeignKey("SessionId")]
    [InverseProperty("SessionOutcomeMappings")]
    public virtual SyllabusSession Session { get; set; } = null!;

    [ForeignKey("OutcomeId")]
    [InverseProperty("SessionOutcomeMappings")]
    public virtual SyllabusLearningOutcome Outcome { get; set; } = null!;
}