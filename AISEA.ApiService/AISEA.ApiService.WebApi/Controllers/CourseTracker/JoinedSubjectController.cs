using AISEA.ApiService.BAL.Services.CourseTracker;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.JoinedSubject;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.HubUtil;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.CourseTracker;

[ApiController]
[Route("api/[controller]")]
public class JoinedSubjectController : BaseController
{
    private readonly JoinedSubjectService _joinedSubjectService;
    private readonly NotificationHubNotifier _notifier;
    public JoinedSubjectController(EndpointSettings endpointSettings) : base(endpointSettings)
    {
    }


    ///<summary>
    /// Singly import a course for a student
    /// 1 Student - 1 Joined Course
    /// </summary>
    [HttpPost("import")]
    [PermissionAuthorize((int)EUserRole.MANAGER, (int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.ADMIN)]
    public async Task<IActionResult> ImportCourseAsync([FromBody] SingleImportJoinedSubjectRequest request)
    {
        // Assuming you have a service to handle the import logic
        var res = await _joinedSubjectService.ImportSubjectAsync(request, AccessToken);

        //notify for the conductor
        NotifyConductor("The course has been imported successfully");

        //notify for the student imported
        NotifyStakeHolder(res.StakeholderUserId, res.Content, res.Title);

        return Ok("Import successful");
    }


    // ///<summary>
    // /// Import N joined courses for ONE student
    // /// 1 Student - N Joined Course
    // /// </summary>
    // [HttpPost("import-multiple")]
    // [PermissionAuthorize((int)EUserRole.MANAGER, (int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.ADMIN)]
    // public async Task<IActionResult> ImportMultipleCoursesAsync([FromBody] ImportMultipleJoinedCoursesRequest request)
    // {
    //     // Assuming you have a service to handle the import logic
    //     var res = await _joinedCourseService.ImportMultipleCoursesAsync(request);

    //     //notify for the conductor
    //     NotifyConductor("The courses have been imported successfully");

    //     //notify for the student imported
    //     NotifyStakeHolder(res.stakeHolderUserId, res.Content, res.Title);

    //     return Ok("Import successful");
    // }

    // ///<summary>
    // /// Import N joined courses for N student
    // /// N Student - N Joined Course
    // /// </summary>
    // [HttpPost("import-multiple-students")]
    // [PermissionAuthorize((int)EUserRole.MANAGER, (int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.ADMIN)]
    // public async Task<IActionResult> ImportMultipleStudentsAsync([FromBody] ImportMultipleJoinedCoursesStudentsRequest request)
    // {
    //     // Assuming you have a service to handle the import logic
    //     var res = await _joinedCourseService.ImportMultipleStudentsAsync(request);

    //     //notify for the conductor
    //     NotifyConductor("The students have been imported successfully");

    //     //notify for the students imported
    //     NotifyStakeHolders(res.Stakeholders, res.Content, res.Title);

    //     return Ok("Import successful");
    // }





    #region private methods support notify

    //Notify the conductor after request to an API
    private void NotifyConductor(string content = "The action has been completed", string title = "Successfully")
    {
        Task.Run(() => _notifier.NotifyUserAsync(AccessToken, title, content));
    }
    //Notify to the stakeholder after having request to an API (1 stakeholder - 1 update - 1 notification || 1 stakeholder - N Updates - 1 same notification)
    private void NotifyStakeHolder(long stakeHolderUserId, string content, string title = "Update")
    {
        Task.Run(() => _notifier.NotifyUserAsync(stakeHolderUserId, title, content));
    }
    //Notify to the stakeholders after having request to an API (N stakeholders - 1/N update foreach - 1 same notification for all stakeholders)
    private void NotifyStakeHolders(IEnumerable<long> stakeHolderUserIds, string content, string title = "Update")
    {
        Parallel.ForEach(stakeHolderUserIds, userId =>
        {
            Task.Run(() => _notifier.NotifyUserAsync(userId, title, content));
        });
    }


    #endregion
}