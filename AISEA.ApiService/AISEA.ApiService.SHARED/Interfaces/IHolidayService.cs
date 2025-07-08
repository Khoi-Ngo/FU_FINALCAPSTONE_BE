namespace AISEA.ApiService.SHARED.Interfaces;

public interface IHolidayService
{
    public Task<bool> IsHoliday(DateTime date);
}