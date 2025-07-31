using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.DAL.Entities;

[Table("Combo")]
public partial class Combo
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [StringLength(255)]
    public string ComboName { get; set; } = null!;

    [Column(TypeName = "text")]
    public string? ComboDescription { get; set; }

    public virtual ICollection<ComboSubject> ComboSubjects { get; set; } = new List<ComboSubject>();
    
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