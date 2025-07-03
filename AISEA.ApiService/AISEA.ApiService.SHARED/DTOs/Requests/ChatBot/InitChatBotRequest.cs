using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.DTOs.Requests.ChatBot
{
    public class InitChatBotRequest
    {
        [Required(ErrorMessage = "Message is required.")]
        public string Message { get; set; }
    }
}