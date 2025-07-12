using System.Text.Json;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.Const.Values;
using AISEA.ApiService.SHARED.DTOs.Requests.Chat;
using AISEA.ApiService.SHARED.DTOs.Requests.ChatBot;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.AdvisorySession1to1;
using AISEA.ApiService.SHARED.DTOs.Responses.ChatBot;
using AISEA.ApiService.SHARED.DTOs.Responses.Message;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.SHARED.Util;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace AISEA.ApiService.BAL.Services.Chat;

public class AdvisorySession1to1Service
{
    private readonly AdvisorySession1to1Repository _advisorySession1To1Repository;
    private readonly IJWTService _jWTService;
    private readonly IRedisRepository _redisRepository;
    private readonly ChatSessionSettings _chatSessionSettings;
    private readonly StaffUserSettings _staffUserSettings;
    private readonly JwtSettings _jwtSettings;
    private readonly IMapper _mapper;
    private readonly MessageRepository _messageRepository;
    private readonly IChatOpenAIService _chatOpenAIService;

    public AdvisorySession1to1Service(
        AdvisorySession1to1Repository advisorySession1To1Repository,
        IJWTService jWTService,
        IRedisRepository redisRepository,
        ChatSessionSettings chatSessionSettings,
        StaffUserSettings staffUserSettings,
        JwtSettings jwtSettings,
        IMapper mapper,
        MessageRepository messageRepository,
        IChatOpenAIService chatOpenAIService)
    {
        _advisorySession1To1Repository = advisorySession1To1Repository;
        _jWTService = jWTService;
        _redisRepository = redisRepository;
        _chatSessionSettings = chatSessionSettings;
        _staffUserSettings = staffUserSettings;
        _jwtSettings = jwtSettings;
        _mapper = mapper;
        _messageRepository = messageRepository;
        _chatOpenAIService = chatOpenAIService;
    }

    public async Task<AdvisorySession1to1> DeleteAsync(long chatSessionId, string accessToken)
    {
        var profileId = GetProfileIdFromToken(accessToken);
        var session = await GetByIdAsync(chatSessionId, profileId);
        await _advisorySession1To1Repository.RemoveAsync(session);
        return session;
    }

    public async Task<AdvisorySession1to1> GetByIdAsync(long chatSessionId, long profileId)
    {
        var cacheKey = $"{_chatSessionSettings.SessionCachePrefix}{chatSessionId}";
        var cachedSession = await _redisRepository.GetValueAsync<AdvisorySession1to1>(cacheKey);

        if (IsValidAccessSession(cachedSession, profileId))
        {
            return cachedSession;
        }

        var session = await _advisorySession1To1Repository.GetByIdAsync(chatSessionId);
        if (!IsValidAccessSession(session, profileId))
        {
            throw new InvalidAccessSession("Not found or invalid access");
        }

        await _redisRepository.SetValueAsync(cacheKey, session, TimeSpan.FromDays(_chatSessionSettings.SessionCacheExpiryDays));
        return session;
    }

    public async Task<AdvisorySession1to1> GetByIdAsync(long chatSessionId)
    {
        var cacheKey = $"{_chatSessionSettings.SessionCachePrefix}{chatSessionId}";
        var cachedSession = await _redisRepository.GetValueAsync<AdvisorySession1to1>(cacheKey);

        if (cachedSession != null)
        {
            return cachedSession;
        }

        var session = await _advisorySession1To1Repository.GetByIdAsync(chatSessionId);
        if (session == null)
        {
            throw new InvalidAccessSession("Not found");
        }

        await _redisRepository.SetValueAsync(cacheKey, session, TimeSpan.FromDays(_chatSessionSettings.SessionCacheExpiryDays));
        return session;
    }

    public async Task<AdvisorySession1to1> CreateSessionAsync(long studentProfileId, EAdvisorySession1to1Type type, long staffId, string title)
    {
        var newSession = new AdvisorySession1to1
        {
            Title = title,
            StaffId = staffId,
            Type = type,
            StudentId = studentProfileId
        };

        await _advisorySession1To1Repository.CreateAsync(newSession);
        await _redisRepository.SetValueAsync(
            $"{_chatSessionSettings.SessionCachePrefix}{newSession.Id}",
            newSession,
            TimeSpan.FromDays(_chatSessionSettings.SessionCacheExpiryDays));
        return newSession;
    }

    public bool IsValidAccessSession(AdvisorySession1to1 session, long profileId) =>
        session != null && (session.StudentId == profileId || session.StaffId == profileId);

    public async Task<PagedResult<GetAdvisorySession1to1ItemsResponse>> GetBotSessionsAsync(PaginationRequest request, string accessToken)
    {
        var profileId = GetProfileIdFromToken(accessToken);
        var (sessions, totalCount) = await _advisorySession1To1Repository.GetSessionsByProfileId(
            request.PageNumber, request.PageSize, isStudentQuery: true, EAdvisorySession1to1Type.BOT, profileId);

        return new PagedResult<GetAdvisorySession1to1ItemsResponse>
        {
            Items = _mapper.Map<List<GetAdvisorySession1to1ItemsResponse>>(sessions),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PagedResult<GetAdvisorySession1to1ItemsResponse>> GetHumanSessionsByStudentAsync(PaginationRequest request, long studentProfileId)
    {
        var (sessions, totalCount) = await _advisorySession1To1Repository.GetSessionsByProfileId(
            request.PageNumber, request.PageSize, true, EAdvisorySession1to1Type.HUMAN, studentProfileId);

        return new PagedResult<GetAdvisorySession1to1ItemsResponse>
        {
            Items = _mapper.Map<List<GetAdvisorySession1to1ItemsResponse>>(sessions),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PagedResult<GetAdvisorySession1to1ItemsResponse>> GetHumanSessionsByStaffAsync(PaginationRequest request, long staffProfileId)
    {
        var (sessions, totalCount) = await _advisorySession1To1Repository.GetSessionsByProfileId(
            request.PageNumber, request.PageSize, false, EAdvisorySession1to1Type.HUMAN, staffProfileId);

        return new PagedResult<GetAdvisorySession1to1ItemsResponse>
        {
            Items = _mapper.Map<List<GetAdvisorySession1to1ItemsResponse>>(sessions),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task UpdateSessionAsync(AdvisorySession1to1 session)
    {
        await _advisorySession1To1Repository.UpdateAsync(session);
        await _redisRepository.SetValueAsync(
            $"{_chatSessionSettings.SessionCachePrefix}{session.Id}",
            session,
            TimeSpan.FromDays(_chatSessionSettings.SessionCacheExpiryDays));
    }

    public async Task<MessageItemResponse> SendMessageAsync(long chatSessionId, string content, string accessToken)
    {
        var senderData = _jWTService.GetAllClaimsFromToken(accessToken);
        var userId = long.Parse(senderData.GetValueOrDefault(_jwtSettings.UserId));
        var message = await CreateMessageAsync(content, userId, chatSessionId);
        return _mapper.Map<MessageItemResponse>(message);
    }

    public async Task<GetAdvisorySession1to1ItemsResponse> JoinSessionAsync(long sessionId, string accessToken)
    {
        var userData = _jWTService.GetAllClaimsFromToken(accessToken);
        var profileId = long.Parse(userData.GetValueOrDefault(_jwtSettings.ProfileId));
        var roleId = int.Parse(userData.GetValueOrDefault(_jwtSettings.AuthProp));
        var staffUserName = userData.GetValueOrDefault(_jwtSettings.UserName);

        var chatSession = await GetByIdAsync(sessionId);

        if (roleId == (int)EUserRole.STUDENT && profileId == chatSession.StudentId)
        {
            chatSession.StudentJoinAt = DateTime.UtcNow;
        }
        else if (roleId == (int)EUserRole.ADVISOR && profileId == chatSession.StaffId)
        {
            chatSession.StaffJoinAt = DateTime.UtcNow;
        }
        else if (chatSession.StaffId == _staffUserSettings.SystemBotUser.StaffId && roleId == (int)EUserRole.ADVISOR)
        {
            chatSession.StaffId = profileId;
            chatSession.UpdatedAt = DateTime.UtcNow;
            chatSession.StaffJoinAt = DateTime.UtcNow;
            chatSession.Title = Advisory1to1Util.GenerateHumanSessionTitle(staffUserName);
        }
        else
        {
            throw new HubException("No permission to join the chat session");
        }

        await UpdateSessionAsync(chatSession);
        return _mapper.Map<GetAdvisorySession1to1ItemsResponse>(chatSession);
    }

    private async Task<Message> CreateMessageAsync(string content, long senderId, long sessionId)
    {
        var newMessage = new Message
        {
            Content = content,
            SenderId = senderId,
            AdvisorySession1to1Id = sessionId
        };
        await _messageRepository.CreateAsync(newMessage);
        return newMessage;
    }
    public async Task<PagedResult<MessageItemResponse>> GetChatBotMessagesAsync(PaginationRequest request, long chatSessionId)
    {
        var session = await GetByIdAsync(chatSessionId);
        if (session.Type != EAdvisorySession1to1Type.BOT)
        {
            throw new InvalidAccessSession("Cannot get other type of messages from this API");
        }

        return await _messageRepository.GetMessagesAsync(chatSessionId, request.PageNumber, request.PageSize);
    }

    public async Task<PagedResult<MessageItemResponse>> GetMessagesAsync(PaginationRequest request, long chatSessionId)
    {
        return await _messageRepository.GetMessagesAsync(chatSessionId, request.PageNumber, request.PageSize);
    }

    private string ConstructPromptInit(string studentName, object? studentJsonData = null, object? fPTUAcademicResourceJsonData = null, string? message = null)
    {
        var studentJson = studentJsonData != null ? JsonSerializer.Serialize(studentJsonData) : "{}";
        var resourceJson = fPTUAcademicResourceJsonData != null ? JsonSerializer.Serialize(fPTUAcademicResourceJsonData) : "{}";
        var msg = message ?? "";

        return ChatBotConst.GeneralMessageStructFromStudent
            .Replace("{studentName}", studentName)
            .Replace("{studentJsonData}", studentJson)
            .Replace("{FPTUAcademicResourceJsonData}", resourceJson)
            .Replace("{message}", msg);
    }

    public async Task<GetChatBotResponse> SendMsgAsync(SendChatBotRequest request, string accessToken)
    {
        var userId = GetUserIdFromToken(accessToken);
        await CreateMessageAsync(request.Message, userId, request.ChatSessionId);
        var aiResponse = await _chatOpenAIService.SendMsgAsync(request.Message);
        aiResponse = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetBytes(aiResponse));
        await CreateMessageAsync(aiResponse, _staffUserSettings.SystemBotUser.Id, request.ChatSessionId);
        return new GetChatBotResponse { Message = aiResponse };
    }

    public async Task<InitChatBotResponse> InitMsgAsync(InitChatBotRequest request, string accessToken)
    {
        var userData = _jWTService.GetAllClaimsFromToken(accessToken);
        var profileId = int.Parse(userData[_jwtSettings.ProfileId]);
        var userId = int.Parse(userData[_jwtSettings.UserId]);
        var studentName = $"{userData.GetValueOrDefault(_jwtSettings.FirstName, string.Empty)} {userData.GetValueOrDefault(_jwtSettings.LastName, string.Empty)}";

        var chatSession = await CreateSessionAsync(
            profileId,
            EAdvisorySession1to1Type.BOT,
            _staffUserSettings.SystemBotUser.StaffId,
            System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetBytes(Advisory1to1Util.GenerateChatBotSessionTitle(request.Message))));

        await CreateMessageAsync(request.Message, userId, chatSession.Id);
        var aiResponse = await _chatOpenAIService.SendMsgAsync(ConstructPromptInit(studentName, null, null, request.Message));
        aiResponse = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetBytes(aiResponse));
        await CreateMessageAsync(aiResponse, _staffUserSettings.SystemBotUser.Id, chatSession.Id);
        return new InitChatBotResponse { Message = aiResponse, ChatSessionId = chatSession.Id };
    }

    public async Task<(GetAdvisorySession1to1ItemsResponse hubResponse, long studentProfileId)> InitHumanChatSessionAsync(InitHumanChatSessionRequest request, string accessToken)
    {
        var userData = _jWTService.GetAllClaimsFromToken(accessToken);
        var profileId = long.Parse(userData.GetValueOrDefault(_jwtSettings.ProfileId));
        var userId = long.Parse(userData.GetValueOrDefault(_jwtSettings.UserId));
        var title = Advisory1to1Util.GenerateHumanSessionTitle(_staffUserSettings.EmptyStaffName);

        var newSession = await CreateSessionAsync(profileId, EAdvisorySession1to1Type.HUMAN, _staffUserSettings.EmptyStaffProfileId, title);
        await CreateMessageAsync(request.Message, userId, newSession.Id);

        return (
            _mapper.Map<GetAdvisorySession1to1ItemsResponse>(newSession),
            profileId);
    }
    public async Task<PagedResult<GetAdvisorySession1to1ItemsResponse>> GetSessionsAsync(PaginationRequest request, string accessToken)
    {
        var profileId = GetProfileIdFromToken(accessToken);
        var userData = _jWTService.GetAllClaimsFromToken(accessToken);
        var roleId = int.Parse(userData.GetValueOrDefault(_jwtSettings.AuthProp));
        var isStudent = roleId == (int)EUserRole.STUDENT;
        var sessionType = isStudent ? EAdvisorySession1to1Type.BOT : EAdvisorySession1to1Type.HUMAN;

        var (sessions, totalCount) = await _advisorySession1To1Repository.GetSessionsByProfileId(
            request.PageNumber, request.PageSize, isStudent, sessionType, profileId);

        return new PagedResult<GetAdvisorySession1to1ItemsResponse>
        {
            Items = _mapper.Map<List<GetAdvisorySession1to1ItemsResponse>>(sessions),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    private long GetProfileIdFromToken(string accessToken) =>
        long.Parse(_jWTService.GetAllClaimsFromToken(accessToken).GetValueOrDefault(_jwtSettings.ProfileId));

    private long GetUserIdFromToken(string accessToken) =>
        long.Parse(_jWTService.GetAllClaimsFromToken(accessToken).GetValueOrDefault(_jwtSettings.UserId));
}