using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.PropConfigs
{
    public class RedisSettings
    {
        public const string Section = "Redis";
        public required string ConnectionString { get; set; }

    }
}