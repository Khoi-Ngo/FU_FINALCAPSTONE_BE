using System;

namespace AISEA.ApiService.SHARED.PropConfigs
{
    public class RedisSettings
    {
        public const string Section = "RedisSettings";
        public required string ConnectionString { get; set; }
    }
}