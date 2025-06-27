using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;

namespace AISEA.ApiService.BAL.Services.Chat
{
    public class AdvisorySession1to1Service
    {
        private readonly AdvisorySession1to1Repository _advisorySession1To1Repository;
        private readonly IJWTService _jWTService;
        private readonly UserRepository _userRepository;
        private readonly IRedisRepository _redisRepository;
        private readonly ChatSessionSettings _chatSessionSettings;
        private readonly StaffUserSettings _staffUserSettings;
        public AdvisorySession1to1Service
        (AdvisorySession1to1Repository advisorySession1To1Repository,
        IJWTService jWTService,
        UserRepository userRepository,
        IRedisRepository redisRepository,
        ChatSessionSettings chatSessionSettings,
        StaffUserSettings staffUserSettings)
        {
            _advisorySession1To1Repository = advisorySession1To1Repository;
            _jWTService = jWTService;
            _userRepository = userRepository;
            _redisRepository = redisRepository;
            _chatSessionSettings = chatSessionSettings;
            _staffUserSettings = staffUserSettings;

        }

        public async Task DeleteAsync(long chatSessionId, string accessToken)
        {
            var username = _jWTService.GetUsernameFromToken(accessToken);
            var roleId = _jWTService.GetUserRoleIdFromToken(accessToken);
            long profileId = roleId == (long)EUserRole.STUDENT ?
             await _userRepository.GetStudentProfileIdByUsernameAsync(username) :
             await _userRepository.GetStaffProfileIdByUsernameAsync(username);

            var session = await _advisorySession1To1Repository.GetByIdAsync(chatSessionId, profileId);
            if (session is null)
            {
                throw new NotFoundException("No permission");
            }

            await _advisorySession1To1Repository.RemoveAsync(session);
        }

        //get the user w profile caching || from the database
        public async Task<DAL.Entities.User> ValidateAndGetSenderAsync(string accessToken)
        {

            var username = _jWTService.GetUsernameFromToken(accessToken);
            var cacheKey = $"{_chatSessionSettings.SenderCachePrefix}{username}";


            // Try to get from Redis
            var cachedUser = await _redisRepository.GetValueAsync<DAL.Entities.User>(cacheKey);

            if (cachedUser is not null)
            {
                return cachedUser;
            }

            var user = await _userRepository.GetUserWProfileAsync(username);

            if (user is null)
            {
                throw new InvalidAccessSession("Invalid user");
            }

            if ((user.StaffProfile is null) && (user.StudentProfile is null))
            {
                throw new InvalidAccessSession("Invalid profile");
            }

            await _redisRepository.SetValueAsync(cacheKey, user, TimeSpan.FromHours(_chatSessionSettings.SenderCacheExpiryHrs));
            return user;
        }


        //Get the session caching || from the database
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

        //Create session then save into cache

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
            // Cache new session
            await _redisRepository
            .SetValueAsync
            ($"{_chatSessionSettings.SessionCachePrefix}{newSession.Id}", newSession, TimeSpan.FromDays(_chatSessionSettings.SessionCacheExpiryDays));
            return newSession;

        }

        public async Task<AdvisorySession1to1> GetByIdAsync(long chatSessionId, string accessToken)
        {
            var user = await ValidateAndGetSenderAsync(accessToken);
            var profileId = user.RoleId == (long)EUserRole.STUDENT ? user.StudentProfile.Id : user.StaffProfile.Id;

            return await GetByIdAsync(chatSessionId, profileId);
        }

        private bool IsValidAccessSession(AdvisorySession1to1 session, long profileId)
         => (session is not null &&
         ((session.StudentId == profileId)
        || (session.StaffId == profileId && profileId != _staffUserSettings.SystemBotUser.StaffId)
         )
         );


    }
}
