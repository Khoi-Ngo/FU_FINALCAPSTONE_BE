using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.BgService.Worker.Abstract;

namespace AISEA.BgService.Worker.Entities;

[Table("SyllabusSession")]
public partial class SyllabusSession : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    
    public long SyllabusId { get; set; }
    
    public int SessionNumber { get; set; }
    
    [StringLength(50)]
    public string Topic { get; set; } = null!;
    
    [Column(TypeName = "text")]
    public string? Mission { get; set; }
    
    [ForeignKey("SyllabusId")]
    [InverseProperty("SyllabusSessions")]
    public virtual Syllabus Syllabus { get; set; } = null!;
    
    [InverseProperty("Session")]
    public virtual ICollection<SessionOutcomeMapping> SessionOutcomeMappings { get; set; } = new List<SessionOutcomeMapping>();
    
}