using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.DTOs.Responses.ChatBot
{
    public class InitChatBotResponse
    {
        public long ChatSessionId { get; set; }
        public string Message { get; set; }
    }
}