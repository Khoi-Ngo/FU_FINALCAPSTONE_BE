using System;
using System.Text.Json;
using System.Threading.Tasks;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.Const.Values;
using AISEA.ApiService.SHARED.DTOs.Requests.ChatBot;
using AISEA.ApiService.SHARED.DTOs.Responses.ChatBot;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.SHARED.Util;
using Microsoft.Extensions.Logging;

namespace AISEA.ApiService.BAL.Services.Chat;

public class ChatBotService
{
    private readonly IChatOpenAIService _chatOpenAIService;
    private readonly ChatBotSettings _chatBotSettings;
    private readonly IJWTService _jWTService;
    private readonly UserRepository _userRepository;
    private readonly AdvisorySession1to1Repository _advisorySession1To1Repository;
    private readonly IRedisRepository _redisRepository;
    private readonly MessageRepository _messageRepository;

    public ChatBotService(
        IChatOpenAIService chatOpenAIService,
        ChatBotSettings chatBotSettings,
        IJWTService jWTService,
        UserRepository userRepository,
        AdvisorySession1to1Repository advisorySession1To1Repository,
        IRedisRepository redisRepository,
        MessageRepository messageRepository)
    {
        _chatOpenAIService = chatOpenAIService;
        _chatBotSettings = chatBotSettings;
        _jWTService = jWTService;
        _userRepository = userRepository;
        _advisorySession1To1Repository = advisorySession1To1Repository;
        _redisRepository = redisRepository;
        _messageRepository = messageRepository;
    }

    public async Task<GetChatBotResponse> SendMsgAsync(SendChatBotRequest request, string accessToken)
    {
        var student = await ValidateAndGetStudentAsync(accessToken);
        var session1To1 = await GetOrCreateSessionAsync(request, student);

        var studentMessage = CreateMessage(request.Message, student.Id, session1To1.Id);
        await _messageRepository.CreateAsync(studentMessage);


        // Get AI response
        var prompt = ConstructPrompt(
            $"{student.FirstName} {student.LastName}",
            null, // Replace with actual studentAcademicPerformanceJsonData
            null, // Replace with actual FPTUAcademicResourceJsonData
            request.Message
        );
        var aiResponse = await _chatOpenAIService.SendMsgAsync(prompt);

        var botMessage = CreateMessage(aiResponse, _chatBotSettings.SystemBotUser.Id, session1To1.Id);
        await _messageRepository.CreateAsync(botMessage);


        return new GetChatBotResponse
        {
            Message = aiResponse,
            SessionId = session1To1.Id
        };
    }


    private async Task<DAL.Entities.User> ValidateAndGetStudentAsync(string accessToken)
    {
        var studentName = _jWTService.GetUsernameFromToken(accessToken);
        var cacheKey = $"{_chatBotSettings.StudentCachePrefix}{studentName}";


        // Try to get from Redis
        var cachedUser = await _redisRepository.GetValueAsync<DAL.Entities.User>(cacheKey);
        if (cachedUser is not null && cachedUser.StudentProfile is not null)
        {
            return cachedUser;
        }

        var student = await _userRepository.GetUserByUsernameWStudentProfileAsync(studentName);
        if (student?.StudentProfile is null)
        {
            throw new InvalidAccessSession("Invalid student profile");
        }

        await _redisRepository.SetValueAsync(cacheKey, student, TimeSpan.FromHours(_chatBotSettings.StudentCacheExpiryHrs));
        return student;
    }

    private async Task<AdvisorySession1to1> GetOrCreateSessionAsync(SendChatBotRequest request, DAL.Entities.User student)
    {
        if (request.ChatSessionId > 0)
        {
            var cacheKey = $"{_chatBotSettings.SessionCachePrefix}{request.ChatSessionId}";
            var cachedSession = await _redisRepository.GetValueAsync<AdvisorySession1to1>(cacheKey);

            if (cachedSession is not null && cachedSession.StudentId == student.StudentProfile.Id)
            {
                return cachedSession;
            }

            var session = await _advisorySession1To1Repository.GetByIdAsync(request.ChatSessionId);
            if (session is null || session.StudentId != student.StudentProfile.Id)
            {
                throw new InvalidAccessSession("The chat session id is invalid");
            }

            await _redisRepository.SetValueAsync(cacheKey, session, TimeSpan.FromDays(_chatBotSettings.SessionCacheExpiryDays));
            return session;
        }

        var title = Advisory1to1Util.GenerateSessionTitle(request.Message, _chatBotSettings.SystemBotUser.FirstName, _chatBotSettings.SystemBotUser.LastName);
        var newSession = new AdvisorySession1to1
        {
            Title = title,
            StaffId = _chatBotSettings.SystemBotUser.StaffId,
            Type = EAdvisorySession1to1Type.BOT,
            StudentId = student.StudentProfile.Id
        };

        await _advisorySession1To1Repository.CreateAsync(newSession);

        // Cache new session
        await _redisRepository.SetValueAsync($"{_chatBotSettings.SessionCachePrefix}{newSession.Id}", newSession, TimeSpan.FromDays(_chatBotSettings.SessionCacheExpiryDays));
        return newSession;
    }

    private Message CreateMessage(string content, long senderId, long sessionId)
    {
        return new Message
        {
            Content = content,
            SenderId = senderId,
            AdvisorySession1to1Id = sessionId
        };
    }

    private string ConstructPrompt(
        string studentName,
        object? studentJsonData = null,
        object? fPTUAcademicResourceJsonData = null,
        string? message = null)
    {
        var studentJson = studentJsonData is not null
            ? JsonSerializer.Serialize(studentJsonData)
            : "{}";
        var resourceJson = fPTUAcademicResourceJsonData is not null
            ? JsonSerializer.Serialize(fPTUAcademicResourceJsonData)
            : "{}";
        var msg = message ?? "";

        return
        ChatBotConst.GeneralMessageStructFromStudent
            .Replace("{studentName}", studentName)
            .Replace("{studentJsonData}", studentJson)
            .Replace("{FPTUAcademicResourceJsonData}", resourceJson)
            .Replace("{message}", msg);
    }
}