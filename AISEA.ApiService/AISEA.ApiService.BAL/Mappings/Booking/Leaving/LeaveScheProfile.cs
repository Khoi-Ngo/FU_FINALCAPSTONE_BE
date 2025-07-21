using AISEA.ApiService.SHARED.DTOs.Requests.Booking;
using AISEA.ApiService.SHARED.DTOs.Responses.Booking;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.Booking.Leaving;

public class LeaveScheProfile : Profile
{
  public LeaveScheProfile()
  {

    CreateMap<DAL.Entities.LeaveSchedule, LeaveScheListSimplyResponse>();

    CreateMap<CreateLeaveScheRequest, DAL.Entities.LeaveSchedule>()
      .ForMember(dest => dest.StartDateTime, opt => opt.MapFrom(src => RoundToMinute(src.StartDateTime)))
        .ForMember(dest => dest.EndDateTime, opt => opt.MapFrom(src => RoundToMinute(src.EndDateTime)));

    CreateMap<UpdateLeaveScheRequest, DAL.Entities.LeaveSchedule>()
      .ForMember(dest => dest.StartDateTime, opt => opt.MapFrom(src => RoundToMinute(src.StartDateTime)))
        .ForMember(dest => dest.EndDateTime, opt => opt.MapFrom(src => RoundToMinute(src.EndDateTime)));

    CreateMap<DAL.Entities.LeaveSchedule, LeaveScheListSimplyResponse>();
  }

  private DateTime RoundToMinute(DateTime dateTime)
  {
    return new DateTime(
        dateTime.Year,
        dateTime.Month,
        dateTime.Day,
        dateTime.Hour,
        dateTime.Minute,
        0,
        DateTimeKind.Utc
    );
  }



}