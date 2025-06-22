using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.SHARED.DTOs.Requests.ChatBot;
using AISEA.ApiService.SHARED.DTOs.Responses.ChatBot;

namespace AISEA.ApiService.SHARED.Interfaces
{
    public interface IChatOpenAIService
    {
        Task<string> SendMsgAsync(string prompt);
    }
}