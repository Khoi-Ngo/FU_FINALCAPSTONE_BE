using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.BgService.Worker.Abstract;

namespace AISEA.BgService.Worker.Entities;

[Table("StudentProfile")]
public partial class StudentProfile : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public long UserId { get; set; }

    public DateTimeOffset EnrolledAt { get; set; }

    public bool DoGraduate { get; set; } = false;


    [Column(TypeName = "text")]
    public string? CareerGoal { get; set; }


    [ForeignKey("UserId")]
    [InverseProperty("StudentProfile")]
    public virtual User User { get; set; } = null!;


    [InverseProperty("Student")]
    public virtual ICollection<AdvisorySession1to1> AdvisorySessions1to1 { get; set; } = new List<AdvisorySession1to1>();
}