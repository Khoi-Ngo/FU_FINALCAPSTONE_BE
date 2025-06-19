using System;

namespace AISEA.ApiService.SHARED.PropConfigs
{
    public class AuthTokenSettings
    {
        public const string Section = "AuthToken";
        public int ExpireAccTokenMilli { get; set; }
        public int ExpireRefreshTokenDay { get; set; }
        public string KeyPrefRefreshToken { get; set; }
        public string KeyPrefExpireAccessToken { get; set; }
        public string FormatValueExpireToken { get; set; }
    }
}