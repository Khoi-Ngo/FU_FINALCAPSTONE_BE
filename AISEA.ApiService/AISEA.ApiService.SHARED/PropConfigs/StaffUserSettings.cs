using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.PropConfigs
{
    public class StaffUserSettings
    {
        public const string Section = "StaffUserSettings";
        public required int EmptyStaffProfileId { get; set; }
        public required string EmptyStaffName { get; set; }
    }
}