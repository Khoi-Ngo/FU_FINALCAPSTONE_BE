
namespace AISEA.ApiService.SHARED.PropConfigs;

public class StaffUserSettings
{
    public const string Section = "StaffUserSettings";
    public required int EmptyStaffProfileId { get; set; }
    public required string EmptyStaffName { get; set; }
    public BotUserconfig SystemBotUser { get; set; }

}
public class BotUserconfig
{
    public int Id { get; set; }
    public int StaffId { get; set; }
}