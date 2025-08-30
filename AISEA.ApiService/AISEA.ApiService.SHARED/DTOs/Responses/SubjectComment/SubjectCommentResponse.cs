using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.DTOs.Responses.SubjectComment
{
    public class SubjectCommentResponse
    {
        public long Id { get; set; }
        public long SubjectId { get; set; }
        public string SubjectName { get; set; } = null!;
        public string SubjectCode { get; set; } = null!;
        public long StudentProfileId { get; set; }
        public string StudentName { get; set; } = null!;
        public string StudentCode { get; set; } = null!;
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        // Reaction counts
        public int LikeCount { get; set; }
        public int UnlikeCount { get; set; }
        
        // Current user's reaction (if authenticated)
        public EReactionType? UserReaction { get; set; }
        


    }
}
