using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;

namespace AISEA.ApiService.DAL.Repositories;

public class BookedMeetingRepository : GenericRepository<BookedMeeting>
{
    public BookedMeetingRepository(AiseaContext context) : base(context)
    {
    }
    
}