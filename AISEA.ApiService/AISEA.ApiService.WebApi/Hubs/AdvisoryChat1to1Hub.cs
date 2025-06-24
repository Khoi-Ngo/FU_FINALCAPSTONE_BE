using AISEA.ApiService.BAL.Services.Chat;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AISEA.ApiService.WebApi.Hubs
{
    //TODO: Recheck hardcode value
    [Authorize]
    public class AdvisoryChat1to1Hub : BaseHub
    {
        private readonly ChatService _chatService;
        private readonly UserRepository _userRepository;
        private readonly AdvisorySession1to1Repository _sessionRepository;
        private readonly IJWTService _jwtService;

        public AdvisoryChat1to1Hub(EndpointSettings endpointSettings,
              ChatService chatService,
            UserRepository userRepository,
            AdvisorySession1to1Repository sessionRepository,
            IJWTService jwtService
        ) : base(endpointSettings)
        {
            _chatService = chatService;
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _jwtService = jwtService;
        }



        /// <summary>
        /// Staff or Student sends a message in an AdvisoryChatSession1to1.
        /// If no session exists, the first message from a Student creates a new session.
        /// The message is saved to the database and broadcast to the session group.
        /// </summary>
        [PermissionAuthorize((int)EUserRole.ADVISOR, (int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.MANAGER, (int)EUserRole.STUDENT)]
        public async Task SendMessage(long sessionId, string content)
        {
            var username = _jwtService.GetUsernameFromToken(AccessToken);
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user is null)
            {
                throw new HubException("User not found.");
            }

            AdvisorySession1to1 session;
            long profileId = user.RoleId == (int)EUserRole.STUDENT
                ? await _userRepository.GetStudentProfileIdByUsernameAsync(username)
                : await _userRepository.GetStaffProfileIdByUsernameAsync(username);

            if (sessionId == 0 && user.RoleId == (int)EUserRole.STUDENT)
            {
                // Create a new session if none exists and the sender is a student
                var studentProfile = await _userRepository.GetUserByUsernameWStudentProfileAsync(username);
                if (studentProfile?.StudentProfile is null)
                {
                    throw new HubException("Student profile not found.");
                }

                session = new AdvisorySession1to1
                {
                    Title = $"Advisory Session for {username}",
                    StudentId = studentProfile.StudentProfile.Id,
                    StaffId = 1, // No staff assigned yet
                    Type = EAdvisorySession1to1Type.HUMAN,
                    CreatedAt = DateTime.UtcNow
                };
                await _sessionRepository.CreateAsync(session);
            }
            else
            {
                // Retrieve existing session
                session = await _sessionRepository.GetByIdAsync(sessionId, profileId);
                if (session is null)
                {
                    throw new HubException("Session not found or access denied.");
                }
            }

            // Create and save the message
            var message = new Message
            {
                Content = content,
                SenderId = user.Id,
                AdvisorySession1to1Id = session.Id,
                CreatedAt = DateTime.UtcNow
            };
            await _chatService.SaveMessageAsync(message);

            // Broadcast the message to the session group
            await Clients.Group($"Session_{session.Id}")
                .SendAsync("ReceiveMessage", new
                {
                    MessageId = message.Id,
                    SessionId = session.Id,
                    SenderUsername = username,
                    Content = message.Content,
                    SentAt = message.CreatedAt
                });
        }

        /// <summary>
        /// Staff or Student joins an AdvisoryChatSession1to1.
        /// If a staff member joins an open session (StaffId <= 0), they are assigned to it.
        /// The user is added to the session's SignalR group for real-time updates.
        /// </summary>
        [PermissionAuthorize((int)EUserRole.ADVISOR, (int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.MANAGER, (int)EUserRole.STUDENT)]
        public async Task JoinSession(long sessionId)
        {
            var username = _jwtService.GetUsernameFromToken(AccessToken);
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user is null)
            {
                throw new HubException("User not found.");
            }

            long profileId = user.RoleId == (int)EUserRole.STUDENT
                ? await _userRepository.GetStudentProfileIdByUsernameAsync(username)
                : await _userRepository.GetStaffProfileIdByUsernameAsync(username);

            var session = await _sessionRepository.GetByIdAsync(sessionId, profileId);
            if (session is null)
            {
                throw new HubException("Session not found or access denied.");
            }

            // If the session is open and the user is a staff member, assign them to the session
            if (session.StaffId <= 0 && user.RoleId != (int)EUserRole.STUDENT)
            {
                session.StaffId = profileId;
                session.UpdatedAt = DateTime.UtcNow;
                await _sessionRepository.UpdateAsync(session);

                // Notify the student that a staff member has joined
                await Clients.Group($"Session_{session.Id}")
                    .SendAsync("StaffJoined", new
                    {
                        SessionId = session.Id,
                        StaffUsername = username
                    });
            }

            // Add the user to the SignalR group for this session
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Session_{session.Id}");

            // Send session details to the joining user
            var messages = await _sessionRepository.GetWMessagesByIdAsync(sessionId, profileId);
            await Clients.Caller.SendAsync("SessionJoined", new
            {
                SessionId = session.Id,
                Title = session.Title,
                Type = session.Type,
                Messages = messages?.Messages.Select(m => new
                {
                    MessageId = m.Id,
                    SenderUsername = m.Sender.Username,
                    Content = m.Content,
                    SentAt = m.CreatedAt
                })
            });
        }

    }
}