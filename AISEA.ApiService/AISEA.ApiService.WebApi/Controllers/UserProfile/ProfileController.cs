using AISEA.ApiService.BAL.Services.SystemProfile;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.SystemProfile;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.InterceptorAPI;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.UserProfile;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : BaseController
{
    private readonly StudentProfileService _studentProfileService;
    private readonly StaffProfileService _staffProfileService;
    public ProfileController(EndpointSettings endpointSettings, StudentProfileService studentProfileService, StaffProfileService staffProfileService) : base(endpointSettings)
    {
        _staffProfileService = staffProfileService;
        _studentProfileService = studentProfileService;
    }
    //TODO: CRUD Combo or Program or Curriculum -> Need Worker Trigger to change data in JoinedSubject table


    /// <summary>
    /// Create student profile with existed user in the system
    /// </summary>
    [HttpPost("student")]
    [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.STUDENT)]
    [AuditLog(Tag = "CREATE_PROFILE_FOR_USER", Description = "")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateStudentProfileRequest request)
    {
        var accessToken = AccessToken;

        await _studentProfileService.CreateAsync(request, accessToken);

        return Ok("Student profile created successfully");
    }



    /// <summary>
    /// Create staff profile with existed user in the system
    /// </summary>
    [HttpPost("staff")]
    [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.ADVISOR, (int)EUserRole.MANAGER)]
    [AuditLog(Tag = "CREATE_PROFILE_FOR_USER", Description = "")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateStaffProfileRequest request)
    {
        var accessToken = AccessToken;

        await _staffProfileService.CreateAsync(request, accessToken);
        return Ok("Staff profile created successfully");
    }


}