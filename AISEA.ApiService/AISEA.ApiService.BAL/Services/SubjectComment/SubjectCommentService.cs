using AutoMapper;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.SubjectComment;
using AISEA.ApiService.SHARED.DTOs.Responses.SubjectComment;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.Exceptions;
using Microsoft.Extensions.Logging;

namespace AISEA.ApiService.BAL.Services.SubjectComment
{
    public class SubjectCommentService
    {
        private readonly SubjectCommentRepository _commentRepository;
        private readonly SubjectRepository _subjectRepository;
        private readonly JoinedSubjectRepository _joinedSubjectRepository;
        private readonly IJWTService _jwtService;
        private readonly IMapper _mapper;
        private readonly ILogger<SubjectCommentService> _logger;

        public SubjectCommentService(
            SubjectCommentRepository commentRepository,
            SubjectRepository subjectRepository,
            JoinedSubjectRepository joinedSubjectRepository,
            IJWTService jwtService,
            IMapper mapper,
            ILogger<SubjectCommentService> logger)
        {
            _commentRepository = commentRepository;
            _subjectRepository = subjectRepository;
            _joinedSubjectRepository = joinedSubjectRepository;
            _jwtService = jwtService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<long> CreateCommentAsync(CreateSubjectCommentRequest request, string accessToken)
        {
            var studentProfileId = _jwtService.GetProfileIdFromToken(accessToken);

            var subject = await _subjectRepository.GetByIdAsync(request.SubjectId);
            var canComment = await _joinedSubjectRepository.IsValidToPostComment(studentProfileId, subject.SubjectCode);
            if (!canComment)
            {
                throw new InvalidUserCreatedException("You can only comment on subjects you have completed and passed.");
            }

            var comment = _mapper.Map<DAL.Entities.SubjectComment>(request);
            comment.StudentProfileId = studentProfileId;
            comment.Email = _jwtService.GetEmailFromToken(accessToken);

            // Construct full name from first and last name
            var firstName = _jwtService.GetFirstNameFromToken(accessToken);
            var lastName = _jwtService.GetLastNameFromToken(accessToken);
            comment.FullName = $"{firstName} {lastName}".Trim();


            await _commentRepository.CreateAsync(comment);
            return comment.Id;
        }


        public async Task<SubjectCommentResponse> GetCommentByIdAsync(long id, string? accessToken = null)
        {
            var comment = await _commentRepository.GetByIdWithDetailsAsync(id);

            var response = _mapper.Map<SubjectCommentResponse>(comment);

            if (_jwtService.GetRoleIdFromToken(accessToken) == (int)EUserRole.STUDENT)
            {
                var studentProfileId = _jwtService.GetProfileIdFromToken(accessToken);
                response.UserReaction = comment.GetUserReaction(studentProfileId);
            }

            return response;
        }

        public async Task<PagedResult<SubjectCommentResponse>> GetSubjectCommentsAsync(
        long subjectId, GetSubjectCommentsRequest request, string? accessToken = null)
        {
            var (comments, totalCount) = await _commentRepository.GetPagedBySubjectAsync(
                subjectId, request.PageNumber, request.PageSize, request.SortBy, request.SortDirection);

            var roleId = _jwtService.GetRoleIdFromToken(accessToken);
            long? currentStudentId = roleId == (int)EUserRole.STUDENT
                ? (long?)_jwtService.GetProfileIdFromToken(accessToken)
                : null;

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

        public async Task DeleteCommentAsync(long commentId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            await _commentRepository.RemoveAsync(comment);
        }
    }
}
