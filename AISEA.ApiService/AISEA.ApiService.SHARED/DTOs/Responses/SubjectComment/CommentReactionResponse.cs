using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.DTOs.Responses.SubjectComment
{
    public class CommentReactionResponse
    {
        public long CommentId { get; set; }
        public int LikeCount { get; set; }
        public int UnlikeCount { get; set; }
        public EReactionType? UserReaction { get; set; }
        public string Action { get; set; } = null!; // "added", "removed", "changed"
    }
}
