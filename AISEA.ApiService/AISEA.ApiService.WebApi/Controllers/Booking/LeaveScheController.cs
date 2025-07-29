using AISEA.ApiService.BAL.Services.Booking;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Booking;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.HubUtil;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.Booking;

[ApiController]
[Route("api/[controller]")]
public class LeaveScheController : BaseController
{
    private readonly LeaveScheduleService _leaveScheduleService;
    private readonly NotificationHubNotifier _notifier;
    public LeaveScheController(
        LeaveScheduleService leaveScheduleService
    , NotificationHubNotifier notifier
    , EndpointSettings endpointSettings) : base(endpointSettings)
    {
        _leaveScheduleService = leaveScheduleService;
        _notifier = notifier;
    }

    #region Command action
    /// <summary>
    /// Creates Leaving Schedule for a staff member.
    /// </summary>
    [HttpPost]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> CreateLeaveScheduleAsync([FromBody] CreateLeaveScheRequest request)
    {
        await _leaveScheduleService.CreateAsync(request, AccessToken);
        await _notifier.NotifyUserAsync(AccessToken, "Successfully", "Leaving Schedule has been created successfully.");
        return Ok("Ok");
    }

    /// <summary>
    /// Updates an existing Leaving Schedule.
    /// </summary>
    [HttpPut("{id}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> UpdateLeaveScheAsync(long id, [FromBody] UpdateLeaveScheRequest request)
    {
        await _leaveScheduleService.UpdateAsync(request, id, AccessToken);
        await _notifier.NotifyUserAsync(AccessToken, "Successfully", "Leaving Schedule has been updated successfully.");
        return Ok("Ok");
    }

    /// <summary>
    /// Deletes a leave schedule by its ID.
    /// </summary>
    [HttpDelete("{id}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        await _leaveScheduleService.DeleteAsync(id, AccessToken);
        await _notifier.NotifyUserAsync(AccessToken, "Successfully", "Leave Schedule has been deleted.");
        return Ok("Ok");
    }
    #endregion

    /// <summary>
    /// STUDENT and ADMIN Retrieves all leave schedule pagination.
    /// </summary>
    [HttpGet]
    [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.STUDENT)]
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
    public async Task<IActionResult> GetAllForAStaffSimply([FromQuery] PaginationRequest request)
    {
        var res = await _leaveScheduleService.GetAllSimplyAsync(request, AccessToken);
        return Ok(res);
    }


    /// <summary>
    /// Get a single simple leave schedule (*NOTE: Support FE Purpose only)
    /// </summary>
    [HttpGet("simply-single/{id}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> GetByIdAsync(long id)
    {
        var res = await _leaveScheduleService.GetSimplyByIdAsync(id);
        return Ok(res);
    }


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
    [HttpGet("check-day-of-week-sql")]
    public async Task<IActionResult> CheckDayOfWeekSQL([FromQuery] DateTime? date = null)
    {
        var res = await _leaveScheduleService.CheckDayOfWeekSQLAsync(date ?? DateTime.Now);
        return Ok(res);
    }

    #endregion

}