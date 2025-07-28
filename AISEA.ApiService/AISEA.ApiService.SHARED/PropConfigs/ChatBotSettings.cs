namespace AISEA.ApiService.SHARED.PropConfigs;

public class ChatBotSettings
{
    public const string Section = "ChatBotSettings";
    public string ApiKey { get; set; }
    public string ApiUrl { get; set; }
    public string Model { get; set; }
}