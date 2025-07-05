using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.DTOs.Responses.Noti
{
    public class NotificationItemResponse
    {
        public long Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public string? Link { get; set; }
        public bool IsRead { get; set; }
        public DateTime? CreatedAt { get; set; } 
    }
}