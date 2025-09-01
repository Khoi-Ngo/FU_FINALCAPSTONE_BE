using AISEA.ApiService.BAL.Services.CourseTracker;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.CheckPoint;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
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
    [HttpPut("{id}")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    [AuditLog(Tag = "UPDATE_CHECKPOINT")]
    public async Task<IActionResult> Update([FromBody] CommandCheckpointRequest request, long id)
    {
        var accessToken = AccessToken;
        await _joinedSubjectCheckPointService.UpdateAsync(request, id, accessToken);
        return Ok("Update checkpoint successfully");
    }

    ///<summary>
    ///  View ONE detail todo item  (All Roles can access)
    /// </summary>
    [HttpGet("{id}")]
    [AuditLog(Tag = "VIEW_CHECKPOINT")]
    public async Task<IActionResult> ViewDetailById(long id)
    {
        var res = await _joinedSubjectCheckPointService.ViewDetailByIdAsync(id);
        return Ok(res);
    }

    ///<summary>
    /// View ALL a to do item for a joined subject  (List) , All roles can access
    /// </summary>
    [HttpGet("joinedSubject/{joinedSubjectId}")]
    [AuditLog(Tag = "VIEW_CHECKPOINT")]
    public async Task<IActionResult> ViewAllBySubjectId(long joinedSubjectId)
    {
        var res = await _joinedSubjectCheckPointService.ViewAllByJoinedSubjectIdAsync(joinedSubjectId);
        return Ok(res);
    }


    ///<summary>
    /// View all by student profile id pagination
    /// </summary>
    [HttpGet("student/{studentProfileId}")]
    [AuditLog(Tag = "VIEW_CHECKPOINT")]
    [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.MANAGER, (int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.ADVISOR)]
    public async Task<IActionResult> ViewAllByStudentProfileId(long studentProfileId
    , [FromQuery] PaginationRequest paginationRequest
    , [FromQuery] bool isInCompletedOnly = true
    , [FromQuery] bool isNoneFilterStatus = false
    , [FromQuery] bool isOrderedByNearToFarDeadline = true
    )
    {
        var accessToken = AccessToken;
        var res = await _joinedSubjectCheckPointService.ViewAllByStudentProfileIdAsync(studentProfileId,
        paginationRequest, isInCompletedOnly, isNoneFilterStatus, isOrderedByNearToFarDeadline, accessToken);
        return Ok(res);
    }


    ///<summary>
    /// View top size nearest upcoming + not completed by self
    /// </summary>
    [HttpGet("upcoming")]
    [AuditLog(Tag = "VIEW_CHECKPOINT")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    public async Task<IActionResult> ViewAllBySelfUpcoming([FromQuery] int limit = 10)
    {
        var accessToken = AccessToken;
        var res = await _joinedSubjectCheckPointService.ViewAllBySelfUpcomingAsync(limit > 10 ? 10 : limit, accessToken);
        return Ok(res);
    }



    ///<summary>
    /// Mark complete a to do item
    /// </summary>
    [HttpPut("complete/{id}")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    [AuditLog(Tag = "COMPLETE_CHECKPOINT")]
    public async Task<IActionResult> Complete(long id)
    {
        var accessToken = AccessToken;
        await _joinedSubjectCheckPointService.CompleteAsync(id, accessToken);
        return Ok("Complete checkpoint successfully");
    }



    ///<summary>
    /// Gen the list of TODO suggestion by AI
    /// </summary>
    [HttpGet("gen/{joinedSubjectId}")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    [AuditLog(Tag = "AI_GENERATE_CHECKPOINT")]
    public async Task<IActionResult> GenerateCheckpoints(long joinedSubjectId, [FromQuery] string studentMessage
    , [FromQuery] string? ownerGitRepo, [FromQuery] string? gitRepoName)
    {
        var res = await _joinedSubjectCheckPointService.GenerateCheckpointsAsync(joinedSubjectId, AccessToken, studentMessage, ownerGitRepo, gitRepoName);
        return Ok(res);
    }



    ///<summary>
    /// Student CREATE bulk to do item for a joined subject of hi or her
    /// </summary>
    [HttpPost("bulk/{joinedSubjectId}")]
    [PermissionAuthorize((int)EUserRole.STUDENT)]
    [AuditLog(Tag = "BULK_CREATE_CHECKPOINT")]
    public async Task<IActionResult> Create([FromBody] List<CommandCheckpointRequest> request, long joinedSubjectId, [FromQuery] bool doReplaceAll = true)
    {
        var accessToken = AccessToken;
        await _joinedSubjectCheckPointService.CreateAsync(request, doReplaceAll, joinedSubjectId, accessToken);
        return Ok("Bulk create checkpoint successfully");
    }




}