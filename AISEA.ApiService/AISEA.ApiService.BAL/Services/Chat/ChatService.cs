using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Chat;
using AISEA.ApiService.SHARED.DTOs.Responses.Chat;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.SHARED.Util;

//BAL mainly for Human Advisory chat 1 to 1
namespace AISEA.ApiService.BAL.Services.Chat;

public class ChatService
{
    private readonly StaffUserSettings _staffUserSettings;
    private readonly MessageService _messageService;
    private readonly AdvisorySession1to1Service _advisorySession1To1Service;

    public ChatService(StaffUserSettings staffUserSettings, MessageService messageService, AdvisorySession1to1Service advisorySession1To1Service)
    {
        _staffUserSettings = staffUserSettings;
        _messageService = messageService;
        _advisorySession1To1Service = advisorySession1To1Service;
    }

    public async Task<InitHumanChatSessionResponse> InitHumanChatSessionAsync(InitHumanChatSessionRequest request, string accessToken)
    {
        var student = await _advisorySession1To1Service.ValidateAndGetSenderAsync(accessToken);
        var title = Advisory1to1Util.GenerateHumanSessionTitle(_staffUserSettings.EmptyStaffName);
        var newSession = await _advisorySession1To1Service.CreateSessionAsync(student.StudentProfile.Id, EAdvisorySession1to1Type.HUMAN, _staffUserSettings.EmptyStaffProfileId, title);
        await _messageService.CreateMessageAsync(request.Message, student.Id, newSession.Id);


        return new InitHumanChatSessionResponse
        {
            ChatSessionId = newSession.Id
        };
    }
}