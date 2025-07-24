using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.PropConfigs
{
    public class JwtSettings
    {
        public const string Section = "JWT";
        public string SecretKey { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int Expires { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string ProfileId { get; set; }
        public required string RandKeySessionPropName { get; set; }
        public required string LoginAt { get; set; }
        public required string UserName { get; set; }
        public required string AuthProp { get; set; }
        public required string UserId { get; set; }
        public required string Email { get; set; }
    }
}