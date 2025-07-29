
namespace AISEA.ApiService.SHARED.PropConfigs;

public class EndpointSettings
{
    public const string Section = "AppEndpoint";
    // public required string CORSPolicy { get; set; } Cannot access in the use of defining the policy due to scope
    public required string AccessTokenPropName { get; set; }
    public required string RefreshTokenPropName { get; set; }
    public required string GoogleAuthTokenPropName { get; set; }
    public required string RefreshTokenEndpointName { get; set; }
    public required string AdvisoryHubEndpoint { get; set; }
    public required string NotificationHubEndpoint { get; set; }
    public required string ProdClientOrigin { get; set; }
    public required string DevClientOrigin { get; set; }
    public required string AdditionalClientOrigin { get; set; }

}