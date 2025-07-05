using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.PropConfigs
{
    public class NotificationSettings
    {
        public const string Section = "NotificationSettings";
        public string IndividualUserGroupPrefix { get; set; }
        public string NotificationCreatedMethod { get; set; }
        public string NotificationReadMethod { get; set; }
        public string NotificationReceivedMethod { get; set; }
    }
}