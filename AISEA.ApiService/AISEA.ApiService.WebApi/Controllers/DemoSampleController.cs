using AISEA.ApiService.BAL.Services;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using AISEA.ApiService.DAL.Repositories;
using AutoMapper;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Responses.Role;
using AISEA.ApiService.SHARED.DTOs.Requests.Role;

namespace AISEA.ApiService.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DemoSampleController : BaseController
{
    private readonly DemoSampleService _demoSampleService;

    private readonly EndpointSettings _endpointSettings;
    private readonly SqlSettings _sqlSettings;
    private readonly RoleRepository _roleRepository;
    private readonly IMapper _mapper;

    public DemoSampleController(
        DemoSampleService demoSampleService,
        EndpointSettings endpointSettings,
        SqlSettings sqlSettings,
        RoleRepository roleRepository,
        IMapper mapper
    ) : base(endpointSettings)
    {
        _demoSampleService = demoSampleService;
        _endpointSettings = endpointSettings;
        _sqlSettings = sqlSettings;
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult Demo()
    {
        return Ok(new { NonKey = "Test NonKey Again + Test New Code Applied" });
    }

    [HttpGet("/demo2")]
    [AllowAnonymous]
    public IActionResult Demo2()
    {
        return Ok(_sqlSettings.ConnectionString);
    }

    [HttpGet("test-connection")]
    [AllowAnonymous]
    public IActionResult TestConnect()
    {
        using (var connection = new SqlConnection(_sqlSettings.ConnectionString))
        {
            connection.Open();
            if (connection.State == System.Data.ConnectionState.Open)
            {
                return Ok(new { Success = true, Message = "Connection to MS SQL database succeeded." });
            }
            else
            {
                return StatusCode(500, new { Success = false, Message = "Connection to MS SQL database failed." });
            }
        }
    }
    [HttpGet("test-get-mapper")]
    [AllowAnonymous]
    public async Task<IActionResult> TestGetMapper()
    {
        var res = await _roleRepository.GetAllAsync();
        return Ok(_mapper.Map<List<GetRoleResponse>>(res));
    }
    [HttpPost("test-update-mapper")]
    [AllowAnonymous]
    public async Task<IActionResult> TestUpdateMapper([FromBody] UpdateRoleRequest request)
    {
        //id have to get from the header request
        var id = 3;
        var role = _mapper.Map<Role>(request);
        role.Id = id; //set the id to the role
        await _roleRepository.UpdateAsync(role);
        return Ok("Role updated successfully.");
    }


    [HttpGet("with-role-specified")]
    [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.MANAGER)]
    public IActionResult DemoWithRoleSpecified()
    {
        return Ok(new { NonKey = "asdksajdsakjdaskjdsa" + "With Role Specified Version" });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> DemoLoginWithRedis([FromBody] DemoLoginRequest request)
    {
        var res = await _demoSampleService.DemoLoginWithRedis(request.UserName, request.Password);
        return Ok(res);
    }


    [HttpGet("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> DemoRefreshTokenWithRedis()
    {
        var res = await _demoSampleService.DemoRefreshTokenWithRedis(AccessToken, RefreshToken);
        return Ok(res);
    }

    [HttpGet("logout")]
    public async Task<IActionResult> DemoLogoutWithRedis()
    {
        await _demoSampleService.DemoLogoutWithRedis(AccessToken);
        return Ok();
    }

}
public class DemoLoginRequest
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}
