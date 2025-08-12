using AISEA.ApiService.BAL.Services.Booking;
using AISEA.ApiService.BAL.Services.SystemProfile;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Booking;
using AISEA.ApiService.SHARED.DTOs.Requests.Noti;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.HubUtil;
using AISEA.ApiService.WebApi.InterceptorAPI;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.Booking;

[ApiController]
[Route("api/[controller]")]
public class MeetingController : BaseController
{

    private readonly BookedMeetingService _bookedMeetingService;
    private readonly NotificationHubNotifier _notifier;
    private readonly IBackgroundTaskQueue _taskQueue;

    public MeetingController(
        EndpointSettings endpointSettings,
        BookedMeetingService bookedMeetingService,
        NotificationHubNotifier notificationHubNotifier,
        IBackgroundTaskQueue taskQueue) : base(endpointSettings)
    {
        _bookedMeetingService = bookedMeetingService;
        _notifier = notificationHubNotifier;
        _taskQueue = taskQueue;
    }

    /// <summary>
    /// Student create a meeting
    /// NOTHING -> PENDING
    /// </summary>
    [HttpPost]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    [AuditLog(Tag = "CREATE_MEETING", Description = "")]
    public async Task<IActionResult> CreateMeeting([FromBody] CreateMeetingRequest request)
    {
        var (parterNoti, partnerUserId) = await _bookedMeetingService.CreateMeetingAsync(request, AccessToken);

        await _notifier.NotifyUserAsync(AccessToken, new NotificationDTO { Title = "Successfully", Content = "The meeting has been canceled successfully" });
        await _notifier.NotifyUserAsync(partnerUserId, new NotificationDTO { Title = parterNoti.Title, Content = parterNoti.Content });

        return Ok("The meeting has been created successfully.");
    }


    /// <summary>
    /// PENDING -> STU_CANCELED
    /// Student cancel a meeting
    /// </summary>
    [HttpPost("stu-cancel-the-pending/{id}")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    [AuditLog(Tag = "STUDENT_CANCEL_PENDING_MEETING", Description = "")]
    public async Task<IActionResult> CancelPending([FromBody] NoteDTO request, long id)
    {
        await _bookedMeetingService.StuCancelPendingAsync(id, request, AccessToken);

        await _notifier.NotifyUserAsync(AccessToken, new NotificationDTO { Title = "Successfully", Content = "The meeting has been canceled successfully" });

        return Ok("The meeting has been canceled successfully");
    }

    /// <summary>
    /// PENDING || CONFIRMED-> ADV_CANCELED
    /// Advisor cancel the PENDING meeting with a note
    /// </summary>
    [HttpPut("adv-cancel/{meetingId}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    [AuditLog(Tag = "ADVISOR_CANCEL_MEETING", Description = "")]
    public async Task<IActionResult> AdvisorCancelMeeting([FromBody] NoteDTO request, long meetingId)
    {
        var (parterNoti, partnerUserId) = await _bookedMeetingService.AdvisorCancelMeetingAsync(AccessToken, request, meetingId);

        await _notifier.NotifyUserAsync(AccessToken, new NotificationDTO { Title = "Successfully", Content = "The meeting has been canceled successfully" });
        await _notifier.NotifyUserAsync(partnerUserId, new NotificationDTO { Title = parterNoti.Title, Content = parterNoti.Content });


        return Ok("The meeting has been canceled successfully.");
    }



    /// <summary>
    /// PENDING -> CONFIRMED
    /// Advisor Confirm the PENDING Meeting
    /// </summary>
    [HttpPut("confirm/{id}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    [AuditLog(Tag = "CONFIRM_MEETING", Description = "")]
    public async Task<IActionResult> ConfirmMeeting(long id)
    {
        var (parterNoti, partnerUserId) = await _bookedMeetingService.ConfirmMeetingAsync(id, AccessToken);

        await _notifier.NotifyUserAsync(AccessToken, new NotificationDTO { Title = "Successfully", Content = "The meeting has been canceled successfully" });
        await _notifier.NotifyUserAsync(partnerUserId, new NotificationDTO { Title = parterNoti.Title, Content = parterNoti.Content });

        return Ok("The meeting has been confirmed successfully.");
    }



    /// <summary>
    /// CONFIRMED -> STU_CANCELED
    /// Student cancel the confirmed meeting
    /// </summary>
    [HttpPut("stu-cancel-the-confirmed/{id}")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    [AuditLog(Tag = "STUDENT_CANCEL_CONFIRMED_MEETING", Description = "")]
    public async Task<IActionResult> StuCancelTheConfirmed(long id, [FromBody] NoteDTO request)
    {
        var (parterNoti, partnerUserId, studentProfileId,numberOfBan) = await _bookedMeetingService.StuCancelTheConfirmedAsync(id, request, AccessToken);

        await _notifier.NotifyUserAsync(AccessToken, new NotificationDTO { Title = "Successfully", Content = "The meeting has been canceled successfully" });
        await _notifier.NotifyUserAsync(partnerUserId, new NotificationDTO { Title = parterNoti.Title, Content = parterNoti.Content });

        _taskQueue.QueueBackgroundWorkItem(async (sp, token) =>
        {
            var qStudentProfileService = sp.GetRequiredService<StudentProfileService>();

            await qStudentProfileService.IncreaseNumberOfBansAsync(studentProfileId, numberOfBan);

            var qNotifier = sp.GetRequiredService<NotificationHubNotifier>();
            await qNotifier.NotifyUserAsync(AccessToken,
            new NotificationDTO { Title = "Warning", Content = $"Your ban point increased {numberOfBan}" });
        });

        return Ok(new { NumberOfBanIncrease = numberOfBan });
    }



    /// <summary>
    /// CONFIRMED -> COMPLETED
    /// Advisor input the checkin code for complete the meeting
    /// </summary>
    [HttpPut("complete/{id}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    [AuditLog(Tag = "COMPLETE_MEETING", Description = "")]
    public async Task<IActionResult> Complete(long id, [FromBody] InputCheckinRequest request)
    {
        var (parterNoti, partnerUserId) = await _bookedMeetingService.CompleteAsync(AccessToken, id, request);

        await _notifier.NotifyUserAsync(AccessToken, new NotificationDTO { Title = "Successfully", Content = "The meeting has been canceled successfully" });
        await _notifier.NotifyUserAsync(partnerUserId, new NotificationDTO { Title = parterNoti.Title, Content = parterNoti.Content });

        return Ok("The meeting has been checked in successfully");
    }

    /// <summary>
    /// ACTIVE but End phase (COMPLETED, MISSED stats) -> Feedback (no change stat)The student giving the feedback for the end of phase && active one just check after the EndTime of the Meeting Time slot to do this action
    /// </summary>
    [HttpPost("feedback/{meetingId}")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    [AuditLog(Tag = "FEEDBACK_MEETING", Description = "")]
    public async Task<IActionResult> Feedback([FromBody] FeedbackRequest request, long meetingId)
    {
        await _bookedMeetingService.FeedbackAsync(AccessToken, request, meetingId);

        await _notifier.NotifyUserAsync(AccessToken, new NotificationDTO { Title = "Successfully", Content = "Give feedback ok!" });

        return Ok("The feedback has been submitted successfully");
    }


    /// <summary>
    /// CONFIRMED -> ADVISOR_MISSED
    /// Student mark the advisor missed
    /// </summary>
    [HttpPut("mark-adv-missed/{id}")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    [AuditLog(Tag = "MARK_ADVISOR_MISSED_MEETING", Description = "")]
    public async Task<IActionResult> MarkAdvisorMissed(long id, [FromBody] NoteDTO request)
    {
        var (parterNoti, partnerUserId) = await _bookedMeetingService.MarkAdvisorMissedAsync(AccessToken, id, request);

        await _notifier.NotifyUserAsync(AccessToken, new NotificationDTO { Title = "Successfully", Content = "The meeting has been canceled successfully" });
        await _notifier.NotifyUserAsync(partnerUserId, new NotificationDTO { Title = parterNoti.Title, Content = parterNoti.Content });


        return Ok("The advisor has been marked as missed successfully");
    }

    /// <summary>
    /// PENDING -> OVERDUE(No Behavior : This shift of stat will be handled in the background job)
    /// advisor create a note for OVERDUE
    /// </summary>
    [HttpPost("reason-for-overdue/{meetingId}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    [AuditLog(Tag = "ADD_REASON_OVERDUE_MEETING", Description = "")]
    public async Task<IActionResult> AddReasonForOverdue([FromBody] NoteDTO request, long meetingId)
    {
        var (parterNoti, partnerUserId) = await _bookedMeetingService.AddReasonForOverdueAsync(AccessToken, request, meetingId);

        await _notifier.NotifyUserAsync(AccessToken, new NotificationDTO { Title = "Successfully", Content = "Give reason successfully" });
        await _notifier.NotifyUserAsync(partnerUserId, new NotificationDTO { Title = parterNoti.Title, Content = parterNoti.Content });

        return Ok("The reason for overdue has been added successfully");
    }


    /// <summary>
    ///  Admin get all
    /// </summary>
    [HttpGet("all/paged")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    [AuditLog(Tag = "VIEW_MEETING", Description = "")]
    public async Task<IActionResult> GetAll([FromQuery] PaginationRequest request)
    {
        var res = await _bookedMeetingService.GetAllAsync(request);
        return Ok(res);
    }


    /// <summary>
    /// Support FrontEnd for Student View Role only
    /// Student view list all basic information of Advisor's Meetings by staffProfileId
    /// </summary>
    [HttpGet("all-of-one-adv/paged/{staffProfileId}")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    [AuditLog(Tag = "VIEW_MEETING", Description = "")]
    public async Task<IActionResult> GetAllByStaffProfileIdForStudentRole([FromQuery] PaginationRequest request, long staffProfileId)
    {
        var res = await _bookedMeetingService.GetAllByStaffProfileIdForStudentRoleAsync(request, staffProfileId);
        return Ok(res);
    }



    /// <summary>
    /// Student view list all of their own meeting by token
    /// </summary>
    [HttpGet("all-stu-self/paged")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    [AuditLog(Tag = "VIEW_MEETING", Description = "")]
    public async Task<IActionResult> GetAllByStudentSelf([FromQuery] PaginationRequest request)
    {
        var res = await _bookedMeetingService.GetAllByStudentSelfAsync(request, AccessToken);
        return Ok(res);
    }

    /// <summary>
    /// Student view list ACTIVE of their own meeting by token
    /// </summary>
    [HttpGet("all-stu-self/active/paged")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    [AuditLog(Tag = "VIEW_MEETING", Description = "")]
    public async Task<IActionResult> GetAllActiveByStudentSelf([FromQuery] PaginationRequest request)
    {
        var res = await _bookedMeetingService.GetAllActiveByStudentSelfAsync(request, AccessToken);
        return Ok(res);
    }




    /// <summary>
    /// Advisor view list all of their own meeting by token
    /// </summary>
    [HttpGet("all-adv-self/paged")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    [AuditLog(Tag = "VIEW_MEETING", Description = "")]
    public async Task<IActionResult> GetAllByAdvSelf([FromQuery] PaginationRequest request)
    {
        var res = await _bookedMeetingService.GetAllByAdvSelfAsync(request, AccessToken);
        return Ok(res);
    }

    /// <summary>
    /// Advisor view list all of their own meeting by token
    /// </summary>
    [HttpGet("all-adv-self/active/paged")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    [AuditLog(Tag = "VIEW_MEETING", Description = "")]
    public async Task<IActionResult> GetAllActiveByAdvSelf([FromQuery] PaginationRequest request)
    {
        var res = await _bookedMeetingService.GetAllActiveByAdvSelfAsync(request, AccessToken);
        return Ok(res);
    }





    #region  View Detail Meeting

    /// <summary>
    ///  Admin || Involved Student Or Advisor view detail of meeting
    /// </summary>
    [HttpGet("detail/{meetingId}")]
    [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.STUDENT, (int)EUserRole.ADVISOR)]
    [AuditLog(Tag = "VIEW_MEETING", Description = "")]
    public async Task<IActionResult> GetDetailMeeting(long meetingId)
    {
        var res = await _bookedMeetingService.GetDetailMeetingAsync(meetingId, AccessToken);
        return Ok(res);
    }

    #endregion


    /// <summary>
    ///  Admin delete by id
    /// </summary>
    [HttpDelete("{id}")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    [AuditLog(Tag = "DELETE_MEETING", Description = "")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        await _bookedMeetingService.DeleteAsync(id);
        await _notifier.NotifyUserAsync(AccessToken, new NotificationDTO { Title = "Successfully", Content = "The meeting has been deleted successfully" });
        return Ok("The meeting has been deleted successfully !");
    }


    /// <summary>
    ///  Student view the max number of ban configured by system
    /// </summary>
    [HttpGet("max-number-of-ban")]
    public async Task<IActionResult> GetMaxNumberOfBan()
    {
        var res = _bookedMeetingService.GetMaxNumberOfBan();
        return Ok(res);
    }


    /// <summary>
    ///  Student self view his or her current number of ban
    /// </summary>
    [HttpGet("cur-number-of-ban")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    [AuditLog(Tag = "VIEW_SELF_NUMBER_OF_BAN", Description = "")]
    public async Task<IActionResult> GetCurNumberOfBan()
    {
        var res = await _bookedMeetingService.GetCurNumberOfBanAsync(AccessToken);
        return Ok(res);
    }

}
