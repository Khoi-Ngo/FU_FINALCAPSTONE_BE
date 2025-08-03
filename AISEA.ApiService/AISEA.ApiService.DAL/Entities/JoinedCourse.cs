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
    public string? CourseCode { get; set; }
    public string? CourseVersionCode { get; set; } // case external course -> no need VersionCode
    public string CourseName { get; set; } //Note: Subject Code + Subject Name + Unique number (none, 1, 2, ...) + Unique Type of Semester Block (None ~ Block10w, Block3w)
    public int? SemesterNumber { get; set; } //CN1, CN2, ...
    public string? SemesterName { get; set; } // FALL2025, SPRING2025, ...
    public bool IsCompleted { get; set; } = false; //mostly used for external course  not in FPT
    public bool IsPassed { get; set; } = false;//used for FPT Course not external course
    public bool IsActive { get; set; } = true;// Case: Change Program~Curriculum, ChangeCombo
    public int? Credits { get; set; }


    [ForeignKey("StudentProfile")]
    public long StudentProfileId { get; set; }



    [ForeignKey("StudentProfileId")]
    [InverseProperty("JoinedCourses")]
    public virtual StudentProfile StudentProfile { get; set; } = null!;
}