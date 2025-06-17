using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Responses.Auth;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.User
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<DAL.Entities.User, AuthResponse>();
        }
    }
}