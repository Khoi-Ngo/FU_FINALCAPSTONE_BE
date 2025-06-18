using System.Security.Claims;

namespace AISEA.ApiService.SHARED.Interfaces;

public interface IJWTService
{
    string GenerateAccessToken(string username);
    string GetUsernameFromToken(string token);
    string GetValueFromPrincipal(ClaimsPrincipal principal, string name);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}