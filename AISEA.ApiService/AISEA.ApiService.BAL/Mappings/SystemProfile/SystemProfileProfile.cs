using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Requests.SystemProfile;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.SystemProfile
{
    public class SystemProfileProfile : Profile
    {
        public SystemProfileProfile()
        {
            CreateMap<CreateStaffProfileRequest, StaffProfile>();
            CreateMap<CreateStudentProfileRequest, StudentProfile>();
        }
    }
}