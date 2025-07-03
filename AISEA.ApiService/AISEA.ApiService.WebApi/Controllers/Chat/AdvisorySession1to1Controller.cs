using AISEA.ApiService.BAL.Services.Chat;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Chat;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AISEA.ApiService.WebApi.Controllers.Chat;

[ApiController]
[Route("api/[controller]")]
public class AdvisorySession1to1Controller : BaseController
{
    private readonly AdvisorySession1to1Service _advisorySession1To1Service;
    private readonly IHubContext<AdvisoryChat1to1Hub> _advSessionHubContext;
    private readonly ChatSessionSettings _chatSessionSettings;
    private readonly StaffUserSettings _staffUserSettings;

    public AdvisorySession1to1Controller(EndpointSettings endpointSettings,
    AdvisorySession1to1Service advisorySession1To1Service,
    IHubContext<AdvisoryChat1to1Hub> advSessionHubContext,
    ChatSessionSettings chatSessionSettings,
    StaffUserSettings staffUserSettings) : base(endpointSettings)
    {
        _advisorySession1To1Service = advisorySession1To1Service;
        _advSessionHubContext = advSessionHubContext;
        _chatSessionSettings = chatSessionSettings;
        _staffUserSettings = staffUserSettings;
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
        await Task.WhenAll(
            _advSessionHubContext.Clients.Group($"{_chatSessionSettings.MulDataSessionsPrefixStaff}{session.StaffId}")
                .SendAsync(_chatSessionSettings.SessionDeletedMethod, id),
            _advSessionHubContext.Clients.Group($"{_chatSessionSettings.MulDataSessionsPrefixStudent}{session.StudentId}")
                .SendAsync(_chatSessionSettings.SessionDeletedMethod, id),
            _advSessionHubContext.Clients.Group($"{_chatSessionSettings.GroupChatADVssPrefix}{id}")
                .SendAsync(_chatSessionSettings.SessionDeletedMethod, id)
        );

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
        await _advSessionHubContext.Clients.Group($"_chatSessionSettings.MulDataSessionsPrefixStaff{_staffUserSettings.EmptyStaffProfileId}")
                .SendAsync(_chatSessionSettings.SessionCreatedMethod, hubRes);
        await _advSessionHubContext.Clients.Group($"_chatSessionSettings.MulDataSessionsPrefixStudent{studentProfileId}")
        .SendAsync(_chatSessionSettings.SessionCreatedMethod, hubRes);
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
