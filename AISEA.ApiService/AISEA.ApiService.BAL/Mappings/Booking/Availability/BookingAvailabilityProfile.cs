using AISEA.ApiService.SHARED.DTOs.Requests.Booking;
using AISEA.ApiService.SHARED.DTOs.Responses.Booking;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.Booking.Availability;

public class BookingAvailabilityProfile : Profile
{
    public BookingAvailabilityProfile()
    {
        CreateMap<CreateBookingAvailabilityRequest, DAL.Entities.BookingAvailability>()
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => RoundToMinute(src.StartTime)))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => RoundToMinute(src.EndTime)));

        CreateMap<UpdateBookingAvailabilityRequest, DAL.Entities.BookingAvailability>()
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => RoundToMinute(src.StartTime)))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => RoundToMinute(src.EndTime)));

        CreateMap<DAL.Entities.BookingAvailability, BookingAvailabilitySimplyResponse>();
    }

    private TimeSpan RoundToMinute(TimeSpan time)
    {
        return new TimeSpan(time.Hours, time.Minutes, 0);
    }
}