using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.PropConfigs
{
    public class MailSettings
    {
        public const string Section = "MailSettings";

        public string DisplayName { get; set; } = null!;      // e.g., "AISEA"
        public string From { get; set; } = null!;             // e.g., "aiseafu@jkh8ing8.online"
        public string SmtpHost { get; set; } = null!;         // e.g., "smtp.zoho.com"
        public int SmtpPort { get; set; }                     // e.g., 587
        public string UserName { get; set; } = null!;         // e.g., "aiseafu@jkh8ing8.online"
        public string Password { get; set; } = null!;         // e.g., "app-password-from-zoho"
        public string SecureSocketOption { get; set; } = null!; // e.g., "StartTls"
    }
}