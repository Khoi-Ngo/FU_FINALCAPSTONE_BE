using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace AISEA.ApiService.SHARED.DTOs.Requests.ChatBot
{
    public class SendChatBotRequest
    {
        [Required(ErrorMessage = "Message is required.")]
        public string Message { get; set; }
        public long ChatSessionId { get; set; }
    }
}