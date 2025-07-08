using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("SubjectPrerequisite")]
public partial class SubjectPrerequisite
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
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

}