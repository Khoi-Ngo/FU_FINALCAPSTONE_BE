using System.Security.Claims;
using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.Interfaces;

public interface IJWTService
{
    string GenerateAccessToken(string username, long roleId);
    string GenerateAccessToken(string username, string roleId);
    string GetUsernameFromToken(string token);
    long GetUserRoleIdFromToken(string token);
    string GetValueFromPrincipal(ClaimsPrincipal principal, string name);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}