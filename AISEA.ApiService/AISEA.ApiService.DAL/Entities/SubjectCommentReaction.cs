using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.DAL.Entities
{
    [Table("SubjectCommentReaction")]
    public class SubjectCommentReaction
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        public long CommentId { get; set; }
        public long StudentProfileId { get; set; }
        
        public EReactionType ReactionType { get; set; } // LIKE, UNLIKE

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("CommentId")]
        [InverseProperty("Reactions")]
        public virtual SubjectComment Comment { get; set; } = null!;

        [ForeignKey("StudentProfileId")]
        public virtual StudentProfile StudentProfile { get; set; } = null!;
    }
}
