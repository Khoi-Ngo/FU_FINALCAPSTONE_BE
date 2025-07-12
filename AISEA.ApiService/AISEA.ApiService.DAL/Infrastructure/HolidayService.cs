using AISEA.ApiService.SHARED.Interfaces;

namespace AISEA.ApiService.DAL.Infrastructure;

public class HolidayService : IHolidayService
{
    public Task<bool> IsHoliday(DateTime date)
    {
        //TODO: Call the external API to check
        throw new NotImplementedException();
    }
}