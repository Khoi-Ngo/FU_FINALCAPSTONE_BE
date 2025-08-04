using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISEA.ApiService.DAL.Entities;

[Table("Semester")]
public class Semester
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string SemesterName { get; set; }

    [InverseProperty("Semester")]
    public virtual ICollection<JoinedCourse> JoinedCourses { get; set; } = new List<JoinedCourse>();
}