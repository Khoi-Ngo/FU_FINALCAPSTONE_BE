using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.PropConfigs
{
    public class ChatSessionSettings
    {
        public const string Section = "ChatSessionSettings";
        public string SenderCachePrefix { get; set; }
        public int SenderCacheExpiryHrs { get; set; }
        public string SessionCachePrefix { get; set; }
        public int SessionCacheExpiryDays { get; set; }
    }
}