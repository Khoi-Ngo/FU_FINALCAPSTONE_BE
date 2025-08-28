using System.Collections.Concurrent;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.JoinedSubject;
using AISEA.ApiService.SHARED.DTOs.Requests.Noti;
using AISEA.ApiService.SHARED.DTOs.Responses.JoinedSubject;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AISEA.ApiService.BAL.Services.CourseTracker;

public class JoinedSubjectService
{
    private readonly UserRepository _userRepository;
    private readonly JoinedSubjectRepository _joinedSubjectRepository;
    private readonly IMapper _mapper;
    private readonly IJWTService _jWTService;
    private readonly SubjectRepository _subjectRepository;
    private readonly ILogger<JoinedSubjectService> _logger;

    public JoinedSubjectService(UserRepository userRepository, JoinedSubjectRepository joinedSubjectRepository, IMapper mapper, IJWTService jWTService, SubjectRepository subjectRepository, ILogger<JoinedSubjectService> logger)
    {
        _userRepository = userRepository;
        _joinedSubjectRepository = joinedSubjectRepository;
        _mapper = mapper;
        _jWTService = jWTService;
        _subjectRepository = subjectRepository;
        _logger = logger;
    }



    #region  IMPORT JOINED SUBJECT
    public async Task<List<(long StakeholderUserId, NotificationDTO stakeHodlerNoti, bool isSuccess)>> ImportMultipleSubjectsAsync(ImportJoinedSubjectsForOneStudentRequest request, string accessToken)
    {

        var notiList = new List<(long StakeholderUserId, NotificationDTO stakeHodlerNoti, bool isSuccess)>();

        //convert to HashSet of single import request
        foreach (var item in request.SubjectsData)
        {
            var singleImportRequest = new SingleImportJoinedSubjectRequest
            {
                StudentUserName = request.StudentUserName,
                SubjectCode = item.SubjectCode,
                SubjectVersionCode = item.SubjectVersionCode,
                SemesterId = item.SemesterId,
                SemesterStudyBlockType = item.SemesterStudyBlockType
            };

            var (stakeHolderNoti, stakeHolderUserId, isSuccess) = await ImportSubjectAsync(singleImportRequest, accessToken);
            notiList.Add((stakeHolderUserId, stakeHolderNoti, isSuccess));
        }

        return notiList;

    }

    public async Task<(NotificationDTO stakeHolderNoti, long StakeholderUserId, bool isSuccess)> ImportSubjectAsync(SingleImportJoinedSubjectRequest request, string accessToken)
    {

        //init fail noti with unidentified exception
        var failNoti = new NotificationDTO
        {
            Title = $"Fail import {request.StudentUserName} with {request.SubjectCode}",
            Content = "Unidentified error while importing subject"
        };

        var conductorUserName = _jWTService.GetUsernameFromToken(accessToken);
        var conductorUserId = _jWTService.GetUserIdFromToken(accessToken);


        //get the student profile id from request.student user name
        var studentUser = await _userRepository.GetUserWStudentProfileAsync(request.StudentUserName);
        if (studentUser is null)
        {
            failNoti.Content = "Student user not found";
            return (failNoti, conductorUserId, false);
        }

        var studentProfile = studentUser.StudentProfile;
        if (studentProfile is null)
        {
            failNoti.Content = "Student profile not found";
            return (failNoti, conductorUserId, false);
        }

        var passedAndCompletedSubjectCodes = await _joinedSubjectRepository.GetAllPassedSubjectCodesAsync(studentProfile.Id);

        try
        {

            //Check existed subject code
            var importableSubjectDTO = await _subjectRepository.GetImportableByCodeAsync(request.SubjectCode);

            if (importableSubjectDTO is null || importableSubjectDTO.Versions.IsNullOrEmpty())
            {
                failNoti.Content = "There is no valid subject code - version";
                return (failNoti, conductorUserId, false);
            }


            var curriculumOfStudent = studentProfile.CurriculumCode;
            var comboCodeOfStudent = studentProfile.RegisteredComboCode;


            //check fit curriculum
            if (!importableSubjectDTO.CurriculumCodes.Contains(curriculumOfStudent))
            {
                failNoti.Content = "The curriculum is not valid";
                return (failNoti, conductorUserId, false);
            }

            //check fit combo
            if (!importableSubjectDTO.ComboNames.IsNullOrEmpty()
            && !importableSubjectDTO.ComboNames.Contains(comboCodeOfStudent))
            {
                failNoti.Content = "The combo is not valid";
                return (failNoti, conductorUserId, false);
            }

            //check prerequisites met
            if (!importableSubjectDTO.PrerequisiteSubjectCodes.IsNullOrEmpty()
            && !HasMetPrerequisites(passedAndCompletedSubjectCodes, importableSubjectDTO.PrerequisiteSubjectCodes))
            {
                failNoti.Content = "The prerequisites is not met yet";
                return (failNoti, conductorUserId, false);
            }







            await _joinedSubjectRepository.
            CreateAsync(MapToJoinedSubject(request, studentUser.StudentProfile.Id, conductorUserName, importableSubjectDTO.SubjectName, importableSubjectDTO.Credits, importableSubjectDTO.Description));

            return (new NotificationDTO
            {
                Content = $"You have been successfully enrolled in the subject: {request.SubjectCode}.",
                Title = "Subject Enrollment Notification",
            }, studentUser.Id, true);

        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                failNoti.Content = HandleMeetingSqlExceptionImport(sqlEx);
                return (failNoti, conductorUserId, false);
            }
        }
        catch (SqlException ex)
        {
            failNoti.Content = HandleMeetingSqlExceptionImport(ex);
            return (failNoti, conductorUserId, false);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while importing joined subject");

        }

        return (failNoti, conductorUserId, false);

    }


    public async Task<List<(long StakeholderUserId, NotificationDTO stakeHodlerNoti, bool isSuccess)>>
    ImportMultipleSubjectsAsync(ImportJoinedSubjectsRequest request, string accessToken)
    {
        var results = new ConcurrentBag<(long StakeholderUserId, NotificationDTO stakeHodlerNoti, bool isSuccess)>();

        var allRequests = request.UserNameToSubjectsMap
            .SelectMany(kvp => kvp.Value.Select(item => new
            {
                UserName = kvp.Key,
                Item = item
            }))
            .ToList();

        await Parallel.ForEachAsync(allRequests, new ParallelOptions
        {
            MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2)

        },
        async (entry, token) =>
        {
            var singleImportRequest = new SingleImportJoinedSubjectRequest
            {
                StudentUserName = entry.UserName,
                SubjectCode = entry.Item.SubjectCode,
                SubjectVersionCode = entry.Item.SubjectVersionCode,
                SemesterId = entry.Item.SemesterId,
                SemesterStudyBlockType = entry.Item.SemesterStudyBlockType
            };

            var (stakeHolderNoti, stakeHolderUserId, isSuccess) =
                await ImportSubjectAsync(singleImportRequest, accessToken);

            results.Add((stakeHolderUserId, stakeHolderNoti, isSuccess));
        });

        return results.ToList();
    }


    #endregion

    #region Private methods


    private string HandleMeetingSqlExceptionImport(SqlException ex)
    {
        switch (ex.Number)
        {

            case 2627: // Unique constraint violation
            case 2601: // Duplicated key row error
                if (ex.Message.Contains("UX_JoinedSubject_Student_Semester_BlockType_Subject"))
                {
                    return "Import Exception, Duplicate pair Block Type - Subject Code in the same semester. " + ex.Message;
                }
                // Handle other unique constraint violations by checking their names here...
                break;
            case 50021:
                return "Student cannot have more than 2 times imported for the same subject in the same semester";

            case 547:
                return "Invalid joined subject data. Please check Profile Data and Semester Data." + ex.Message;


        }
        return "Unidentified the error, maybe the subject is on progress of development";
    }



    private JoinedSubject MapToJoinedSubject(SingleImportJoinedSubjectRequest request
    , long studentProfileId
    , string createdByUserName
    , string subjectName
    , int credits
    , string description)
    {
        return new JoinedSubject
        {
            StudentProfileId = studentProfileId,

            SubjectCode = request.SubjectCode,
            SubjectVersionCode = request.SubjectVersionCode,
            SemesterId = request.SemesterId,
            SemesterStudyBlockType = request.SemesterStudyBlockType,
            Name = $"{request.SubjectCode}{request.SemesterStudyBlockType.ToString()} + {subjectName}",
            Credits = credits,
            CreatedByUserName = createdByUserName,
            SubjectDescription = description
        };
    }


    private bool IsValidAccessView(string accessToken, JoinedSubject joinedSubject)
    {
        if (_jWTService.GetRoleIdFromToken(accessToken) == (int)EUserRole.STUDENT)
        {
            return joinedSubject.StudentProfileId == _jWTService.GetProfileIdFromToken(accessToken);
        }
        return true;
    }

    private bool HasMetPrerequisites(IEnumerable<string> studentPassedSubjects, IEnumerable<string> prerequisites)
    {
        return !prerequisites.Except(studentPassedSubjects).Any();
    }


    #endregion

    #region Response

    public async Task<List<JoinedSubjectResponse>> GetAllBySelfAsync(string accessToken)
    {
        var res = await _joinedSubjectRepository.GetAllActiveByStudentProfileIDWithSemesteDataAsync(_jWTService.GetProfileIdFromToken(accessToken));
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

    #region DELETE || DEACTIVATE
    public async Task<(NotificationDTO stakeHolderNoti, long stakeHolderUserId)> DeleteSubjectAsync(long id)
    {
        var joinedSubject = await _joinedSubjectRepository.GetByIdWStudentProfileAsync(id);

        //TODO: Check prerequisites + check wether the student having coursegrade or not




        _joinedSubjectRepository.RemoveAsync(joinedSubject);

        return (new NotificationDTO
        {
            Content = $"You have been removed from the subject: {joinedSubject.SubjectCode}.",
            Title = "Subject Removal Notification"
        }, joinedSubject.StudentProfile.UserId);

    }

    public async Task RemoveAllNonUseAsync()
    {
        await _joinedSubjectRepository.RemoveAllNonUseAsync();
    }

    public async Task DeActivateNonUseJoinedSubjectAsync(StudentProfile studentProfile)
    {
        //get all joined subject basing on the student profile
        var joinedSubjects = await _joinedSubjectRepository.GetAllByStudentProfileIDNoSemesterAsync(studentProfile.Id);

        //get all subject code via curriculum code and combo -> Combo must be in the curriculum
        var subjectsByCur = await _subjectRepository.GetAllViaCurriculumAsync(studentProfile.CurriculumCode);

        //check + deactivate the subject not in curriculum & combo
        foreach (var joinSubject in joinedSubjects)
        {
            var check = subjectsByCur.Find(s => s.SubjectCode == joinSubject.SubjectCode);
            joinSubject.IsActive = true;
            if (check is null)
            {
                //deactivate the joined subject
                joinSubject.IsActive = false;
                continue;
            }
            if (!check.Combos.IsNullOrEmpty()) // subject has combos
            {
                // if student's combo is not included -> deactivate
                if (!check.Combos.Contains(studentProfile.RegisteredComboCode))
                {
                    joinSubject.IsActive = false;
                    continue;
                }
            }

        }

        //bulk update
        await _joinedSubjectRepository.BulkUpdateAsync(joinedSubjects);

        Console.WriteLine("DEACTIVATE SUBJECT AFTER UPDATE CURRICULUM OR COMBO OK");
    }

    #endregion
}