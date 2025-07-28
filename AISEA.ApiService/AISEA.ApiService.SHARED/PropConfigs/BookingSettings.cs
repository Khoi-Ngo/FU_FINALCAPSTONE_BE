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
    public int MinTimeStudentCancelTheConfirmMeetingDays { get; set; }
    public int NumberOfBanWhenStuCancelTheConfirm { get; set; }
    public int NumberOfBanWhenStuMissingTheMeeting { get; set; }

    public int MaxNumberOfBan { get; set; }
    public int MaxStuCancelStatPerStuInMonth { get; set; }
    public int MaxLateTimeForAdvToMeetingMins { get; set; }

    #endregion

    #region Booking Durations

    public List<int> AllowedBookingDurationsMinutes { get; set; }

    #endregion

    #region Background service props
    public long EstimateNumberOfBanIncreasedToAntiSpam { get; set; }
    public long CheckStudentCancelPendingMeetingSpamIntervalMillis { get; set; }
    public long GeneralPurposeIntervalMillis { get; set; }
    public int ResetNumberOfBanIntervalDays { get; set; }
    public int ResetCheckIntervalHours { get; set; }
    public int ErrorRetryDelayMinutes { get; set; }


    #endregion
}