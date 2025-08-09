using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Requests.JoinedSubject;
using AISEA.ApiService.SHARED.DTOs.Responses.JoinedSubject;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.CourseTrack;

public class JoinedSubjectProfile : Profile
{
    public JoinedSubjectProfile()
    {
        // Single subject mapping
        CreateMap<SingleImportJoinedSubjectRequest, JoinedSubject>();




        //RESPONSE
        CreateMap<JoinedSubject, JoinedSubjectListItemResponse>();
    }
}
