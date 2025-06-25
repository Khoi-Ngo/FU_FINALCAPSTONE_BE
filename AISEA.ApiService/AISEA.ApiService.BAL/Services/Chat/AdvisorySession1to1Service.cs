using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.AdvisorySession1to1;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AutoMapper;

namespace AISEA.ApiService.BAL.Services.Chat
{
    public class AdvisorySession1to1Service
    {
        private readonly AdvisorySession1to1Repository _advisorySession1To1Repository;
        private readonly IJWTService _jWTService;
        private readonly IMapper _mapper;
        private readonly UserRepository _userRepository;
        public AdvisorySession1to1Service(AdvisorySession1to1Repository advisorySession1To1Repository, IJWTService jWTService, IMapper mapper, UserRepository userRepository)
        {
            _advisorySession1To1Repository = advisorySession1To1Repository;
            _jWTService = jWTService;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task DeleteAsync(long id, string accessToken)
        {
            var username = _jWTService.GetUsernameFromToken(accessToken);
            var roleId = _jWTService.GetUserRoleIdFromToken(accessToken);
            long profileId = roleId == (long)EUserRole.STUDENT ?
             await _userRepository.GetStudentProfileIdByUsernameAsync(username) :
             await _userRepository.GetStaffProfileIdByUsernameAsync(username);

            var session = await _advisorySession1To1Repository.GetByIdAsync(id, profileId);
            if (session is null)
            {
                throw new NotFoundException("No permission");
            }

            await _advisorySession1To1Repository.RemoveAsync(session);
        }

    }
}


#region Ignore
        // public async Task<PagedResult<GetAdvisorySession1to1ListResponse>> GetAllByStaffSelfPagedAsync(PaginationRequest request, string accessToken)
        // {
        //     var staffUsername = _jWTService.GetUsernameFromToken(accessToken);
        //     var staffProfileId = await _userRepository.GetStaffProfileIdByUsernameAsync(staffUsername);

        //     var (sessions, totalCount) = await _advisorySession1To1Repository.GetAllByStaffSelfPagedAsync(request.PageNumber, request.PageSize, staffProfileId);
        //     return new PagedResult<GetAdvisorySession1to1ListResponse>
        //     {
        //         Items = _mapper.Map<List<GetAdvisorySession1to1ListResponse>>(sessions),
        //         TotalCount = totalCount,
        //         PageNumber = request.PageNumber,
        //         PageSize = request.PageSize
        //     };
        // }

        // public async Task<PagedResult<GetAdvisorySession1to1ListResponse>> GetAllByStudentSelfAsync(PaginationRequest request, string accessToken)
        // {
        //     var studentUsername = _jWTService.GetUsernameFromToken(accessToken);
        //     var studentProfileId = await _userRepository.GetStudentProfileIdByUsernameAsync(studentUsername);

        //     var (sessions, totalCount) = await _advisorySession1To1Repository.GetAllByStudentSelfPagedAsync(request.PageNumber, request.PageSize, studentProfileId);
        //     return new PagedResult<GetAdvisorySession1to1ListResponse>
        //     {
        //         Items = _mapper.Map<List<GetAdvisorySession1to1ListResponse>>(sessions),
        //         TotalCount = totalCount,
        //         PageNumber = request.PageNumber,
        //         PageSize = request.PageSize
        //     };
        // }

        // public async Task<PagedResult<GetAdvisorySession1to1ListResponse>> GetAllOpenAsync(PaginationRequest request)
        // {

        //     var (sessions, totalCount) = await _advisorySession1To1Repository.GetAllOpenPagedAsync(request.PageNumber, request.PageSize);
        //     return new PagedResult<GetAdvisorySession1to1ListResponse>
        //     {
        //         Items = _mapper.Map<List<GetAdvisorySession1to1ListResponse>>(sessions),
        //         TotalCount = totalCount,
        //         PageNumber = request.PageNumber,
        //         PageSize = request.PageSize
        //     };
        // }

#endregion