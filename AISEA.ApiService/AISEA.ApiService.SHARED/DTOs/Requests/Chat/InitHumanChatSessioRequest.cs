using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.DTOs.Requests.Chat
{
    public class InitHumanChatSessioRequest
    {
        [Required(ErrorMessage = "Message is required.")]
        public string Message { get; set; }
    }
}