using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Requests.Combo;
using AISEA.ApiService.SHARED.DTOs.Responses.Combo;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.Combo
{
    public class ComboProfile : Profile
    {
        public ComboProfile()
        {
            CreateMap<CreateComboRequest, DAL.Entities.Combo>();
            CreateMap<UpdateComboRequest, DAL.Entities.Combo>();
            
            CreateMap<DAL.Entities.Combo, GetComboResponse>()
                .ForMember(dest => dest.SubjectCount, opt => opt.MapFrom(src => src.ComboSubjects.Count));

            CreateMap<DAL.Entities.Combo, GetComboDetailResponse>()
                .ForMember(dest => dest.SubjectCount, opt => opt.MapFrom(src => src.ComboSubjects.Count))
                .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.ComboSubjects));

            CreateMap<ComboSubject, ComboSubjectResponse>()
                .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.Subject.Id))
                .ForMember(dest => dest.SubjectCode, opt => opt.MapFrom(src => src.Subject.SubjectCode))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName))
                .ForMember(dest => dest.Credits, opt => opt.MapFrom(src => src.Subject.Credits))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Subject.Description));
        }
    }
}