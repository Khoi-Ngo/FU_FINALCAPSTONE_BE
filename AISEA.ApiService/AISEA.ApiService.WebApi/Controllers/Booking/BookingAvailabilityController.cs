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
    public BookingAvailabilityController(BookingAvailabilityService bookingAvailabilityService, NotificationHubNotifier notifier, EndpointSettings endpointSettings) : base(endpointSettings)
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
        await _notifier.NotifyUser(AccessToken, "Successfully", "Booking availability has been created successfully.");
        return NoContent();
    }

    /// <summary>
    /// Creates multiple booking availabilities for a staff member.
    /// </summary>
    [HttpPost("bulk")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> BulkCreateBookingAvailability([FromBody] List<CreateBookingAvailabilityRequest> request)
    {
        await _bookingAvailabilityService.
       BulkCreateBookingAvailabilityAsync(request, AccessToken);
        await _notifier.NotifyUser(AccessToken, "Successfully", "Booking availabilities have been bulk created successfully.");
        return NoContent();
    }

    /// <summary>
    /// Retrieves all booking availabilities for a staff member.
    /// </summary>
    [HttpGet("{staffProfileId}")]
    public async Task<IActionResult> GetBookingAvailabilities(long staffProfileId)
    {
        var result = await _bookingAvailabilityService.GetBookingAvailabilitiesAsync(staffProfileId);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all booking availabilities with staff data pagination.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllBookingAvailability([FromQuery] PaginationRequest request)
    {
        var result = await _bookingAvailabilityService.GetAllPagedAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing booking availability.
    /// </summary>
    [HttpPut("{id}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> UpdateBookingAvailability(long id, [FromBody] UpdateBookingAvailabilityRequest request)
    {
        await _bookingAvailabilityService.UpdateAsync(id, request, AccessToken);
        await _notifier.NotifyUser(AccessToken, "Successfully", "Booking availabilities have been updated successfully.");
        return NoContent();
    }

    /// <summary>
    /// Deletes a booking availability by its ID.
    /// </summary>
    [HttpDelete("{id}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> DeleteBookingAvailability(long id)
    {
        await _bookingAvailabilityService.DeleteAsync(id, AccessToken);
        await _notifier.NotifyUser(AccessToken, "Successfully", "Booking availability has been deleted.");
        return NoContent();
    }

    /// <summary>
    /// Get a booking
    /// </summary>
    [HttpGet("detail/{id}")]
    [PermissionAuthorize((int)EUserRole.ADVISOR)]
    public async Task<IActionResult> GetByIdAsync(long id)
    {
        var res = await _bookingAvailabilityService.GetByIdAsync(id);
        return Ok(res);
    }

}