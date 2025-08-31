using AISEA.ApiService.BAL.Services.CourseTracker;
using AISEA.ApiService.BAL.Services.Notification;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.JoinedSubject;
using AISEA.ApiService.SHARED.DTOs.Requests.Noti;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.HubUtil;
using AISEA.ApiService.WebApi.InterceptorAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AISEA.ApiService.WebApi.Controllers.CourseTracker;

[ApiController]
[Route("api/[controller]")]
public class JoinedSubjectController : BaseController
{

    //NOTE: no need to get all because all Staff can get via Student User || Student Profile

    #region Init

    private readonly JoinedSubjectService _joinedSubjectService;
    private readonly IBackgroundTaskQueue _taskQueue;

    public JoinedSubjectController(EndpointSettings endpointSettings
    , JoinedSubjectService joinedSubjectService
    , IBackgroundTaskQueue taskQueue) : base(endpointSettings)
    {
        _joinedSubjectService = joinedSubjectService;
        _taskQueue = taskQueue;
    }


    #endregion



    ///<summary>
    /// Singly import a subject for a student
    /// 1 Student - 1 Joined subject
    /// </summary>
    [HttpPost("import")]
    [PermissionAuthorize((int)EUserRole.MANAGER, (int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.ADMIN)]
    [AuditLog(Tag = "IMPORT_SUBJECT")]
    public async Task<IActionResult> ImportSubjectAsync([FromBody] SingleImportJoinedSubjectRequest request)
    {
        var accessToken = AccessToken;

        _taskQueue.QueueBackgroundWorkItem(async (sp, token) =>
      {
          var qJoinedSubjectService = sp.GetRequiredService<JoinedSubjectService>();
          var (stakeHolderNoti, StakeholderUserId, isSuccess) = await qJoinedSubjectService.ImportSubjectAsync(request, accessToken);

          var qNotifier = sp.GetRequiredService<NotificationHubNotifier>();
          await qNotifier.NotifyUserAsync(StakeholderUserId, stakeHolderNoti);
      });

        return Ok("Import queued successful");
    }


    ///<summary>
    /// Import N joined subjects for ONE student
    /// 1 Student - N Joined Subjects
    /// </summary>
    [HttpPost("import-multiple")]
    [PermissionAuthorize((int)EUserRole.MANAGER, (int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.ADMIN)]
    [AuditLog(Tag = "BULK_IMPORT_SUBJECT")]
    public async Task<IActionResult> ImportMultipleSubjectsAsync([FromBody] ImportJoinedSubjectsForOneStudentRequest request)
    {
        var accessToken = AccessToken;


        _taskQueue.QueueBackgroundWorkItem(async (sp, token) =>
    {
        var qJoinedSubjectService = sp.GetRequiredService<JoinedSubjectService>();
        List<(long stakeHolderUserId, NotificationDTO stakeHolderNoti, bool isSuccess)> res
        = await qJoinedSubjectService.ImportMultipleSubjectsAsync(request, accessToken);


        var successList = res
            .Where(x => x.isSuccess)
            .Select(x => (UserId: x.stakeHolderUserId, Notification: x.stakeHolderNoti))
            .ToList();


        //notify only success notification

        var qNotifier = sp.GetRequiredService<NotificationHubNotifier>();
        await qNotifier.NotifyUsersAsync(successList);

        var failList = res
            .Where(x => !x.isSuccess)
            .Select(x => x.stakeHolderNoti)
            .ToList();

        if (failList.IsNullOrEmpty())
        {
            await qNotifier.NotifyUserAsync(AccessToken, new NotificationDTO { Title = "Fail import detected", Content = $"Fail import subjects detected, please check your email " });
            var qNotificationService = sp.GetRequiredService<NotificationService>();
            await qNotificationService.SendBulkNotificationDataAsMail(accessToken, failList);
        }

    });


        return Ok("Import queued successful");
    }


    ///<summary>
    /// Import N joined subjects for N student
    /// N Student - N Joined Subject
    /// </summary>
    [HttpPost("import-multiple-students")]
    [PermissionAuthorize((int)EUserRole.MANAGER, (int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.ADMIN)]
    [AuditLog(Tag = "BULK_IMPORT_SUBJECT")]
    public async Task<IActionResult> ImportMultipleStudentsAsync([FromBody] ImportJoinedSubjectsRequest request)
    {
        var accessToken = AccessToken;


        _taskQueue.QueueBackgroundWorkItem(async (sp, token) =>
        {
            var qJoinedSubjectService = sp.GetRequiredService<JoinedSubjectService>();
            List<(long stakeHolderUserId, NotificationDTO stakeHolderNoti, bool isSuccess)> res = await qJoinedSubjectService.ImportMultipleSubjectsAsync(request, accessToken);

            var successList = res
            .Where(x => x.isSuccess)
            .Select(x => (UserId: x.stakeHolderUserId, Notification: x.stakeHolderNoti))
            .ToList();


            //notify only success notification

            var qNotifier = sp.GetRequiredService<NotificationHubNotifier>();
            await qNotifier.NotifyUsersAsync(successList);

            var failList = res
                .Where(x => !x.isSuccess)
                .Select(x => x.stakeHolderNoti)
                .ToList();

            if (failList.IsNullOrEmpty())
            {
                await qNotifier.NotifyUserAsync(AccessToken, new NotificationDTO { Title = "Fail import detected", Content = $"Fail import subjects detected, please check your email " });
                var qNotificationService = sp.GetRequiredService<NotificationService>();
                await qNotificationService.SendBulkNotificationDataAsMail(accessToken, failList);
            }

        });


        return Ok("Import action has been queued");
    }


    ///<summary>
    /// Delete a joined subject for a student
    /// </summary>
    [HttpDelete("{id}")]
    [PermissionAuthorize((int)EUserRole.MANAGER, (int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.ADMIN)]
    [AuditLog(Tag = "DELETE_JOINED_SUBJECT")]
    public async Task<IActionResult> DeleteSubjectAsync(long id)
    {
        var accessToken = AccessToken;
        var removedJoinedSubjectId = id;

        _taskQueue.QueueBackgroundWorkItem(async (sp, token) =>
               {
                   var qJoinedSubjectService = sp.GetRequiredService<JoinedSubjectService>();
                   var qNotifier = sp.GetRequiredService<NotificationHubNotifier>();

                   var (stakeHolderNoti, stakeHolderUserId) = await qJoinedSubjectService.DeleteSubjectAsync(removedJoinedSubjectId, accessToken);

                   await qNotifier.NotifyUserAsync(stakeHolderUserId, stakeHolderNoti);
               });

        return Ok("Delete job registered successful");
    }


    ///<summary>
    /// The student view all data by him self
    /// </summary>
    [HttpGet("self")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    [AuditLog(Tag = "VIEW_JOINED_SUBJECT")]
    public async Task<IActionResult> GetAllBySelf()
    {
        var accessToken = AccessToken;

        var res = await _joinedSubjectService.GetAllBySelfAsync(accessToken);
        return Ok(res);
    }

    /// <summary>
    /// Get syllabus ID for a specific joined subject (Students only - own subjects)
    /// </summary>
    /// <param name="joinedSubjectId">The ID of the joined subject</param>
    /// <returns>Joined subject info with syllabus ID if available</returns>
    [HttpGet("{joinedSubjectId}/syllabus")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    [AuditLog(Tag = "VIEW_JOINED_SUBJECT_SYLLABUS")]
    public async Task<IActionResult> GetJoinedSubjectSyllabus(long joinedSubjectId)
    {
        var result = await _joinedSubjectService.GetJoinedSubjectSyllabusAsync(joinedSubjectId, AccessToken);
        return Ok(result);
    }



    ///<summary>
    /// The ACADEMIC_STAFF || MANAGER | ADMIN | Advisor View all by  student profile id 
    /// </summary>
    [HttpGet("{studentProfileId}/all")]
    [PermissionAuthorize((int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.MANAGER, (int)EUserRole.ADMIN, (int)EUserRole.ADVISOR)]
    [AuditLog(Tag = "VIEW_JOINED_SUBJECT")]
    public async Task<IActionResult> GetAllByStudentProfileIdPaged(long studentProfileId)
    {
        var res = await _joinedSubjectService.GetAllByStudentProfileIdAsync(studentProfileId);
        return Ok(res);
    }


    ///<summary>
    /// Get Single Item only
    /// </summary>
    [HttpGet("{id}")]
    [AuditLog(Tag = "VIEW_JOINED_SUBJECT")]
    public async Task<IActionResult> GetById(long id)
    {

        var res = await _joinedSubjectService.GetByIdAsync(AccessToken, id);
        return Ok(res);
    }


    ///<summary>
    /// Get status of all joined subject per student profile
    /// </summary>
    [HttpGet("map-status/{studentProfileID}")]
    public async Task<IActionResult> GetMapJoinedSubjectStatusByStudentProfileID(long studentProfileID)
    {
        var res = await _joinedSubjectService.GetMapJoinedSubjectStatusByStudentProfileIDAsync(studentProfileID);
        return Ok(res);
    }


    ///<summary>
    /// Get progress-checkpoints of all joined subject per student profile
    /// </summary>
    [HttpGet("map-complete-checkpoint-percentage/{studentProfileID}")]
    public async Task<IActionResult> GetMapJoinedSubjectProgressCheckpointByStudentProfileID(long studentProfileID)
    {
        var res = await _joinedSubjectService.GetMapJoinedSubjectProgressCheckpointByStudentProfileIDAsync(studentProfileID);
        return Ok(res);
    }




}