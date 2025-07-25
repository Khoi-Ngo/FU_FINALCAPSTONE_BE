namespace AISEA.ApiService.SHARED.PropConfigs;

public class BookingSettings
{
    public const string Section = "BookingSettings";

    #region caching object LeaveSchedule + BookingAvailability
    public string BookingAvaiPrefix { get; set; }
    public int ExpiredBookingAvaiDaysCached { get; set; }
    public string LeaveSchePrefix { get; set; }
    public int ExpiredLeaveScheDaysCached { get; set; }
    #endregion

    #region holiday external api
    public string CountryCode_holiday { get; set; }
    public string abstractapi_HolidayApiApiKey { get; set; }
    public string Abstractapi_HolidayApiBaseUrl { get; set; }
    #endregion

    #region Meeting Values
    public int MinTimeAdvConfirmOrCancelMeetingDays { get; set; }
    public int MinTimeToGoStuCreateMeetingDays { get; set; }
    public int MaxNumberOfBan { get; set; }
    public int MaxStuCancelStatPerStuInMonth { get; set; }
    public int MaxLateTimeForAdvToMeetingMins { get; set; }

    #endregion

    #region Booking Durations

    public List<int> AllowedBookingDurationsMinutes { get; set; }

    #endregion
}