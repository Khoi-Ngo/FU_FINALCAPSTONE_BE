using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.DTOs.Responses.Message
{
    public class MessageItemResponse
    {
        public long MessageId { get; set; }
        public long AdvisorySession1to1Id { get; set; }
        public long SenderId { get; set; }
        public string Content { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    }
}