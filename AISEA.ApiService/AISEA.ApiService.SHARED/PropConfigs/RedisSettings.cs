using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.PropConfigs
{
    public class RedisSettings
    {
        public const string Section = "RedisSettings";
        public required string ConnectionString { get; set; }
        public required string KeyPrefRefreshToken { get; set; }
        public required string KeyPrefExpireAccessToken { get; set; }
        public required string FormatValueExpireToken { get; set; }
        public required string KeyPrefVerificationResetPassCode { get; set; }
    }
}