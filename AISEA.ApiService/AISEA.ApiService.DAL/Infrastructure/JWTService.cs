using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.SHARED.Interfaces;

namespace AISEA.ApiService.DAL.Infrastructure;

public class JWTService : IJWTService
{

    #region INIT
    private readonly JwtSettings _jwtSettings;

    public JWTService(
        IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }
    #endregion

    public string GenerateAccessToken(string username, long roleId, string firstName, string lastName, long profileId, long userId, string email)
    {
        // Claim attribute for the token
        List<Claim> claims = new List<Claim>
        {
            new Claim(_jwtSettings.UserName, username),
            new Claim(_jwtSettings.LoginAt, DateTimeOffset.UtcNow.ToString()),
            new Claim(_jwtSettings.RandKeySessionPropName,Guid.NewGuid().ToString() ),
            new Claim(_jwtSettings.AuthProp, roleId + ""),
            new Claim(_jwtSettings.FirstName, firstName),
            new Claim(_jwtSettings.LastName, lastName),
            new Claim(_jwtSettings.ProfileId, profileId + ""),
            new Claim(_jwtSettings.UserId, userId + ""),
            new Claim(_jwtSettings.Email, email)


        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            SecurityAlgorithms.HmacSha256
        );


        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.Expires),
            signingCredentials: credentials
        );
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }


    /// <summary>
    /// Get all claims from a valid token efficiently.
    /// </summary>
    /// <param name="token">JWT string.</param>
    /// <returns>Dictionary of claim type and value.</returns>
    public Dictionary<string, string> GetAllClaimsFromToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentNullException(nameof(token));

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            ValidateLifetime = true // Ensure only valid (non-expired) tokens are processed
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);

        var claimsDict = principal.Claims
            .GroupBy(c => c.Type) // If there are duplicate claim types, pick first or join values as needed
            .ToDictionary(g => g.Key, g => g.First().Value);

        return claimsDict;
    }


    public string GetValueFromPrincipal(ClaimsPrincipal principal, string name)
    {
        return principal.Claims.FirstOrDefault(c => c.Type == name)?.Value;
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            ValidateLifetime = false // Ignore lifetime due to this is the refresh action
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
            throw new SecurityTokenException("Invalid token");
        return principal;
    }

    public string GenerateAccessToken(ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));

        // Extract required claims
        string username = GetValueFromPrincipal(principal, _jwtSettings.UserName)
                          ?? throw new Exception("Username claim is missing");

        string roleIdStr = GetValueFromPrincipal(principal, _jwtSettings.AuthProp)
                           ?? throw new Exception("RoleId claim is missing");

        string firstName = GetValueFromPrincipal(principal, _jwtSettings.FirstName)
                           ?? throw new Exception("FirstName claim is missing");

        string lastName = GetValueFromPrincipal(principal, _jwtSettings.LastName)
                          ?? throw new Exception("LastName claim is missing");

        string profileIdStr = GetValueFromPrincipal(principal, _jwtSettings.ProfileId)
                              ?? throw new Exception("ProfileId claim is missing");

        string userIdStr = GetValueFromPrincipal(principal, _jwtSettings.UserId)
                           ?? throw new Exception("UserId claim is missing");
        string email = GetValueFromPrincipal(principal, _jwtSettings.Email)
                        ?? throw new Exception("Email claim is missing");

        // Parse numeric claims safely
        if (!long.TryParse(roleIdStr, out var roleId))
            throw new Exception("Invalid RoleId claim value");

        if (!long.TryParse(profileIdStr, out var profileId))
            throw new Exception("Invalid ProfileId claim value");

        if (!long.TryParse(userIdStr, out var userId))
            throw new Exception("Invalid UserId claim value");

        // Generate token with existing method
        return GenerateAccessToken(username, roleId, firstName, lastName, profileId, userId, email);
    }

    /// <summary>
    /// Get username from active token.
    /// </summary>
    public string GetUsernameFromToken(string token)
    {
        var claims = GetAllClaimsFromToken(token);
        return claims.TryGetValue(_jwtSettings.UserName, out var username)
            ? username
            : throw new Exception("Username claim not found");
    }

    /// <summary>
    /// Get roleId from active token.
    /// </summary>
    public long GetRoleIdFromToken(string token)
    {
        var claims = GetAllClaimsFromToken(token);
        if (claims.TryGetValue(_jwtSettings.AuthProp, out var roleIdStr) && long.TryParse(roleIdStr, out var roleId))
        {
            return roleId;
        }
        throw new Exception("RoleId claim not found or invalid");
    }

    /// <summary>
    /// Get first name from active token.
    /// </summary>
    public string GetFirstNameFromToken(string token)
    {
        var claims = GetAllClaimsFromToken(token);
        return claims.TryGetValue(_jwtSettings.FirstName, out var firstName)
            ? firstName
            : throw new Exception("FirstName claim not found");
    }

    /// <summary>
    /// Get last name from active token.
    /// </summary>
    public string GetLastNameFromToken(string token)
    {
        var claims = GetAllClaimsFromToken(token);
        return claims.TryGetValue(_jwtSettings.LastName, out var lastName)
            ? lastName
            : throw new Exception("LastName claim not found");
    }

    /// <summary>
    /// Get profileId from active token.
    /// </summary>
    public long GetProfileIdFromToken(string token)
    {
        var claims = GetAllClaimsFromToken(token);
        if (claims.TryGetValue(_jwtSettings.ProfileId, out var profileIdStr) && long.TryParse(profileIdStr, out var profileId))
        {
            return profileId;
        }
        throw new Exception("ProfileId claim not found or invalid");
    }

    /// <summary>
    /// Get userId from active token.
    /// </summary>
    public long GetUserIdFromToken(string token)
    {
        var claims = GetAllClaimsFromToken(token);
        if (claims.TryGetValue(_jwtSettings.UserId, out var userIdStr) && long.TryParse(userIdStr, out var userId))
        {
            return userId;
        }
        throw new Exception("UserId claim not found or invalid");
    }

    /// <summary>
    /// Get email from active token.
    /// </summary>
    public string GetEmailFromToken(string token)
    {
        var claims = GetAllClaimsFromToken(token);
        return claims.TryGetValue(_jwtSettings.Email, out var email)
            ? email
            : throw new Exception("Email claim not found");
    }

    /// <summary>
    /// Get full name from active token.
    /// </summary>
    public string GetFullNameFromToken(string token)
    {
        var claims = GetAllClaimsFromToken(token);
        var firstName = claims.TryGetValue(_jwtSettings.FirstName, out var fName) ? fName : "";
        var lastName = claims.TryGetValue(_jwtSettings.LastName, out var lName) ? lName : "";
        return $"{firstName} {lastName}".Trim();
    }


}