using AutoMapper;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.SubjectComment;
using AISEA.ApiService.SHARED.DTOs.Responses.SubjectComment;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.Exceptions;

namespace AISEA.ApiService.BAL.Services.SubjectComment
{
    public class SubjectCommentService
    {
        private readonly SubjectCommentRepository _commentRepository;
        private readonly SubjectRepository _subjectRepository;
        private readonly JoinedSubjectRepository _joinedSubjectRepository;
        private readonly IJWTService _jwtService;
        private readonly IMapper _mapper;

        public SubjectCommentService(
            SubjectCommentRepository commentRepository,
            SubjectRepository subjectRepository,
            JoinedSubjectRepository joinedSubjectRepository,
            IJWTService jwtService,
            IMapper mapper)
        {
            _commentRepository = commentRepository;
            _subjectRepository = subjectRepository;
            _joinedSubjectRepository = joinedSubjectRepository;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        public async Task<long> CreateCommentAsync(CreateSubjectCommentRequest request, string accessToken)
        {
            var studentProfileId = _jwtService.GetProfileIdFromToken(accessToken);

            // 1. Validate subject exists
            var subject = await _subjectRepository.GetByIdAsync(request.SubjectId);
            if (subject == null || subject.IsDeleted)
            {
                throw new NotFoundException("Subject not found.");
            }

            // 2. Validate student has completed the subject
            var canComment = await _joinedSubjectRepository.IsValidToPostComment(studentProfileId, subject.SubjectCode);
            if (!canComment)
            {
                throw new InvalidUserCreatedException("You can only comment on subjects you have completed and passed.");
            }

            // 3. Check if student already commented on this subject
            var existingComment = await _commentRepository.GetByStudentAndSubjectAsync(studentProfileId, request.SubjectId);
            if (existingComment != null)
            {
                throw new InvalidUserCreatedException("You have already commented on this subject.");
            }

            // 4. Create comment
            var comment = _mapper.Map<DAL.Entities.SubjectComment>(request);
            comment.StudentProfileId = studentProfileId;
            comment.Email = _jwtService.GetEmailFromToken(accessToken);
            comment.FullName = _jwtService.GetFullNameFromToken(accessToken);
            comment.CreatedAt = DateTime.UtcNow;

            await _commentRepository.CreateAsync(comment);
            return comment.Id;
        }





        public async Task<SubjectCommentResponse> GetCommentByIdAsync(long id, string? accessToken = null)
        {
            var comment = await _commentRepository.GetByIdWithDetailsAsync(id);
            if (comment == null)
            {
                throw new NotFoundException("Comment not found.");
            }

            var response = _mapper.Map<SubjectCommentResponse>(comment);

            // Set user reaction if authenticated
            if (!string.IsNullOrEmpty(accessToken))
            {
                try
                {
                    var studentProfileId = _jwtService.GetProfileIdFromToken(accessToken);
                    response.UserReaction = comment.GetUserReaction(studentProfileId);
                }
                catch
                {
                    // Ignore if token is invalid
                }
            }

            return response;
        }

        public async Task<PagedResult<SubjectCommentResponse>> GetSubjectCommentsAsync(
            long subjectId, PaginationRequest request, string? accessToken = null)
        {
            var (comments, totalCount) = await _commentRepository.GetPagedBySubjectAsync(
                subjectId, request.PageNumber, request.PageSize);

            long? currentStudentId = null;
            if (!string.IsNullOrEmpty(accessToken))
            {
                try
                {
                    currentStudentId = _jwtService.GetProfileIdFromToken(accessToken);
                }
                catch
                {
                    // Ignore if token is invalid
                }
            }

            var responses = comments.Select(comment =>
            {
                var response = _mapper.Map<SubjectCommentResponse>(comment);
                if (currentStudentId.HasValue)
                {
                    response.UserReaction = comment.GetUserReaction(currentStudentId.Value);
                }
                return response;
            }).ToList();

            return new PagedResult<SubjectCommentResponse>
            {
                Items = responses,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }



        public async Task<CommentReactionResponse> ToggleReactionAsync(long commentId, ToggleReactionRequest request, string accessToken)
        {
            var studentProfileId = _jwtService.GetProfileIdFromToken(accessToken);
            var comment = await _commentRepository.GetByIdAsync(commentId);

            if (comment == null)
            {
                throw new NotFoundException("Comment not found.");
            }



            var currentReaction = comment.GetUserReaction(studentProfileId);
            string action;

            if (currentReaction == null)
            {
                // Add new reaction
                if (request.ReactionType == EReactionType.LIKE)
                    comment.AddLike(studentProfileId);
                else
                    comment.AddUnlike(studentProfileId);
                action = "added";
            }
            else if (currentReaction == request.ReactionType)
            {
                // Remove same reaction
                comment.RemoveReaction(studentProfileId);
                action = "removed";
            }
            else
            {
                // Change reaction type
                if (request.ReactionType == EReactionType.LIKE)
                    comment.AddLike(studentProfileId);
                else
                    comment.AddUnlike(studentProfileId);
                action = "changed";
            }

            await _commentRepository.UpdateAsync(comment);

            return new CommentReactionResponse
            {
                CommentId = commentId,
                LikeCount = comment.LikeCount,
                UnlikeCount = comment.UnlikeCount,
                UserReaction = comment.GetUserReaction(studentProfileId),
                Action = action
            };
        }
    }
}
