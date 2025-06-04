using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AISEA.ApiService.SHARED.PropConfigs;
using Microsoft.IdentityModel.Tokens;

namespace AISEA.ApiService.SHARED.Util
{
    public static class JWTTokenUtil
    {
        public static object GetValueFromPrincipal(ClaimsPrincipal principal, string name)
        {
            return principal.Claims.FirstOrDefault(c => c.Type == name).Value;
        }
        public static ClaimsPrincipal GetPrincipalFromExpiredToken(string token, JwtSettings jwtSettings)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ValidateLifetime = false // Ignore expiration for refresh
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }
    }
}