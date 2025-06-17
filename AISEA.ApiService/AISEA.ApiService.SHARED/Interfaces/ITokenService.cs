using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.Interfaces
{
    public interface ITokenService
    {
        Task<bool> IsValidAccessTokenAsync(string accessToken);
        Task<bool> IsValidRefreshTokenAsync(string username, string refreshToken);
        string GenerateRefreshToken();
        Task<string> GetRefreshTokenAsync(string username);
        Task StoreRefreshTokenAsync(string username, string refreshToken);
        Task BlacklistAccessTokenAsync(string accessToken);
    }
}