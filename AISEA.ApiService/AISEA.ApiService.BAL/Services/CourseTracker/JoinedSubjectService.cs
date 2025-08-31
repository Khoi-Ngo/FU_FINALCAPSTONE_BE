using System.Collections.Concurrent;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.JoinedSubject;
using AISEA.ApiService.SHARED.DTOs.Requests.Noti;
using AISEA.ApiService.SHARED.DTOs.Responses.JoinedSubject;
using AISEA.ApiService.SHARED.DTOs.Responses.Subject;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
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
    private readonly StudentProfileRepository _studentProfileRepository;
    private readonly IMapper _mapper;
    private readonly IJWTService _jWTService;
    private readonly SubjectRepository _subjectRepository;
    private readonly ILogger<JoinedSubjectService> _logger;

    public JoinedSubjectService(UserRepository userRepository
    , JoinedSubjectRepository joinedSubjectRepository
    , IMapper mapper
    , IJWTService jWTService
    , SubjectRepository subjectRepository
    , ILogger<JoinedSubjectService> logger
    , StudentProfileRepository studentProfileRepository)
    {
        _userRepository = userRepository;
        _joinedSubjectRepository = joinedSubjectRepository;
        _mapper = mapper;
        _jWTService = jWTService;
        _subjectRepository = subjectRepository;
        _logger = logger;
        _studentProfileRepository = studentProfileRepository;
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
            var importableSubjectDTO = await _subjectRepository.GetSubjectWCurNComNPreNVerAsync(request.SubjectCode);

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

    public async Task<JoinedSubjectSyllabusResponse> GetJoinedSubjectSyllabusAsync(long joinedSubjectId, string accessToken)
    {
        var studentProfileId = _jWTService.GetProfileIdFromToken(accessToken);

        var (joinedSubject, syllabusId) = await _joinedSubjectRepository.GetJoinedSubjectWithSyllabusIdAsync(joinedSubjectId, studentProfileId);

        if (joinedSubject == null)
        {
            throw new NotFoundException("Joined subject not found or you don't have access to it.");
        }

        // Get subject name
        var subject = await _subjectRepository.GetByCodeAsync(joinedSubject.SubjectCode);
        var subjectName = subject?.SubjectName ?? "Unknown Subject";

        return new JoinedSubjectSyllabusResponse
        {
            JoinedSubjectId = joinedSubject.Id,
            SubjectCode = joinedSubject.SubjectCode,
            SubjectVersionCode = joinedSubject.SubjectVersionCode,
            SubjectName = subjectName,
            SyllabusId = syllabusId,
            HasSyllabus = syllabusId.HasValue,
            Message = syllabusId.HasValue
                ? "Syllabus found successfully"
                : "No syllabus available for this subject version"
        };
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
    public async Task<(NotificationDTO stakeHolderNoti, long stakeHolderUserId)> DeleteSubjectAsync(long id, string accessToken)
    {
        var conductorUserId = _jWTService.GetUserIdFromToken(accessToken);
        var (removedJoinedSubject, otherJoinedSubjects) = await _joinedSubjectRepository.GetByIdToRemoveAsync(id);

        if (!removedJoinedSubject.SubjectMarkReports.IsNullOrEmpty())
        {
            return (new NotificationDTO
            {
                Content = $"Cannot remove the subject: {removedJoinedSubject.SubjectCode}. The joined subject already has mark reports",
                Title = "Subject Removal ERROR"
            }, conductorUserId);
        }

        try
        {
            //the student is only assigned this subject code once -> constraint
            if (otherJoinedSubjects.FirstOrDefault(js => js.SubjectCode == removedJoinedSubject.SubjectCode) is null)
            {
                //filter prerequisites
                foreach (var otherJs in otherJoinedSubjects)
                {
                    //query the prerequisite subject codes of each other JS
                    var checkSubject = await _subjectRepository.GetSubjectWCurNComNPreNVerAsync(otherJs.SubjectCode);

                    if (!checkSubject.PrerequisiteSubjectCodes.IsNullOrEmpty() && checkSubject.PrerequisiteSubjectCodes.Contains(removedJoinedSubject.SubjectCode))
                    {
                        return (new NotificationDTO
                        {
                            Content = $"Cannot remove the subject: {removedJoinedSubject.SubjectCode}. Prerequisites exception",
                            Title = "Subject Removal ERROR"
                        }, conductorUserId);
                    }
                }

            }

            await _joinedSubjectRepository.RemoveAsync(removedJoinedSubject);
            return (new NotificationDTO
            {
                Content = $"You have been removed from the subject: {removedJoinedSubject.SubjectCode}.",
                Title = "Subject Removal Notification"
            }, removedJoinedSubject.StudentProfile.UserId);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return (new NotificationDTO
            {
                Content = $"Cannot remove the subject: {removedJoinedSubject.SubjectCode}. Undefined Error",
                Title = "Subject Removal ERROR"
            }, conductorUserId);
        }
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


    public async Task<List<SimpleSubjectResponse>> ViewPersonalCurriculumSubjectAsync(string accessToken)
    {
        var studentProfileId = _jWTService.GetProfileIdFromToken(accessToken);
        var studentProfile = await _studentProfileRepository.GetByIdAsync(studentProfileId);
        var studentCurriculumCode = studentProfile.CurriculumCode;

        var subjects = await _subjectRepository.GetAllViaCurriculumNotIncludeComboAsync(studentCurriculumCode);

        return subjects;
    }

    public async Task<List<SimpleSubjectResponse>> ViewPersonalComboSubjectAsync(string accessToken)
    {
        var studentProfileId = _jWTService.GetProfileIdFromToken(accessToken);
        var studentProfile = await _studentProfileRepository.GetByIdAsync(studentProfileId);
        var studentComboName = studentProfile.RegisteredComboCode;

        var subjects = await _subjectRepository.GetAllViaComboNameAsync(studentComboName);

        return subjects;
    }

    public async Task<object> GetMapJoinedSubjectStatusByStudentProfileIDAsync(long studentProfileID)
    {
        return await _joinedSubjectRepository.GetMapJoinedSubjectStatusByStudentProfileIDAsync(studentProfileID);
    }
}