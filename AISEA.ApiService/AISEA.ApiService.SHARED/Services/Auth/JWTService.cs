using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using AISEA.ApiService.SHARED.Util;
using AISEA.ApiService.SHARED.PropConfigs;

namespace AISEA.ApiService.SHARED.Services.Auth
{
    public class JWTService
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

        public string GenerateAccessToken(string userName)
        {
            // Claim attribute for the token
            List<Claim> claims = new List<Claim>
            {
                new Claim(_endpointSettings.UserNameClaimName, userName),
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
                //TODO: Enable audience later
                // audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.Expires),
                signingCredentials: credentials
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
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
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }
    }
}