using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.SystemProfile;
using AutoMapper;

namespace AISEA.ApiService.BAL.Services.SystemProfile;

public class StaffProfileService
{
    private readonly IMapper _mapper;
    private readonly StaffProfileRepository _staffProfileRepository;

    public StaffProfileService(IMapper mapper, StaffProfileRepository staffProfileRepository)
    {
        _mapper = mapper;
        _staffProfileRepository = staffProfileRepository;
    }

    public async Task CreateAsync(CreateStaffProfileRequest request, string accessToken)
    {
        var staffProfile = _mapper.Map<StaffProfile>(request);
        await _staffProfileRepository.CreateAsync(staffProfile);
    }

}