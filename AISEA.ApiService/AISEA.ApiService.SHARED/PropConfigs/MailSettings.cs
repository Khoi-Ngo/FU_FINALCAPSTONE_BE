using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.PropConfigs
{
    public class MailSettings
    {
        public const string Section = "MailSettings";

        public string DisplayName { get; set; } = null!;      
        public string From { get; set; } = null!;             
        public string SmtpHost { get; set; } = null!;         
        public int SmtpPort { get; set; }                     
        public string UserName { get; set; } = null!;         
        public string Password { get; set; } = null!;
        public string SecureSocketOption { get; set; } = null!;
    }
}