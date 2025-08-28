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



        //LIST ITEM RESPONSE
        CreateMap<JoinedSubjectCheckPoint, CheckpointListItemResponse>();




        //DETAIL RESPONSE
        CreateMap<JoinedSubjectCheckPoint, CheckpointDetailResponse>();
    }
}
