
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

    public ChatBotService(ILogger<ChatBotService> logger, IChatOpenAIService chatOpenAIService, ChatBotSettings chatBotSettings, IJWTService jWTService, UserRepository userRepository, AdvisorySession1to1Repository advisorySession1To1Repository, MessageRepository messageRepository)
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
            var studentName = _jWTService.GetUsernameFromToken(accessToken);
            var student = await _userRepository.GetUserByUsernameWStudentProfileAsync(studentName);

            // 1. Find or create an AdvisorySession1to1 for this student (have to verify the owner chat session)
            AdvisorySession1to1 session1To1;
            if (request.ChatSessionId > 0)
            {
                session1To1 = await _advisorySession1To1Repository.GetByIdAsync(request.ChatSessionId);
                if (session1To1 is null || session1To1?.StudentId != student.StudentProfile.Id)
                {
                    throw new InvalidAccessSession("The chat session id is invalid");
                }
            }
            else
            {
                string title;
                if (!string.IsNullOrWhiteSpace(request.Message))
                {
                    var trimmed = request.Message.Trim();
                    int endIdx = trimmed.IndexOfAny(new[] { '.', '!', '?' });
                    if (endIdx > 0 && endIdx < 40)
                        title = trimmed.Substring(0, endIdx + 1);
                    else
                        title = trimmed.Length > 40 ? trimmed.Substring(0, 40) + "..." : trimmed;
                }
                else
                {
                    var staff = _chatBotSettings.SystemUser;
                    title = $"{staff.FirstName} {staff.LastName} at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
                }

                session1To1 = new AdvisorySession1to1
                {
                    Title = title,
                    StaffId = _chatBotSettings.SystemUser.StaffId,
                    Type = EAdvisorySessionType.BOT,
                    StudentId = student.StudentProfile.Id
                };
                await _advisorySession1To1Repository.CreateAsync(session1To1);

            }


            // 2. Save the student's message

            var studentMessage = new Message
            {
                Content = request.Message,
                SenderId = student.Id,
                AdvisorySession1to1Id = session1To1.Id
            };

            await _messageRepository.CreateAsync(studentMessage);

            //logic call ChatOpenAI

            var prompt = ConstructPrompt(
                student.FirstName + " " + student.LastName,
                null, // Replace with actual studentJsonData when available
                null, // Replace with actual FPTUAcademicResourceJsonData when available
                request.Message
            );
            var res = await _chatOpenAIService.SendMsgAsync(prompt);


            //3. Save the chat bot response to the chat session

            var botMessage = new Message
            {
                Content = res.Message,
                SenderId = _chatBotSettings.SystemUser.Id,
                AdvisorySession1to1Id = session1To1.Id
            };

            await _messageRepository.CreateAsync(botMessage);


            return res;

        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return new ChatBotResponse
            {
                Message = _chatBotSettings.DefaultErrorResponse
            };
        }
    }

    private string ConstructPrompt(
        string studentName,
        object? studentJsonData = null,
        object? fPTUAcademicResourceJsonData = null,
        string? message = null
    )
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