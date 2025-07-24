using AISEA.ApiService.SHARED.DTOs.Requests.Booking;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.Booking.Meeting;

public class MeetingProfile : Profile
{

    public MeetingProfile()
    {
        //map Create Request -> Entity
        CreateMap<CreateMeetingRequest, DAL.Entities.BookedMeeting>();
        
    }
}