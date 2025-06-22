using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.BgService.Worker.PropConfig
{
    public class ChatSettings
    {
        public const string Section = "ChatSettings";
        public int SessionExpiryDays { get; set; }
        public int IntervalMillis { get; set; }
    }
}