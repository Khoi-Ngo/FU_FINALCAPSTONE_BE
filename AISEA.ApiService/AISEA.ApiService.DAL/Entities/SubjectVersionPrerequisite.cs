using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISEA.ApiService.DAL.Entities;

[Table("SubjectVersionPrerequisite")]
public partial class SubjectVersionPrerequisite
{
    [Key]
    [Column("subject_version_id")]
    public long SubjectVersionId { get; set; }

    [Key]
    [Column("prerequisite_subject_version_id")]
    public long PrerequisiteSubjectVersionId { get; set; }

    [ForeignKey("SubjectVersionId")]
    public virtual SubjectVersion SubjectVersion { get; set; } = null!;

    [ForeignKey("PrerequisiteSubjectVersionId")]
    public virtual SubjectVersion PrerequisiteSubjectVersion { get; set; } = null!;
    
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}
