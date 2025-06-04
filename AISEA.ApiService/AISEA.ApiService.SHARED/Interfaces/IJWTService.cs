using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.Interfaces
{
    public interface IJWTService
    {
        string GenerateAccessToken(string userName);
    }
}