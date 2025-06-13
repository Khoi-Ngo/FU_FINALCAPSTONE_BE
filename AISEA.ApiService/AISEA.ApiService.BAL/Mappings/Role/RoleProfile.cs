using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Requests;
using AISEA.ApiService.SHARED.DTOs.Responses;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.Role
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<DAL.Entities.Role, GetRoleResponse>();
            CreateMap<UpdateRoleRequest, DAL.Entities.Role>();
        }
    }
}