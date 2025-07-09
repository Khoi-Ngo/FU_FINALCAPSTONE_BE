using AISEA.ApiService.DAL.Repositories;

namespace AISEA.ApiService.BAL.Services.Booking;

public class LeaveScheduleService
{
    private readonly LeaveScheduleRepository _leaveScheduleRepository;
    public LeaveScheduleService(LeaveScheduleRepository leaveScheduleRepository)
    {
        _leaveScheduleRepository = leaveScheduleRepository;
    }

    //create a leave schedule with constraints

    //delete a leave without any constraints

    //update a leave schedule with constraints

    //get all of a advisor with data staff also

    //get all pagination with data staff also

    //caching in the redis database


}