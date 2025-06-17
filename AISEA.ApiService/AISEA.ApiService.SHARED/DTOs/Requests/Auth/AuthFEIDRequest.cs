using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.DTOs.Requests.Auth;

public class AuthFEIDRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}