using AISEA.ApiService.BAL.Services.Role;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Role;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.HubUtil;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.Role;

[ApiController]
[Route("api/[controller]")]
[PermissionAuthorize((int)EUserRole.ADMIN)]
public class RoleController : BaseController
{

    private readonly RoleService _roleService;
    private readonly NotificationHubNotifier _notifier;
    public RoleController(EndpointSettings endpointSettings, RoleService roleService, NotificationHubNotifier notifier) : base(endpointSettings)
    {
        _roleService = roleService;
        _notifier = notifier;
    }

    /// <summary>
    /// Get all roles.
    /// </summary>
    /// <returns>A list of roles.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        var res = await _roleService.GetAllRolesAsync();
        return Ok(res);
    }

    /// <summary>
    /// Get a role by its ID.
    /// </summary>
    /// <param name="id">The ID of the role.</param>
    /// <returns>The role with the specified ID.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoleById(long id)
    {
        var res = await _roleService.GetRoleByIdAsync(id);
        return Ok(res);
    }

    /// <summary>
    /// Create a new role.
    /// </summary>
    /// <param name="role">The role to create.</param>
    /// <returns>The created role.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest role)
    {
        await _roleService.CreateRoleAsync(role);
        //notify that the role created successfully
        _notifier.NotifyUser(AccessToken, "Successfully", "New role is created successfully");
        return Ok("Ok");
    }

    /// <summary>
    /// Update an existing role.
    /// </summary>
    /// <param name="id">The ID of the role to update.</param>
    /// <param name="role">The updated role information.</param>
    /// <returns>The updated role.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(long id, [FromBody] UpdateRoleRequest role)
    {
        await _roleService.UpdateRoleAsync(id, role);
        _notifier.NotifyUser(AccessToken, "Successfully", "The role is updated successfully");
        return Ok("Ok");
    }
}