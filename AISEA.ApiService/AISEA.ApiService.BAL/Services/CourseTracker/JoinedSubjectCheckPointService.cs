using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.CheckPoint;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AutoMapper;

namespace AISEA.ApiService.BAL.Services.CourseTracker;

public class JoinedSubjectCheckPointService
{
    #region  Init 

    private readonly IJWTService _jWTService;
    private readonly JoinedSubjectRepository _joinedSubjectRepository;
    private readonly JoinedSubjectCheckPointRepository _joinedSubjectCheckPointRepository;
    private readonly IMapper _mapper;

    public JoinedSubjectCheckPointService(IJWTService jWTService, JoinedSubjectRepository joinedSubjectRepository, JoinedSubjectCheckPointRepository joinedSubjectCheckPointRepository, IMapper mapper)
    {
        _jWTService = jWTService;
        _joinedSubjectRepository = joinedSubjectRepository;
        _joinedSubjectCheckPointRepository = joinedSubjectCheckPointRepository;
        _mapper = mapper;
    }

    #endregion


    public async Task CreateAsync(CommandCheckpointRequest request, long joinedSubjectId, string accessToken)
    {
        var joinedSubject = await _joinedSubjectRepository.GetByIdAsync(joinedSubjectId);
        if (!IsValidAccessJoinedSubject(accessToken, joinedSubject)) throw new InvalidAccessJoinedSubject("You cannot create checkpoint in this subject");
        await _joinedSubjectCheckPointRepository.CreateAsync(_mapper.Map<JoinedSubjectCheckPoint>(request));
    }

    public async Task RemoveAsync(long joinedSubjectCheckpointId, string accessToken)
    {
        var checkpoint = await _joinedSubjectCheckPointRepository.GetByIdWithJoinedSubjectAsync(joinedSubjectCheckpointId);
        if (!IsValidAccessCheckpoint(accessToken, checkpoint)) throw new InvalidAccessCheckpoint("No permission for this checkpoint");
        await _joinedSubjectCheckPointRepository.RemoveAsync(checkpoint);
    }


    #region Private methods

    //validate the access to the joined subject
    private bool IsValidAccessJoinedSubject(string accessToken, JoinedSubject joinedSubject)
    => joinedSubject.StudentProfileId == _jWTService.GetProfileIdFromToken(accessToken);

    //validate the access to the joined subject's checkpoint
    private bool IsValidAccessCheckpoint(string accessToken, JoinedSubjectCheckPoint checkPoint)
    => checkPoint.JoinedSubject.StudentProfileId == _jWTService.GetProfileIdFromToken(accessToken);

    #endregion
}