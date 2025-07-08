using AISEA.ApiService.DAL.Repositories;

namespace AISEA.ApiService.BAL.Services.Booking;

public class LeaveScheduleService
{
    private readonly LeaveScheduleRepository _leaveScheduleRepository;
    public LeaveScheduleService(LeaveScheduleRepository leaveScheduleRepository)
    {
        _leaveScheduleRepository = leaveScheduleRepository;
    }
}