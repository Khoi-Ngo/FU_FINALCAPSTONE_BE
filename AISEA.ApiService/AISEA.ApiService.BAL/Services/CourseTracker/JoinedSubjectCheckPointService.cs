using System.Text.Json;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.Const.Values;
using AISEA.ApiService.SHARED.DTOs.Requests.CheckPoint;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.CheckPoint;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.User;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AutoMapper;

namespace AISEA.ApiService.BAL.Services.CourseTracker;

public class JoinedSubjectCheckPointService
{
    #region  Init 

    private readonly IJWTService _jWTService;
    private readonly JoinedSubjectRepository _joinedSubjectRepo;
    private readonly JoinedSubjectCheckPointRepository _checkpointRepo;
    private readonly IMapper _mapper;
    private readonly UserRepository _userRepository;
    private readonly SubjectRepository _subjectRepository;
    private readonly IChatOpenAIService _chatOpenAIService;

    public JoinedSubjectCheckPointService(IJWTService jWTService, JoinedSubjectRepository joinedSubjectRepo, JoinedSubjectCheckPointRepository checkpointRepo, IMapper mapper, UserRepository userRepository, SubjectRepository subjectRepository, IChatOpenAIService chatOpenAIService)
    {
        _jWTService = jWTService;
        _joinedSubjectRepo = joinedSubjectRepo;
        _checkpointRepo = checkpointRepo;
        _mapper = mapper;
        _userRepository = userRepository;
        _subjectRepository = subjectRepository;
        _chatOpenAIService = chatOpenAIService;
    }




    #endregion


    public async Task CreateAsync(CommandCheckpointRequest request, long joinedSubjectId, string accessToken)
    {
        var joinedSubject = await _joinedSubjectRepo.GetByIdAsync(joinedSubjectId);
        if (!IsValidAccessJoinedSubject(accessToken, joinedSubject)) throw new InvalidAccessJoinedSubject("You cannot create checkpoint in this subject");
        var insertedCheckpoint = _mapper.Map<JoinedSubjectCheckPoint>(request);
        insertedCheckpoint.JoinedSubjectId = joinedSubjectId;
        await _checkpointRepo.CreateAsync(insertedCheckpoint);
    }


    public async Task CreateAsync(List<CommandCheckpointRequest> request, bool doReplaceAll, long joinedSubjectId, string accessToken)
    {
        var joinedSubject = await _joinedSubjectRepo.GetByIdAsync(joinedSubjectId);
        if (!IsValidAccessJoinedSubject(accessToken, joinedSubject)) throw new InvalidAccessJoinedSubject("You cannot create checkpoint in this subject");
        if (doReplaceAll) await _checkpointRepo.RemoveByJoinedSubjectIdAsync(joinedSubjectId);
        var checkPoints = _mapper.Map<List<JoinedSubjectCheckPoint>>(request, opt =>
        {
            opt.Items["JoinedSubjectId"] = joinedSubjectId;
        });
        await _checkpointRepo.BulkInsertAsync(checkPoints);
    }

    public async Task RemoveAsync(long id, string accessToken)
    {
        var checkpoint = await _checkpointRepo.GetByIdWithJoinedSubjectAsync(id);
        if (!IsValidAccessCheckpoint(accessToken, checkpoint)) throw new InvalidAccessCheckpoint("No permission for this checkpoint");
        await _checkpointRepo.RemoveAsync(checkpoint);
    }

    public async Task UpdateAsync(CommandCheckpointRequest request, long id, string accessToken)
    {
        var checkpoint = await _checkpointRepo.GetByIdAsync(id);
        if (!IsValidAccessCheckpoint(accessToken, checkpoint)) throw new InvalidAccessCheckpoint("No permission for this checkpoint");
        _mapper.Map(request, checkpoint);
        await _checkpointRepo.UpdateAsync(checkpoint);
    }


    public async Task CompleteAsync(long id, string accessToken)
    {
        var checkpoint = await _checkpointRepo.GetByIdAsync(id);
        if (!IsValidAccessCheckpoint(accessToken, checkpoint)) throw new InvalidAccessCheckpoint("No permission for this checkpoint");
        checkpoint.IsCompleted = true;
        await _checkpointRepo.UpdateAsync(checkpoint);
    }


    public async Task<CheckpointDetailResponse> ViewDetailByIdAsync(long id)
    {
        //no validation for access for better performance
        var checkpoint = await _checkpointRepo.GetByIdAsync(id);
        return _mapper.Map<CheckpointDetailResponse>(checkpoint);
    }


    public async Task<List<CheckpointListItemResponse>> ViewAllByJoinedSubjectIdAsync(long joinedSubjectId)
    {
        var checkpoints = await _checkpointRepo.GetByJoinedSubjectIdAsync(joinedSubjectId);
        return _mapper.Map<List<CheckpointListItemResponse>>(checkpoints);
    }



    public async Task<PagedResult<CheckpointListItemResponse>> ViewAllByStudentProfileIdAsync(long studentProfileId, PaginationRequest paginationRequest, bool isInCompletedOnly, bool isNoneFilterStatus, bool isOrderedByNearToFarDeadline, string accessToken)
    {
        isInCompletedOnly = isNoneFilterStatus ? false : isInCompletedOnly;
        bool isActiveOnly = _jWTService.GetRoleIdFromToken(accessToken) == (int)EUserRole.STUDENT;
        var (checkpoints, totalCount) = await _checkpointRepo.GetAllByStudentProfileIdAsync(studentProfileId, isInCompletedOnly, isOrderedByNearToFarDeadline, isActiveOnly, paginationRequest);

        return new PagedResult<CheckpointListItemResponse>
        {
            Items = _mapper.Map<List<CheckpointListItemResponse>>(checkpoints),
            TotalCount = totalCount,
            PageNumber = paginationRequest.PageNumber,
            PageSize = paginationRequest.PageSize
        };

    }

    public async Task<List<CheckpointListItemResponse>> ViewAllBySelfUpcomingAsync(int limit, string accessToken)
    {
        var studentProfileId = _jWTService.GetProfileIdFromToken(accessToken);
        var checkpoints = await _checkpointRepo.GetAllByStuProfileIdUpcomingAsync(studentProfileId, limit);
        return _mapper.Map<List<CheckpointListItemResponse>>(checkpoints);
    }



    public async Task<List<CommandCheckpointRequest>> GenerateCheckpointsAsync(long joinedSubjectId, string accessToken, string studentMessage)
    {
        var studentSenderName = _jWTService.GetFirstNameFromToken(accessToken) + _jWTService.GetLastNameFromToken(accessToken);
        //query joined subject with mark report
        var joinedSubjectData = await GetJoinedSubjectData(joinedSubjectId);
        //query the FLM Resource data for the subject
        // var flmSylabusData = await GetFlmSylabusData(joinedSubjectData.SubjectCode, joinedSubjectData.SubjectVersionCode);
        //query student data
        var studentData = await GetStudentData(accessToken);

        //Construct Prompt
        var joinedSubjectJson = JsonSerializer.Serialize(joinedSubjectData, new JsonSerializerOptions { WriteIndented = true });
        // var flmSylabusJson = JsonSerializer.Serialize(flmSylabusData, new JsonSerializerOptions { WriteIndented = true });
        var studentJson = JsonSerializer.Serialize(studentData, new JsonSerializerOptions { WriteIndented = true });



        var userPrompt = CallAIConst.TemplatePromptForGenTodoForJoinedSubject
            // .Replace("{FLMSylabusData}", flmSylabusJson)
            .Replace("{JoinedSubjectData}", joinedSubjectJson)
            .Replace("{StudentMessage}", studentMessage)
            .Replace("{StudentData}", studentJson)
            .Replace("{StudentSenderName}", studentSenderName)
            .Replace("{EnrolledDateTime}", joinedSubjectData.CreatedAt.ToString("o")) // ISO 8601 format
            .Replace("{CurrentDateTime}", DateTime.UtcNow.ToString("o"));
        //call OpenAI then return the result
        var res = await _chatOpenAIService.GenerateCheckpoints(userPrompt);
        return res;
    }



    #region Private methods

    //validate the access to the joined subject
    private bool IsValidAccessJoinedSubject(string accessToken, JoinedSubject joinedSubject)
    => joinedSubject.StudentProfileId == _jWTService.GetProfileIdFromToken(accessToken);

    //validate the access to the joined subject's checkpoint
    private bool IsValidAccessCheckpoint(string accessToken, JoinedSubjectCheckPoint checkPoint)
    => checkPoint.JoinedSubject.StudentProfileId == _jWTService.GetProfileIdFromToken(accessToken);


    private async Task<JoinedSubject> GetJoinedSubjectData(long joinedSubjectId)
    {
        //TODO: include mark report of this joined subject later + Get From redis
        var joinedSubject = await _joinedSubjectRepo.GetByIdAsync(joinedSubjectId);
        return joinedSubject;
    }
    private async Task<object> GetFlmSylabusData(string subjectCode, string subjectVersion)
    {
        //TODO: Get from redis later
        return "";
    }
    private async Task<GetStudentDetailResponse> GetStudentData(string accessToken)
    {
        //TODO: Get from redis later
        var userId = _jWTService.GetUserIdFromToken(accessToken);

        var student = await _userRepository.GetStudentByIdAsync(userId);

        if (student is null)
        {
            throw new NotFoundException("Student not found.");
        }

        return _mapper.Map<GetStudentDetailResponse>(student);
    }


    #endregion
}