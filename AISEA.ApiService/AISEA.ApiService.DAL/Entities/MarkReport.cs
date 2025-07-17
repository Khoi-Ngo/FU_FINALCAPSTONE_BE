using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISEA.ApiService.DAL.Entities;

[Table("MarkReport")]
public partial class MarkReport
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [ForeignKey("StudentProfileId")]
    public long StudentProfileId { get; set; }

    public int SemesterNumber { get; set; }
    public string SubjectCode { get; set; } = null!;
    public decimal Mark { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    public virtual StudentProfile StudentProfile { get; set; } = null!;
}