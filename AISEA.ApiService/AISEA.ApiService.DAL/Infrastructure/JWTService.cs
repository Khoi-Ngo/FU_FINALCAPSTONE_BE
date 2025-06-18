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
    private readonly EndpointSettings _endpointSettings;

    public JWTService(
        IOptions<JwtSettings> jwtSettings,
        IOptions<EndpointSettings> endpointSettings)
    {
        _jwtSettings = jwtSettings.Value;
        _endpointSettings = endpointSettings.Value;
    }
    #endregion

    public string GenerateAccessToken(string username)
    {
        // Claim attribute for the token
        List<Claim> claims = new List<Claim>
        {
            new Claim(_endpointSettings.UserNameClaimName, username),
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

    
}
