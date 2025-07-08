using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;

namespace AISEA.ApiService.DAL.Repositories;

public class BookingAvailabilityRepository : GenericRepository<BookingAvailability>
{
    public BookingAvailabilityRepository(AiseaContext context) : base(context)
    {
    }
}