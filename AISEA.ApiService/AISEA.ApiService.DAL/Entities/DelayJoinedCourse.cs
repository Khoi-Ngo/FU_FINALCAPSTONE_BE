using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISEA.ApiService.DAL.Entities;

[Table("DelayJoinedCourse")]
public class DelayJoinedCourse
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    public string SubjectCode { get; set; }
    public DateTime StartValidDateTime { get; set; }
    public DateTime EndValidDateTime { get; set; }
    public bool IsActive { get; set; }
    public string ReasonDelay { get; set; }

    [ForeignKey("StudentProfile")]
    public long StudentProfileId { get; set; }



    [ForeignKey("StudentProfileId")]
    [InverseProperty("DelayJoinedCourses")]
    public virtual StudentProfile StudentProfile { get; set; } = null!;
}