using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Requests.User;
using AISEA.ApiService.SHARED.DTOs.Responses.Auth;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.User
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<DAL.Entities.User, AuthResponse>();
            CreateMap<CreateUserRequest, DAL.Entities.User>();
            CreateMap<DAL.Entities.User, GetUserListResponse>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));

            //TODO: Map User to GetUserDetailResponse more specifically
            CreateMap<DAL.Entities.User, GetUserDetailResponse>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));
        }
    }
}