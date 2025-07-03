using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.DTOs.Responses.Role
{
    public class GetRoleResponse
    {

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}