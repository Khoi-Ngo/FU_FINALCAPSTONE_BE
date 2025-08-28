using System.ComponentModel.DataAnnotations;
using AISEA.ApiService.BAL.Services.CourseTracker;
using AISEA.ApiService.BAL.Services.SystemProfile;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.SystemProfile;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.Interfaces;
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
    private readonly IBackgroundTaskQueue _taskQueue;

    public ProfileController(EndpointSettings endpointSettings
    , StudentProfileService studentProfileService
    , StaffProfileService staffProfileService
    , IBackgroundTaskQueue taskQueue) : base(endpointSettings)
    {
        _staffProfileService = staffProfileService;
        _studentProfileService = studentProfileService;
        _taskQueue = taskQueue;
    }


    /// <summary>
    /// Create student profile with existed user in the system
    /// </summary>
    [HttpPost("student")]
    [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.STUDENT)]
    [AuditLog(Tag = "CREATE_PROFILE_FOR_USER")]
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
    [AuditLog(Tag = "CREATE_PROFILE_FOR_USER")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateStaffProfileRequest request)
    {
        var accessToken = AccessToken;

        await _staffProfileService.CreateAsync(request, accessToken);
        return Ok("Staff profile created successfully");
    }


    #region  UPDATE COMBO + UPDATE CURRICULUM CODE

    /// <summary>
    /// Update the Curriculum or Combo of student
    /// </summary>
    [HttpPut("student-profile/{stuproID}")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    [AuditLog(Tag = "UPDATE COMBO OR CURRICULUM")]
    public async Task<IActionResult> UpdateComborCuri(
    [FromBody] UpdateComboOrCurriRequest request,
    [Range(1, long.MaxValue, ErrorMessage = "Student Profile ID must be greater than 0.")]
    long stuproID)
    {
        var studentProfile = await _studentProfileService.UpdateComborCuriAsync(request, stuproID);
        //trigger the worker run in background to deactivate joined subject not in the curriculum or combo code
        _taskQueue.QueueBackgroundWorkItem(async (sp, token) =>
    {
        var qJoinedSubjectService = sp.GetRequiredService<JoinedSubjectService>();

        await qJoinedSubjectService.DeActivateNonUseJoinedSubjectAsync(studentProfile);

    });


        return Ok("Update curriculum-combo successfully");
    }

    #endregion


}