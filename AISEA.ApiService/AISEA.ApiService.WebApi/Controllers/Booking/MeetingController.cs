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
//TODO

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
    /// If there is no matching booking avai 400
    /// If match but the booking avai slot booked by other then show the left list timerange in that slot via exception
    /// </summary>
    [HttpPost]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    public async Task<IActionResult> CreateMeetingAsync([FromBody] CreateMeetingRequest request)
    {
        // await _bookedMeetingService.CreateMeetingAsync(request, AccessToken);
        await _notifier.NotifyUser(AccessToken, "Successfully", "The meeting has been created successfully.");
        return NoContent();
    }


    /// <summary>
    /// PENDING -> CANCELED
    /// Student  cancel a meeting
    /// Student cancel before advisor approve (no ban)
    /// MUST HAVE NOTE
    /// </summary>
    [HttpPut("cancel-the-pending/{id}")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    public async Task<IActionResult> CancelPendingAsync([FromBody] NoteDTO request, long id)
    {
        // await _bookedMeetingService.CancelPendingAsync(id, request, AccessToken);
        await _notifier.NotifyUser(AccessToken, "Successfully", "The meeting has been canceled successfully");
        return NoContent();
    }

    /// <summary>
    /// PENDING -> NOT_APPROVED
    /// ADvisor disapprove bulk to create a leave
    /// </summary>
    [HttpPost("disapprove")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> DisapprovePendingMeetings([FromBody] DisApproveRequest request)
    {
        // await _bookedMeetingService.DisapprovePendingMeetingsAsync(AccessToken, request);
        await _notifier.NotifyUser(AccessToken, "Successful", "The meeting(s) have/has been disapproved already");
        return NoContent();
    }


    /// <summary>
    /// PENDING -> CONFIRMED
    /// ADvisor Confirm the PENDING Meeting
    /// </summary>
    [HttpPut("confirm/{id}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> ConfirmMeetingAsync(long id)
    {
        // await _bookedMeetingService.ConfirmMeetingAsync(id, AccessToken);
        await _notifier.NotifyUser(AccessToken, "Successfully", "The meeting has been confirmed");
        return NoContent();
    }



    /// <summary>
    /// CONFIRMED -> CANCELED
    /// Student cancel the confirmed meeting
    /// MUST HAVE NOTE ~ Depend on the time then Ban or not
    /// </summary>
    [HttpPut("stu-cancel-the-confirmed/{id}")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    public async Task<IActionResult> StuCancelTheConfirmedAsync(long id, [FromBody] NoteDTO request)
    {
        // var res = await _bookedMeetingService.StuCancelTheConfirmedAsync(id, request, AccessToken);
        // if (res.numberOfBan > 0)
        // {
        //     await _notifier.NotifyUser(AccessToken, "Successfully", "The meeting has been canceled and you get " + (-res.numberOfBan) + " on booking point");
        // }
        // else
        // {
        //     await _notifier.NotifyUser(AccessToken, "Successfully", "The meeting has been canceled");
        // }
        return NoContent();
    }


    /// <summary>
    /// CONFIRMED -> CANCELED
    /// MUST HAVE NOTE
    /// Advisor cancel the confirmed meeting
    /// </summary>
    [HttpPut("advisor-cancel-the-confirmed/{id}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> AdvCancelTheConfirmedAsync(long id, [FromBody] NoteDTO request)
    {
        // await _bookedMeetingService.AdvCancelTheConfirmedAsync(id, request, AccessToken);
        await _notifier.NotifyUser(AccessToken, "Successfully", "The meeting has been canceled");
        return NoContent();
    }


    /// <summary>
    /// CONFIRMED -> COMPLETED
    /// Advisor input the checkin code for complete the meeting
    /// </summary>
    [HttpPut("complete/{id}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> CompleteAsync(long id, [FromBody] InputCheckinRequest request)
    {
        // await _bookedMeetingService.CompleteAsync(AccessToken, id, request);
        await _notifier.NotifyUser(AccessToken, "Successfully", "The meeting has been checked in successfully");
        return NoContent();
    }

    /// <summary>
    /// COMPLETED -> Feedback (no change stat)
    /// The student giving the feedback for the completed one
    /// </summary>
    [HttpPost("feedback")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    public async Task<IActionResult> FeedbackAsync([FromBody] FeedbackRequest request)
    {
        // await _bookedMeetingService.FeedbackAsync(AccessToken, request);
        await _notifier.NotifyUser(AccessToken, "Successfully", "Give feedback ok!");
        return NoContent();
    }


    /// <summary>
    /// CONFIRMED -> STUDENT_MISSED
    /// Advisor mark the student missed the meeting then ban
    ///TODO: Have to check time also Cur in RangeTIme of Meeting
    /// </summary>
    [HttpPut("mark-stu-missed/{id}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> MarkStudentMissedAsync(long id)
    {
        // var studentUserIdToNotify = await _bookedMeetingService.MarkStudentMissedAsync(AccessToken, id);
        await _notifier.NotifyUser(AccessToken, "Successfully", "Mark student missing the meeting successfully");
        // await _notifier.NotifyUser(studentUserIdToNotify, "Alert", "Existing meeting you missed please check");
        return NoContent();
    }


    /// <summary>
    /// CONFIRMED -> ADIVSOR_MISSED
    /// Student mark the advisor missed
    /// HAVE to have NOTE
    ///TODO: Have to check time also Cur in RangeTIme of Meeting
    /// </summary>
    [HttpPut("mark-adv-missed/{id}")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    public async Task<IActionResult> MarkAdvisorMissedAsync(long id, [FromBody] NoteDTO request)
    {
        // var advisorUserIdToNotify = await _bookedMeetingService.MarkAdvisorMissedAsync(AccessToken, id, request);
        await _notifier.NotifyUser(AccessToken, "Successfully", "Mark advisor missing the meeting successfully");
        // await _notifier.NotifyUser(advisorUserIdToNotify, "Alert", "Existing meeting you missed please check");
        return NoContent();
    }

    /// <summary>
    /// advisor create a note for OVERDUE
    /// TODO: Having a bgjob to continue notify the advisor update the reason notify and saving database also
    /// </summary>
    [HttpPost("reason-for-overdue")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> AddReasonForOverdue([FromBody] ReasonOverdueRequest request)
    {
        // var studentUserIdToNotify = await _bookedMeetingService.AddReasonForOverdue(AccessToken, request);
        await _notifier.NotifyUser(AccessToken, "Successfully", "Give reason ok!");
        // await _notifier.NotifyUser(studentUserIdToNotify, "Info", "An overdue meeting has been updated ");
        return NoContent();
    }


    /// <summary>
    ///  Admin get all
    /// </summary>
    [HttpGet("all/paged")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> GetAllAsync([FromQuery] PaginationRequest request)
    {
        // var res = await _bookedMeetingService.GetAllAsync(request);
        // return Ok(res);
        throw new NotImplementedException();

    }


    /// <summary>
    /// Student Or Staff Get all by their profile
    /// </summary>
    [HttpGet("all-by-profile/paged")]
    [PermissionAuthorize((int)EUserRole.ADVISOR, (int)EUserRole.STUDENT)]
    public async Task<IActionResult> GetAllByProfileAsync([FromQuery] PaginationRequest request)
    {
        throw new NotImplementedException();

        // var res = await _bookedMeetingService.GetAllByProfileAsync(request, AccessToken);
        // return Ok(res);
    }

    /// <summary>
    /// Get Detail By Id (admin, advisor, student)
    /// </summary>
    [HttpGet("{id}")]
    [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ADVISOR, (int)EUserRole.STUDENT)]
    public async Task<IActionResult> GetByIdAsync(long id)
    {
        throw new NotImplementedException();
        // var res = await _bookedMeetingService.GetByIdAsync(id, AccessToken);
        // return Ok(res);
    }


    /// <summary>
    ///  Admin delete by id
    /// </summary>
    [HttpDelete("{id}")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        // await _bookedMeetingService.DeleteAsync(id);
        return NoContent();
    }



}
