using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Requests.CheckPoint;
using AISEA.ApiService.SHARED.DTOs.Responses.CheckPoint;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.CourseTrack;

public class CheckPointProfile : Profile
{
    public CheckPointProfile()
    {
        // COMMAND → ENTITY
        CreateMap<CommandCheckpointRequest, JoinedSubjectCheckPoint>()
            .ForMember(dest => dest.JoinedSubjectId,
                opt => opt.MapFrom((src, dest, _, context) => 
                    (long)context.Items["SubjectId"]));

        // ENTITY → LIST RESPONSE
        CreateMap<JoinedSubjectCheckPoint, CheckpointListItemResponse>();

        // ENTITY → DETAIL RESPONSE
        CreateMap<JoinedSubjectCheckPoint, CheckpointDetailResponse>();
    }
}
