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
        public SystemUserConfig SystemUser { get; set; }

        public class SystemUserConfig
        {
            public int Id { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int StaffId { get; set; }
        }
    }
}