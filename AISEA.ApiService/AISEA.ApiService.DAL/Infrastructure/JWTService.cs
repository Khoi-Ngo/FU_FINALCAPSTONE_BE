using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.DAL.Infrastructure;

public class JWTService : IJWTService
{

    #region INIT
    private readonly JwtSettings _jwtSettings;
    private readonly EndpointSettings _endpointSettings;

    public JWTService(
        IOptions<JwtSettings> jwtSettings,
        IOptions<EndpointSettings> endpointSettings)
    {
        _jwtSettings = jwtSettings.Value;
        _endpointSettings = endpointSettings.Value;
    }
    #endregion

    public string GenerateAccessToken(string username, long roleId)
    {
        // Claim attribute for the token
        List<Claim> claims = new List<Claim>
        {
            new Claim(_endpointSettings.UserNameClaimName, username),
            new Claim(_endpointSettings.RoleClaimName, roleId.ToString()),
            new Claim(_endpointSettings.LoginAtPropName, DateTimeOffset.UtcNow.ToString()),
            new Claim(_endpointSettings.RandKeySessionPropName,Guid.NewGuid().ToString() ),
            new Claim(_endpointSettings.AuthorPropName, new Random().Next(1,3) +  "")
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
    public string GenerateAccessToken(string username, string roleId)
    {
        // Claim attribute for the token
        List<Claim> claims = new List<Claim>
        {
            new Claim(_endpointSettings.UserNameClaimName, username),
            new Claim(_endpointSettings.RoleClaimName, roleId),
            new Claim(_endpointSettings.LoginAtPropName, DateTimeOffset.UtcNow.ToString()),
            new Claim(_endpointSettings.RandKeySessionPropName,Guid.NewGuid().ToString() ),
            new Claim(_endpointSettings.AuthorPropName, new Random().Next(1,3) +  "")
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

    public string GetUsernameFromToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            ValidateLifetime = true // Only valid (non-expired) tokens
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
        var usernameClaim = principal.Claims.FirstOrDefault(c => c.Type == _endpointSettings.UserNameClaimName);
        return usernameClaim.Value;

    }

    public long GetUserRoleIdFromToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            ValidateLifetime = true
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
        var roleClaim = principal.Claims.FirstOrDefault(c => c.Type == _endpointSettings.RoleClaimName);
        return Convert.ToInt64(roleClaim.Value);
    }
    public string GetValueFromPrincipal(ClaimsPrincipal principal, string name)
    {
        return principal.Claims.FirstOrDefault(c => c.Type == name)?.Value;
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            ValidateLifetime = false // Ignore expiration for refresh
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        return principal;
    }


}