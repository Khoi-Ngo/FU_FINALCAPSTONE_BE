using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.DAL.Entities;

[Table("JoinCourse")]
public class JoinedCourse
{

    [Key]
    [Column("id")]
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public JoinedCourseType type { get; set; }
    public string GithubRepositoryURL { get; set; } = "Unidentified";
    public string? CourseCode { get; set; }//~SubjectCode
    public string? CourseVersionCode { get; set; } // case external course -> no need VersionCode || This for link to FLM Data only
    public string CourseName { get; set; } //Note: Subject Code + Subject Name + Unique number (none, 1, 2, ...) + SemesterName (within a Semester can not have more than one CourseName)
    public int? SemesterNumber { get; set; } //CN1, CN2, ...
    public string? SemesterName { get; set; } // FALL2025, SPRING2025, ...
    public bool IsCompleted { get; set; } = false; //used as pass for FPTU Subject and Complete for External Course
    public bool IsActive { get; set; } = true;// Case: Change Program~Curriculum, ChangeCombo
    public int? Credits { get; set; }

    [ForeignKey("StudentProfile")]
    public long StudentProfileId { get; set; }

    [ForeignKey("Semester")]
    public long SemesterId { get; set; }

    // [ForeignKey("StudentProfileId")]
    [InverseProperty("JoinedCourses")]
    public virtual StudentProfile StudentProfile { get; set; } = null!;

    [InverseProperty("JoinedCourses")]
    public virtual Semester Semester { get; set; } = null!;
}