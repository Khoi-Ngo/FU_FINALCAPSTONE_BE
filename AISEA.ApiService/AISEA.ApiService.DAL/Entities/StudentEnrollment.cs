using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("StudentEnrollment")]
public partial class StudentEnrollment : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public long UserId { get; set; }

    public long SubjectId { get; set; }

    [StringLength(20)]
    public string Semester { get; set; } = null!;

    public double? Grade { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!; // Passed, Failed, Studying

    [ForeignKey("UserId")]
    [InverseProperty("StudentEnrollments")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("SubjectId")]
    [InverseProperty("StudentEnrollments")]
    public virtual Subject Subject { get; set; } = null!;
}