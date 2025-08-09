using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISEA.ApiService.DAL.Entities;
//TODO: Recheck the DTO response for this Entity
[Table("StudentProfile")]
public partial class StudentProfile
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    public long UserId { get; set; }
    public DateTimeOffset EnrolledAt { get; set; }
    public bool DoGraduate { get; set; } = false;
    public int NumberOfBan { get; set; } = 0;

    [Column(TypeName = "text")]
    public string? CareerGoal { get; set; }

    [ForeignKey("ProgramId")]
    public long? ProgramId { get; set; }
    public virtual Program Program { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("StudentProfile")]
    public virtual User User { get; set; } = null!;

    [InverseProperty("Student")]
    public virtual ICollection<AdvisorySession1to1> AdvisorySessions1to1 { get; set; } = new List<AdvisorySession1to1>();

    [InverseProperty("StudentProfile")]
    public virtual ICollection<BookedMeeting> BookedMeetings { get; set; } = new List<BookedMeeting>();

    [InverseProperty("StudentProfile")]
    public virtual ICollection<JoinedSubject> JoinedCourses { get; set; } = new List<JoinedSubject>();

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public string RegisteredComboCode { get; set; } = "";
    public string CurriculumCode { get; set; } = "";



}