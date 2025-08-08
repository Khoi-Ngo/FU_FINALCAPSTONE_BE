using AISEA.ApiService.BAL.Services.Chat;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Chat;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.HubUtil;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.Chat;

[ApiController]
[Route("api/[controller]")]
[PermissionAuthorize((int)EUserRole.STUDENT)]
public class AdvisorySession1to1Controller : BaseController
{
    private readonly AdvisorySession1to1Service _advisorySession1To1Service;
    private readonly AdvisorySessionHubNotifier _advisorySessionHubNotifier;
    private readonly NotificationHubNotifier _notifier;
    private readonly ILogger<AdvisorySession1to1Controller> _logger;

    public AdvisorySession1to1Controller(EndpointSettings endpointSettings,
    AdvisorySession1to1Service advisorySession1To1Service,
    AdvisorySessionHubNotifier advisorySessionHubNotifier,
    NotificationHubNotifier notifier,
    ILogger<AdvisorySession1to1Controller> logger) : base(endpointSettings)
    {
        _advisorySession1To1Service = advisorySession1To1Service;
        _advisorySessionHubNotifier = advisorySessionHubNotifier;
        _notifier = notifier;
        _logger = logger;
    }

    /// <summary>
    /// Delete Chat Session
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        var sessionDeleted = await _advisorySession1To1Service.DeleteAsync(id, AccessToken);

        // Notify student, staff, and session groups
        await _advisorySessionHubNotifier.NotifySessionDeletedAsync(sessionDeleted.Id, sessionDeleted.StaffId, sessionDeleted.StudentId);
        try
        {
            await _notifier.NotifyUserAsync(AccessToken, "Delete", "The chat session got deleted successfully");

        }
        catch (Exception e)
        {
            // Log the exception or handle it accordingly
            _logger.LogError(e, "Error notifying user about deleted chat session");
        }
        return Ok("The chat session got deleted successfully");
    }

    /// <summary>
    /// Initialize the chat session with Staffs User
    /// </summary>
    [HttpPost("human")]
    public async Task<IActionResult> InitHumanChatSessionAsync([FromBody] InitHumanChatSessionRequest request)
    {
        var (hubRes, studentProfileId) = await _advisorySession1To1Service.InitHumanChatSessionAsync(request, AccessToken);
        //call hub context -> push
        await _advisorySessionHubNotifier.NotifySessionCreatedAsync(studentProfileId, hubRes);
        return Ok("The chat session got created successfully");
    }

    /// <summary>
    /// Get AI CHATBOTSessions paginated
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] PaginationRequest request)
    {
        var res = await _advisorySession1To1Service.GetBotSessionsAsync(request, AccessToken);
        return Ok(res);
    }

}
