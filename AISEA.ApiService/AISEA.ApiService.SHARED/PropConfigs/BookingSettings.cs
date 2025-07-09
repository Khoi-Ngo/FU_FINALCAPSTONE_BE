namespace AISEA.ApiService.SHARED.PropConfigs;

public class BookingSettings
{
    public const string Section = "BookingSettings";
    public string BookingAvaiPrefix { get; set; }
    public int ExpiredBookingAvaiDays { get; set; }
}