using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Requests.JoinedSubject;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.CourseTrack;

public class JoinedSubjectProfile : Profile
{
    public JoinedSubjectProfile()
    {
        CreateMap<SingleImportJoinedSubjectRequest, JoinedSubject>();
    }
}
