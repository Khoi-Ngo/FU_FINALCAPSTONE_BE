using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Requests.SubjectVersion;
using AISEA.ApiService.SHARED.DTOs.Responses.SubjectVersion;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.SubjectVersion
{
    public class SubjectVersionProfile : Profile
    {
        public SubjectVersionProfile()
        {
            // Request to Entity mappings
            CreateMap<CreateSubjectVersionRequest, DAL.Entities.SubjectVersion>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Subject, opt => opt.Ignore())
                .ForMember(dest => dest.Syllabi, opt => opt.Ignore())
                .ForMember(dest => dest.SubjectClasses, opt => opt.Ignore())
                .ForMember(dest => dest.CurriculumSubjects, opt => opt.Ignore());

            CreateMap<UpdateSubjectVersionRequest, DAL.Entities.SubjectVersion>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SubjectId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Subject, opt => opt.Ignore())
                .ForMember(dest => dest.Syllabi, opt => opt.Ignore())
                .ForMember(dest => dest.SubjectClasses, opt => opt.Ignore())
                .ForMember(dest => dest.CurriculumSubjects, opt => opt.Ignore());

            // Entity to Response mappings
            CreateMap<DAL.Entities.SubjectVersion, GetSubjectVersionResponse>()
                .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject));
        }
    }
}
