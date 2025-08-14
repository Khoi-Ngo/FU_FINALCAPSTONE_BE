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
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.Booking;

[ApiController]
[Route("api/[controller]")]
public class BookingAvailabilityController : BaseController
{
    private readonly BookingAvailabilityService _bookingAvailabilityService;
    private readonly IBackgroundTaskQueue _taskQueue;


    public BookingAvailabilityController(
        BookingAvailabilityService bookingAvailabilityService
    , EndpointSettings endpointSettings
    , IBackgroundTaskQueue taskQueue
    ) : base(endpointSettings)
    {
        _bookingAvailabilityService = bookingAvailabilityService;
        _taskQueue = taskQueue;
    }

    /// <summary>
    /// Creates a new booking availability for a staff member.
    /// </summary>
    [HttpPost]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    [AuditLog(Tag = "CREATE_BOOKING_AVAILABILITY", Description = "")]
    public async Task<IActionResult> CreateBookingAvailability([FromBody] CreateBookingAvailabilityRequest request)
    {
        var accessToken = AccessToken;

        await _bookingAvailabilityService.CreateBookingAvailabilityAsync(request, accessToken);
        return Ok("Booking availability has been created successfully!");

    }

    /// <summary>
    /// Creates multiple booking availabilities for a staff member.
    /// </summary>
    [HttpPost("bulk")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    [AuditLog(Tag = "BULK_CREATE_BOOKING_AVAILABILITY", Description = "")]
    public async Task<IActionResult> BulkCreateBookingAvailability([FromBody] List<CreateBookingAvailabilityRequest> request)
    {
        var accessToken = AccessToken;

        _taskQueue.QueueBackgroundWorkItem(async (sp, token) =>
              {
                  var qBookingAvaiService = sp.GetRequiredService<BookingAvailabilityService>();

                  await qBookingAvaiService.BulkCreateBookingAvailabilityAsync(request, accessToken);

                  var qNotifier = sp.GetRequiredService<NotificationHubNotifier>();

                  await qNotifier.NotifyUserAsync(accessToken,
                  new NotificationDTO { Title = "Warning", Content = "Booking availabilities have been created successfully" });
              });

        return Ok("Bulk create booking availabilities has been queued successfully!");

    }

    /// <summary>
    /// ADMIN and STUDENT Retrieves all booking availabilities for a staff member.
    /// </summary>
    [HttpGet("{staffProfileId}")]
    [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.STUDENT)]
    [AuditLog(Tag = "VIEW_BOOKING_AVAILABILITY", Description = "")]
    public async Task<IActionResult> GetBookingAvailabilities(long staffProfileId)
    {
        var result = await _bookingAvailabilityService.GetBookingAvailabilitiesAsync(staffProfileId);
        return Ok(result);
    }


    /// <summary>
    ///ADVISOR Self viewing their booking availabilities
    /// </summary>
    [HttpGet("self")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    [AuditLog(Tag = "VIEW_BOOKING_AVAILABILITY", Description = "")]
    public async Task<IActionResult> SelfGetBookingAvailabilities()
    {
        var accessToken = AccessToken;

        var result = await _bookingAvailabilityService.GetBookingAvailabilitiesAsync(accessToken);
        return Ok(result);
    }


    /// <summary>
    ///ADMIN and STUDENT Retrieves all booking availabilities with staff data pagination.
    /// </summary>
    [HttpGet]
    [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.STUDENT)]
    [AuditLog(Tag = "VIEW_BOOKING_AVAILABILITY", Description = "")]
    public async Task<IActionResult> GetBookingAvailabilities([FromQuery] PaginationRequest request)
    {
        var res = await _bookingAvailabilityService.GetBookingAvailabilitiesAsync(request);
        return Ok(res);
    }

    /// <summary>
    /// Updates an existing booking availability.
    /// </summary>
    [HttpPut("{id}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    [AuditLog(Tag = "UPDATE_BOOKING_AVAILABILITY", Description = "")]
    public async Task<IActionResult> UpdateBookingAvailability(long id, [FromBody] UpdateBookingAvailabilityRequest request)
    {
        var accessToken = AccessToken;

        await _bookingAvailabilityService.UpdateAsync(id, request, accessToken);
        return Ok("Booking availability has been updated successfully!");
    }

    /// <summary>
    /// Deletes a booking availability by its ID.
    /// </summary>
    [HttpDelete("{id}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    [AuditLog(Tag = "DELETE_BOOKING_AVAILABILITY", Description = "")]
    public async Task<IActionResult> DeleteBookingAvailability(long id)
    {
        var accessToken = AccessToken;

        await _bookingAvailabilityService.DeleteAsync(id, accessToken);
        return Ok("Booking availability has been deleted!");
    }

    /// <summary>
    /// Get a single booking availability (*NOTE: Support FE Purpose only)
    /// </summary>
    [HttpGet("simply-single/{id}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> GetByIdAsync(long id)
    {
        var res = await _bookingAvailabilityService.GetSimplyByIdAsync(id);
        return Ok(res);
    }

}