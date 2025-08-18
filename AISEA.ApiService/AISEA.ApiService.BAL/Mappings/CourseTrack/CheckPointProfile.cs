using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Requests.CheckPoint;
using AISEA.ApiService.SHARED.DTOs.Responses.CheckPoint;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.CourseTrack;

public class CheckPointProfile : Profile
{
    public CheckPointProfile()
    {

        //COMMAND
        CreateMap<CommandCheckpointRequest, JoinedSubjectCheckPoint>();

        CreateMap<CommandCheckpointRequest, OptionalSubjectCheckPoint>();


        //LIST ITEM RESPONSE
        CreateMap<JoinedSubjectCheckPoint, CheckpointListItemResponse>();

        CreateMap<OptionalSubjectCheckPoint, CheckpointListItemResponse>();



        //DETAIL RESPONSE
        CreateMap<JoinedSubjectCheckPoint, CheckpointDetailResponse>();

        CreateMap<OptionalSubjectCheckPoint, CheckpointDetailResponse>();


    }
}
