using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.DAL.Entities;

[Table("Syllabus")]
public partial class Syllabus
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column(TypeName = "text")]
    public string Content { get; set; } = null!;
    
    public long SubjectVersionId { get; set; }

    public virtual ICollection<SyllabusAssessment> SyllabusAssessments { get; set; } = new List<SyllabusAssessment>();

    public virtual ICollection<SyllabusLearningMaterial> SyllabusLearningMaterials { get; set; } = new List<SyllabusLearningMaterial>();

    public virtual ICollection<SyllabusLearningOutcome> SyllabusLearningOutcomes { get; set; } = new List<SyllabusLearningOutcome>();

    public virtual ICollection<SyllabusSession> SyllabusSessions { get; set; } = new List<SyllabusSession>();
    
    [ForeignKey(nameof(SubjectVersionId))]
    [InverseProperty("Syllabi")]
    public virtual SubjectVersion SubjectVersion { get; set; } = null!;
    
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