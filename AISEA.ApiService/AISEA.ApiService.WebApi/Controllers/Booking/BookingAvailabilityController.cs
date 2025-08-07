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
public class BookingAvailabilityController : BaseController
{
    private readonly BookingAvailabilityService _bookingAvailabilityService;
    private readonly NotificationHubNotifier _notifier;
    public BookingAvailabilityController(
        BookingAvailabilityService bookingAvailabilityService
    , NotificationHubNotifier notifier
    , EndpointSettings endpointSettings) : base(endpointSettings)
    {
        _bookingAvailabilityService = bookingAvailabilityService;
        _notifier = notifier;
    }

    /// <summary>
    /// Creates a new booking availability for a staff member.
    /// </summary>
    [HttpPost]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> CreateBookingAvailability([FromBody] CreateBookingAvailabilityRequest request)
    {
        await _bookingAvailabilityService.CreateBookingAvailabilityAsync(request, AccessToken);
        return await NotifyAndResponseOkAsync("Booking availability has been created successfully.");

    }

    /// <summary>
    /// Creates multiple booking availabilities for a staff member.
    /// </summary>
    [HttpPost("bulk")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> BulkCreateBookingAvailability([FromBody] List<CreateBookingAvailabilityRequest> request)
    {
        await _bookingAvailabilityService.BulkCreateBookingAvailabilityAsync(request, AccessToken);
       return await NotifyAndResponseOkAsync("Booking availabilities have been created successfully.");

    }

    /// <summary>
    /// ADMIN and STUDENT Retrieves all booking availabilities for a staff member.
    /// </summary>
    [HttpGet("{staffProfileId}")]
    [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.STUDENT)]
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
    public async Task<IActionResult> SelfGetBookingAvailabilities()
    {
        var result = await _bookingAvailabilityService.GetBookingAvailabilitiesAsync(AccessToken);
        return Ok(result);
    }


    /// <summary>
    ///ADMIN and STUDENT Retrieves all booking availabilities with staff data pagination.
    /// </summary>
    [HttpGet]
    [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.STUDENT)]
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
    public async Task<IActionResult> UpdateBookingAvailability(long id, [FromBody] UpdateBookingAvailabilityRequest request)
    {
        await _bookingAvailabilityService.UpdateAsync(id, request, AccessToken);
        return await NotifyAndResponseOkAsync("Booking availability has been updated successfully.");
    }

    /// <summary>
    /// Deletes a booking availability by its ID.
    /// </summary>
    [HttpDelete("{id}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> DeleteBookingAvailability(long id)
    {
        await _bookingAvailabilityService.DeleteAsync(id, AccessToken);
        return await NotifyAndResponseOkAsync("Booking availability has been deleted.");
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


    private async Task<IActionResult> NotifyAndResponseOkAsync(string message)
    {
        await _notifier.NotifyUserAsync(AccessToken, "Successfully", message);
        return Ok(new { message });
    }


}