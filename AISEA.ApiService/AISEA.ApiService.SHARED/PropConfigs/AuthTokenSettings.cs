using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.PropConfigs
{
    public class AuthTokenSettings
    {
        public const string Section = "AuthToken";
        public int ExpireAccTokenMilli { get; set; }
        public int ExpireRefreshTokenDay { get; set; }
    }
}