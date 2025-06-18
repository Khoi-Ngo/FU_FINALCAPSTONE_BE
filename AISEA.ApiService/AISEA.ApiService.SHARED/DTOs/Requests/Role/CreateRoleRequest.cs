using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.DTOs.Requests.Role
{
    public class CreateRoleRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}