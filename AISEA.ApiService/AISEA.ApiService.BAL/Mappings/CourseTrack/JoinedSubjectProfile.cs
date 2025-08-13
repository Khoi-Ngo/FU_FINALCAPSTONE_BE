using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Responses.JoinedSubject;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.CourseTrack;

public class JoinedSubjectProfile : Profile
{
    public JoinedSubjectProfile()
    {

        //RESPONSE
        CreateMap<JoinedSubject, JoinedSubjectResponse>()
        .ForMember(dest => dest.SemesterName
        , opt => opt.MapFrom(src => src.Semester.SemesterName));
    }
}
