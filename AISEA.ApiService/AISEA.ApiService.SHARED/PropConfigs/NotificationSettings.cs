namespace AISEA.ApiService.SHARED.PropConfigs
{
    public class NotificationSettings
    {
        public const string Section = "NotificationSettings";
        public string IndividualUserGroupPrefix { get; set; }
        public string NotificationCreatedMethod { get; set; }
        public string NotificationReadMethod { get; set; }
        public string NotificationReceivedMethod { get; set; }
        public int ExpiredDays { get; set; }
        public int IntervalMillis { get; set; }
    }
}