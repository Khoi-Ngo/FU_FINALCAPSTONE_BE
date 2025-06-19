
using System.Text.Json;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Values;
using AISEA.ApiService.SHARED.DTOs.Requests.ChatBot;
using AISEA.ApiService.SHARED.DTOs.Responses.ChatBot;
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

    public ChatBotService(ILogger<ChatBotService> logger, IChatOpenAIService chatOpenAIService, ChatBotSettings chatBotSettings, IJWTService jWTService, UserRepository userRepository)
    {
        _logger = logger;
        _chatOpenAIService = chatOpenAIService;
        _chatBotSettings = chatBotSettings;
        _jWTService = jWTService;
        _userRepository = userRepository;
    }

    public async Task<ChatBotResponse> SendMsgAsync(SendChatBotRequest request, string accessToken)
    {
        try
        {
            var studentName = _jWTService.GetUsernameFromToken(accessToken);
            var user = await _userRepository.GetUserByUsernameAsync(studentName);
            //TODO: Apply query studentJsonData and FPTUAcademicResourceJsonData later
            var message = ConstructMessage(
                user.FirstName + " " + user.LastName,
                null, // Replace with actual studentJsonData when available
                null, // Replace with actual FPTUAcademicResourceJsonData when available
                request.Message
            );
            var res = await _chatOpenAIService.SendMsgAsync(message);
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

    private string ConstructMessage(
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