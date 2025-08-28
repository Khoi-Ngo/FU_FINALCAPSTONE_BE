using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.DAL.Entities;

[Table("Subject")]
public partial class Subject
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

    public virtual ICollection<SubjectVersion> SubjectVersions { get; set; } = new List<SubjectVersion>();

    public virtual ICollection<ComboSubject> ComboSubjects { get; set; } = new List<ComboSubject>();

    public virtual ICollection<SubjectComment> Comments { get; set; } = new List<SubjectComment>();

    // public virtual ICollection<StudentEnrollment> StudentEnrollments { get; set; } = new List<StudentEnrollment>();
    
    // Approval properties
    public EApprovalStatus ApprovalStatus { get; set; } = EApprovalStatus.PENDING;
    public string? CreatedBy { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }
    
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}