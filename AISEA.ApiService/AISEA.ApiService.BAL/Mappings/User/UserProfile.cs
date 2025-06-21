using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Requests.User;
using AISEA.ApiService.SHARED.DTOs.Responses.Auth;
using AISEA.ApiService.SHARED.DTOs.Responses.User;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.User
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            //Auth
            CreateMap<DAL.Entities.User, AuthResponse>();

            //Create User
            CreateMap<CreateUserRequest, DAL.Entities.User>()
                .ForMember(dest => dest.StudentProfile, opt => opt.MapFrom(src => src.StudentProfileData))
                .ForMember(dest => dest.StaffProfile, opt => opt.MapFrom(src => src.StaffProfileData));

            //User list simply
            CreateMap<DAL.Entities.User, GetUserListResponse>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));

            #region User with Profile View Detail
            CreateMap<DAL.Entities.User, GetStudentDetailResponse>()
                            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
                            .ForMember(dest => dest.StudentDataDetailResponse, opt => opt.MapFrom(src => src.StudentProfile));

            CreateMap<StudentProfile, StudentDataDetailResponse>();

            // Mapping for GetStaffDetailResponse
            CreateMap<DAL.Entities.User, GetStaffDetailResponse>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(dest => dest.StaffDataDetailResponse, opt => opt.MapFrom(src => src.StaffProfile));

            CreateMap<StaffProfile, StaffDataDetailResponse>();
            #endregion


            #region ProfileController
            // Map StudentProfileData to StudentProfile
            CreateMap<StudentProfileData, StudentProfile>();

            // Map StaffProfileData to StaffProfile
            CreateMap<StaffProfileData, StaffProfile>();
            #endregion

            #region User with Profile View List
            // List Student Response
            CreateMap<DAL.Entities.User, GetStudentListResponse>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(dest => dest.StudentDataListResponse, opt => opt.MapFrom(src => src.StudentProfile));

            // List Staff Response
            CreateMap<DAL.Entities.User, GetStaffListResponse>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(dest => dest.StaffDataDetailResponse, opt => opt.MapFrom(src => src.StaffProfile));

            CreateMap<StaffProfile, StaffDataListResponse>();
            CreateMap<StudentProfile, StudentDataListResponse>();
            #endregion

            #region UPDATE: user with profile
            
            // Update Student with Profile
            CreateMap<UpdateStudentRequest, DAL.Entities.User>();
            CreateMap<StudentDataUpdateRequest, StudentProfile>();

            // Update Staff with Profile
            CreateMap<UpdateStaffRequest, DAL.Entities.User>();
            CreateMap<StaffDataUpdateRequest, StaffProfile>();

            #endregion


        }
    }
}