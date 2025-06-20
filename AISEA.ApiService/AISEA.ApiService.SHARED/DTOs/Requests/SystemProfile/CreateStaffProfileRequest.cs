using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.DTOs.Requests.SystemProfile
{
    public class CreateStaffProfileRequest
    {
        public string Campus { get; set; }

        public string Department { get; set; }

        public string Position { get; set; }

        public DateTimeOffset? StartWorkAt { get; set; }

        public DateTimeOffset? EndWorkAt { get; set; }

        public long UserId { get; set; }
    }
}