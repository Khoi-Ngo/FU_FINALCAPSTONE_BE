using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.PropConfigs
{
    public class GoogleAuthSettings
    {
        public const string Section = "GoogleAuth";

        public string client_id { get; set; }
        public string project_id { get; set; }
        public string auth_uri { get; set; }
        public string token_uri { get; set; }
        public string auth_provider_x509_cert_url { get; set; }
        public string client_secret { get; set; }
        public List<string> redirect_uris { get; set; }
        public string TokenInfoUrl { get; set; }
        public string AudResponsePropertyName { get; set; }
        public string EmailResponsePropertyName { get; set; }
    }
}