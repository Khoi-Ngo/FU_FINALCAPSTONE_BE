using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.PropConfigs
{
    public class SqlSettings
    {
        public const string Section = "SqlSettings";
        public required string ConnectionString { get; set; }
    }
}