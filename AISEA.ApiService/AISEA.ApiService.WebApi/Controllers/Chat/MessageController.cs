using AISEA.ApiService.BAL.Services.Chat;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.Chat;

[ApiController]
[Route("api/[controller]")]
public class MessageController : BaseController
{
    private readonly MessageService _messageService;
    public MessageController(EndpointSettings endpointSettings, MessageService messageService) : base(endpointSettings)
    {
        _messageService = messageService;
    }

    /// <summary>
    /// Retrieves paginated  messages
    /// </summary>
    [HttpGet("{chatSessionId}")]
    public async Task<IActionResult> GetAllStaffsPaged([FromQuery] PaginationRequest request, long chatSessionId)
    {
        var result = await _messageService.GetMessagesAsync(chatSessionId);
        return Ok(result);
    }
}