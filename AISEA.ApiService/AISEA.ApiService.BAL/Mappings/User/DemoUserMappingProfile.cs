using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Requests;
using AISEA.ApiService.SHARED.DTOs.Responses;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.User
{
    public class DemoUserMappingProfile : Profile
    {
        public DemoUserMappingProfile()
        {
            CreateMap<DemoUserEntity, GetUserResponse>();
            CreateMap<CreateUserRequest, DemoUserEntity>();
        }
    }
}