using AISEA.ApiService.BAL.Services.AuditLog;
using AISEA.ApiService.BAL.Services.User;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Requests.User;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.User;

[ApiController]
[Route("api/[controller]")]
public class UserController : BaseController
{
    private readonly UserService _userService;
    private readonly AuditLogService _auditLogService;
    public UserController(EndpointSettings endpointSettings, UserService userService, AuditLogService auditLogService) : base(endpointSettings)
    {
        _userService = userService;
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// Creates a new user with the provided information. Duplicate usernames or emails will result in bad request errors. There are 3 cases : No profile, Student Profile only and Staff Profile only
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        await _userService.CreateUserAsync(request);
        await _auditLogService.CreateAsync(EAuditLogTag.CREATE_USER);
        return Ok(new { Message = "User created successfully." });
    }

    /// <summary>
    /// Creates multiple users with the provided information. Duplicate usernames or emails will result in bad request errors.
    /// Admin and Manager no need to have profile
    /// </summary>
    [HttpPost("bulk")]
    public async Task<IActionResult> CreateUsers([FromBody] List<CreateUserRequest> requests)
    {
        await _userService.CreateUsersAsync(requests);
        return Ok(new { Message = "Users created successfully." });
    }

    /// <summary>
    /// Retrieves all users from the system.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Retrieves all active users from the system.
    /// </summary>
    [HttpGet("active")]
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
    /// Retrieves a student by ID.
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
        await _userService.UpdateUserAsync(id, request);
        return Ok("Updated successfully");
    }

    /// <summary>
    /// Updates an existing staff.
    /// </summary>
    [HttpPut("staff/{id}")]
    public async Task<IActionResult> UpdateStaff(long id, [FromBody] UpdateStaffRequest request)
    {
        await _userService.UpdateUserAsync(id, request);
        return Ok("Updated successfully");
    }


    /// <summary>
    /// Disables a user by their ID.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DisableUser(long id)
    {
        await _userService.DisableUserAsync(id);
        await _auditLogService.CreateAsync(EAuditLogTag.DELETE_USER);
        return Ok("User disabled successfully");
    }
    /// <summary>
    /// Retrieves paginated users
    /// </summary>
    [HttpGet("paged")]
    public async Task<IActionResult> GetAllUsersPaged([FromQuery] PaginationRequest request)
    {
        var result = await _userService.GetAllUsersPagedAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves paginated active users from the system.
    /// </summary>
    [HttpGet("active/paged")]
    public async Task<IActionResult> GetAllActiveUsersPaged([FromQuery] PaginationRequest request)
    {
        var result = await _userService.GetAllActiveUsersPagedAsync(request);
        return Ok(result);
    }


    /// <summary>
    /// Retrieves paginated  STUDENT users from the system.
    /// </summary>
    [HttpGet("student/paged")]
    public async Task<IActionResult> GetAllStudentsPaged([FromQuery] PaginationRequest request)
    {
        var result = await _userService.GetAllStudentsPagedAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves paginated  STAFF users from the system.
    /// </summary>
    [HttpGet("staff/paged")]
    public async Task<IActionResult> GetAllStaffsPaged([FromQuery] PaginationRequest request)
    {
        var result = await _userService.GetAllStaffsPagedAsync(request);
        return Ok(result);
    }



}