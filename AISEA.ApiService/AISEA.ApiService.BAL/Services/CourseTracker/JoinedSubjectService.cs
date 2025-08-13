using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.JoinedSubject;
using AISEA.ApiService.SHARED.DTOs.Requests.Noti;
using AISEA.ApiService.SHARED.DTOs.Responses.JoinedSubject;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.BAL.Services.CourseTracker;

public class JoinedSubjectService
{
    private readonly UserRepository _userRepository;
    private readonly JoinedSubjectRepository _joinedSubjectRepository;
    private readonly IMapper _mapper;
    private readonly IJWTService _jWTService;

    public JoinedSubjectService(UserRepository userRepository, JoinedSubjectRepository joinedSubjectRepository, IMapper mapper, IJWTService jWTService)
    {
        _userRepository = userRepository;
        _joinedSubjectRepository = joinedSubjectRepository;
        _mapper = mapper;
        _jWTService = jWTService;
    }

    public async Task<(NotificationDTO stakeHolderNoti, long stakeHolderUserId)> DeleteSubjectAsync(long id)
    {
        try
        {
            var subject = await _joinedSubjectRepository.GetByIdWStudentProfileAsync(id);

            _joinedSubjectRepository.RemoveAsync(subject);

            return (new NotificationDTO
            {
                Content = $"You have been removed from the subject: {subject.SubjectCode}.",
                Title = "Subject Removal Notification"
            }, subject.StudentProfile.UserId);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                HandleMeetingSqlException(sqlEx);

            }
            throw;
        }
        catch (SqlException ex)
        {
            HandleMeetingSqlException(ex);
            throw;

        }
    }



    public async Task<(NotificationDTO stakeHodlerNoti, long StakeholderUserId)> ImportMultipleSubjectsAsync(ImportJoinedSubjectsForOneStudentRequest request, string accessToken)
    {
        try
        {
            //get the student profile id from request.student user name
            var studentUser = await _userRepository.GetUserWStudentProfileAsync(request.StudentUserName);
            await _joinedSubjectRepository.BulkInsertAsync(MapToJoinedSubjects(request.SubjectsData, studentUser.StudentProfile.Id, _jWTService.GetUsernameFromToken(accessToken)));

            return (new NotificationDTO
            {
                Content = $"You have been successfully enrolled in the subjects: {string.Join(", ", request.SubjectsData.Select(s => s.SubjectCode))}.",
                Title = "Subject Enrollment Notification",
            }, studentUser.Id);

        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                HandleMeetingSqlException(sqlEx);

            }
            throw;
        }
        catch (SqlException ex)
        {
            HandleMeetingSqlException(ex);
            throw;

        }
    }

    public async Task<List<(long stakeHolderUserId, NotificationDTO stakeHolderNoti)>> ImportMultipleSubjectsAsync(ImportJoinedSubjectsRequest request, string accessToken)
    {
        var createdByUserName = _jWTService.GetUsernameFromToken(accessToken);

        var studentUsers = await _userRepository
            .GetUsersWStudentProfilesAsync(request.UserNameToSubjectsMap.Keys.ToList());

        var studentUserDict = studentUsers.ToDictionary(u => u.Username, u => u);

        var notifications = new List<(long stakeHolderUserId, NotificationDTO stakeHolderNoti)>();

        foreach (var kvp in request.UserNameToSubjectsMap)
        {
            if (!studentUserDict.TryGetValue(kvp.Key, out var studentUser))
            {
                // If student not found, mark all subjects for this user as failed
                notifications.Add((
                 stakeHolderUserId: studentUser.Id,
                 stakeHolderNoti: new NotificationDTO
                 {
                     Title = "Subject Enrollment Failed",
                     Content = $"Failed to enroll in subject: {string.Join(", ", kvp.Value.Select(s => s.SubjectCode))}."
                 }
             ));
                continue;
            }

            var joinedSubjects = MapToJoinedSubjects(kvp.Value, studentUser.StudentProfile.Id, createdByUserName);

            try
            {
                await _joinedSubjectRepository.BulkInsertAsync(joinedSubjects);

                notifications.Add((
               stakeHolderUserId: studentUser.Id,
               stakeHolderNoti: new NotificationDTO
               {
                   Content = $"You have been successfully enrolled in the subjects: {string.Join(", ", kvp.Value.Select(s => s.SubjectCode))}.",
                   Title = "Subject Enrollment Notification"
               }
           ));
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {


                notifications.Add((
                stakeHolderUserId: studentUser.Id,
                stakeHolderNoti: new NotificationDTO
                {
                    Content = $"Failed to enroll in subject: {string.Join(", ", kvp.Value.Select(s => s.SubjectCode))}.",
                    Title = "Subject Enrollment Failed"
                }
            ));



                HandleMeetingSqlException(sqlEx);
            }
            catch (SqlException sqlEx)
            {
                notifications.Add((
                stakeHolderUserId: studentUser.Id,
                stakeHolderNoti: new NotificationDTO
                {
                    Content = $"Failed to enroll in subject: {string.Join(", ", kvp.Value.Select(s => s.SubjectCode))}.",
                    Title = "Subject Enrollment Failed"
                }
            ));

                HandleMeetingSqlException(sqlEx);
            }
        }

        return notifications;
    }

    public async Task<(NotificationDTO stakeHolderNoti, long StakeholderUserId)> ImportSubjectAsync(SingleImportJoinedSubjectRequest request, string accessToken)
    {
        try
        {
            //get the student profile id from request.student user name
            var studentUser = await _userRepository.GetUserWStudentProfileAsync(request.StudentUserName);
            await _joinedSubjectRepository.CreateAsync(MapToJoinedSubject(request, studentUser.StudentProfile.Id, _jWTService.GetUsernameFromToken(accessToken)));

            return (new NotificationDTO
            {
                Content = $"You have been successfully enrolled in the subject: {request.SubjectCode}.",
                Title = "Subject Enrollment Notification",
            }, studentUser.Id);

        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                HandleMeetingSqlException(sqlEx);

            }
            throw;
        }
        catch (SqlException ex)
        {
            HandleMeetingSqlException(ex);
            throw;

        }

    }


    #region Private methods

    private void HandleMeetingSqlException(SqlException ex)
    {
        switch (ex.Number)
        {
            #region Importing

            case 50013:
                throw new InvalidOperationException("Import Exception, Import-Prerequisite not met" + ex.Message);
            case 50015:
                throw new InvalidOperationException("Import Exception, Invalid subject code" + ex.Message);
            case 50016:
                throw new InvalidOperationException("Import Exception, Invalid subject version code of subject code" + ex.Message);
            case 50017:
                throw new InvalidOperationException("Import Exception, Invalid combo code of student" + ex.Message);
            case 50018:
                throw new InvalidOperationException("Import Exception, Invalid curriculum code of student" + ex.Message);
            case 50020:
                throw new InvalidOperationException("Import Exception, Student must have not graduated" + ex.Message);
            case 50021:
                throw new InvalidOperationException("Import Exception, More than 2 subject code in the same semester" + ex.Message);

            #endregion

            #region Deleting 

            case 50022:
                throw new InvalidOperationException("Delete Exception, Conflict Prerequisite" + ex.Message);
            case 50023:
                throw new InvalidOperationException("Delete Exception, The subject(s) having marks already" + ex.Message);

            #endregion

            case 547:
                throw new InvalidOperationException("Invalid joined subject data. Please check Profile Data and Semester Data." + ex.Message);

        }
        throw ex;
    }


    private JoinedSubject MapToJoinedSubject(SingleImportJoinedSubjectRequest request, long studentProfileId, string createdByUserName)
    {
        var joinedSubject = _mapper.Map<JoinedSubject>(request);
        joinedSubject.StudentProfileId = studentProfileId;
        joinedSubject.CreatedByUserName = createdByUserName;
        // joinedSubject.Name = $"{request.SubjectCode} ({request.SemesterStudyBlockType.ToString()})  {request.SubjectName}";
        return joinedSubject;
    }

    private List<JoinedSubject> MapToJoinedSubjects(
    HashSet<ImportJoinedSubjects_Data> subjectsData,
    long studentProfileId,
    string createdByUserName)
    {
        //using this will enhance performance instead of AutoMapper
        return subjectsData.Select(subject => new JoinedSubject
        {
            SubjectCode = subject.SubjectCode,
            SubjectVersionCode = subject.SubjectVersionCode,
            StudentProfileId = studentProfileId,
            CreatedByUserName = createdByUserName,
            // Name = $"{subject.SubjectCode} ({subject.SemesterStudyBlockType.ToString()})  {subject.SubjectName}",
            CreatedAt = DateTime.Now,
            IsPassed = false,
            IsActive = true
        }).ToList();
    }

    private bool IsValidAccessView(string accessToken, JoinedSubject joinedSubject)
    {
        if (_jWTService.GetRoleIdFromToken(accessToken) == (int)EUserRole.STUDENT)
        {
            return joinedSubject.StudentProfileId == _jWTService.GetProfileIdFromToken(accessToken);
        }
        return true;
    }

    #endregion


    #region Response

    public async Task<List<JoinedSubjectResponse>> GetAllBySelfAsync(string accessToken)
    {
        var res = await _joinedSubjectRepository.GetAllActiveByStudentProfileIDAsync(_jWTService.GetProfileIdFromToken(accessToken));
        return _mapper.Map<List<JoinedSubjectResponse>>(res);
    }

    public async Task<List<JoinedSubjectResponse>> GetAllBySelfLatestSemesterAsync(string accessToken)
    {
        var res = await _joinedSubjectRepository.GetAllActiveByStudentProfileIDLatestSemesAsync(_jWTService.GetProfileIdFromToken(accessToken));
        return _mapper.Map<List<JoinedSubjectResponse>>(res);
    }

    public async Task<List<JoinedSubjectResponse>> GetAllByStudentProfileIdAsync(long studentProfileId)
    {
        var res = await _joinedSubjectRepository.GetAllByStudentProfileIDAsync(studentProfileId);
        return _mapper.Map<List<JoinedSubjectResponse>>(res);
    }

    public async Task<JoinedSubjectResponse> GetByIdAsync(string accessToken, long id)
    {
        var res = await _joinedSubjectRepository.GetByIdAsync(id);
        if (!IsValidAccessView(accessToken, res)) throw new InvalidAccessJoinedSubject("You have no permission to access");
        return _mapper.Map<JoinedSubjectResponse>(res);

    }


    #endregion
}