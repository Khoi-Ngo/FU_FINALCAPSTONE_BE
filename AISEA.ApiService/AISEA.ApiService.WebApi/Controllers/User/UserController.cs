using AISEA.ApiService.BAL.Services.User;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Requests.User;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.HubUtil;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.User;

[ApiController]
[Route("api/[controller]")]
public class UserController : BaseController
{
    private readonly UserService _userService;
    private readonly NotificationHubNotifier _notifier;
    public UserController(EndpointSettings endpointSettings
    , UserService userService
    , NotificationHubNotifier notificationHubNotifier) : base(endpointSettings)
    {
        _userService = userService;
        _notifier = notificationHubNotifier;
    }

    /// <summary>
    /// Retrieves all users from the system.
    /// </summary>
    [HttpGet]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Retrieves all active users from the system.
    /// </summary>
    [HttpGet("active")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> GetAllActiveUsers()
    {
        var users = await _userService.GetAllActiveUsersAsync();
        return Ok(users);
    }
    /// <summary>
    /// Retrieves a student by ID.
    /// </summary>
    [HttpGet("student/{id}")]
    public async Task<IActionResult> GetStudentById(long id)
    {
        var student = await _userService.GetStudentByIdAsync(id);
        return Ok(student);
    }

    /// <summary>
    /// Retrieves a staff by ID.
    /// </summary>
    [HttpGet("staff/{id}")]
    public async Task<IActionResult> GetStaffById(long id)
    {
        var staff = await _userService.GetStaffByIdAsync(id);
        return Ok(staff);
    }

    /// <summary>
    /// Updates an existing student.
    /// </summary>
    [HttpPut("student/{id}")]
    public async Task<IActionResult> UpdateStudent(long id, [FromBody] UpdateStudentRequest request)
    {
        await _userService.UpdateUserAsync(id, request, AccessToken);
        return NotifyAndResponseDummy("Update user successfully");

    }

    /// <summary>
    /// Updates an existing staff.
    /// </summary>
    [HttpPut("staff/{id}")]
    public async Task<IActionResult> UpdateStaff(long id, [FromBody] UpdateStaffRequest request)
    {
        await _userService.UpdateUserAsync(id, request, AccessToken);
        return NotifyAndResponseDummy("Update user successfully");

    }


    /// <summary>
    /// Disables a user by their ID.
    /// </summary>
    [HttpDelete("{id}")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> DisableUser(long id)
    {
        await _userService.DisableUserAsync(id);
        return NotifyAndResponseDummy("Disable user successfully");

    }
    /// <summary>
    /// Retrieves paginated users from the system.
    /// </summary>
    [HttpGet("paged")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> GetAllUsersPaged([FromQuery] PaginationRequest request)
    {
        var result = await _userService.GetAllUsersPagedAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves paginated active users from the system.
    /// </summary>
    [HttpGet("active/paged")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> GetAllActiveUsersPaged([FromQuery] PaginationRequest request)
    {
        var result = await _userService.GetAllActiveUsersPagedAsync(request);
        return Ok(result);
    }


    /// <summary>
    /// Retrieves paginated  STUDENT users from the system.
    /// </summary>
    [HttpGet("student/paged")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> GetAllStudentsPaged([FromQuery] PaginationRequest request)
    {
        var result = await _userService.GetAllStudentsPagedAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves paginated  academic staff users from the system.
    /// </summary>
    [HttpGet("academic-staffs/paged")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> GetAllAcademicStaffsPaged([FromQuery] PaginationRequest request)
    {
        var result = await _userService.GetAllStaffsPagedAsync(request, EUserRole.ACADEMIC_STAFF);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves paginated  admin users from the system.
    /// </summary>
    [HttpGet("admins/paged")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> GetAllAdminsPaged([FromQuery] PaginationRequest request)
    {
        var result = await _userService.GetAllStaffsPagedAsync(request, EUserRole.ADMIN);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves paginated  manager users from the system.
    /// </summary>
    [HttpGet("managers/paged")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> GetAllManagersPaged([FromQuery] PaginationRequest request)
    {
        var result = await _userService.GetAllStaffsPagedAsync(request, EUserRole.MANAGER);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves paginated  advisor users from the system.
    /// </summary>
    [HttpGet("advisors/paged")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> GetAllAdvisorsPaged([FromQuery] PaginationRequest request)
    {
        var result = await _userService.GetAllStaffsPagedAsync(request, EUserRole.ADVISOR);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all active Advisors from the system. (Support Booking Feature ~ Student Access only)
    /// </summary>
    [HttpGet("advisors/active/paged")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    public async Task<IActionResult> GetAllActiveAdvisors([FromQuery] PaginationRequest request)
    {
        var advisors = await _userService.GetAllActiveAdvisorsAsync(request);
        return Ok(advisors);
    }

    /// <summary>
    /// Creates a new user with the provided information. Duplicate usernames or emails will result in bad request errors.
    /// There are 3 cases: No profile, Student Profile only, and Staff Profile only.
    /// </summary>
    [HttpPost]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        await _userService.CreateUserAsync(request);
        return NotifyAndResponseDummy("New user is created successfully");
    }

    /// <summary>
    /// Creates multiple users with the provided information. Duplicate usernames or emails will result in bad request errors.
    /// Admin and Manager no need to have profile.
    /// </summary>
    [HttpPost("bulk")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> CreateUsers([FromBody] List<CreateUserRequest> requests)
    {
        await _userService.CreateUsersAsync(requests);
        return NotifyAndResponseDummy("New users are bulk created successfully");
    }

    #region Bulk Create Users By Role

    /// <summary>
    /// Bulk Creating Students
    /// </summary>
    [HttpPost("student-bulk")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> CreateStudents([FromBody] List<BulkCreateStudentRequest> requests)
    {
        await _userService.CreateUsersAsync(requests);
        return NotifyAndResponseDummy("New students are bulk created successfully");
    }

    /// <summary>
    /// Bulk Creating Advisors
    /// </summary>
    [HttpPost("advisor-bulk")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> CreateAdvisors([FromBody] List<BulkCreateStaffByRoleRequest> requests)
    {
        await _userService.CreateUsersAsync(requests, EUserRole.ADVISOR);
        return NotifyAndResponseDummy("New advisors are bulk created successfully");
    }

    /// <summary>
    /// Bulk Creating Academic Staffs
    /// </summary>
    [HttpPost("academic-staff-bulk")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> CreateAcademicStaffs([FromBody] List<BulkCreateStaffByRoleRequest> requests)
    {
        await _userService.CreateUsersAsync(requests, EUserRole.ACADEMIC_STAFF);
        return NotifyAndResponseDummy("New academic staffs are bulk created successfully");
    }

    /// <summary>
    /// Bulk Creating Admins
    /// </summary>
    [HttpPost("admin-bulk")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> CreateAdmins([FromBody] List<BulkCreateStaffByRoleRequest> requests)
    {
        await _userService.CreateUsersAsync(requests, EUserRole.ADMIN);
        return NotifyAndResponseDummy("New admins are bulk created successfully");
    }

    /// <summary>
    /// Bulk Creating Managers
    /// </summary>
    [HttpPost("manager-bulk")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> CreateManagers([FromBody] List<BulkCreateStaffByRoleRequest> requests)
    {
        await _userService.CreateUsersAsync(requests, EUserRole.MANAGER);
        return NotifyAndResponseDummy("New managers are bulk created successfully");
    }

    #endregion

    /// <summary>
    ///  Admin Reset NumberOfBan of student
    /// </summary>
    [HttpPut("reset-noOfBan/{studentProfileId}")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> ResetNumberOfBan(long studentProfileId)
    {
        await _userService.ResetNumberOfBanAsync(studentProfileId);
        return NotifyAndResponseDummy("The number of ban for the student profile id has been reset");
    }


    /// <summary>
    /// User can update the avatar the input is link firebase provided by front end
    /// </summary>
    [HttpPut("update-avatar")]
    public async Task<IActionResult> UpdateAvatar([FromBody] UpdateAvatarRequest request)
    {
        await _userService.UpdateAvatarAsync(AccessToken, request);
        return NotifyAndResponseDummy("The avatar has been updated successfully");
    }
    /// <summary>
    /// User can update the avatar the input is link firebase provided by front end
    /// </summary>
    [HttpPut("staff-update-avatar/{userId}")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public async Task<IActionResult> UpdateAvatarByAdmin([FromBody] UpdateAvatarRequest request, long userId)
    {
        await _userService.UpdateAvatarAsync(userId, request);
        return NotifyAndResponseDummy("The avatar has been updated successfully");
    }


    /// <summary>
    /// Helper to notify success and return NoContent
    /// </summary>
    private IActionResult NotifyAndResponseDummy(string message)
    {
        _ = _notifier.NotifyUserAsync(AccessToken, "Successfully", message);
        return Ok("Ok");
    }




}