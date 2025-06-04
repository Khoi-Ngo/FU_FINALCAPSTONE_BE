namespace AISEA.ApiService.BAL.Interfaces
{
    public interface IDemoSampleService
    {

        Task<object?> DemoLoginWithRedis(string userName, string password);
        Task<object?> DemoRefreshTokenWithRedis(string expiredToken, string refreshToken);
        Task DemoLogoutWithRedis(string accessToken);

    }
}