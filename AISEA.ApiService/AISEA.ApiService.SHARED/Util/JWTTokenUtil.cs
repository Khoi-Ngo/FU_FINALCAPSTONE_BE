using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.Util
{
    public static class JWTTokenUtil
    {
         public static object GetValueFromPrincipal(ClaimsPrincipal principal, string name)
        {
            return principal.Claims.FirstOrDefault(c => c.Type == name).Value;
        }
    }
}