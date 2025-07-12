using AISEA.ApiService.BAL.Services.Chat;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.Chat;

[ApiController]
[Route("api/[controller]")]
[PermissionAuthorize((int)EUserRole.STUDENT)]
public class MessageController : BaseController
{
    private readonly AdvisorySession1to1Service _advisorySession1To1Service;
    public MessageController(EndpointSettings endpointSettings,
    AdvisorySession1to1Service advisorySession1To1Service) : base(endpointSettings)
    {
        _advisorySession1To1Service = advisorySession1To1Service;
    }

    /// <summary>
    /// Retrieves paginated  messages only for the chatbot messages
    /// </summary>
    [HttpGet("{chatSessionId}")]
    public async Task<IActionResult> Get([FromQuery] PaginationRequest request, long chatSessionId)
    {
        var result = await _advisorySession1To1Service.GetChatBotMessagesAsync(request,chatSessionId);
        return Ok(result);
    }
}