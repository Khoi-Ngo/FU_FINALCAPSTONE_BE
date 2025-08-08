using AISEA.ApiService.BAL.Services.CourseTracker;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.JoinedSubject;
using AISEA.ApiService.SHARED.DTOs.Responses.JoinedSubject;
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
    private readonly ILogger<JoinedSubjectController> _logger;
    public JoinedSubjectController(EndpointSettings endpointSettings) : base(endpointSettings)
    {
    }


    ///<summary>
    /// Singly import a subject for a student
    /// 1 Student - 1 Joined subject
    /// </summary>
    [HttpPost("import")]
    [PermissionAuthorize((int)EUserRole.MANAGER, (int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.ADMIN)]
    public async Task<IActionResult> ImportSubjectAsync([FromBody] SingleImportJoinedSubjectRequest request)
    {
        // Assuming you have a service to handle the import logic
        var res = await _joinedSubjectService.ImportSubjectAsync(request, AccessToken);

        //notify for the conductor
        await NotifyConductorAsync("The subject has been imported successfully");

        //notify for the student imported
        await NotifyStakeHolderAsync(res.StakeholderUserId, res.Content, res.Title);

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
        var res = await _joinedSubjectService.ImportMultipleSubjectsAsync(request, AccessToken);

        //notify for the conductor
        await NotifyConductorAsync("The courses have been imported successfully");

        //notify for the student imported
        await NotifyStakeHolderAsync(res.StakeholderUserId, res.Content, res.Title);

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
        // Assuming you have a service to handle the import logic
        var res = await _joinedSubjectService.ImportMultipleSubjectsAsync(request, AccessToken);

        //notify for the conductor
        await NotifyConductorAsync("The students have been imported successfully");

        await NotifyStakeHoldersAsync(res);

        return Ok("Import successful");
    }





    #region private methods support notify

    //Notify the conductor after request to an API
    private async Task NotifyConductorAsync(string content = "The action has been completed", string title = "Successfully")
    {
        try {
            await _notifier.NotifyUserAsync(AccessToken, title, content);
        } catch (Exception e) {
            _logger.LogError(e, "Error while notifying conductor");
        }
    }

    // (1 stakeholder - 1 update - 1 notification || 1 stakeholder - N Updates - 1 same notification)
    //Notify to the stakeholder after having request to an API
    private async Task NotifyStakeHolderAsync(long stakeHolderUserId, string content, string title = "Update")
    {
        try {
            await _notifier.NotifyUserAsync(stakeHolderUserId, title, content);
        } catch (Exception e) {
            _logger.LogError(e, "Error while notifying stakeholder");
        }
    }

    // Notify multiple stakeholders efficiently
    private async Task NotifyStakeHoldersAsync(List<JoinedSubjectStakeholderNotification> notifications)
    {
        try
        {
            if (notifications == null || notifications.Count == 0)
                return;

            var tasks = notifications.Select(n =>
            {
                var title = string.IsNullOrWhiteSpace(n.Title) ? "Notification" : n.Title;
                var content = n.Content ?? string.Empty;

                return _notifier.NotifyUserAsync(n.StakeholderUserId, title, content);
            });

            await Task.WhenAll(tasks); // Run all notifications in parallel
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while notifying stakeholders");
        }
    }



    #endregion
}