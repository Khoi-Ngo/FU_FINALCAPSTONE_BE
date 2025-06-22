using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.BgService.Worker.PropConfig
{
    public class SqlSettings
    {
        public const string Section = "SqlSettings";
        public required string ConnectionString { get; set; }
    }
}