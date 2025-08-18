using AISEA.ApiService.BAL.Services.Booking;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.Booking;

[ApiController]
[Route("api/[controller]")]
public class LeaveScheController : BaseController
{
    private readonly LeaveScheduleService _leaveScheduleService;
    private readonly IBackgroundTaskQueue _taskQueue;

    public LeaveScheController(
        LeaveScheduleService leaveScheduleService,
        EndpointSettings endpointSettings,
        IBackgroundTaskQueue taskQueue) : base(endpointSettings)
    {
        _leaveScheduleService = leaveScheduleService;
        _taskQueue = taskQueue;
    }

    #region Command action
    /// <summary>
    /// Creates Leaving Schedule for a advisor member.
    /// </summary>
    [HttpPost]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    [AuditLog(Tag = "CREATE_LEAVE_SCHEDULE")]
    public async Task<IActionResult> CreateLeaveScheduleAsync([FromBody] CreateLeaveScheRequest request)
    {
        var accessToken = AccessToken;
        await _leaveScheduleService.CreateAsync(request, accessToken);
        return Ok("Leaving Schedule has been created successfully.");


    }

    [HttpPost("bulk")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    [AuditLog(Tag = "BULK_CREATE_LEAVE_SCHEDULE")]
    public async Task<IActionResult> CreateLeaveScheduleAsync([FromBody] List<CreateLeaveScheRequest> requests)
    {
        var accessToken = AccessToken;

        _taskQueue.QueueBackgroundWorkItem(async (sp, token) =>
            {
                var qLeaveScheService = sp.GetRequiredService<LeaveScheduleService>();
                await qLeaveScheService.CreateBulkAsync(requests, accessToken);
                var qNotifier = sp.GetRequiredService<NotificationHubNotifier>();
                await qNotifier.NotifyUserAsync(accessToken,
                 new NotificationDTO { Title = "Successfully", Content = "Leaving Schedule has been created successfully" });
            });

        return Ok("Bulk create leave schedules has been queued successfully!");

    }

    /// <summary>
    /// Updates an existing Leaving Schedule.
    /// </summary>
    [HttpPut("{id}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    [AuditLog(Tag = "UPDATE_LEAVE_SCHEDULE")]
    public async Task<IActionResult> UpdateLeaveScheAsync(long id, [FromBody] UpdateLeaveScheRequest request)
    {
        var accessToken = AccessToken;

        await _leaveScheduleService.UpdateAsync(request, id, accessToken);
        return Ok("Leaving Schedule has been updated successfully!");
    }

    /// <summary>
    /// Deletes a leave schedule by its ID.
    /// </summary>
    [HttpDelete("{id}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    [AuditLog(Tag = "DELETE_LEAVE_SCHEDULE")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        var accessToken = AccessToken;

        await _leaveScheduleService.DeleteAsync(id, accessToken);
        return Ok("Leave Schedule has been deleted!");

    }

    #endregion

    #region Query action

    /// <summary>
    /// STUDENT and ADMIN Retrieves all leave schedule pagination.
    /// </summary>
    [HttpGet]
    [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.STUDENT)]
    [AuditLog(Tag = "VIEW_LEAVE_SCHEDULE")]
    public async Task<IActionResult> GetAllSimply([FromQuery] PaginationRequest request)
    {
        var result = await _leaveScheduleService.GetAllSimplyAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// STUDENT and ADMIN Retrieves all leave schedule FOR ONE STAFF pagination
    /// </summary>
    [HttpGet("{staffProfileId}")]
    [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.STUDENT)]
    [AuditLog(Tag = "VIEW_LEAVE_SCHEDULE")]
    public async Task<IActionResult> GetAllForAStaffAsync([FromQuery] PaginationRequest request, long staffProfileId)
    {
        var result = await _leaveScheduleService.GetAllSimplyAsync(request, staffProfileId);
        return Ok(result);
    }

    /// <summary>
    /// ADVISOR self Retrieves all leave schedule 
    /// </summary>
    [HttpGet("self")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    [AuditLog(Tag = "VIEW_LEAVE_SCHEDULE")]
    public async Task<IActionResult> GetAllForAStaffSimply([FromQuery] PaginationRequest request)
    {
        var accessToken = AccessToken;

        var res = await _leaveScheduleService.GetAllSimplyAsync(request, accessToken);
        return Ok(res);
    }


    /// <summary>
    /// Get a single simple leave schedule (*NOTE: Support FE Purpose only)
    /// </summary>
    [HttpGet("simply-single/{id}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    [AuditLog(Tag = "VIEW_LEAVE_SCHEDULE")]
    public async Task<IActionResult> GetByIdAsync(long id)
    {
        var res = await _leaveScheduleService.GetSimplyByIdAsync(id);
        return Ok(res);
    }

    #endregion


    #region using for check time only

    [AllowAnonymous]
    [HttpGet("check-datetime-backend")]
    public IActionResult CheckDateTime()
    {
        return Ok(new
        {
            TheDateTime = DateTime.Now,
            TheDateTimeUtc = DateTime.UtcNow
        });
    }

    [AllowAnonymous]
    [HttpGet("check-datetime-database")]
    public async Task<IActionResult> CheckDateTimeDB()
    {
        var res = await _leaveScheduleService.CheckDateTimeDBAsync();
        return Ok(res);
    }

    [AllowAnonymous]
    [HttpGet("check-datetime-database-check")]
    public async Task<IActionResult> CheckDateTimeDBHAHaaa()
    {
        var res = await _leaveScheduleService.CheckDateTimeDBAsync();
        return Ok("haha");
    }

    [AllowAnonymous]
    [HttpGet("check-day-of-week-sql")]
    public async Task<IActionResult> CheckDayOfWeekSQL([FromQuery] DateTime? date = null)
    {
        var res = await _leaveScheduleService.CheckDayOfWeekSQLAsync(date ?? DateTime.Now);
        return Ok(res);
    }

    #endregion



}
