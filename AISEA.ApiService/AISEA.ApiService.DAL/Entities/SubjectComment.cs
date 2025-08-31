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
        public bool IsScannedToValidate { get; set; } = false;

        // Foreign Keys (NO JoinedSubjectId)
        public long SubjectId { get; set; }
        public long StudentProfileId { get; set; }
        public required string Email { get; set; } // Get From JWT Token
        public required string FullName { get; set; } // Get From JWT Token
        // Comment Content
        [Column(TypeName = "text")]
        public string Content { get; set; } = null!;

        // Reaction Storage - Store student IDs as comma-separated strings
        public string? LikedByStudentIds { get; set; } // "1,2,3,5"
        public string? UnlikedByStudentIds { get; set; } // "4,6,7"

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; } = null!;

        [ForeignKey("StudentProfileId")]
        public virtual StudentProfile StudentProfile { get; set; } = null!;

        // Computed properties for reactions
        [NotMapped]
        public List<long> LikedByStudents => ParseStudentIds(LikedByStudentIds);

        [NotMapped]
        public List<long> UnlikedByStudents => ParseStudentIds(UnlikedByStudentIds);

        [NotMapped]
        public int LikeCount => LikedByStudents.Count;

        [NotMapped]
        public int UnlikeCount => UnlikedByStudents.Count;

        // Helper methods for reaction management
        private List<long> ParseStudentIds(string? ids)
        {
            if (string.IsNullOrEmpty(ids)) return new List<long>();

            return ids.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(id => long.TryParse(id.Trim(), out var result) ? result : 0)
                      .Where(id => id > 0)
                      .ToList();
        }

        public void AddLike(long studentId)
        {
            RemoveReaction(studentId); // Remove from both first
            var likes = LikedByStudents;
            if (!likes.Contains(studentId))
            {
                likes.Add(studentId);
                LikedByStudentIds = string.Join(",", likes);
            }
        }

        public void AddUnlike(long studentId)
        {
            RemoveReaction(studentId); // Remove from both first
            var unlikes = UnlikedByStudents;
            if (!unlikes.Contains(studentId))
            {
                unlikes.Add(studentId);
                UnlikedByStudentIds = string.Join(",", unlikes);
            }
        }

        public void RemoveReaction(long studentId)
        {
            var likes = LikedByStudents;
            var unlikes = UnlikedByStudents;

            likes.Remove(studentId);
            unlikes.Remove(studentId);

            LikedByStudentIds = likes.Any() ? string.Join(",", likes) : null;
            UnlikedByStudentIds = unlikes.Any() ? string.Join(",", unlikes) : null;
        }

        public EReactionType? GetUserReaction(long studentId)
        {
            if (LikedByStudents.Contains(studentId)) return EReactionType.LIKE;
            if (UnlikedByStudents.Contains(studentId)) return EReactionType.UNLIKE;
            return null;
        }
    }
}
