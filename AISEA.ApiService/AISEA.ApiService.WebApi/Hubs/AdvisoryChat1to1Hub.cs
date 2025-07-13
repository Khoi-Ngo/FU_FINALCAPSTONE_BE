using AISEA.ApiService.BAL.Services.Chat;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.AdvisorySession1to1;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace AISEA.ApiService.WebApi.Hubs;

public class AdvisoryChat1to1Hub : BaseHub
{

    #region Init
    private readonly AdvisorySession1to1Service _advisorySession1To1Service;
    private readonly StaffUserSettings _staffUserSettings;
    private readonly ChatSessionSettings _chatSessionSettings;
    private readonly JwtSettings _jwtSettings;
    private readonly IJWTService _jWTService;
    private readonly IMapper _mapper;

    public AdvisoryChat1to1Hub(EndpointSettings endpointSettings,
    AdvisorySession1to1Service advisorySession1To1Service,
    StaffUserSettings staffUserSettings,
    ChatSessionSettings chatSessionSettings,
    IJWTService jWTService,
    JwtSettings jwtSettings,
    IMapper mapper) : base(endpointSettings)
    {
        _advisorySession1To1Service = advisorySession1To1Service;
        _staffUserSettings = staffUserSettings;
        _chatSessionSettings = chatSessionSettings;
        _jwtSettings = jwtSettings;
        _jWTService = jWTService;
        _mapper = mapper;
    }

    #endregion

    /// <summary>
    /// Staff or Student sends a message in an AdvisoryChatSession1to1.
    /// The session have to be initialized in advanced.
    /// The message is saved to the database and broadcast to the session group.
    /// </summary>
    public async Task SendMessage(long sessionId, string content)
    {

        //allowing utf 8
        content = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetBytes(content));

        var message = await _advisorySession1To1Service.SendMessageAsync(sessionId, content, AccessToken);
        var session = await _advisorySession1To1Service.GetByIdAsync(sessionId);
        session.UpdatedAt = DateTime.UtcNow;

        var sessionGroup = $"{_chatSessionSettings.GroupChatADVssPrefix}{sessionId}";
        var broadcastSession = _mapper.Map<GetAdvisorySession1to1ItemsResponse>(session);

        await Task.WhenAll(
            Clients.Group(sessionGroup).SendAsync(_chatSessionSettings.SendADVSSMethod, message),
            Clients.Group($"{_chatSessionSettings.MulDataSessionsPrefixStaff}{session.StaffId}")
                .SendAsync(_chatSessionSettings.GetSessionsHUBMethod, broadcastSession),
            Clients.Group($"{_chatSessionSettings.MulDataSessionsPrefixStudent}{session.StudentId}")
                .SendAsync(_chatSessionSettings.GetSessionsHUBMethod, broadcastSession)
        );

        await _advisorySession1To1Service.UpdateSessionAsync(session);
    }

    /// <summary>
    ///Advisor or Student join the Advisory Chat Session
    ///This includes verification before joining
    /// </summary>
    public async Task JoinSession(long sessionId)
    {

        EnsureUserHasRole(AccessToken, EUserRole.ADVISOR, EUserRole.STUDENT);

        var session = await _advisorySession1To1Service.JoinSessionAsync(sessionId, AccessToken);
        await Groups.AddToGroupAsync(Context.ConnectionId, $"{_chatSessionSettings.GroupChatADVssPrefix}{sessionId}");
        if (session.StaffId != _staffUserSettings.EmptyStaffProfileId)
        {
            await Clients.Group($"{_chatSessionSettings.MulDataSessionsPrefixStaff}{_staffUserSettings.EmptyStaffProfileId}")
                .SendAsync(_chatSessionSettings.RemoveSessionFromUnassigned, session);

            await Clients.Group($"{_chatSessionSettings.MulDataSessionsPrefixStaff}{session.StaffId}")
                .SendAsync(_chatSessionSettings.AddSessionAsAssigned, session);
        }

        await Clients.Caller.SendAsync(
            _chatSessionSettings.JoinSSMethod,
            await _advisorySession1To1Service.GetMessagesAsync(new PaginationRequest(), sessionId)
        );
    }

    /// <summary>
    /// Student Get the mul data of chat sessions related to them
    /// </summary>
    public async Task ListAllSessionByStudent()
    {
        EnsureUserHasRole(AccessToken, EUserRole.STUDENT);
        var studentProfileId = _jWTService.GetProfileIdFromToken(AccessToken);
        var sessionsResponse = await _advisorySession1To1Service.GetHumanSessionsByStudentAsync(new PaginationRequest (), studentProfileId);
        await Task.WhenAll(
            Clients.Caller.SendAsync(_chatSessionSettings.GetSessionsHUBMethod, sessionsResponse),
            Groups.AddToGroupAsync(Context.ConnectionId, $"{_chatSessionSettings.MulDataSessionsPrefixStudent}{studentProfileId}")
        );
    }

    /// <summary>
    /// Staff Get the mul data of chat sessions related to them
    /// </summary>
    public async Task ListAllSessionByStaff()
    {
        EnsureUserHasRole(AccessToken, EUserRole.ADVISOR);
        var staffProfileId = _jWTService.GetProfileIdFromToken(AccessToken);

        var sessionsResponse = await _advisorySession1To1Service.GetHumanSessionsByStaffAsync(new PaginationRequest (), staffProfileId);
        await Task.WhenAll(
            Clients.Caller.SendAsync(_chatSessionSettings.GetSessionsHUBMethod, sessionsResponse),
            Groups.AddToGroupAsync(Context.ConnectionId, $"{_chatSessionSettings.MulDataSessionsPrefixStaff}{staffProfileId}")
        );
    }

    /// <summary>
    ///Staff can access this chanel to view real time unassigned sessions
    /// </summary>
    public async Task ListOpenedSession()
    {
        EnsureUserHasRole(AccessToken, EUserRole.ADVISOR);
        
        var sessionsResponse = await _advisorySession1To1Service.GetHumanSessionsByStaffAsync(new PaginationRequest (), _staffUserSettings.EmptyStaffProfileId);
        await Task.WhenAll(
            Clients.Caller.SendAsync(_chatSessionSettings.GetSessionsHUBMethod, sessionsResponse),
            Groups.AddToGroupAsync(Context.ConnectionId, $"{_chatSessionSettings.MulDataSessionsPrefixStaff}{_staffUserSettings.EmptyStaffProfileId}")
        );
    }

    /// <summary>
    /// Load more messages for a session when scrolling
    /// </summary>
    public async Task LoadMoreMessages(long sessionId, PaginationRequest pagination)
    {
        EnsureUserHasRole(AccessToken, EUserRole.ADVISOR, EUserRole.STUDENT);
        var messages = await _advisorySession1To1Service.GetMessagesAsync(pagination, sessionId);
        await Clients.Caller.SendAsync(_chatSessionSettings.LoadMoreMessagesMethod, messages);
    }

    /// <summary>
    /// Loads additional chat sessions for a user when scrolling, supporting infinite scroll pagination.
    /// </summary>
    /// <param name="pagination">Pagination parameters (page number and size).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task LoadMoreSessions(PaginationRequest pagination)
    {
        // Extract user data from token
        var profileId = _jWTService.GetProfileIdFromToken(AccessToken);
        var roleId = _jWTService.GetRoleIdFromToken(AccessToken);

        // Determine group name based on user role
        string groupName = roleId switch
        {
            (int)EUserRole.STUDENT => $"{_chatSessionSettings.MulDataSessionsPrefixStudent}{profileId}",
            (int)EUserRole.ADVISOR => $"{_chatSessionSettings.MulDataSessionsPrefixStaff}{profileId}",
            _ => throw new HubException("Invalid role for session access")
        };

        // Fetch sessions and add to group concurrently
        var sessions = await _advisorySession1To1Service.GetSessionsAsync(pagination, AccessToken);
        await Task.WhenAll(
            Clients.Caller.SendAsync(_chatSessionSettings.GetSessionsHUBMethod, sessions),
            Groups.AddToGroupAsync(Context.ConnectionId, groupName)
        );
    }

    private void EnsureUserHasRole(string token, params EUserRole[] allowedRoles)
    {
        var userData = _jWTService.GetAllClaimsFromToken(token);

        if (!userData.TryGetValue(_jwtSettings.AuthProp, out var strRole) ||
            !int.TryParse(strRole, out var roleId) ||
            !allowedRoles.Select(r => (int)r).Contains(roleId))
        {
            throw new HubException("Forbidden: insufficient role permission");
        }
    }



}