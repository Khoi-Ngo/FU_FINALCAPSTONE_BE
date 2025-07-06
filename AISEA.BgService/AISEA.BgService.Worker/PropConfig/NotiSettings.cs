namespace AISEA.BgService.Worker.PropConfig;

public class NotiSettings
{
    public const string Section = "NotiSettings";
    public int ExpiredDays { get; set; }
    public int IntervalMillis { get; set; }
}   