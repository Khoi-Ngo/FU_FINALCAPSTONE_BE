using System;

namespace AISEA.ApiService.SHARED.PropConfigs
{
    public class VerifyResetPassCodeSettings
    {
        public const string Section = "VerifyResetPassCode";
        public int ExpireMilli { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}