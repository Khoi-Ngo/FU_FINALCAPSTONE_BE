using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Entities;

[Table("StudentProfile")]
[Index("StudentCode", Name = "student_code_unique", IsUnique = true)]
public partial class StudentProfile : BaseEntity
{
    [Key]
    [Column("user_id")]
    public long UserId { get; set; }

    [StringLength(20)]
    public string StudentCode { get; set; } = null!;

    public DateTimeOffset EnrollDate { get; set; }

    public double CurrentGpa { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("StudentProfiles")]
    public virtual User User { get; set; } = null!;
}