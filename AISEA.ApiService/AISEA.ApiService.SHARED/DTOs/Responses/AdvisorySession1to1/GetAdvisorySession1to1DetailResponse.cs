using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.DTOs.Responses.AdvisorySession1to1
{
    public class GetAdvisorySession1to1DetailResponse
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public EAdvisorySession1to1Type Type { get; set; }
        public DateTime? CreatedAt { get; set; }
        public ICollection<MessageDataListResponse> MessagesDataList { get; set; }

    }
    public class MessageDataListResponse
    {
        public string SenderUserName { get; set; }
        public string Content { get; set; }
        public DateTime? CreatedAt { get; set; }

    }
}