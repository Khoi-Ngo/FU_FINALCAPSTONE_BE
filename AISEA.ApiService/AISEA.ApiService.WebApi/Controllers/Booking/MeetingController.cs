using AISEA.ApiService.BAL.Services.Booking;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Booking;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.HubUtil;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.Booking;

[ApiController]
[Route("api/[controller]")]
public class MeetingController : BaseController
{

    private readonly BookedMeetingService _bookedMeetingService;
    private readonly NotificationHubNotifier _notifier;

    public MeetingController(
        EndpointSettings endpointSettings,
        BookedMeetingService bookedMeetingService,
        NotificationHubNotifier notificationHubNotifier) : base(endpointSettings)
    {
        _bookedMeetingService = bookedMeetingService;
        _notifier = notificationHubNotifier;
    }

    /// <summary>
    /// Student create a meeting
    /// NOTHING -> PENDING
    /// </summary>
    [HttpPost]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    public async Task<IActionResult> CreateMeeting([FromBody] CreateMeetingRequest request)
    {
        var res = await _bookedMeetingService.CreateMeetingAsync(request, AccessToken);
        await _notifier.NotifyUser(AccessToken, "Successfully", "The meeting has been created successfully.");
        await _notifier.NotifyUser(res.PartnerUserId, res.StatusChangedTo.ToString(), $"There is new meeting {res.MeetingStartDateTime} - {res.MeetingEndDateTime} with status {res.StatusChangedTo.ToString()} !");
        return Ok("Ok");
    }


    /// <summary>
    /// PENDING -> STU_CANCELED
    /// Student cancel a meeting
    /// </summary>
    [HttpPost("stu-cancel-the-pending/{id}")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    public async Task<IActionResult> CancelPending([FromBody] NoteDTO request, long id)
    {
        await _bookedMeetingService.StuCancelPendingAsync(id, request, AccessToken);
        await _notifier.NotifyUser(AccessToken, "Successfully", "The meeting has been canceled successfully");
        return Ok("Ok");
    }

    /// <summary>
    /// PENDING || CONFIRMED-> ADV_CANCELED
    /// Advisor cancel the PENDING meeting with a note
    /// </summary>
    [HttpPut("adv-cancel/{meetingId}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> AdvisorCancelMeeting([FromBody] NoteDTO request, long meetingId)
    {
        var res = await _bookedMeetingService.AdvisorCancelMeetingAsync(AccessToken, request, meetingId);

        await _notifier.NotifyUser(AccessToken, "Successfully", "The meeting has been canceled");

        await _notifier.NotifyUser(res.PartnerUserId
        , res.StatusChangedTo.ToString(), $"The meeting {res.MeetingStartDateTime} to {res.MeetingEndDateTime} has been {res.StatusChangedTo.ToString()}");


        return Ok("OK");
    }



    /// <summary>
    /// PENDING -> CONFIRMED
    /// Advisor Confirm the PENDING Meeting
    /// </summary>
    [HttpPut("confirm/{id}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> ConfirmMeeting(long id)
    {
        var res = await _bookedMeetingService.ConfirmMeetingAsync(id, AccessToken);
        await _notifier.NotifyUser(AccessToken, "Successfully", "The meeting has been confirmed");

        await _notifier.NotifyUser(res.PartnerUserId
        , res.StatusChangedTo.ToString(), $"The meeting {res.MeetingStartDateTime} to {res.MeetingEndDateTime} has been {res.StatusChangedTo.ToString()}");

        return Ok("Ok");
    }



    /// <summary>
    /// CONFIRMED -> STU_CANCELED
    /// Student cancel the confirmed meeting
    /// </summary>
    [HttpPut("stu-cancel-the-confirmed/{id}")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    public async Task<IActionResult> StuCancelTheConfirmed(long id, [FromBody] NoteDTO request)
    {
        var numberOfBan = await _bookedMeetingService.StuCancelTheConfirmedAsync(id, request, AccessToken);
        if (numberOfBan > 0)
        {
            await _notifier.NotifyUser(AccessToken, "Successfully", "The meeting has been canceled and you get " + (-numberOfBan) + " on booking point");
        }
        else
        {
            await _notifier.NotifyUser(AccessToken, "Successfully", "The meeting has been canceled");
        }
        return Ok("Ok");
    }



    /// <summary>
    /// CONFIRMED -> COMPLETED
    /// Advisor input the checkin code for complete the meeting
    /// </summary>
    [HttpPut("complete/{id}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> Complete(long id, [FromBody] InputCheckinRequest request)
    {
        var res = await _bookedMeetingService.CompleteAsync(AccessToken, id, request);
        await _notifier.NotifyUser(AccessToken, "Successfully", "The meeting has been checked in successfully");

        await _notifier.NotifyUser(res.PartnerUserId, res.StatusChangedTo.ToString(), $"The meeting {res.MeetingStartDateTime} to {res.MeetingEndDateTime} has been {res.StatusChangedTo.ToString()}");
        return Ok("Ok");
    }

    /// <summary>
    /// ACTIVE but End phase (COMPLETED, MISSED stats) -> Feedback (no change stat)
    /// The student giving the feedback for the completed one
    /// </summary>
    [HttpPost("feedback")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    public async Task<IActionResult> Feedback([FromBody] FeedbackRequest request)
    {
        await _bookedMeetingService.FeedbackAsync(AccessToken, request);
        await _notifier.NotifyUser(AccessToken, "Successfully", "Give feedback ok!");
        return Ok("Ok");
    }


    /// <summary>
    /// CONFIRMED -> ADVISOR_MISSED
    /// Student mark the advisor missed
    /// </summary>
    [HttpPut("mark-adv-missed/{id}")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    public async Task<IActionResult> MarkAdvisorMissed(long id, [FromBody] NoteDTO request)
    {
        var advisorUserIdToNotify = await _bookedMeetingService.MarkAdvisorMissedAsync(AccessToken, id, request);
        await _notifier.NotifyUser(AccessToken, "Successfully", "Mark advisor missing the meeting successfully");
        await _notifier.NotifyUser(advisorUserIdToNotify, "Alert", "Existing meeting you missed please check");
        return Ok("Ok");
    }

    /// <summary>
    /// PENDING -> OVERDUE(No Behavior : This shift of stat will be handled in the background job)
    /// advisor create a note for OVERDUE
    /// </summary>
    [HttpPost("reason-for-overdue")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> AddReasonForOverdue([FromBody] ReasonOverdueRequest request)
    {
        var studentUserIdToNotify = await _bookedMeetingService.AddReasonForOverdueAsync(AccessToken, request);
        await _notifier.NotifyUser(AccessToken, "Successfully", "Give reason ok!");
        await _notifier.NotifyUser(studentUserIdToNotify, "Info", "An overdue meeting has been updated ");
        return Ok("Ok");
    }


    /// <summary>
    ///  Admin get all
    /// </summary>
    [HttpGet("all/paged")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
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
    public async Task<IActionResult> GetAllByStudentSelf([FromQuery] PaginationRequest request)
    {
        var res = await _bookedMeetingService.GetAllByStudentSelfAsync(request, AccessToken);
        return Ok(res);
    }



    /// <summary>
    /// Advisor view list all of their own meeting by token
    /// </summary>
    [HttpGet("all-adv-self/paged")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> GetAllByAdvSelf([FromQuery] PaginationRequest request)
    {
        var res = await _bookedMeetingService.GetAllByAdvSelfAsync(request, AccessToken);
        return Ok(res);
    }




    #region  View Detail Meeting

    /// <summary>
    ///  Admin || Involved Student Or Advisor view detail of meeting
    /// </summary>
    [HttpGet("detail/{meetingId}")]
    [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.STUDENT, (int)EUserRole.ADVISOR)]
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
    public async Task<IActionResult> DeleteAsync(long id)
    {
        await _bookedMeetingService.DeleteAsync(id);
        _notifier.NotifyUser(AccessToken, "Successfully", "The meeting has been deleted successfully !");
        return Ok("Ok");
    }



}
