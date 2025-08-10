using AISEA.ApiService.BAL.Services.SystemProfile;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Noti;
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
    public ProfileController(EndpointSettings endpointSettings, StudentProfileService studentProfileService, StaffProfileService staffProfileService, NotificationHubNotifier notifier) : base(endpointSettings)
    {
        _staffProfileService = staffProfileService;
        _studentProfileService = studentProfileService;
        _notifier = notifier;
    }
    //TODO: CRUD Combo or Program or Curriculum -> Need Worker Trigger to change data in JoinedSubject table


    /// <summary>
    /// Create student profile with existed user in the system
    /// </summary>
    [HttpPost("student")]
    [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.STUDENT)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateStudentProfileRequest request)
    {
        await _studentProfileService.CreateAsync(request, AccessToken);
        await _notifier.NotifyUserAsync(AccessToken,
         new NotificationDTO { Title = "Profile Created", Content = "The student profile has been created successfully." });
        return Ok("Student profile created successfully");
    }



    /// <summary>
    /// Create staff profile with existed user in the system
    /// </summary>
    [HttpPost("staff")]
    [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.ADVISOR, (int)EUserRole.MANAGER)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateStaffProfileRequest request)
    {
        await _staffProfileService.CreateAsync(request, AccessToken);
        await _notifier.NotifyUserAsync(AccessToken,
        new NotificationDTO { Title = "Profile Created", Content = "The staff profile has been created successfully." });
        return Ok("Staff profile created successfully");
    }


}