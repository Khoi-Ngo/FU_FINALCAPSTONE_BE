using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.DTOs.Responses.Message
{
    public class MessageItemResponse
    {
        public long MessageId { get; set; }
        public string SenderUsername { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}