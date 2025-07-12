using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;

namespace AISEA.ApiService.DAL.Repositories;

public class LeaveScheduleRepository : GenericRepository<LeaveSchedule>
{
    public LeaveScheduleRepository(AiseaContext context) : base(context)
    {
    }
}