using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.BAL.Services.Chat;
using AISEA.ApiService.SHARED.DTOs.Requests.ChatBot;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.ChatBot;

[ApiController]
[Route("api/[controller]")]
public class ChatBotController : BaseController
{
    private readonly ChatBotService _chatBotService;
    public ChatBotController(EndpointSettings endpointSettings, ChatBotService chatBotService) : base(endpointSettings)
    {
        _chatBotService = chatBotService;
    }
    [HttpPost]
    public async Task<IActionResult> SendMsgAsync([FromBody] SendChatBotRequest request)
    {
        var res = await _chatBotService.SendMsgAsync(request, AccessToken);
        return Ok(res);
    }

}