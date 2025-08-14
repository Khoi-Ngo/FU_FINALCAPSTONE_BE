using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISEA.ApiService.DAL.Entities;

[Table("OptionalPersonalSubject")]
public class OptionalPersonalSubject
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? GithubRepositoryURL { get; set; }
    public string SubjectCode { get; set; }
    public string? Name { get; set; }
    public bool IsCompleted { get; set; } = false;

    [ForeignKey("StudentProfile")]
    public long StudentProfileId { get; set; }

    [ForeignKey("Semester")]
    public long SemesterId { get; set; }

    [InverseProperty("OptionalPersonalSubjects")]
    public virtual StudentProfile StudentProfile { get; set; } = null!;

    [InverseProperty("OptionalPersonalSubjects")]
    public virtual Semester Semester { get; set; } = null!;

    [InverseProperty("OptionalPersonalSubject")]
    public virtual ICollection<OptionalSubjectCheckPoint> Checkpoints { get; set; }
    = new List<OptionalSubjectCheckPoint>();

}
