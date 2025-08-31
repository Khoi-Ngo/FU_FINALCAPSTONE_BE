using Microsoft.AspNetCore.Mvc;
using AISEA.ApiService.BAL.Services.SubjectComment;
using AISEA.ApiService.SHARED.DTOs.Requests.SubjectComment;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.InterceptorAPI;

namespace AISEA.ApiService.WebApi.Controllers.SubjectComment
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectCommentController : BaseController
    {
        private readonly SubjectCommentService _subjectCommentService;

        public SubjectCommentController(EndpointSettings endpointSettings, SubjectCommentService subjectCommentService) : base(endpointSettings)
        {
            _subjectCommentService = subjectCommentService;
        }

        /// <summary>
        /// Create a new comment for a subject (Students only - must have completed the subject)
        /// </summary>
        [HttpPost]
        [PermissionAuthorize((int)EUserRole.STUDENT)]
        [AuditLog(Tag = "CREATE_SUBJECT_COMMENT")]
        public async Task<IActionResult> CreateComment([FromBody] CreateSubjectCommentRequest request)
        {
            var commentId = await _subjectCommentService.CreateCommentAsync(request, AccessToken);
            return Ok(new { 
                CommentId = commentId, 
                Message = "Comments Successfully" 
            });
        }





        /// <summary>
        /// Get comments for a specific subject with sorting options (Public)
        /// </summary>
        /// <param name="subjectId">Subject ID</param>
        /// <param name="request">Pagination and sorting parameters</param>
        /// <returns>Paginated list of comments</returns>
        [HttpGet("subject/{subjectId}")]
        public async Task<IActionResult> GetSubjectComments(
            long subjectId,
            [FromQuery] GetSubjectCommentsRequest request)
        {
            var result = await _subjectCommentService.GetSubjectCommentsAsync(
                subjectId, request, AccessToken);
            return Ok(result);
        }

        /// <summary>
        /// Get a specific comment by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentById(long id)
        {
            var result = await _subjectCommentService.GetCommentByIdAsync(id, AccessToken);
            return Ok(result);
        }



        /// <summary>
        /// Toggle reaction on a comment (Students only - like/unlike)
        /// </summary>
        [HttpPost("{id}/reactions")]
        [PermissionAuthorize((int)EUserRole.STUDENT)]
        [AuditLog(Tag = "TOGGLE_COMMENT_REACTION")]
        public async Task<IActionResult> ToggleReaction(long id, [FromBody] ToggleReactionRequest request)
        {
            var result = await _subjectCommentService.ToggleReactionAsync(id, request, AccessToken);
            return Ok(result);
        }

        /// <summary>
        /// Get reaction counts for a comment
        /// </summary>
        [HttpGet("{id}/reactions")]
        public async Task<IActionResult> GetCommentReactions(long id)
        {
            var comment = await _subjectCommentService.GetCommentByIdAsync(id, AccessToken);
            return Ok(new
            {
                CommentId = id,
                LikeCount = comment.LikeCount,
                UnlikeCount = comment.UnlikeCount,
                UserReaction = comment.UserReaction
            });
        }

        /// <summary>
        /// Delete a comment (Students can delete their own comments, Admins can delete any comment)
        /// </summary>
        [HttpDelete("{id}")]
        [PermissionAuthorize((int)EUserRole.STUDENT, (int)EUserRole.ADMIN)]
        [AuditLog(Tag = "DELETE_SUBJECT_COMMENT")]
        public async Task<IActionResult> DeleteComment(long id)
        {
            await _subjectCommentService.DeleteCommentAsync(id);
            return Ok("Comment deleted successfully");
        }
    }
}
