using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.PropConfigs
{
    public class ChatBotSettings
    {
        public const string Section = "ChatBotSettings";
        public string ApiKey { get; set; }
        public string ApiUrl { get; set; }
        public string Model { get; set; }
        public string DefaultErrorResponse { get; set; }
        public SystemUserConfig SystemBotUser { get; set; }
        public string StudentCachePrefix { get; set; }
        public int StudentCacheExpiryHrs { get; set; }
        public string SessionCachePrefix { get; set; }
        public int SessionCacheExpiryDays { get; set; }

    }
    public class SystemUserConfig
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int StaffId { get; set; }
    }
}