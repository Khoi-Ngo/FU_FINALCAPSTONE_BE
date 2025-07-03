using System.Security.Claims;
using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.Interfaces;

public interface IJWTService
{
    string GenerateAccessToken(string username, long roleId, string firstName, string lastName, long profileId, long userId);
    string GenerateAccessToken(ClaimsPrincipal principal);
    Dictionary<string, string> GetAllClaimsFromToken(string token);
    string GetValueFromPrincipal(ClaimsPrincipal principal, string name);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}