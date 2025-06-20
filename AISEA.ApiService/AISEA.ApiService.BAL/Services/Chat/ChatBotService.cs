using System.Text.Json;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.Const.Values;
using AISEA.ApiService.SHARED.DTOs.Requests.ChatBot;
using AISEA.ApiService.SHARED.DTOs.Responses.ChatBot;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using Microsoft.Extensions.Logging;

namespace AISEA.ApiService.BAL.Services.Chat;

public class ChatBotService
{
    private readonly ILogger<ChatBotService> _logger;
    private readonly IChatOpenAIService _chatOpenAIService;
    private readonly ChatBotSettings _chatBotSettings;
    private readonly IJWTService _jWTService;
    private readonly UserRepository _userRepository;
    private readonly AdvisorySession1to1Repository _advisorySession1To1Repository;
    private readonly MessageRepository _messageRepository;

    public ChatBotService(
        ILogger<ChatBotService> logger,
        IChatOpenAIService chatOpenAIService,
        ChatBotSettings chatBotSettings,
        IJWTService jWTService,
        UserRepository userRepository,
        AdvisorySession1to1Repository advisorySession1To1Repository,
        MessageRepository messageRepository)
    {
        _logger = logger;
        _chatOpenAIService = chatOpenAIService;
        _chatBotSettings = chatBotSettings;
        _jWTService = jWTService;
        _userRepository = userRepository;
        _advisorySession1To1Repository = advisorySession1To1Repository;
        _messageRepository = messageRepository;
    }

    public async Task<ChatBotResponse> SendMsgAsync(SendChatBotRequest request, string accessToken)
    {
        try
        {
            var student = await ValidateAndGetStudentAsync(accessToken);
            var session1To1 = await GetOrCreateSessionAsync(request, student);
            
            // Save student's message
            var studentMessage = CreateMessage(request.Message, student.Id, session1To1.Id);
            await _messageRepository.CreateAsync(studentMessage);

            // Get AI response
            var prompt = ConstructPrompt(
                $"{student.FirstName} {student.LastName}",
                null, // Replace with actual studentJsonData when available
                null, // Replace with actual FPTUAcademicResourceJsonData when available
                request.Message
            );
            var aiResponse = await _chatOpenAIService.SendMsgAsync(prompt);

            // Save bot's response
            var botMessage = CreateMessage(aiResponse.Message, _chatBotSettings.SystemUser.Id, session1To1.Id);
            await _messageRepository.CreateAsync(botMessage);

            return aiResponse;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error processing chat message");
            return new ChatBotResponse
            {
                Message = _chatBotSettings.DefaultErrorResponse
            };
        }
    }

    private async Task<DAL.Entities.User> ValidateAndGetStudentAsync(string accessToken)
    {
        var studentName = _jWTService.GetUsernameFromToken(accessToken);
        var student = await _userRepository.GetUserByUsernameWStudentProfileAsync(studentName);
        
        if (student?.StudentProfile == null)
        {
            throw new InvalidAccessSession("Invalid student profile");
        }
        
        return student;
    }

    private async Task<AdvisorySession1to1> GetOrCreateSessionAsync(SendChatBotRequest request, DAL.Entities.User student)
    {
        if (request.ChatSessionId > 0)
        {
            var session = await _advisorySession1To1Repository.GetByIdAsync(request.ChatSessionId);
            if (session == null || session.StudentId != student.StudentProfile.Id)
            {
                throw new InvalidAccessSession("The chat session id is invalid");
            }
            return session;
        }

        var title = GenerateSessionTitle(request.Message);
        var newSession = new AdvisorySession1to1
        {
            Title = title,
            StaffId = _chatBotSettings.SystemUser.StaffId,
            Type = EAdvisorySessionType.BOT,
            StudentId = student.StudentProfile.Id
        };
        
        await _advisorySession1To1Repository.CreateAsync(newSession);
        return newSession;
    }

    private string GenerateSessionTitle(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            var staff = _chatBotSettings.SystemUser;
            return $"{staff.FirstName} {staff.LastName} at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
        }

        var trimmed = message.Trim();
        int endIdx = trimmed.IndexOfAny(new[] { '.', '!', '?' });
        return endIdx > 0 && endIdx < 40
            ? trimmed.Substring(0, endIdx + 1)
            : trimmed.Length > 40 ? trimmed.Substring(0, 40) + "..." : trimmed;
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
        var studentJson = studentJsonData != null
            ? JsonSerializer.Serialize(studentJsonData)
            : "{}";
        var resourceJson = fPTUAcademicResourceJsonData != null
            ? JsonSerializer.Serialize(fPTUAcademicResourceJsonData)
            : "{}";
        var msg = message ?? "";

        return ChatBotConst.GeneralMessageStructFromStudent
            .Replace("{studentName}", studentName)
            .Replace("{studentJsonData}", studentJson)
            .Replace("{FPTUAcademicResourceJsonData}", resourceJson)
            .Replace("{message}", msg);
    }
}