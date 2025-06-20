using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

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
    [InverseProperty("Prerequisites")]
    public virtual Subject Subject { get; set; } = null!;

    [ForeignKey("PrerequisiteSubjectId")]
    [InverseProperty("DependentSubjects")]
    public virtual Subject PrerequisiteSubject { get; set; } = null!;
}