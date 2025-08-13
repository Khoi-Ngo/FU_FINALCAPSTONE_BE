using AISEA.ApiService.BAL.Services.Chat;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.ChatBot;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.InterceptorAPI;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.Chat;

[ApiController]
[Route("api/[controller]")]
[PermissionAuthorize((int)EUserRole.STUDENT)]
public class ChatBotController : BaseController
{
    private readonly AdvisorySession1to1Service _advisorySession1To1Service;

    public ChatBotController(EndpointSettings endpointSettings,
    AdvisorySession1to1Service advisorySession1To1Service) : base(endpointSettings)
    {
        _advisorySession1To1Service = advisorySession1To1Service;
    }

    /// <summary>
    /// This is for sending message to Chat Bot only with Existed Session
    /// </summary>
    [HttpPost("send")]
    [AuditLog(Tag = "SEND_CHATBOT_MESSAGE", Description = "")]
    public async Task<IActionResult> SendMsgAsync([FromBody] SendChatBotRequest request)
    {
        var accessToken = AccessToken;
        var res = await _advisorySession1To1Service.SendMsgAsync(request, accessToken);
        return Ok(res);
    }

    /// <summary>
    /// This is for initializing new session with AI ChatBot
    /// </summary>
    [HttpPost("init")]
    [AuditLog(Tag = "INIT_CHATBOT_CHAT_SESSION", Description = "")]
    public async Task<IActionResult> InitMsgAsync([FromBody] InitChatBotRequest request)
    {
        var accessToken = AccessToken;
        var res = await _advisorySession1To1Service.InitMsgAsync(request, accessToken);
        return Ok(res);
    }

}