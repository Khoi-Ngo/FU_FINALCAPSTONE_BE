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
        CreateMap<SingleImportJoinedSubjectRequest, JoinedSubject>()
            .ForMember(dest => dest.StudentProfileId, opt => opt.MapFrom((src, dest, destMember, ctx) => (long)ctx.Items["StudentProfileId"]))
            .ForMember(dest => dest.CreatedByUserName, opt => opt.MapFrom((src, dest, destMember, ctx) => (string)ctx.Items["CreatedByUserName"]));




        //RESPONSE
        CreateMap<JoinedSubject, JoinedSubjectListItemResponse>();
    }
}
