using AISEA.ApiService.DAL.Repositories;

namespace AISEA.ApiService.BAL.Services.Booking;

public class BookingAvailabilityService
{
    //TODO : implement BookingAvailabilityService
    private readonly BookingAvailabilityRepository _bookingAvailabilityRepository;
    public BookingAvailabilityService(BookingAvailabilityRepository bookingAvailabilityRepository)
    {
        _bookingAvailabilityRepository = bookingAvailabilityRepository;
    }

    //create booking availability for a staff

    //bulk create booking availability for a staff

    //get all booking availability for a staff

    //get all booking availability pagination

    //edit booking availability

    //delete booking availability

    //caching with redis database
}