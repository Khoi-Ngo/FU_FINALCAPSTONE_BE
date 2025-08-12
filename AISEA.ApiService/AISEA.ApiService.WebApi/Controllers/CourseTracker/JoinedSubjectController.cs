using AISEA.ApiService.BAL.Services.CourseTracker;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.JoinedSubject;
using AISEA.ApiService.SHARED.DTOs.Requests.Noti;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.HubUtil;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.CourseTracker;

[ApiController]
[Route("api/[controller]")]
public class JoinedSubjectController : BaseController
{

    #region Init

    private readonly JoinedSubjectService _joinedSubjectService;
    private readonly NotificationHubNotifier _notifier;
    private readonly IBackgroundTaskQueue _taskQueue;

    public JoinedSubjectController(EndpointSettings endpointSettings, JoinedSubjectService joinedSubjectService, NotificationHubNotifier notifier, IBackgroundTaskQueue taskQueue) : base(endpointSettings)
    {
        _joinedSubjectService = joinedSubjectService;
        _notifier = notifier;
        _taskQueue = taskQueue;
    }


    #endregion



    ///<summary>
    /// Singly import a subject for a student
    /// 1 Student - 1 Joined subject
    /// </summary>
    [HttpPost("import")]
    [PermissionAuthorize((int)EUserRole.MANAGER, (int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.ADMIN)]
    public async Task<IActionResult> ImportSubjectAsync([FromBody] SingleImportJoinedSubjectRequest request)
    {
        // Assuming you have a service to handle the import logic
        var (stakeHolderNoti, StakeholderUserId) = await _joinedSubjectService.ImportSubjectAsync(request, AccessToken);

        await _notifier.NotifyUserAsync(AccessToken,
        new NotificationDTO { Title = "Successfully", Content = "The subject has been imported successfully" });

        await _notifier.NotifyUserAsync(StakeholderUserId, stakeHolderNoti);

        return Ok("Import successful");
    }


    ///<summary>
    /// Import N joined subjects for ONE student
    /// 1 Student - N Joined Subjects
    /// </summary>
    [HttpPost("import-multiple")]
    [PermissionAuthorize((int)EUserRole.MANAGER, (int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.ADMIN)]
    public async Task<IActionResult> ImportMultipleSubjectsAsync([FromBody] ImportJoinedSubjectsForOneStudentRequest request)
    {
        // Assuming you have a service to handle the import logic
        var (stakeHodlerNoti, stakeHolderUserId) = await _joinedSubjectService.ImportMultipleSubjectsAsync(request, AccessToken);

        //notify for the conductor
        await _notifier.NotifyUserAsync(AccessToken,
               new NotificationDTO { Title = "Successfully", Content = "The subjects have been imported successfully" });

        await _notifier.NotifyUserAsync(stakeHolderUserId, stakeHodlerNoti);

        return Ok("Import successful");
    }

    ///<summary>
    /// Import N joined subjects for N student
    /// N Student - N Joined Subject
    /// </summary>
    [HttpPost("import-multiple-students")]
    [PermissionAuthorize((int)EUserRole.MANAGER, (int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.ADMIN)]
    public async Task<IActionResult> ImportMultipleStudentsAsync([FromBody] ImportJoinedSubjectsRequest request)
    {

        _taskQueue.QueueBackgroundWorkItem(async (sp, token) =>
        {
            var qJoinedSubjectService = sp.GetRequiredService<JoinedSubjectService>();
            List<(long stakeHolderUserId, NotificationDTO stakeHolderNoti)> res = await qJoinedSubjectService.ImportMultipleSubjectsAsync(request, AccessToken);

            var qNotifier = sp.GetRequiredService<NotificationHubNotifier>();
            await qNotifier.NotifyUsersAsync(res);
        });


        await _notifier.NotifyUserAsync(AccessToken,
        new NotificationDTO { Title = "Successfully", Content = "Import action has been queued" });

        return Ok("Import action has been queued");
    }


    ///<summary>
    /// Delete a joined subject for a student
    /// </summary>
    [HttpDelete("{id}")]
    [PermissionAuthorize((int)EUserRole.MANAGER, (int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.ADMIN)]
    public async Task<IActionResult> DeleteSubjectAsync(long id)
    {
        // Assuming you have a service to handle the delete logic
        var (stakeHodlerNoti, stakeHolderUserId) = await _joinedSubjectService.DeleteSubjectAsync(id, AccessToken);

        await _notifier.NotifyUserAsync(AccessToken,
        new NotificationDTO { Title = "Successfully", Content = "The subject has been deleted successfully" });

        await _notifier.NotifyUserAsync(stakeHolderUserId, stakeHodlerNoti);

        return Ok("Delete successful");
    }


    ///<summary>
    /// The student view all data by him self
    /// </summary>
    [HttpGet("self")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    public async Task<IActionResult> GetAllBySelfPaged([FromQuery] PaginationRequest request)
    {
        var res = await _joinedSubjectService.GetAllBySelfPagedAsync(request, AccessToken);
        return Ok(res);
    }


    ///<summary>
    /// The student view all data by him self AND By the Latest Semester
    /// </summary>
    [HttpGet("self/latest-semester")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    public async Task<IActionResult> GetAllBySelfLatestSemesterPaged([FromQuery] PaginationRequest request)
    {
        var res = await _joinedSubjectService.GetAllBySelfLatestSemesterPagedAsync(request, AccessToken);
        return Ok(res);
    }


    ///<summary>
    /// The ACADEMIC_STAFF || MANAGER | ADMIN View all by  student profile id 
    /// </summary>
    [HttpGet("{studentProfileId}/all")]
    [PermissionAuthorize((int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.MANAGER, (int)EUserRole.ADMIN)]
    public async Task<IActionResult> GetAllByStudentProfileIdPaged([FromQuery] PaginationRequest request, long studentProfileId)
    {
        var res = await _joinedSubjectService.GetAllByStudentProfileIdPagedAsync(request, studentProfileId);
        return Ok(res);
    }


}