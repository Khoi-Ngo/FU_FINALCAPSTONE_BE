using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.BgService.Worker.Abstract;

namespace AISEA.BgService.Worker.Entities;

[Table("SubjectPrerequisite")]
public partial class SubjectPrerequisite : BaseEntity
{
    [Key]
    [Column("subject_id")]
    public long SubjectId { get; set; }
    
    [Key]
    [Column("prerequisite_subject_id")]
    public long PrerequisiteSubjectId { get; set; }
    
    [ForeignKey("SubjectId")]
    public virtual Subject Subject { get; set; } = null!;
    
    [ForeignKey("PrerequisiteSubjectId")]
    public virtual Subject PrerequisiteSubject { get; set; } = null!;
    
}