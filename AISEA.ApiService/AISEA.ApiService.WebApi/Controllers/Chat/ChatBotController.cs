using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.BAL.Services.Chat;
using AISEA.ApiService.SHARED.DTOs.Requests.ChatBot;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.Chat;

[ApiController]
[Route("api/[controller]")]
public class ChatBotController : BaseController
{
    private readonly ChatBotService _chatBotService;
    public ChatBotController(EndpointSettings endpointSettings, ChatBotService chatBotService) : base(endpointSettings)
    {
        _chatBotService = chatBotService;
    }
    
    /// <summary>
    /// This is for sending message to Chat Bot only with Existed Session
    /// </summary>
    [HttpPost("send")]
    public async Task<IActionResult> SendMsgAsync([FromBody] SendChatBotRequest request)
    {
        var res = await _chatBotService.SendMsgAsync(request, AccessToken);
        return Ok(res);
    }

    /// <summary>
    /// This is for initializing new session with AI ChatBot
    /// </summary>
    [HttpPost("init")]
    public async Task<IActionResult> InitMsgAsync([FromBody] InitChatBotRequest request)
    {
        var res = await _chatBotService.InitMsgAsync(request, AccessToken);
        return Ok(res);
    }

    /// <summary>
    /// Get AI CHATBOTSession By Id
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAIChatBotSessionByIdAsync(long id)
    {
        var res = await _chatBotService.GetAIChatBotSessionByIdAsync(id, AccessToken);
        return Ok(res);
    }

}