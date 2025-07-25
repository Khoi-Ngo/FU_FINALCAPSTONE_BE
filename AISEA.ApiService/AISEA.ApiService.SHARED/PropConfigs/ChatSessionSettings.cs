namespace AISEA.ApiService.SHARED.PropConfigs;

public class ChatSessionSettings
{
    public const string Section = "ChatSessionSettings";
    public string SessionCachePrefix { get; set; }
    public int SessionCacheExpiryDays { get; set; }
    public string SendADVSSMethod { get; set; }
    public string JoinSSMethod { get; set; }
    public string GetSessionsHUBMethod { get; set; }
    public string SessionCreatedMethod { get; set; }
    public string GroupChatADVssPrefix { get; set; }
    public string MulDataSessionsPrefixStaff { get; set; }
    public string MulDataSessionsPrefixStudent { get; set; }
    public string LoadMoreMessagesMethod { get; set; }
    public string SessionDeletedMethod { get; set; }
    public string AddSessionAsAssigned { get; set; }
    public string RemoveSessionFromUnassigned { get; set; }
    public int SessionExpiryDays { get; set; }
    public int IntervalMillis { get; set; }
    public ChatBotHelperSetting ChatBotHelperSetting { get; set; }
}
public class ChatBotHelperSetting
{
    public string CachedKeyForFLMQueriedToChatBotData { get; set; }
    public string CachedKeyForSysFeedBackMeetingQueriedToChatBotData { get; set; }
    public string PrefixCachedKeyForStudentPersonalData { get; set; }
    public string PrefixCachedKeyForStudentPersonalRoadMap { get; set; }
    public string PrefixCachedKeyForStudentPersonalDetaiAcaPerform { get; set; }
}