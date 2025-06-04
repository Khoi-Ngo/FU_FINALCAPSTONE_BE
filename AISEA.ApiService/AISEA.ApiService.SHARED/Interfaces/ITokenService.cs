using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.Interfaces
{
    public interface ITokenService
    {
        Task<bool> IsValidAccessTokenAsync(string accessToken);
        Task<bool> IsValidRefreshTokenAsync(string userName, string refreshToken);
        string GenerateRefreshToken();
        Task<string> GetRefreshTokenAsync(string userName);
        Task StoreRefreshTokenAsync(string userName, string refreshToken);
        Task BlacklistAccessTokenAsync(string accessToken);
    }
}