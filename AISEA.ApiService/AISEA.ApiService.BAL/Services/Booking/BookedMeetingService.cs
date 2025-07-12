using AISEA.ApiService.DAL.Repositories;

namespace AISEA.ApiService.BAL.Services.Booking;

public class BookedMeetingService
{
    private readonly BookedMeetingRepository _bookedMeetingRepository;

    public BookedMeetingService(BookedMeetingRepository bookedMeetingRepository)
    {
        _bookedMeetingRepository = bookedMeetingRepository;
    }
}