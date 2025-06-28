using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("SubjectPrerequisite")]
public partial class SubjectPrerequisite : BaseEntity
{
    [Column("subject_id")]
    public long SubjectId { get; set; }
    
    [Column("prerequisite_subject_id")]
    public long PrerequisiteSubjectId { get; set; }
    
    [ForeignKey("SubjectId")]
    public virtual Subject Subject { get; set; } = null!;
    
    [ForeignKey("PrerequisiteSubjectId")]
    public virtual Subject PrerequisiteSubject { get; set; } = null!;
}