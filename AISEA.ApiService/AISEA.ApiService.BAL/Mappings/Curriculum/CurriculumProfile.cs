using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Requests.Curriculum;
using AISEA.ApiService.SHARED.DTOs.Responses.Curriculum;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.Curriculum
{
    public class CurriculumProfile : Profile
    {
        public CurriculumProfile()
        {
            CreateMap<CreateCurriculumRequest, DAL.Entities.Curriculum>();
            CreateMap<UpdateCurriculumRequest, DAL.Entities.Curriculum>();
            
            CreateMap<DAL.Entities.Curriculum, GetCurriculumResponse>()
                .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.Program.ProgramName))
                .ForMember(dest => dest.ProgramCode, opt => opt.MapFrom(src => src.Program.ProgramCode));

            CreateMap<DAL.Entities.Curriculum, GetCurriculumDetailResponse>()
                .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.Program.ProgramName))
                .ForMember(dest => dest.ProgramCode, opt => opt.MapFrom(src => src.Program.ProgramCode))
                .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.CurriculumSubjects));

            CreateMap<CurriculumSubject, CurriculumSubjectResponse>()
                .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.Subject.Id))
                .ForMember(dest => dest.SubjectCode, opt => opt.MapFrom(src => src.Subject.SubjectCode))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName))
                .ForMember(dest => dest.Credits, opt => opt.MapFrom(src => src.Subject.Credits))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Subject.Description));

            CreateMap<AddSubjectToCurriculumRequest, CurriculumSubject>();
        }
    }
}