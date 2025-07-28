using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.SystemProfile;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AutoMapper;

namespace AISEA.ApiService.BAL.Services.SystemProfile;

public class StudentProfileService
{
    private readonly IMapper _mapper;
    private readonly StudentProfileRepository _studentProfileRepository;
    private readonly IJWTService _jWTService;

    public StudentProfileService(IMapper mapper, StudentProfileRepository studentProfileRepository, IJWTService jWTService)
    {
        _mapper = mapper;
        _studentProfileRepository = studentProfileRepository;
        _jWTService = jWTService;
    }

    public async Task CreateAsync(CreateStudentProfileRequest request, string accessToken)
    {
        if (!IsValidAccess(accessToken, request.UserId)) throw new InvalidAccessUserException("No permission to create new profile for this user");
        var studentProfile = _mapper.Map<StudentProfile>(request);
        await _studentProfileRepository.CreateAsync(studentProfile);
    }

    public async Task<StudentProfile> GetByIdAsync(long studentProfileId)
    {
        return await _studentProfileRepository.GetByIdAsync(studentProfileId);
    }

    private bool IsValidAccess(string accessToken, long userId)
    => _jWTService.GetRoleIdFromToken(accessToken) == (long)EUserRole.ADMIN
    || _jWTService.GetUserIdFromToken(accessToken) == userId;
}