using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("SyllabusSession")]
public partial class SyllabusSession : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public long SyllabusId { get; set; }

    public int SessionNumber { get; set; }

    [StringLength(500)]
    public string Topic { get; set; } = null!;

    [Column(TypeName = "text")]
    public string? Mission { get; set; }

    [ForeignKey("SyllabusId")]
    [InverseProperty("SyllabusSessions")]
    public virtual Syllabus Syllabus { get; set; } = null!;

    [InverseProperty("Session")]
    public virtual ICollection<SessionOutcomeMapping> SessionOutcomeMappings { get; set; } = new List<SessionOutcomeMapping>();
}