using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.BAL.Services.User;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Requests.User;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.User;

[ApiController]
[Route("api/[controller]")]
public class UserController : BaseController
{
    private readonly UserService _userService;
    public UserController(EndpointSettings endpointSettings, UserService userService) : base(endpointSettings)
    {
        _userService = userService;
    }

    //create single user
    /// <summary>
    /// Creates a new user with the provided information. Duplicate usernames or emails will result in bad request errors.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        await _userService.CreateUserAsync(request);
        return Ok(new { Message = "User created successfully." });
    }

    //create multiple users
    /// <summary>
    /// Creates multiple users with the provided information. Duplicate usernames or emails will result in bad request errors.
    /// </summary>
    [HttpPost("bulk")]
    public async Task<IActionResult> CreateUsers([FromBody] List<CreateUserRequest> requests)
    {
        await _userService.CreateUsersAsync(requests);
        return Ok(new { Message = "Users created successfully." });
    }

    //get all users
    /// <summary>
    /// Retrieves all users from the system.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    //get all active users
    /// <summary>
    /// Retrieves all active users from the system.
    /// </summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetAllActiveUsers()
    {
        var users = await _userService.GetAllActiveUsersAsync();
        return Ok(users);
    }
    //get user by id
    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(long id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return Ok(user);
    }

    //update user
    /// <summary>
    /// Updates an existing user.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(long id, [FromBody] UpdateUserRequest request)
    {
        await _userService.UpdateUserAsync(id, request);
        return Ok("Updated successfully");
    }

    //disable user by id
    /// <summary>
    /// Disables a user by their ID.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DisableUser(long id)
    {
        await _userService.DisableUserAsync(id);
        return Ok("User disabled successfully");
    }
    /// <summary>
    /// Retrieves paginated users from the system.
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

}