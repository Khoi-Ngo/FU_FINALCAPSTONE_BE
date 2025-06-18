using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.DTOs.Requests.Auth
{
    public class ResetPasswordFEIDRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}