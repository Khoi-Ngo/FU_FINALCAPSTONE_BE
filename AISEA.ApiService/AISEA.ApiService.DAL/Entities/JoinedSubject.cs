using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.DAL.Entities;

[Table("JoinedSubject")]
public class JoinedSubject
{

    [Key]
    [Column("id")]
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? GithubRepositoryURL { get; set; }
    public string SubjectCode { get; set; }
    public string SubjectVersionCode { get; set; }
    public string? Name { get; set; }
    public string CreatedByUserName { get; set; }
    public ESemesterStudyBlockType SemesterStudyBlockType { get; set; }
    public bool IsPassed { get; set; } = false;
    public bool IsCompleted { get; set; } = false;
    public bool IsActive { get; set; } = true;// Case: Change Program~Curriculum, ChangeCombo ==> 
    public int? Credits { get; set; }

    [ForeignKey("StudentProfile")]
    public long StudentProfileId { get; set; }

    [ForeignKey("Semester")]
    public long SemesterId { get; set; }

    [InverseProperty("JoinedCourses")]
    public virtual StudentProfile StudentProfile { get; set; } = null!;

    [InverseProperty("JoinedCourses")]
    public virtual Semester Semester { get; set; } = null!;

    public virtual ICollection<SubjectMarkReport> SubjectMarkReports { get; set; } = new List<SubjectMarkReport>();

    public virtual ICollection<JoinedSubjectCheckPoint> JoinedSubjectCheckPoints { get; set; } 
    = new List<JoinedSubjectCheckPoint>();


}