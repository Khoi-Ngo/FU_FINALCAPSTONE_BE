using AISEA.ApiService.BAL.Services.SystemProfile;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.SystemProfile;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.HubUtil;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.UserProfile;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : BaseController
{
    private readonly StudentProfileService _studentProfileService;
    private readonly StaffProfileService _staffProfileService;
    private readonly NotificationHubNotifier _notifier;
    public ProfileController(EndpointSettings endpointSettings, StudentProfileService studentProfileService, StaffProfileService staffProfileService, NotificationHubNotifier notificationHubNotifier) : base(endpointSettings)
    {
        _staffProfileService = staffProfileService;
        _studentProfileService = studentProfileService;
        _notifier = notificationHubNotifier;
    }

    #region Student Profile

    /// <summary>
    /// Create student profile with existed user in the system
    /// </summary>
    [HttpPost("student")]
    [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.STUDENT)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateStudentProfileRequest request)
    {
        await _studentProfileService.CreateAsync(request, AccessToken);
        return NoContent();
    }

    #endregion

    #region Staff Profile

    /// <summary>
    /// Create staff profile with existed user in the system
    /// </summary>
    [HttpPost("staff")]
    [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.ADVISOR, (int)EUserRole.MANAGER)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateStaffProfileRequest request)
    {
        await _staffProfileService.CreateAsync(request, AccessToken);
        return NoContent();
    }


    #endregion
}