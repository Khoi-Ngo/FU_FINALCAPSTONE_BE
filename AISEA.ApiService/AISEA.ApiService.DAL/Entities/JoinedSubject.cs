using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    public string Name { get; set; } //Note: Subject Code + Subject Name + Unique number (none, 1, 2, ...) + SemesterName (within a Semester can not have more than one CourseName)
    public string SemesterName { get; set; } // FALL2025, SPRING2025, ...
    public string CreatedByUserName { get; set; }
    public bool IsPassed { get; set; } = false;
    public bool IsCompleted { get; set; } = false;
    public bool IsActive { get; set; } = true;// Case: Change Program~Curriculum, ChangeCombo ==> 
    // //TODO: TRIGGER to update after change of Program or Curriculum or Combo
    public int? Credits { get; set; }// TODO: Trigger to update after import

    [ForeignKey("StudentProfile")]
    public long StudentProfileId { get; set; }

    [ForeignKey("Semester")]
    public long SemesterId { get; set; }

    [InverseProperty("JoinedCourses")]
    public virtual StudentProfile StudentProfile { get; set; } = null!;

    [InverseProperty("JoinedCourses")]
    public virtual Semester Semester { get; set; } = null!;
}