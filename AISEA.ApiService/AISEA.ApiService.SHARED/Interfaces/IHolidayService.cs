using AISEA.ApiService.SHARED.DTOs.Responses.Booking;

namespace AISEA.ApiService.SHARED.Interfaces;

public interface IHolidayService
{
    Task<List<HolidayResponse>> CheckHolidayAsync(DateOnly date);
}