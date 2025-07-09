using AISEA.ApiService.SHARED.DTOs.Requests.Booking;
using AISEA.ApiService.SHARED.DTOs.Responses.Booking;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.Booking.Availability;

public class BookingAvailabilityProfile : Profile
{
    public BookingAvailabilityProfile()
    {
        CreateMap<CreateBookingAvailabilityRequest, DAL.Entities.BookingAvailability>();
        CreateMap<UpdateBookingAvailabilityRequest, DAL.Entities.BookingAvailability>();
        CreateMap<DAL.Entities.BookingAvailability, BookingAvailabilityListItemResponse>();
    }
}