namespace AISEA.ApiService.SHARED.PropConfigs;

public class BookingSettings
{
    public const string Section = "BookingSettings";
    public string BookingAvaiPrefix { get; set; }
    public int ExpiredBookingAvaiDaysCached { get; set; }
    public string LeaveSchePrefix { get; set; }
    public int ExpiredLeaveScheDaysCached { get; set; }
    public string CountryCode_holiday { get; set; }
    public string abstractapi_HolidayApiApiKey { get; set; }
    public string Abstractapi_HolidayApiBaseUrl { get; set; }
}