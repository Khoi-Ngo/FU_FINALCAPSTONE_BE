using AISEA.ApiService.BAL.Services.Role;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Noti;
using AISEA.ApiService.SHARED.DTOs.Requests.Role;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.HubUtil;
using AISEA.ApiService.WebApi.InterceptorAPI;
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
    [AuditLog(Tag = "VIEW_ROLE", Description = "")]
    public async Task<IActionResult> GetAllRoles()
    {
        var res = await _roleService.GetAllRolesAsync();
        return Ok(res);
    }

    /// <summary>
    /// Get a role by its ID.
    /// </summary>
    [HttpGet("{id}")]
    [AuditLog(Tag = "VIEW_ROLE", Description = "")]
    public async Task<IActionResult> GetRoleById(long id)
    {
        var res = await _roleService.GetRoleByIdAsync(id);
        return Ok(res);
    }

    /// <summary>
    /// Create a new role.
    /// </summary>
    [HttpPost]
    [AuditLog(Tag = "CREATE_ROLE", Description = "The role has been created")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest role)
    {
        await _roleService.CreateRoleAsync(role);

        await _notifier.NotifyUserAsync(AccessToken,
                      new NotificationDTO { Title = "Successfully", Content = "New role is created successfully" });
        return Ok("New role is created successfully");
    }

    /// <summary>
    /// Update an existing role.
    /// </summary>
    [HttpPut("{id}")]
    [AuditLog(Tag = "UPDATE_ROLE", Description = "The role has been updated")]
    public async Task<IActionResult> UpdateRole(long id, [FromBody] UpdateRoleRequest role)
    {
        await _roleService.UpdateRoleAsync(id, role);

        await _notifier.NotifyUserAsync(AccessToken,
              new NotificationDTO { Title = "Successfully", Content = "The role is updated successfully" });
        return Ok("The role is updated successfully");
    }

}
