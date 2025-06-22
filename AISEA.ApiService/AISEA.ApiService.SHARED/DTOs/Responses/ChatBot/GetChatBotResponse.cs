using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.DTOs.Responses.ChatBot
{
    public class GetChatBotResponse
    {
        public string Message { get; set; }
        public long? SessionId { get; set; }
    }
}