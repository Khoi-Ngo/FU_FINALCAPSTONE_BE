using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Requests.Role;
using AISEA.ApiService.SHARED.DTOs.Responses.Role;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.Role
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<DAL.Entities.Role, GetRoleResponse>();
            CreateMap<UpdateRoleRequest, DAL.Entities.Role>();
            CreateMap<CreateRoleRequest, DAL.Entities.Role>();
        }
    }
}