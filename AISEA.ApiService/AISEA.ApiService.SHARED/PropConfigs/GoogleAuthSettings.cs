using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.PropConfigs
{
    public class GoogleAuthSettings
    {
        public const string Section = "GoogleAuth";

        public string UserInfoUrl { get; set; } = null!;
    }
}