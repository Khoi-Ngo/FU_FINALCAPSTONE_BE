using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.JoinedSubject;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.JoinedSubject;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
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

    public async Task<JoinedSubjectStakeholderNotification> DeleteSubjectAsync(long id, string accessToken)
    {
        try
        {
            var subject = await _joinedSubjectRepository.GetByIdWStudentUserIdAsync(id);

            _joinedSubjectRepository.RemoveAsync(subject);

            return new JoinedSubjectStakeholderNotification
            {
                StakeholderUserId = subject.StudentProfile.UserId,
                Content = $"You have been removed from the subject: {subject.SubjectCode}.",
                Title = "Subject Removal Notification"
            };
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



    public async Task<JoinedSubjectStakeholderNotification> ImportMultipleSubjectsAsync(ImportJoinedSubjectsForOneStudentRequest request, string accessToken)
    {
        try
        {
            //get the student profile id from request.student user name
            var studentUser = await _userRepository.GetUserWStudentProfileAsync(request.StudentUserName);
            await _joinedSubjectRepository.BulkInsertAsync(MapToJoinedSubjects(request.SubjectsData, studentUser.StudentProfile.Id, _jWTService.GetUsernameFromToken(accessToken)));

            return new JoinedSubjectStakeholderNotification
            {
                StakeholderUserId = studentUser.Id,
                Content = $"You have been successfully enrolled in the subjects: {string.Join(", ", request.SubjectsData.Select(s => s.SubjectCode))}.",
                Title = "Subject Enrollment Notification",
            };

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

    public async Task<List<JoinedSubjectStakeholderNotification>> ImportMultipleSubjectsAsync(
        ImportJoinedSubjectsRequest request,
        string accessToken)
    {
        //TODO: Split the importing of each student
        try
        {
            var createdByUserName = _jWTService.GetUsernameFromToken(accessToken);

            // 1️⃣ Load all required students in a single query
            var studentUsers = await _userRepository
                .GetUsersWStudentProfilesAsync(request.UserNameToSubjectsMap.Keys.ToList());

            var studentUserDict = studentUsers.ToDictionary(u => u.Username, u => u);

            var allJoinedSubjects = new List<JoinedSubject>(
                request.UserNameToSubjectsMap.Sum(kvp => kvp.Value.Count)
            );
            var notifications = new List<JoinedSubjectStakeholderNotification>(studentUsers.Count);

            // 2️⃣ Loop through each student and prepare subjects + notification
            foreach (var kvp in request.UserNameToSubjectsMap)
            {
                if (!studentUserDict.TryGetValue(kvp.Key, out var studentUser))
                    continue; // Skip if student not found

                var joinedSubjects = MapToJoinedSubjects(kvp.Value, studentUser.StudentProfile.Id, createdByUserName);
                allJoinedSubjects.AddRange(joinedSubjects);

                // One notification per student
                notifications.Add(new JoinedSubjectStakeholderNotification
                {
                    StakeholderUserId = studentUser.Id,
                    Content = $"You have been successfully enrolled in the subjects: {string.Join(", ", kvp.Value.Select(s => s.SubjectCode))}.",
                    Title = "Subject Enrollment Notification"
                });
            }

            // 3️⃣ Bulk insert all subjects in one DB call
            await _joinedSubjectRepository.BulkInsertAsync(allJoinedSubjects);

            return notifications;
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
        {
            HandleMeetingSqlException(sqlEx);
            throw;
        }
        catch (SqlException ex)
        {
            HandleMeetingSqlException(ex);
            throw;
        }
    }



    public async Task<JoinedSubjectStakeholderNotification> ImportSubjectAsync(SingleImportJoinedSubjectRequest request, string accessToken)
    {
        try
        {
            //get the student profile id from request.student user name
            var studentUser = await _userRepository.GetUserWStudentProfileAsync(request.StudentUserName);
            await _joinedSubjectRepository.CreateAsync(MapToJoinedSubject(request, studentUser.StudentProfile.Id, _jWTService.GetUsernameFromToken(accessToken)));

            return new JoinedSubjectStakeholderNotification
            {
                StakeholderUserId = studentUser.Id,
                Content = $"You have been successfully enrolled in the subject: {request.SubjectCode}.",
                Title = "Subject Enrollment Notification",
            };

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
                throw new InvalidOperationException("Import Exception, Import-Prerequisite not met");
            case 50015:
                throw new InvalidOperationException("Import Exception, Invalid subject code");
            case 50016:
                throw new InvalidOperationException("Import Exception, Invalid subject version code");
            case 50017:
                throw new InvalidOperationException("Import Exception, Invalid combo code");
            case 50018:
                throw new InvalidOperationException("Import Exception, Invalid curriculum code");
            case 50019:
                throw new InvalidOperationException("Import Exception, Invalid semester name, the semester name must be existed");
            case 50020:
                throw new InvalidOperationException("Import Exception, Student must have not graduated");
            case 50021:
                throw new InvalidOperationException("Invalid subject name");
            #endregion

            #region Deleting 
            //TODO: Refine business logic clearly

            #endregion

            case 547:
                throw new InvalidOperationException("Invalid joined subject data. Ensure student profile(s) exist.");

        }
        throw ex;
    }


    private JoinedSubject MapToJoinedSubject(SingleImportJoinedSubjectRequest request, long studentProfileId, string createdByUserName)
    {
        var joinedSubject = _mapper.Map<JoinedSubject>(request);
        joinedSubject.StudentProfileId = studentProfileId;
        joinedSubject.CreatedByUserName = createdByUserName;
        joinedSubject.Name = $"{request.SubjectCode} ({request.SemesterStudyBlockType.ToString()})  {request.SubjectName}";
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
            Name = $"{subject.SubjectCode} ({subject.SemesterStudyBlockType.ToString()})  {subject.SubjectName}",
            CreatedAt = DateTime.Now,
            IsPassed = false,
            IsActive = true
        }).ToList();
    }


    #endregion


    #region Response

    public async Task<PagedResult<JoinedSubjectListItemResponse>> GetAllBySelfPagedAsync(PaginationRequest request, string accessToken)
    {
        var (joinedSubjects, totalCount) = await _joinedSubjectRepository.GetAllByStudentProfileIDPagedAsync(request.PageNumber, request.PageSize, _jWTService.GetProfileIdFromToken(accessToken));
        return new PagedResult<JoinedSubjectListItemResponse>
        {
            Items = _mapper.Map<List<JoinedSubjectListItemResponse>>(joinedSubjects),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PagedResult<JoinedSubjectListItemResponse>> GetAllBySelfLatestSemesterPagedAsync(PaginationRequest request, string accessToken)
    {
        var (joinedSubjects, totalCount) = await _joinedSubjectRepository.GetAllBySelfLatestSemesterPagedAsync(request.PageNumber, request.PageSize, _jWTService.GetProfileIdFromToken(accessToken));
        return new PagedResult<JoinedSubjectListItemResponse>
        {
            Items = _mapper.Map<List<JoinedSubjectListItemResponse>>(joinedSubjects),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PagedResult<JoinedSubjectListItemResponse>> GetAllByStudentProfileIdPagedAsync(PaginationRequest request, long studentProfileId)
    {
        var (joinedSubjects, totalCount) = await _joinedSubjectRepository.GetAllByStudentProfileIDPagedAsync(request.PageNumber, request.PageSize, studentProfileId);
        return new PagedResult<JoinedSubjectListItemResponse>
        {
            Items = _mapper.Map<List<JoinedSubjectListItemResponse>>(joinedSubjects),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }


    #endregion
}