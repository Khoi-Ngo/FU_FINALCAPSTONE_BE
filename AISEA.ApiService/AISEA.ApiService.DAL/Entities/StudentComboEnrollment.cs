using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("StudentComboEnrollment")]
public partial class StudentComboEnrollment : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    
    public long StudentId { get; set; }
    
    public long ComboId { get; set; }
    
    public DateTimeOffset EnrolledAt { get; set; }
    
    [StringLength(50)]
    public string Status { get; set; } = "Active"; // Active, Completed, Dropped, Withdrawn
    
    [Column(TypeName = "nvarchar(max)")]
    public string? Notes { get; set; }
    
    [ForeignKey("StudentId")]
    public virtual StudentProfile Student { get; set; } = null!;
    
    [ForeignKey("ComboId")]
    public virtual Combo Combo { get; set; } = null!;
}