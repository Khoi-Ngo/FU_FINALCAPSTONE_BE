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

    public RoleController(
        EndpointSettings endpointSettings,
        RoleService roleService,
        NotificationHubNotifier notifier) : base(endpointSettings)
    {
        _roleService = roleService;
        _notifier = notifier;
    }

    /// <summary>
    /// Get all roles.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        var res = await _roleService.GetAllRolesAsync();
        return Ok(res);
    }

    /// <summary>
    /// Get a role by its ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoleById(long id)
    {
        var res = await _roleService.GetRoleByIdAsync(id);
        return Ok(res);
    }

    /// <summary>
    /// Create a new role.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest role)
    {
        await _roleService.CreateRoleAsync(role);
        return await NotifyAndResponseOkAsync("New role is created successfully");
    }

    /// <summary>
    /// Update an existing role.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(long id, [FromBody] UpdateRoleRequest role)
    {
        await _roleService.UpdateRoleAsync(id, role);
        return await NotifyAndResponseOkAsync("The role is updated successfully");
    }

    /// <summary>
    /// Helper to notify and return a success response
    /// </summary>
    private async Task<IActionResult> NotifyAndResponseOkAsync(string message)
    {
        await _notifier.NotifyUserAsync(AccessToken, "Successfully", message);
        return Ok(new { Message = message });
    }
}
