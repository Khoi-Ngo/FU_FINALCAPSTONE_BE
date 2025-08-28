using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.DAL.Entities
{
    [Table("SubjectComment")]
    public class SubjectComment
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        // Foreign Keys
        public long SubjectId { get; set; }
        public long StudentProfileId { get; set; }
        public long JoinedSubjectId { get; set; } // Reference to completed subject

        // Comment Content
        [Column(TypeName = "text")]
        public string Content { get; set; } = null!;

        // Status
        public bool IsAnonymous { get; set; } = false;
        public bool IsApproved { get; set; } = false; // Moderation
        public string? ModerationReason { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("SubjectId")]
        [InverseProperty("Comments")]
        public virtual Subject Subject { get; set; } = null!;

        [ForeignKey("StudentProfileId")]
        [InverseProperty("SubjectComments")]
        public virtual StudentProfile StudentProfile { get; set; } = null!;

        [ForeignKey("JoinedSubjectId")]
        [InverseProperty("SubjectComment")]
        public virtual JoinedSubject JoinedSubject { get; set; } = null!;

        // Reactions collection
        [InverseProperty("Comment")]
        public virtual ICollection<SubjectCommentReaction> Reactions { get; set; } = new List<SubjectCommentReaction>();
    }
}
