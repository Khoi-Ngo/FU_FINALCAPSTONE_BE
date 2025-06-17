using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AISEA.ApiService.SHARED.DTOs.Requests.Role
{
    public class UpdateRoleRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}