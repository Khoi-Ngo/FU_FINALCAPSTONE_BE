using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISEA.ApiService.DAL.Entities;

[Table("SubjectClass")]
public partial class SubjectClass
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [ForeignKey("SubjectVersionId")]
    public long SubjectVersionId { get; set; }

    public int SemesterNumber { get; set; }
    public string ClassCode { get; set; } = null!;
    public int MaxStudents { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    public virtual SubjectVersion SubjectVersion { get; set; } = null!;
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}