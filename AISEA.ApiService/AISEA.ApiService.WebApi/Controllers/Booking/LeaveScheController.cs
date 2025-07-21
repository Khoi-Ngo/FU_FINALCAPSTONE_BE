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

    /// <summary>
    /// Creates Leaving Schedule for a staff member.
    /// </summary>
    [HttpPost]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> CreateLeaveScheduleAsync([FromBody] CreateLeaveScheRequest request)
    {
        await _leaveScheduleService.CreateAsync(request, AccessToken);
        await _notifier.NotifyUser(AccessToken, "Successfully", "Leaving Schedule has been created successfully.");
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
        await _notifier.NotifyUser(AccessToken, "Successfully", "Leaving Schedule has been updated successfully.");
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
        await _notifier.NotifyUser(AccessToken, "Successfully", "Leave Schedule has been deleted.");
        return Ok("Ok");
    }

    /// <summary>
    /// Retrieves all leave schedule with staff data pagination.
    /// </summary>
    [HttpGet]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> GetAllAsync([FromQuery] PaginationRequest request)
    {
        var result = await _leaveScheduleService.GetPagedResultAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// STUDENT Retrieves all leave schedule  pagination.
    /// </summary>
    [HttpGet]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    public async Task<IActionResult> GetAllSimply([FromQuery] PaginationRequest request)
    {
        var result = await _leaveScheduleService.GetAllSimplyAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all leave schedule FOR ONE STAFF with staff data pagination.
    /// </summary>
    [HttpGet("{staffProfileId}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR, (int)EUserRole.ADMIN)]
    public async Task<IActionResult> GetAllForAStaffAsync([FromQuery] PaginationRequest request, long staffProfileId)
    {
        var result = await _leaveScheduleService.GetPagedResultOfOneStaffAsync(request, staffProfileId);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all leave schedule FOR ONE STAFF with staff data pagination.
    /// </summary>
    [HttpGet("{staffProfileId}")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    public async Task<IActionResult> GetAllForAStaffSimply([FromQuery] PaginationRequest request, long staffProfileId)
    {
        var result = await _leaveScheduleService.GetAllForAStaffSimplyAsync(request, staffProfileId);
        return Ok(result);
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


    [HttpGet("check-datetime-backend")]
    public IActionResult CheckDateTime()
    {
        return Ok(new
        {
            TheDateTime = DateTime.Now,
            TheDateTimeUtc = DateTime.UtcNow
        });
    }

    [HttpGet("check-datetime-database")]
    public async Task<IActionResult> CheckDateTimeDB()
    {
        var res = await _leaveScheduleService.CheckDateTimeDBAsync();
        return Ok(res);
    }

}