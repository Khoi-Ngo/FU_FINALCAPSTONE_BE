using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.DTOs.Responses.AdvisorySession1to1
{
    public class GetAdvisorySession1to1ItemsResponse
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public EAdvisorySession1to1Type Type { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long StaffId { get; set; }
        public long StudentId { get; set; }
    }
}