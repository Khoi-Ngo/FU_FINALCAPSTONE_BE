using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("Subject")]
public partial class Subject : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [StringLength(50)]
    public string SubjectCode { get; set; } = null!;

    [StringLength(255)]
    public string SubjectName { get; set; } = null!;

    public int Credits { get; set; }

    [Column(TypeName = "text")]
    public string? Description { get; set; }

    [InverseProperty("Subject")]
    public virtual ICollection<Syllabus> Syllabi { get; set; } = new List<Syllabus>();

    [InverseProperty("Subject")]
    public virtual ICollection<CurriculumSubject> CurriculumSubjects { get; set; } = new List<CurriculumSubject>();

    [InverseProperty("Subject")]
    public virtual ICollection<ComboSubject> ComboSubjects { get; set; } = new List<ComboSubject>();

    [InverseProperty("Subject")]
    public virtual ICollection<StudentEnrollment> StudentEnrollments { get; set; } = new List<StudentEnrollment>();

    [InverseProperty("Subject")]
    public virtual ICollection<SubjectPrerequisite> Prerequisites { get; set; } = new List<SubjectPrerequisite>();

    [InverseProperty("PrerequisiteSubject")]
    public virtual ICollection<SubjectPrerequisite> DependentSubjects { get; set; } = new List<SubjectPrerequisite>();
}