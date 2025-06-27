using System.Text.Json;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.Const.Values;
using AISEA.ApiService.SHARED.DTOs.Requests.ChatBot;
using AISEA.ApiService.SHARED.DTOs.Responses.ChatBot;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.SHARED.Util;

namespace AISEA.ApiService.BAL.Services.Chat;

public class ChatBotService
{
    private readonly IChatOpenAIService _chatOpenAIService;
    private readonly MessageService _messageService;
    private readonly AdvisorySession1to1Service _advisorySession1To1Service;
    private readonly StaffUserSettings _staffUserSettings;

    public ChatBotService(
        IChatOpenAIService chatOpenAIService,
        AdvisorySession1to1Service advisorySession1To1Service,
        StaffUserSettings staffUserSettings,
        MessageService messageService)
    {
        _chatOpenAIService = chatOpenAIService;
        _advisorySession1To1Service = advisorySession1To1Service;
        _staffUserSettings = staffUserSettings;
        _messageService = messageService;
    }

    public async Task<GetChatBotResponse> SendMsgAsync(SendChatBotRequest request, string accessToken)
    {
        var student = await _advisorySession1To1Service.ValidateAndGetSenderAsync(accessToken);
        var chatSession = await _advisorySession1To1Service.GetByIdAsync(request.ChatSessionId, student.StudentProfile.Id);

        await _messageService.CreateMessageAsync(request.Message, student.Id, chatSession.Id);


        var prompt = ConstructPrompt(
            $"{student.FirstName} {student.LastName}",
            null, // Replace with actual studentAcademicPerformanceJsonData
            null, // Replace with actual FPTUAcademicResourceJsonData
            request.Message
        );
        var aiResponse = await _chatOpenAIService.SendMsgAsync(prompt);

        await _messageService.CreateMessageAsync(aiResponse, _staffUserSettings.SystemBotUser.Id, chatSession.Id);


        return new GetChatBotResponse
        {
            Message = aiResponse
        };
    }

    public async Task<InitChatBotResponse> InitMsgAsync(InitChatBotRequest request, string accessToken)
    {
        var student = await _advisorySession1To1Service.ValidateAndGetSenderAsync(accessToken);
        var title = Advisory1to1Util.GenerateChatBotSessionTitle(request.Message);

        var chatSession = await _advisorySession1To1Service.CreateSessionAsync(student.StudentProfile.Id, EAdvisorySession1to1Type.BOT, _staffUserSettings.SystemBotUser.StaffId, title);
        await _messageService.CreateMessageAsync(request.Message, student.Id, chatSession.Id);

        var prompt = ConstructPrompt(
             $"{student.FirstName} {student.LastName}",
             null, // Replace with actual studentAcademicPerformanceJsonData
             null, // Replace with actual FPTUAcademicResourceJsonData
             request.Message
         );

        var aiResponse = await _chatOpenAIService.SendMsgAsync(prompt);
        await _messageService.CreateMessageAsync(aiResponse, _staffUserSettings.SystemBotUser.Id, chatSession.Id);

        return new InitChatBotResponse
        {
            Message = aiResponse,
            ChatSessionId = chatSession.Id
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