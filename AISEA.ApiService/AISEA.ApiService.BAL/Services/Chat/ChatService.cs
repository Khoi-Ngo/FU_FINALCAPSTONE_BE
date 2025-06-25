using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Chat;
using AISEA.ApiService.SHARED.DTOs.Responses.Chat;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.SHARED.Util;

//BAL mainly for Human Advisory chat 1 to 1
namespace AISEA.ApiService.BAL.Services.Chat
{
    public class ChatService
    {
        private readonly MessageRepository _messageRepository;
        private readonly IJWTService _jWTService;
        private readonly UserRepository _userRepository;
        private readonly StaffUserSettings _staffUserSettings;

        private readonly AdvisorySession1to1Repository _advisorySession1To1Repository;
        public ChatService(MessageRepository messageRepository, IJWTService jWTService, UserRepository userRepository, StaffUserSettings staffUserSettings, AdvisorySession1to1Repository advisorySession1To1Repository)
        {
            _messageRepository = messageRepository;
            _jWTService = jWTService;
            _userRepository = userRepository;
            _staffUserSettings = staffUserSettings;
            _advisorySession1To1Repository = advisorySession1To1Repository;
        }

        public async Task<InitHumanChatSessioResponse> InitHumanChatSessionAsync(InitHumanChatSessioRequest request, string accessToken)
        {
            var student = await ValidateAndGetStudentAsync(accessToken);
            var newSession = await CreateSessionAsync(student.StudentProfile.Id);
            var studentMessage = CreateMessage(request.Message, student.Id, newSession.Id);

            await _messageRepository.CreateAsync(studentMessage);

            return new InitHumanChatSessioResponse
            {
                ChatSessionId = newSession.Id
            };
        }

        public async Task SaveMessageAsync(Message message)
        {
            await _messageRepository.CreateAsync(message);
        }

        private async Task<DAL.Entities.User> ValidateAndGetStudentAsync(string accessToken)
        {
            var studentUserName = _jWTService.GetUsernameFromToken(accessToken);
            var student = await _userRepository.GetUserByUsernameWStudentProfileAsync(studentUserName);
            if (student?.StudentProfile is null)
            {
                throw new InvalidAccessSession("Invalid student profile");
            }

            return student;
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

        private async Task<AdvisorySession1to1> CreateSessionAsync(long studentProfileId)
        {
            var title = Advisory1to1Util.GenerateHumanSessionTitle(_staffUserSettings.EmptyStaffName);
            var newSession = new AdvisorySession1to1
            {
                Title = title,
                StaffId = _staffUserSettings.EmptyStaffProfileId,
                Type = EAdvisorySession1to1Type.HUMAN,
                StudentId = studentProfileId
            };
            await _advisorySession1To1Repository.CreateAsync(newSession);
            return newSession;

        }
    }
}