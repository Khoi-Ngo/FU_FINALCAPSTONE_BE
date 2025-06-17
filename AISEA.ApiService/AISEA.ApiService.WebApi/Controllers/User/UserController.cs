using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.BAL.Services.User;
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

    //get all users

    //get user by id

    //update user 

    //disable user by id
}