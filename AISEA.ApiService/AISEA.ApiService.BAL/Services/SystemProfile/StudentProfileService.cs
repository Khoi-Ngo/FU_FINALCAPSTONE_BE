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

    public async Task ResetNumberOfBansAsync()
    {
        await _studentProfileRepository.ResetNumberOfBansAsync();
    }
    public async Task IncreaseNumberOfBansAsync(Dictionary<long, int> studentProfileIdToBanIncrement)
    {
        await _studentProfileRepository.IncreaseNumberOfBansAsync(studentProfileIdToBanIncrement);
    }

    private bool IsValidAccess(string accessToken, long userId)
    => _jWTService.GetRoleIdFromToken(accessToken) == (long)EUserRole.ADMIN
    || _jWTService.GetUserIdFromToken(accessToken) == userId;

    public async Task IncreaseNumberOfBansAsync(long studentProfileId, int numberOfBan)
    {
        var studentProfile = await _studentProfileRepository.GetByIdAsync(studentProfileId);
        studentProfile.NumberOfBan = studentProfile.NumberOfBan + numberOfBan;
        await _studentProfileRepository.UpdateAsync(studentProfile);
    }
}