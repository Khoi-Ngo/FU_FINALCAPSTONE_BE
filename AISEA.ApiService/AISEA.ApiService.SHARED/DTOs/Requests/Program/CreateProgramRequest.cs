using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.DTOs.Requests.Program
{
    public class CreateProgramRequest
    {
        public string ProgramCode { get; set; } = null!;
        public string ProgramName { get; set; } = null!;
    }
}
