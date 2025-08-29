using System.Security.Claims;

namespace AISEA.ApiService.SHARED.Interfaces;

public interface IJWTService
{
    string GenerateAccessToken(string username, long roleId, string firstName, string lastName, long profileId, long userId, string email);
    string GenerateAccessToken(ClaimsPrincipal principal);
    Dictionary<string, string> GetAllClaimsFromToken(string token);
    string GetValueFromPrincipal(ClaimsPrincipal principal, string name);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    string GetUsernameFromToken(string token);
    long GetRoleIdFromToken(string token);
    string GetFirstNameFromToken(string token);
    string GetLastNameFromToken(string token);
    long GetProfileIdFromToken(string token);
    long GetUserIdFromToken(string token);
    string GetEmailFromToken(string token);
    string GetFullNameFromToken(string token);
}