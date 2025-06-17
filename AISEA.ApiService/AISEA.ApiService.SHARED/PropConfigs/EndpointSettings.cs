using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.PropConfigs
{
    public class EndpointSettings
    {
        public const string Section = "AppEndpoint";
        public required string AuthorPropName { get; set; }
        // public required string CORSPolicy { get; set; } Cannot access in the use of defining the policy due to scope
        public required string UserNameClaimName { get; set; }
        public required string AccessTokenPropName { get; set; }
        public required string RefreshTokenPropName { get; set; }
        public required string LoginAtPropName { get; set; }
        public required string RandKeySessionPropName { get; set; }
        public required string GoogleAuthTokenPropName { get; set; }
        public required string RefreshTokenEndpointName { get; set; }

    }
}