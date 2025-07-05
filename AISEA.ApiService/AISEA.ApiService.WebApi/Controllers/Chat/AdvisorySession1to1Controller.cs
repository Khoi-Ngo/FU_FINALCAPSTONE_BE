using AISEA.ApiService.BAL.Services.Chat;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Chat;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.Hubs;
using AISEA.ApiService.WebApi.HubUtil;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AISEA.ApiService.WebApi.Controllers.Chat;

[ApiController]
[Route("api/[controller]")]
public class AdvisorySession1to1Controller : BaseController
{
    private readonly AdvisorySession1to1Service _advisorySession1To1Service;
    private readonly AdvisorySessionHubNotifier _advisorySessionHubNotifier;

    public AdvisorySession1to1Controller(EndpointSettings endpointSettings,
    AdvisorySession1to1Service advisorySession1To1Service,
    AdvisorySessionHubNotifier advisorySessionHubNotifier) : base(endpointSettings)
    {
        _advisorySession1To1Service = advisorySession1To1Service;
        _advisorySessionHubNotifier = advisorySessionHubNotifier;
    }

    /// <summary>
    /// Delete Chat Session
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        var session = await _advisorySession1To1Service.GetByIdAsync(id); // Fetch session for StaffId and StudentId
        await _advisorySession1To1Service.DeleteAsync(id, AccessToken);

        // Notify student, staff, and session groups
        await _advisorySessionHubNotifier.NotifySessionDeletedAsync(session.Id, session.StaffId, session.StudentId);
        return Ok("Delete successfully");
    }

    /// <summary>
    /// Initialize the chat session with Staffs User
    /// </summary>
    [HttpPost("human")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    public async Task<IActionResult> InitHumanChatSessionAsync([FromBody] InitHumanChatSessionRequest request)
    {
        var (res, hubRes, studentProfileId) = await _advisorySession1To1Service.InitHumanChatSessionAsync(request, AccessToken);
        //call hub context -> push
        await _advisorySessionHubNotifier.NotifySessionCreatedAsync(studentProfileId, hubRes);
        return Ok(res);
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
