using System;

namespace AISEA.ApiService.SHARED.PropConfigs
{
    public class VerifyResetPassCodeMailSettings
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class VerifyResetPassCodeSettings
    {
        public const string Section = "VerifyResetPassCode";
        public int ExpireMilli { get; set; }
        public string KeyPrefVerificationResetPassCode { get; set; }
        public VerifyResetPassCodeMailSettings Mail { get; set; }
    }
}