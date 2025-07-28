using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Requests.Syllabus;
using AISEA.ApiService.SHARED.DTOs.Responses.Syllabus;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.Syllabus
{
    public class SyllabusProfile : Profile
    {
        public SyllabusProfile()
        {
            CreateMap<CreateSyllabusRequest, DAL.Entities.Syllabus>();
            CreateMap<UpdateSyllabusRequest, DAL.Entities.Syllabus>();
            
            CreateMap<DAL.Entities.Syllabus, GetSyllabusResponse>()
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.SubjectVersion.Subject.SubjectName))
                .ForMember(dest => dest.SubjectCode, opt => opt.MapFrom(src => src.SubjectVersion.Subject.SubjectCode));

            CreateMap<DAL.Entities.Syllabus, GetSyllabusDetailResponse>()
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.SubjectVersion.Subject.SubjectName))
                .ForMember(dest => dest.SubjectCode, opt => opt.MapFrom(src => src.SubjectVersion.Subject.SubjectCode))
                .ForMember(dest => dest.Assessments, opt => opt.MapFrom(src => src.SyllabusAssessments))
                .ForMember(dest => dest.LearningMaterials, opt => opt.MapFrom(src => src.SyllabusLearningMaterials))
                .ForMember(dest => dest.LearningOutcomes, opt => opt.MapFrom(src => src.SyllabusLearningOutcomes))
                .ForMember(dest => dest.Sessions, opt => opt.MapFrom(src => src.SyllabusSessions));

            CreateMap<SyllabusAssessment, SyllabusAssessmentResponse>();
            CreateMap<SyllabusLearningMaterial, SyllabusLearningMaterialResponse>();
            CreateMap<SyllabusLearningOutcome, SyllabusLearningOutcomeResponse>();
            
            CreateMap<SyllabusSession, SyllabusSessionResponse>()
                .ForMember(dest => dest.LearningOutcomeCodes, opt => opt.MapFrom(src => 
                    src.SessionOutcomeMappings.Select(som => som.Outcome.OutcomeCode).ToList()));

            CreateMap<CreateSyllabusAssessmentRequest, SyllabusAssessment>();
            CreateMap<CreateSyllabusLearningMaterialRequest, SyllabusLearningMaterial>();
            CreateMap<CreateSyllabusLearningOutcomeRequest, SyllabusLearningOutcome>();
            CreateMap<CreateSyllabusSessionRequest, SyllabusSession>();
        }
    }
}