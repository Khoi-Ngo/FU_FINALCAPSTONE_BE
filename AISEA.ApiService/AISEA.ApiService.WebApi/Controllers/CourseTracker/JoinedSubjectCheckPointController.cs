using AISEA.ApiService.BAL.Services.CourseTracker;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.CheckPoint;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.InterceptorAPI;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.CourseTracker;

[ApiController]
[Route("api/[controller]")]
public class JoinedSubjectCheckPointController : BaseController
{

    //View besides self will allow all roles view this kind of data per student also
    #region Init
    private readonly JoinedSubjectCheckPointService _joinedSubjectCheckPointService;

    public JoinedSubjectCheckPointController(EndpointSettings endpointSettings
    , JoinedSubjectCheckPointService joinedSubjectCheckPointService) : base(endpointSettings)
    {
        _joinedSubjectCheckPointService = joinedSubjectCheckPointService;
    }

    #endregion


    ///<summary>
    /// Student CREATE a to do item for a joined subject of hi or her
    /// </summary>
    [HttpPost("{joinedSubjectId}")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    [AuditLog(Tag = "CREATE_CHECKPOINT")]
    public async Task<IActionResult> Create([FromBody] CommandCheckpointRequest request, long joinedSubjectId)
    {
        var accessToken = AccessToken;
        await _joinedSubjectCheckPointService.CreateAsync(request, joinedSubjectId, accessToken);
        return Ok("Create checkpoint successfully");
    }



    ///<summary>
    /// Student DELETE a to do item for a joined subject of hi or her
    /// </summary>
    [HttpDelete("{id}")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    [AuditLog(Tag = "DELETE_CHECKPOINT")]
    public async Task<IActionResult> Delete(long id)
    {
        var accessToken = AccessToken;
        await _joinedSubjectCheckPointService.RemoveAsync(id, accessToken);
        return Ok("Remove checkpoint successfully");
    }



    ///<summary>
    /// Student UPDATE a to do item for a joined subject of hi or her
    /// </summary>


    ///<summary>
    /// Student View ONE to do item for a joined subject of hi or her
    /// </summary>


    ///<summary>
    /// Student View ALL a to do item for a joined subject of hi or her
    /// </summary>


    ///<summary>
    /// Student View ALL ACTIVE  to do item for all non completed joined subject of hi or her
    /// </summary>


    ///<summary>
    /// Mark complete a to do item
    /// </summary>



    ///Gen the list of TODO - including the existed - completed items


    /// Bulk insertion



    /// Bulk update


    /// Gen without data existed



    /// Gen with having data -> Only keep the completed item
}